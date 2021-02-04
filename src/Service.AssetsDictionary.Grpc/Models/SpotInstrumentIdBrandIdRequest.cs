using System.Runtime.Serialization;
using MyJetWallet.Domain;
using MyJetWallet.Domain.Assets;

namespace Service.AssetsDictionary.Grpc.Models
{
    [DataContract]
    public class SpotInstrumentIdBrandIdRequest
    {
        public SpotInstrumentIdBrandIdRequest()
        {
        }

        public SpotInstrumentIdBrandIdRequest(SpotInstrumentIdentity spotInstrumentId, JetBrandIdentity brandId)
        {
            SpotInstrumentId = spotInstrumentId;
            BrandId = brandId;
        }

        [DataMember(Order = 1)] public SpotInstrumentIdentity SpotInstrumentId { get; set; }
        [DataMember(Order = 2)] public JetBrandIdentity BrandId { get; set; }
    }
}