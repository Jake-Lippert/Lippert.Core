using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Lippert.Core.Collections
{
	/// <summary>
	/// Generic pool class - http://florianreischl.blogspot.com/2011/08/generic-c-resource-pool.html
	/// </summary>
	/// <typeparam name="T">The type of items to be stored in the pool</typeparam>
	public class ResourcePool<T> : IDisposable where T : class
	{
		private readonly Func<ResourcePool<T>, T?> _factoryMethod;
		private ConcurrentQueue<PoolItem> _freeItems = new ConcurrentQueue<PoolItem>();
		private ConcurrentQueue<AutoResetEvent> _waitLocks = new ConcurrentQueue<AutoResetEvent>();
		private ConcurrentDictionary<AutoResetEvent, PoolItem> _syncContext = new ConcurrentDictionary<AutoResetEvent, PoolItem>();

		/// <summary>
		/// Creates a new pool
		/// </summary>
		/// <param name="factory">The factory method to create new items to be stored in the pool</param>
		public ResourcePool(Func<T?> factory)
		{
			if (factory is null)
			{
				throw new ArgumentNullException(nameof(factory));
			}

			_factoryMethod = (ResourcePool<T> pool) => factory();
		}
		/// <summary>
		/// Creates a new pool
		/// </summary>
		/// <param name="factory">The factory method to create new items to be stored in the pool</param>
		public ResourcePool(Func<ResourcePool<T>, T?> factory)
		{
			_factoryMethod = factory ?? throw new ArgumentNullException(nameof(factory));
		}


		public Action<T>? CleanupPoolItem { get; set; }

		/// <summary>
		/// Gets the current count of items in the pool
		/// </summary>
		public int Count { get; private set; }

		public void Dispose()
		{
			lock (this)
			{
				if (Count != _freeItems.Count)
				{
					throw new InvalidOperationException("Cannot dispose the resource pool while one or more pooled items are in use");
				}

				if (CleanupPoolItem is Action<T> cleanMethod)
				{
					foreach (var poolItem in _freeItems)
					{
						cleanMethod(poolItem.Resource);
					}
				}

				Count = 0;
				//--We're disposing this, therefore these nulls shouldn't ever be seen by anything
				_freeItems = null!;
				_waitLocks = null!;
				_syncContext = null!;
			}
		}

		/// <summary>
		/// Gets a free resource from the pool. If no free items available this method
		/// tries to create a new item. If no new item could be created this method
		/// waits until another thread frees one resource.
		/// </summary>
		/// <returns>A resource item</returns>
		public PoolItem GetItem()
		{
			// try to get an item
			if (!TryGetItem(out var item))
			{
				AutoResetEvent? waitLock = null;

				lock (this)
				{
					// try to get an entry in exclusive mode
					if (!TryGetItem(out item))
					{
						// no item available, create a wait lock and enqueue it
						waitLock = new AutoResetEvent(false);
						_waitLocks.Enqueue(waitLock);
					}
				}

				if (waitLock != null)
				{
					// wait until a new item is available
					waitLock.WaitOne();
					_syncContext.TryRemove(waitLock, out item);
					waitLock.Dispose();
				}
			}

			return item ?? throw new InvalidOperationException("Cannot return a null pool item");
		}

		private bool TryGetItem(out PoolItem? item)
		{
			// try to get an already pooled resource
			if (_freeItems.TryDequeue(out item))
			{
				return true;
			}

			lock (this)
			{
				// try to create a new resource
				if (_factoryMethod(this) is T resource)
				{
					// a new resource was created and can be returned
					Count++;
					item = new PoolItem(this, resource);
					return true;
				}
				else if (Count == 0)
				{
					throw new InvalidOperationException("Pool empty and no item created");
				}

				return false;
			}
		}

		/// <summary>
		/// Called from <see cref="PoolItem{T}"/> to free previously taken resources
		/// </summary>
		/// <param name="resource">The resource to send back into the pool.</param>
		internal void SendBackToPool(T resource)
		{
			lock (this)
			{
				var item = new PoolItem(this, resource);

				if (_waitLocks.TryDequeue(out var waitLock))
				{
					_syncContext.TryAdd(waitLock, item);
					waitLock.Set();
				}
				else
				{
					_freeItems.Enqueue(item);
				}
			}
		}


		/// <summary>
		/// Represents an item in the <see cref="ResourcePool{T}"/>
		/// </summary>
		/// <typeparam name="T">The type of the resource to be held</typeparam>
		public sealed class PoolItem : IDisposable
		{
			private readonly ResourcePool<T> _pool;

			internal PoolItem(ResourcePool<T> pool, T resource)
			{
				_pool = pool;
				Resource = resource;
			}


			/// <summary>
			/// Gets the resource held by this resource pool item
			/// </summary>
			public T Resource { get; private set; }

			public static implicit operator T(PoolItem item) => item.Resource;

			/// <summary>
			/// Disposes this instance of a resource pool item and sends the resource back to the pool
			/// </summary>
			public void Dispose()
			{
				_pool.SendBackToPool(Resource);
				Resource = null!;//--We're disposing this, therefore this null shouldn't ever be seen by anything
			}
		}
	}
}