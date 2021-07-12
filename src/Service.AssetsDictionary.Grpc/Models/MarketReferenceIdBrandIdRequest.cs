using System.Runtime.Serialization;
using MyJetWallet.Domain;
using MyJetWallet.Domain.Assets;

namespace Service.AssetsDictionary.Grpc.Models
{
    [DataContract]
    public class MarketReferenceIdBrandIdRequest
    {
        public MarketReferenceIdBrandIdRequest()
        {
        }

        public MarketReferenceIdBrandIdRequest(MarketReferenceIdentity referenceId, JetBrandIdentity brandId)
        {
            ReferenceId = referenceId;
            BrandId = brandId;
        }

        [DataMember(Order = 1)] public MarketReferenceIdentity ReferenceId { get; set; }
        [DataMember(Order = 2)] public JetBrandIdentity BrandId { get; set; }
    }
}