#if !TARGET_NET_STANDARD_2_0
using System.Configuration;

namespace Lippert.Core.Configuration.Extensions
{
	/// <summary>
	/// https://offbyoneerrors.wordpress.com/2015/10/16/writing-a-configuration-property-aspect-with-postsharp/
	/// </summary>
	public static class ConfigurationElementExtensions
	{
		public static T OnlyIfPresent<T>(this T element)
			where T : ConfigurationElement => element.ElementInformation.IsPresent ? element : null;
	}
}
#endif