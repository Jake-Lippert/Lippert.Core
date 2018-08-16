using System;
using System.Collections.Generic;
using System.Linq;
using Lippert.Core.Configuration;
using Lippert.Core.Data.Contracts;

namespace Lippert.Core.Data
{
	public static class TableMapSource
	{
		public static ITableMap<T> GetTableMap<T>() => (ITableMap<T>)GetTableMap(typeof(T));

		public static ITableMap GetTableMap(Type type) => GetTableMaps()
			.Where(x => x.ModelType == type)
			.Single();

		public static List<ITableMap> GetTableMaps() =>
			ReflectingRegistrationSource.GetCodebaseTypesAssignableTo<ITableMap>()
				.Where(t => t.IsClass && !t.IsAbstract && !t.ContainsGenericParameters)
				.Select(t => (ITableMap)Activator.CreateInstance(t))
				.ToList();

		public static List<ITableMapBuilder> GetTableMapBuilders<T>() => GetTableMapBuilders()
			.Where(mb => mb.HandlesType<T>())
			.ToList();

		public static List<ITableMapBuilder> GetTableMapBuilders() =>
			ReflectingRegistrationSource.GetCodebaseTypesAssignableTo<ITableMapBuilder>()
				.Where(t => t.IsClass && !t.IsAbstract && !t.ContainsGenericParameters)
				.Select(t => (ITableMapBuilder)Activator.CreateInstance(t))
				.ToList();
	}
}