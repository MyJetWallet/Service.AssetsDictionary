using MyNoSqlServer.Abstractions;
using Service.AssetsDictionary.Domain.Models;

namespace Service.AssetsDictionary.MyNoSql
{
    public class SpotInstrumentNoSqlEntity : MyNoSqlDbEntity, ISpotInstrument
    {
        public const string TableName = "myjetwallet-dictionary-spot-instruments";

        public static string GeneratePartitionKey(string brokerId) => $"broker:{brokerId}";
        public static string GenerateRowKey(string instrumentSymbol) => instrumentSymbol;

        public static SpotInstrumentNoSqlEntity Create(ISpotInstrument instrument)
        {
            return new SpotInstrumentNoSqlEntity()
            {
                PartitionKey = GeneratePartitionKey(instrument.BrokerId),
                RowKey = GenerateRowKey(instrument.Symbol),
                BrokerId = instrument.BrokerId,
                Symbol = instrument.Symbol,
            }.Apply(instrument);
        }

        public string BrokerId { get; set; }
        public string Symbol { get; set; }
        public string BaseAsset { get; set; }
        public string QuoteAsset { get; set; }
        public int Accuracy { get; set; }
        public decimal MinVolume { get; set; }
        public decimal MaxVolume { get; set; }
        public decimal MaxOppositeVolume { get; set; }
        public decimal MarketOrderPriceThreshold { get; set; }
        public bool IsEnabled { get; set; }
        public string MatchingEngineId { get; set; }
        public bool KycRequiredForTrade { get; set; }
        public string IconUrl { get; set; }

        public SpotInstrumentNoSqlEntity Apply(ISpotInstrument instrument)
        {
            Accuracy = instrument.Accuracy;
            BaseAsset = instrument.BaseAsset;
            QuoteAsset = instrument.QuoteAsset;
            IsEnabled = instrument.IsEnabled;
            MarketOrderPriceThreshold = instrument.MarketOrderPriceThreshold;
            MaxOppositeVolume = instrument.MaxOppositeVolume;
            MaxVolume = instrument.MaxVolume;
            MinVolume = instrument.MinVolume;
            MatchingEngineId = instrument.MatchingEngineId;
            KycRequiredForTrade = instrument.KycRequiredForTrade;
            IconUrl = instrument.IconUrl;

            return this;
        }
    }
}