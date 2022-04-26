using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.UserTokenAccount.Settings
{
	public class SettingsModel
	{
		[YamlProperty("UserTokenAccount.SeqServiceUrl")]
		public string SeqServiceUrl { get; set; }

		[YamlProperty("UserTokenAccount.ZipkinUrl")]
		public string ZipkinUrl { get; set; }

		[YamlProperty("UserTokenAccount.ElkLogs")]
		public LogElkSettings ElkLogs { get; set; }

		[YamlProperty("UserTokenAccount.PostgresConnectionString")]
		public string PostgresConnectionString { get; set; }

		[YamlProperty("UserTokenAccount.ServerKeyValueServiceUrl")]
		public string ServerKeyValueServiceUrl { get; set; }

		[YamlProperty("UserTokenAccount.ServiceBusReader")]
		public string ServiceBusReader { get; set; }

		[YamlProperty("UserTokenAccount.KeyTutorialProgressPrc")]
		public string KeyTutorialProgressPrc { get; set; }

		[YamlProperty("UserTokenAccount.TokenIncreaseValues")]
		public TokenIncreaseValues TokenIncreaseValues { get; set; }
	}

	public class TokenIncreaseValues
	{
		[YamlProperty("Register")]
		public decimal Register { get; set; }

		[YamlProperty("Daily")]
		public decimal Daily { get; set; }

		[YamlProperty("TutorialFinished80")]
		public decimal TutorialFinished80 { get; set; }

		[YamlProperty("TutorialFinished100")]
		public decimal TutorialFinished100 { get; set; }

		[YamlProperty("AchievementStandard")]
		public decimal AchievementStandard { get; set; }

		[YamlProperty("AchievementRare")]
		public decimal AchievementRare { get; set; }

		[YamlProperty("AchievementSuperRare")]
		public decimal AchievementSuperRare { get; set; }

		[YamlProperty("AchievementUltraRare")]
		public decimal AchievementUltraRare { get; set; }

		[YamlProperty("AchievementUnique")]
		public decimal AchievementUnique { get; set; }

		[YamlProperty("StatusMasterOfOpenness")]
		public decimal StatusMasterOfOpenness { get; set; }

		[YamlProperty("StatusNewbie")]
		public decimal StatusNewbie { get; set; }

		[YamlProperty("StatusSecondYearStudent")]
		public decimal StatusSecondYearStudent { get; set; }

		[YamlProperty("StatusBachelor")]
		public decimal StatusBachelor { get; set; }

		[YamlProperty("StatusMagister")]
		public decimal StatusMagister { get; set; }

		[YamlProperty("StatusAnalyst")]
		public decimal StatusAnalyst { get; set; }

		[YamlProperty("StatusStrategist")]
		public decimal StatusStrategist { get; set; }

		[YamlProperty("StatusFinancier")]
		public decimal StatusFinancier { get; set; }

		[YamlProperty("StatusInvestor")]
		public decimal StatusInvestor { get; set; }

		[YamlProperty("StatusBestFriend")]
		public decimal StatusBestFriend { get; set; }

		[YamlProperty("StatusLongLiver")]
		public decimal StatusLongLiver { get; set; }

		[YamlProperty("StatusExpert")]
		public decimal StatusExpert { get; set; }

		[YamlProperty("StatusRewarded")]
		public decimal StatusRewarded { get; set; }

		[YamlProperty("StatusKeyMaster")]
		public decimal StatusKeyMaster { get; set; }
	}
}