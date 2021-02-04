using System.ServiceModel;
using System.Threading.Tasks;
using MyJetWallet.Domain;
using Service.AssetsDictionary.Grpc.Models;

namespace Service.AssetsDictionary.Grpc
{
    [ServiceContract]
    public interface IBrandAssetsAndInstrumentsService
    {
        [OperationContract] Task AddAssetAsync(AssetIdBrandIdRequest request);
        [OperationContract] Task RemoveAssetAsync(AssetIdBrandIdRequest request);
        [OperationContract] Task<AssetsListResponse> GetAllAssetsByBrandAsync(JetBrandIdentity brandId);

        [OperationContract] Task AddSpotInstrumentAsync(SpotInstrumentIdBrandIdRequest request);
        [OperationContract] Task RemoveSpotInstrumentAsync(SpotInstrumentIdBrandIdRequest request);
        [OperationContract] Task<SpotInstrumentsListResponse> GetAllSpotInstrumentsByBrandAsync(JetBrandIdentity brandId);
    }

}