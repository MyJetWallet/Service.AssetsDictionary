using System;
using MyNoSqlServer.Abstractions;
using Service.AssetsDictionary.Domain.Models;

namespace Service.AssetsDictionary.MyNoSql
{
    public class AssetNoSqlEntity: MyNoSqlDbEntity, IAsset
    {
        public const string TableName = "myjetwallet.dictionary.assets";

        public static string GeneratePartitionKey(string brokerId) => $"broker:{brokerId}";
        public static string GenerateRowKey(string assetSymbol) => assetSymbol;

        public static AssetNoSqlEntity Create(IAsset asset)
        {
            return new AssetNoSqlEntity()
            {
                PartitionKey = GeneratePartitionKey(asset.BrokerId),
                RowKey = GenerateRowKey(asset.Symbol),
                BrokerId = asset.BrokerId,
                Symbol = asset.Symbol
            }.Apply(asset);
        }

        public string BrokerId { get; set; }
        public string Symbol { get; set; }
        public string Description { get; set; }
        public int Accuracy { get; set; }
        public bool IsEnabled { get; set; }
        public string MatchingEngineId { get; set; }


        public AssetNoSqlEntity Apply(IAsset asset)
        {
            Description = asset.Description;
            Accuracy = asset.Accuracy;
            IsEnabled = asset.IsEnabled;
            MatchingEngineId = asset.MatchingEngineId;

            return this;
        }
    }
}
