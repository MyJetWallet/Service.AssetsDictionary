using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using MyJetWallet.Domain;
using MyJetWallet.Domain.Assets;
using Service.AssetsDictionary.Domain.Models;
using Service.AssetsDictionary.Grpc.Models;

namespace Service.AssetsDictionary.Grpc
{
    [ServiceContract]
    public interface IAssetsDictionaryService
    {
        [OperationContract] ValueTask<AssetDictionaryResponse<Asset>> CreateAssetAsync(Asset asset);
        [OperationContract] ValueTask<AssetDictionaryResponse<Asset>> UpdateAssetAsync(Asset asset);
        [OperationContract] ValueTask<AssetDictionaryResponse<Asset>> DeleteAssetAsync(Asset asset);

        [OperationContract] ValueTask<NullableValue<Asset>> GetAssetByIdAsync(AssetIdentity assetId);
        [OperationContract] ValueTask<AssetsListResponse> GetAssetsByBrokerAsync(JetBrokerIdentity brokerId);
        [OperationContract] ValueTask<AssetsListResponse> GetAllAssetsAsync();
    }
}