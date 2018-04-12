using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;

namespace Lippert.Core.Extensions
{
	public static class AutoMapperExtensions
	{
		public static IMappingExpression<TSource, TDestination> ForMemberMapFrom<TSource, TMember, TDestination>(
			this IMappingExpression<TSource, TDestination> mappingExpression,
			Expression<Func<TDestination, object>> destinationMember, Expression<Func<TSource, TMember>> sourceMember) =>
			mappingExpression.ForMember(destinationMember, m => m.MapFrom(sourceMember));

		public static IMappingExpression<TSource, TDestination> IgnoreMembers<TSource, TDestination>(
			this IMappingExpression<TSource, TDestination> mappingExpression,
			params Expression<Func<TDestination, object>>[] destinationMembers) =>
			destinationMembers.Aggregate(mappingExpression, (mapExpression, destinationMember) => mapExpression.ForMember(destinationMember, mo => mo.Ignore()));

		public static object Map(this object source, Type sourceType, Type destinationType, Action<IMappingOperationOptions> opts) =>
			opts == default ? Mapper.Map(source, sourceType, destinationType) : Mapper.Map(source, sourceType, destinationType, opts);

		public static object Map(this object source, object destination, Type sourceType, Type destinationType, Action<IMappingOperationOptions> opts = null) =>
			opts == default ? Mapper.Map(source, destination, sourceType, destinationType) : Mapper.Map(source, destination, sourceType, destinationType, opts);

		public static TDestination Map<TDestination>(this object source, Action<IMappingOperationOptions> opts = null) =>
			opts == default ? Mapper.Map<TDestination>(source) : Mapper.Map<TDestination>(source, opts);

		public static TDestination Map<TSource, TDestination>(this TSource source, Action<IMappingOperationOptions<TSource, TDestination>> opts = null) =>
			opts == default ? Mapper.Map<TSource, TDestination>(source) : Mapper.Map(source, opts);

		public static TDestination Map<TSource, TDestination>(this TSource source, TDestination destination, Action<IMappingOperationOptions<TSource, TDestination>> opts = null) =>
			opts == default ? Mapper.Map(source, destination) : Mapper.Map(source, destination, opts);

		public static IEnumerable<TDestination> MapAs<TDestination>(this IEnumerable<object> source, Action<IMappingOperationOptions> opts = null) =>
			source.Select(s => s.Map<TDestination>(opts));

		public static IEnumerable<TDestination> MapAs<TSource, TDestination>(this IEnumerable<TSource> source, Action<IMappingOperationOptions<TSource, TDestination>> opts = null) =>
			source.Select(s => s.Map(opts));

		public static IEnumerable<TDestination> MapArray<TDestination>(this IEnumerable<object> source, Action<IMappingOperationOptions> opts = null) =>
			source.MapAs<TDestination>(opts).ToArray();

		public static IEnumerable<TDestination> MapArray<TSource, TDestination>(this IEnumerable<TSource> source, Action<IMappingOperationOptions<TSource, TDestination>> opts = null) =>
			source.MapAs(opts).ToArray();

		public static IEnumerable<TDestination> MapList<TDestination>(this IEnumerable<object> source, Action<IMappingOperationOptions> opts = null) =>
			source.MapAs<TDestination>(opts).ToList();

		public static IEnumerable<TDestination> MapList<TSource, TDestination>(this IEnumerable<TSource> source, Action<IMappingOperationOptions<TSource, TDestination>> opts = null) =>
			source.MapAs(opts).ToList();
	}
}