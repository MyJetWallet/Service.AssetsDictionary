using System.Runtime.Serialization;
using MyJetWallet.Domain;
using MyJetWallet.Domain.Assets;

namespace Service.AssetsDictionary.Domain.Models
{
    public interface IMarketReference : IMarketReferenceIdentity
    {
        string Id { get; }
        string BrokerId { get; }
        string Name { get; }
        string IconUrl { get; }
        string AssociateAsset { get; }
        string AssociateAssetPair { get; }
        int Weight { get; }
    }
    
    [DataContract]
    public class MarketReference : IMarketReference
    {
        [DataMember(Order = 1)]public string Id { get; set; }
        [DataMember(Order = 2)]public string BrokerId { get; set; }
        [DataMember(Order = 3)]public string Name { get; set; }
        [DataMember(Order = 4)]public string IconUrl { get; set; }
        [DataMember(Order = 5)]public string AssociateAsset { get; set; }
        [DataMember(Order = 6)]public string AssociateAssetPair { get; set; }
        [DataMember(Order = 7)]public int Weight { get; set; }

        public static MarketReference Create(IMarketReference reference)
        {
            return new MarketReference()
            {
                Id = reference.Id,
                AssociateAsset = reference.Id,
                AssociateAssetPair = reference.AssociateAssetPair,
                BrokerId = reference.BrokerId,
                IconUrl = reference.IconUrl,
                Name = reference.Name,
                Weight = reference.Weight
            };
        }

    }
    

}