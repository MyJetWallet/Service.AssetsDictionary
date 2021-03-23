using MyJetWallet.Domain.Assets;
using MyNoSqlServer.Abstractions;
using Service.AssetsDictionary.Domain.Models;

namespace Service.AssetsDictionary.MyNoSql
{
    public class AssetPaymentSettingsNoSqlEntity : MyNoSqlDbEntity, IAssetIdentity
    {
        public const string TableName = "myjetwallet-dictionary-assets-payment-settings";

        public static string GeneratePartitionKey(string brokerId) => $"broker:{brokerId}";
        public static string GenerateRowKey(string assetSymbol) => assetSymbol;

        public static AssetPaymentSettingsNoSqlEntity Create(IAssetIdentity asset, AssetPaymentSettings paymentSettings)
        {
            return new AssetPaymentSettingsNoSqlEntity()
            {
                PartitionKey = GeneratePartitionKey(asset.BrokerId),
                RowKey = GenerateRowKey(asset.Symbol),
                BrokerId = asset.BrokerId,
                Symbol = asset.Symbol,

                PaymentSettings = paymentSettings
            };
        }

        public string BrokerId { get; set; }
        public string Symbol { get; set; }

        public AssetPaymentSettings PaymentSettings { get; set; }
    }
}