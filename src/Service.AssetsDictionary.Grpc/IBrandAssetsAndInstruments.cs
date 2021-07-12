using System.ServiceModel;
using System.Threading.Tasks;
using MyJetWallet.Domain;
using Service.AssetsDictionary.Domain.Models;
using Service.AssetsDictionary.Grpc.Models;

namespace Service.AssetsDictionary.Grpc
{
    [ServiceContract]
    public interface IBrandAssetsAndInstrumentsService
    {
        [OperationContract] Task<AssetDictionaryResponse<bool>> AddAssetAsync(AssetIdBrandIdRequest request);
        [OperationContract] Task<AssetDictionaryResponse<bool>> RemoveAssetAsync(AssetIdBrandIdRequest request);
        [OperationContract] Task<AssetsListResponse> GetAllAssetsByBrandAsync(JetBrandIdentity brandId);

        [OperationContract] Task<AssetDictionaryResponse<bool>> AddSpotInstrumentAsync(SpotInstrumentIdBrandIdRequest request);
        [OperationContract] Task<AssetDictionaryResponse<bool>> RemoveSpotInstrumentAsync(SpotInstrumentIdBrandIdRequest request);
        [OperationContract] Task<SpotInstrumentsListResponse> GetAllSpotInstrumentsByBrandAsync(JetBrandIdentity brandId);
        
        [OperationContract] Task<AssetDictionaryResponse<bool>> AddMarketReferenceAsync(MarketReferenceIdBrandIdRequest request);
        [OperationContract] Task<AssetDictionaryResponse<bool>> RemoveMarketReferenceAsync(MarketReferenceIdBrandIdRequest request);
        [OperationContract] Task<MarketReferenceListResponse> GetAllMarketReferencesByBrandAsync(JetBrandIdentity brandId);

        [OperationContract] Task<AllBrandMappingResponse> GetAllBrandMappingAsync();
    }

}