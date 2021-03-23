using System.Runtime.Serialization;

namespace Service.AssetsDictionary.Domain.Models
{
    [DataContract]
    public class AssetPaymentSettings
    {
        [DataMember(Order = 1)] public string AssetSymbol { get; set; }

        [DataMember(Order = 2)] public PciDssSettings PciDss { get; set; }

        [DataMember(Order = 3)] public BitGoCryptoSettings BitGoCrypto { get; set; }
        
        [DataContract]
        public class BitGoCryptoSettings
        {
            [DataMember(Order = 1)] public bool IsEnabledDeposit { get; set; }
            [DataMember(Order = 2)] public bool IsEnabledWithdrawal { get; set; }

            [DataMember(Order = 3)] public double MinWithdrawalAmount { get; set; }
        }
        
        [DataContract]
        public class PciDssSettings
        {
            [DataMember(Order = 1)] public bool IsEnabledDeposit { get; set; }
        }

        public enum PaymentProcessor
        {
            PciDss=1,
            BitGo = 2,
            SwiftTransfer = 3
        }

        public AssetPaymentSettings Clone()
        {
            var settings = new AssetPaymentSettings();

            settings.AssetSymbol = AssetSymbol;

            if (PciDss != null)
            {
                settings.PciDss = new PciDssSettings()
                {
                    IsEnabledDeposit = PciDss.IsEnabledDeposit
                };
            }

            if (BitGoCrypto != null)
            {
                settings.BitGoCrypto = new BitGoCryptoSettings()
                {
                    IsEnabledDeposit = BitGoCrypto.IsEnabledDeposit,
                    IsEnabledWithdrawal = BitGoCrypto.IsEnabledWithdrawal,
                    MinWithdrawalAmount = BitGoCrypto.MinWithdrawalAmount
                };
            }

            return settings;
        }
    }
}