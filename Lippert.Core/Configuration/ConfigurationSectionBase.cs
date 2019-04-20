#if !TARGET_NET_STANDARD_2_0
using System.Configuration;

namespace Lippert.Core.Configuration
{
	/// <summary>
	/// https://offbyoneerrors.wordpress.com/2015/10/16/writing-a-configuration-property-aspect-with-postsharp/
	/// </summary>
	/// <typeparam name="TSection">The type of the inheriting configuration section</typeparam>
	public abstract class ConfigurationSectionBase<TSection> : ConfigurationSection
		where TSection : ConfigurationSectionBase<TSection>
	{
		private static readonly string SectionName = typeof(TSection).Assembly.GetName().Name;

		public static TSection Instance
		{
			get
			{
				if (ConfigurationManager.GetSection(SectionName) is TSection section)
				{
					ProcessMissingElements(section, SectionName);
					return section;
				}

				throw new System.InvalidOperationException($"No config section could be found for the name '{SectionName}'.");
			}
		}

		/// <summary>
		/// Based on: http://stackoverflow.com/a/2492170
		/// </summary>
		/// <param name="element"></param>
		private static void ProcessMissingElements(ConfigurationElement element, string path)
		{
			foreach (PropertyInformation propertyInformation in element.ElementInformation.Properties)
			{
				if (propertyInformation.Value is ConfigurationElement complexProperty)
				{
					if (propertyInformation.IsRequired && !complexProperty.ElementInformation.IsPresent)
					{
						throw new ConfigurationErrorsException($"ConfigProperty: '{path}.{propertyInformation.Name}' is required but not present.");
					}

					if (complexProperty.ElementInformation.IsPresent)
					{
						ProcessMissingElements(complexProperty, $"{path}.{propertyInformation.Name}");
					}
				}
			}
		}
	}
}
#endif