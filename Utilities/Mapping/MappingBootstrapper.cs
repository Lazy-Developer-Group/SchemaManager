using System;
using System.Linq;
using System.Reflection;
using AutoMapper;

namespace Utilities.Mapping
{
	public static class MappingBootstrapper
	{
		public static void Bootstrap(Assembly targetAssembly)
		{		
			Mapper.Initialize(config => LoadAllMappingClasses(targetAssembly, config));
		}

		private static void LoadAllMappingClasses(Assembly assembly, IConfiguration mappingConfig)
		{
			var maps = from type in assembly.GetTypes()
			           where !type.IsAbstract && !type.IsInterface
			           where typeof (IMap).IsAssignableFrom(type)
			           select (IMap) Activator.CreateInstance(type);

			foreach (var map in maps)
			{
				map.CreateMappings(mappingConfig);
			}
		}
	}
}