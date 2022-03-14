using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.PaymentDeposit.Settings
{
    public class SettingsModel
    {
        [YamlProperty("PaymentDeposit.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("PaymentDeposit.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("PaymentDeposit.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }

        [YamlProperty("PaymentDeposit.PaymentDepositRepositoryServiceUrl")]
        public string PaymentDepositRepositoryServiceUrl { get; set; }

        [YamlProperty("PaymentDeposit.PaymentProviderRouterServiceUrl")]
        public string PaymentProviderRouterServiceUrl { get; set; }
    }
}
