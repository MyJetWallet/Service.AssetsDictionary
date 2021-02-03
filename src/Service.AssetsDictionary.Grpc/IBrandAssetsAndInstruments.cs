using System.ServiceModel;
using System.Threading.Tasks;
using MyJetWallet.Domain;
using MyJetWallet.Domain.Assets;
using Service.AssetsDictionary.Grpc.Models;

namespace Service.AssetsDictionary.Grpc
{
    [ServiceContract]
    public interface IBrandAssetsAndInstruments
    {
        [OperationContract] Task AddAssetAsync(AssetIdentity assetId, JetBrandIdentity brandId);
        [OperationContract] Task RemoveAssetAsync(AssetIdentity assetId, JetBrandIdentity brandId);
        [OperationContract] Task<AssetsListResponse> GetAllAssetsByBrandAsync(JetBrandIdentity brandId);

        [OperationContract] Task AddSpotInstrumentAsync(SpotInstrumentIdentity spotInstrumentId, JetBrandIdentity brandId);
        [OperationContract] Task RemoveSpotInstrumentAsync(SpotInstrumentIdentity spotInstrumentId, JetBrandIdentity brandId);
        [OperationContract] Task<SpotInstrumentIdentity> GetAllSpotInstrumentsByBrandAsync(JetBrandIdentity brandId);
    }
}