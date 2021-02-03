using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using MyJetWallet.Domain;
using MyJetWallet.Domain.Assets;

namespace Service.AssetsDictionary.Domain.Models
{
    public interface IAsset: IAssetIdentity
    {
        string Description { get; }

        int Accuracy { get; }

        bool IsEnabled { get; }

        string MatchingEngineId { get; }
    }

    [DataContract]
    public class Asset : IAsset
    {
        [DataMember(Order = 1)] public string BrokerId { get; set; }
        [DataMember(Order = 2)] public string Symbol { get; set; }
        [DataMember(Order = 3)] public string Description { get; set; }
        [DataMember(Order = 4)] public int Accuracy { get; set; }
        [DataMember(Order = 5)] public bool IsEnabled { get; set; }
        [DataMember(Order = 6)] public string MatchingEngineId { get; set; }

        public static Asset Create(IAsset asset)
        {
            return new Asset().Apply(asset);
        }

        public Asset Apply(IAsset asset)
        {
            BrokerId = asset.BrokerId;
            Symbol = asset.Symbol;
            Description = asset.Description;
            Accuracy = asset.Accuracy;
            IsEnabled = asset.IsEnabled;
            MatchingEngineId = asset.MatchingEngineId;
            return this;
        }
    }
}