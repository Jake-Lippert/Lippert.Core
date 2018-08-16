using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lippert.Core.Collections;
using NUnit.Framework;
using Moq;
using System.Collections.Generic;

namespace Lippert.Core.Tests.Collections
{
	[TestFixture]
	public class ResourcePoolTests
	{
		[Test]
		public void TestResourcePoolCreatesAndReusesItems()
		{
			var resources = new List<string>();
			var factoryMock = new Mock<IStringFactory>();
			factoryMock.Setup(x => x.GetString())
				.Returns(() =>
				{
					if (resources.Count < 5)
					{
						var resource = Guid.NewGuid().ToString();
						resources.Add(resource);
						return resource;
					}

					return null;
				});


			using (var pool = new ResourcePool<string>(x => factoryMock.Object.GetString())
			{
				CleanupPoolItem = (resource) => CollectionAssert.Contains(resources, resource)
			})
			{
				var list = Enumerable.Range(0, 10).ToList();
				var start = DateTime.Now;
				Parallel.ForEach(list, x =>
				{
					using (var item = pool.GetItem())
					{
						string resource = item;
						Thread.Sleep(5000);
					}
				});

				var duration = DateTime.Now - start;
				Assert.GreaterOrEqual(duration, TimeSpan.FromSeconds(9.9));
				Assert.Less(duration, TimeSpan.FromSeconds(25));


				Parallel.ForEach(list, x =>
				{
					using (var item = pool.GetItem())
					{
						string resource = item;
					}
				});
			}
		}

		[Test]
		public void TestEmptyPoolThrowsExceptionIfResourcesCantBeCreated()
		{
			var factoryMock = new Mock<IStringFactory>();
			
			using (var pool = new ResourcePool<string>(x => factoryMock.Object.GetString()))
			{
				Assert.Throws<InvalidOperationException>(() => pool.GetItem());
			}
		}

		[Test]
		public void TestPoolThrowsExceptionIfResourcesInUse()
		{
			var resources = 0;
			var factoryMock = new Mock<IStringFactory>();
			factoryMock.Setup(x => x.GetString())
				.Returns(() => resources++ < 5 ? Guid.NewGuid().ToString() : null);

			var pool = new ResourcePool<string>(x => factoryMock.Object.GetString());
			var item = pool.GetItem();
			Assert.Throws<InvalidOperationException>(() => pool.Dispose());
		}


		public interface IStringFactory
		{
			string GetString();
		}
	}
}