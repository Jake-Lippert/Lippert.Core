using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lippert.Core.Collections
{
	/// <summary>
	/// A generic collection of IDisposables that all get disposed upon disposing of the container
	/// </summary>
	public sealed class CompositeDisposable<T> : IEnumerable<T>, IDisposable
		where T : IDisposable
	{
		private readonly List<T> _disposables;

		public CompositeDisposable(IEnumerable<T> disposables) => _disposables = disposables?.ToList() ?? new List<T>();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public IEnumerator<T> GetEnumerator() => _disposables.GetEnumerator();
		
		public void Dispose()
		{
			_disposables.ForEach(d => d?.Dispose());
			_disposables.Clear();
		}
	}
}