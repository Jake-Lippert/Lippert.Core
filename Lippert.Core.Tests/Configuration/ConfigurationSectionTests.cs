#if !TARGET_NET_CORE_APP_2_0
using System.Configuration;
using Lippert.Core.Configuration;
using Lippert.Core.Configuration.Extensions;
using NUnit.Framework;

namespace Lippert.Core.Tests.Configuration
{
	[TestFixture]
	public class ConfigurationSectionTests
	{
		[Test]
		public void TestLoadsProjectConfigSection()
		{
			//--Act
			var config = Config.Instance;

			//--Assert
			Assert.IsNotNull(config);
			Assert.AreEqual("NoReply@example.com", config.FromAddress);
			Assert.AreEqual(string.Empty, config.AddressOverride);
			Assert.AreEqual(60, config.ElementA.Timeout);
			Assert.IsNull(config.ElementB);
		}
	}

	public class Config : ConfigurationSectionBase<Config>
	{
		[ConfigurationProperty(nameof(ElementA), IsRequired = true)]
		public ConfigElementA ElementA => (ConfigElementA)base[nameof(ElementA)];

		/// <summary>
		/// Avoids System.Configuration.ConfigurationErrorsException: The property 'ElementB' must not return null from the property's get method.  Typically the getter should return base["ElementB"].
		/// </summary>
		[ConfigurationProperty(nameof(ElementB), IsRequired = false)]
		private ConfigElementB ElementBPrivate => (ConfigElementB)base[nameof(ElementB)];
		public ConfigElementB? ElementB => ElementBPrivate.OnlyIfPresent();

		[ConfigurationProperty(nameof(FromAddress), IsRequired = true)]
		public string FromAddress => (string)base[nameof(FromAddress)];

		[ConfigurationProperty(nameof(AddressOverride), IsRequired = false)]
		public string AddressOverride => (string)base[nameof(AddressOverride)];
	}
	public class ConfigElementA : ConfigurationElement
	{
		[ConfigurationProperty(nameof(Timeout), IsRequired = true)]
		public int Timeout => (int)base[nameof(Timeout)];
	}
	public class ConfigElementB : ConfigurationElement
	{
		[ConfigurationProperty(nameof(EnableFeature), IsRequired = true)]
		public bool EnableFeature => (bool)base[nameof(EnableFeature)];
	}
}
#endif