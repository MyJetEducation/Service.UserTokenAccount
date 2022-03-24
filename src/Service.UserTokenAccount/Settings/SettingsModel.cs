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
    }
}
