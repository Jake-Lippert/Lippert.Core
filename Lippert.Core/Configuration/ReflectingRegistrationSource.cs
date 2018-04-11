using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lippert.Core.Configuration
{
	/// <summary>
	/// https://offbyoneerrors.wordpress.com/2018/01/06/reflecting-through-your-codebase-for-dependencies-and-other-registrations/
	/// </summary>
	public static class ReflectingRegistrationSource
	{
		public static string CodebaseNamespacePrefix { get; set; }

		/// <summary>
		/// Does this assembly's name match the pattern of assemblies within our codebase?
		/// </summary>
		private static bool IsCodebaseAssembly(Assembly assembly) => assembly.GetName().Name.StartsWith($"{CodebaseNamespacePrefix}.");

		/// <summary>
		/// Finds all of the assemblies belonging to our codebase.
		/// </summary>
		/// <remarks>
		/// If a starting assembly is not provided, the current AppDomain's assemblies will be used as starting points for recursive calls.
		/// </remarks>
		public static List<Assembly> GetAllCodebaseAssemblies(Assembly parent = null) =>
			(parent?.GetReferencedAssemblies().Select(Assembly.Load) ?? AppDomain.CurrentDomain.GetAssemblies())
				.Where(IsCodebaseAssembly)
				.SelectMany(GetAllCodebaseAssemblies)
				.Concat(parent == null ? Enumerable.Empty<Assembly>() : new[] { parent })
				.Where(IsCodebaseAssembly)
				.Distinct()
				.ToList();

		/// <summary>
		/// Builds a list of classes in our codebase and their interfaces where there is only a single implementation
		/// </summary>
		public static List<(TypeInfo Class, List<Type> ImplementedInterfaces)> GetCodebaseDependencies()
		{
			var classes = GetAllCodebaseAssemblies()
				.SelectMany(a => a.DefinedTypes)
				.Where(c => c.IsClass && !c.IsAbstract)
				.ToList();

			return classes.Select(@class =>
			(
				Class: @class,
				Interfaces: @class.GetInterfaces()
					.Where(i => IsCodebaseAssembly(i.Assembly) && classes.Count(i.IsAssignableFrom) == 1)
					.ToList()
			)).Where(x => x.Interfaces.Any()).ToList();
		}

		/// <summary>
		/// Builds a list of the types in our codebase that can be assigned to the specified type.
		/// </summary>
		public static List<TypeInfo> GetCodebaseTypesAssignableTo<T>() => GetCodebaseTypesAssignableTo(typeof(T));
		/// <summary>
		/// Builds a list of the types in our codebase that can be assigned to the specified type.
		/// </summary>
		public static List<TypeInfo> GetCodebaseTypesAssignableTo(Type targetType) => GetAllCodebaseAssemblies()
			.SelectMany(a => a.DefinedTypes)
			.Where(targetType.IsAssignableFrom)
			.ToList();
	}
}