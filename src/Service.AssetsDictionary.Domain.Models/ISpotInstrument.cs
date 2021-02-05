using System.Runtime.Serialization;
using MyJetWallet.Domain;
using MyJetWallet.Domain.Assets;

namespace Service.AssetsDictionary.Domain.Models
{
    public interface ISpotInstrument: ISpotInstrumentIdentity
    {
        string BaseAsset { get; }
        
        string QuoteAsset { get; }
        
        int Accuracy { get; }
        
        decimal MinVolume { get; }
        
        decimal MaxVolume { get; }
        
        decimal MaxOppositeVolume { get; }
        
        decimal MarketOrderPriceThreshold { get; }

        bool IsEnabled { get; }

        string MatchingEngineId { get; }
    }

    [DataContract]
    public class SpotInstrument : ISpotInstrument
    {
        [DataMember(Order = 1)] public string BrokerId { get; set; }
        [DataMember(Order = 2)] public string Symbol { get; set; }
        [DataMember(Order = 3)] public string BaseAsset { get; set; }
        [DataMember(Order = 4)] public string QuoteAsset { get; set; }
        [DataMember(Order = 5)] public int Accuracy { get; set; }
        [DataMember(Order = 6)] public decimal MinVolume { get; set; }
        [DataMember(Order = 7)] public decimal MaxVolume { get; set; }
        [DataMember(Order = 8)] public decimal MaxOppositeVolume { get; set; }
        [DataMember(Order = 9)] public decimal MarketOrderPriceThreshold { get; set; }
        [DataMember(Order = 10)] public bool IsEnabled { get; set; }
        [DataMember(Order = 11)] public string MatchingEngineId { get; set; }

        public static SpotInstrument Create(ISpotInstrument instrument)
        {
            return new SpotInstrument()
            {
                BrokerId = instrument.BrokerId,
                Symbol = instrument.Symbol,
                BaseAsset = instrument.BaseAsset,
                QuoteAsset = instrument.QuoteAsset,
                Accuracy = instrument.Accuracy,
                MaxVolume = instrument.MaxVolume,
                MinVolume = instrument.MinVolume,
                MaxOppositeVolume = instrument.MaxOppositeVolume,
                MarketOrderPriceThreshold = instrument.MarketOrderPriceThreshold,
                MatchingEngineId = instrument.MatchingEngineId,
                IsEnabled = instrument.IsEnabled
            };
        }
    }
}