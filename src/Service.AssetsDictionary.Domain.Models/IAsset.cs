using System.Runtime.Serialization;
using MyJetWallet.Domain.Assets;

namespace Service.AssetsDictionary.Domain.Models
{
    public interface IAsset : IAssetIdentity
    {
        string Description { get; }
        int Accuracy { get; }
        bool IsEnabled { get; }
        string MatchingEngineId { get; }
        bool KycRequiredForDeposit { get; }
        bool KycRequiredForWithdrawal { get; }
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
        [DataMember(Order = 7)] public bool KycRequiredForDeposit { get; set; }
        [DataMember(Order = 8)] public bool KycRequiredForWithdrawal { get; set; }

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
            KycRequiredForDeposit = asset.KycRequiredForDeposit;
            KycRequiredForWithdrawal = asset.KycRequiredForWithdrawal;
            return this;
        }
    }
}