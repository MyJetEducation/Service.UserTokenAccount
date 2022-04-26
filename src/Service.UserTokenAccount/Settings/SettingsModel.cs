using System;
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

		[YamlProperty("UserTokenAccount.KeyUserLoginInfo")]
		public string KeyUserLoginInfo { get; set; }

		[YamlProperty("UserTokenAccount.TokenIncreaseValues")]
		public TokenIncreaseValues TokenIncreaseValues { get; set; }
	}

	public class TokenIncreaseValues
	{
		[YamlProperty("Register")]
		public int Register { get; set; }

		[YamlProperty("Daily")]
		public int Daily { get; set; }

		[YamlProperty("TutorialFinished80")]
		public int TutorialFinished80 { get; set; }

		[YamlProperty("TutorialFinished100")]
		public int TutorialFinished100 { get; set; }

		[YamlProperty("AchievementStandard")]
		public int AchievementStandard { get; set; }

		[YamlProperty("AchievementRare")]
		public int AchievementRare { get; set; }

		[YamlProperty("AchievementSuperRare")]
		public int AchievementSuperRare { get; set; }

		[YamlProperty("AchievementUltraRare")]
		public int AchievementUltraRare { get; set; }

		[YamlProperty("AchievementUnique")]
		public int AchievementUnique { get; set; }

		[YamlProperty("StatusMasterOfOpenness")]
		public int StatusMasterOfOpenness { get; set; }

		[YamlProperty("StatusNewbie")]
		public int StatusNewbie { get; set; }

		[YamlProperty("StatusSecondYearStudent")]
		public int StatusSecondYearStudent { get; set; }

		[YamlProperty("StatusBachelor")]
		public int StatusBachelor { get; set; }

		[YamlProperty("StatusMagister")]
		public int StatusMagister { get; set; }

		[YamlProperty("StatusAnalyst")]
		public int StatusAnalyst { get; set; }

		[YamlProperty("StatusStrategist")]
		public int StatusStrategist { get; set; }

		[YamlProperty("StatusFinancier")]
		public int StatusFinancier { get; set; }

		[YamlProperty("StatusInvestor")]
		public int StatusInvestor { get; set; }

		[YamlProperty("StatusBestFriend")]
		public int StatusBestFriend { get; set; }

		[YamlProperty("StatusLongLiver")]
		public int StatusLongLiver { get; set; }

		[YamlProperty("StatusExpert")]
		public int StatusExpert { get; set; }

		[YamlProperty("StatusRewarded")]
		public int StatusRewarded { get; set; }

		[YamlProperty("StatusKeyMaster")]
		public int StatusKeyMaster { get; set; }
	}
}