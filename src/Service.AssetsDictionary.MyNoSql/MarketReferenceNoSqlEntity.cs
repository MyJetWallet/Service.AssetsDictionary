using MyNoSqlServer.Abstractions;
using Service.AssetsDictionary.Domain.Models;

namespace Service.AssetsDictionary.MyNoSql
{
    public class MarketReferenceNoSqlEntity : MyNoSqlDbEntity, IMarketReference
    {
        public const string TableName = "myjetwallet-dictionary-market-references";

        public static string GeneratePartitionKey(string brokerId) => $"broker:{brokerId}";
        public static string GenerateRowKey(string instrumentSymbol) => instrumentSymbol;

        public static MarketReferenceNoSqlEntity Create(IMarketReference instrument)
        {
            return new MarketReferenceNoSqlEntity()
            {
                PartitionKey = GeneratePartitionKey(instrument.BrokerId),
                RowKey = GenerateRowKey(instrument.Id),
                BrokerId = instrument.BrokerId,
                Id = instrument.Id,
                Name = instrument.Name,
                IconUrl = instrument.IconUrl,
                AssociateAsset = instrument.AssociateAsset,
                AssociateAssetPair = instrument.AssociateAssetPair,
                Weight = instrument.Weight,
            };
        }
        
        public string Id { get; set; }
        public string BrokerId { get; set; }
        public string Name { get; set; }
        public string IconUrl { get; set; }
        public string AssociateAsset { get; set; }
        public string AssociateAssetPair { get; set; }
        public int Weight { get; set; }
        public MarketReferenceNoSqlEntity Apply(IMarketReference instrument)
        {
            Id = instrument.Id;
            Name = instrument.Name;
            IconUrl = instrument.IconUrl;
            AssociateAsset = instrument.AssociateAsset;
            AssociateAssetPair = instrument.AssociateAssetPair;
            Weight = instrument.Weight;

            return this;
        }
    }
}