using System;
using AutoMapper;

namespace Lippert.Core.Configuration
{
	public static class AutoMapperProfileInitializer
	{
		public static void Initialize() => Mapper.Initialize(mce =>
			ReflectingRegistrationSource.GetCodebaseTypesAssignableTo<Profile>()
				.ForEach(pt => mce.AddProfile((Profile)Activator.CreateInstance(pt))));
	}
}