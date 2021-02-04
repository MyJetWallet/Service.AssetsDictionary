using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using MyJetWallet.Domain;
using MyJetWallet.Domain.Assets;

namespace Service.AssetsDictionary.Grpc.Models
{
    [DataContract]
    public class AssetIdBrandIdRequest
    {
        public AssetIdBrandIdRequest()
        {
        }

        public AssetIdBrandIdRequest(AssetIdentity assetId, JetBrandIdentity brandId)
        {
            AssetId = assetId;
            BrandId = brandId;
        }

        [DataMember(Order = 1)] public AssetIdentity AssetId { get; set; }
        [DataMember(Order = 2)] public JetBrandIdentity BrandId { get; set; }
    }
}