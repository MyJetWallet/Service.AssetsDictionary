using System.Collections.Generic;
using System.Linq;
using MyNoSqlServer.Abstractions;
using Service.AssetsDictionary.Domain.Models;

namespace Service.AssetsDictionary.MyNoSql
{
    public class BrandAssetsAndInstrumentsNoSqlEntity : MyNoSqlDbEntity, IBrandAssetsAndInstruments
    {
        public const string TableName = "myjetwallet-dictionary-brand-asset-and-spot-instruments";

        public static string GeneratePartitionKey(string brokerId) => $"broker:{brokerId}";
        public static string GenerateRowKey(string brandId) => brandId;

        public static BrandAssetsAndInstrumentsNoSqlEntity Create(IBrandAssetsAndInstruments item)
        {
            return new BrandAssetsAndInstrumentsNoSqlEntity()
            {
                PartitionKey = GeneratePartitionKey(item.BrokerId),
                RowKey = GenerateRowKey(item.BrandId),
                BrokerId = item.BrokerId,
                BrandId = item.BrandId,
            }.Apply(item);
        }

        public string BrokerId { get; set; }
        public string BrandId { get; set; }
        public List<string> AssetSymbolsList { get; set; }
        public List<string> SpotInstrumentSymbolsList { get; set; }

        IReadOnlyList<string> IBrandAssetsAndInstruments.AssetSymbolsList => AssetSymbolsList;

        IReadOnlyList<string> IBrandAssetsAndInstruments.SpotInstrumentSymbolsList => SpotInstrumentSymbolsList;

        public BrandAssetsAndInstrumentsNoSqlEntity Apply(IBrandAssetsAndInstruments data)
        {
            AssetSymbolsList = data.AssetSymbolsList.ToList();
            SpotInstrumentSymbolsList = data.AssetSymbolsList.ToList();
            return this;
        }
    }
}