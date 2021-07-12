using System.ServiceModel;
using System.Threading.Tasks;
using MyJetWallet.Domain;
using MyJetWallet.Domain.Assets;
using Service.AssetsDictionary.Domain.Models;
using Service.AssetsDictionary.Grpc.Models;

namespace Service.AssetsDictionary.Grpc
{
    [ServiceContract]
    public interface IMarketReferencesDictionaryService
    {
        [OperationContract] Task<AssetDictionaryResponse<MarketReference>> CreateMarketReferenceAsync(MarketReference reference);
        [OperationContract] Task<AssetDictionaryResponse<MarketReference>> UpdateMarketReferenceAsync(MarketReference reference);
        [OperationContract] Task<AssetDictionaryResponse<MarketReference>> DeleteMarketReferenceAsync(MarketReference reference);

        [OperationContract] Task<NullableValue<MarketReference>> GetMarketReferenceByIdAsync(MarketReferenceIdentity identity);
        [OperationContract] Task<MarketReferenceListResponse> GetMarketReferencesByBrokerAsync(JetBrokerIdentity brokerId);
        [OperationContract] Task<MarketReferenceListResponse> GetAllMarketReferencesAsync();
    }
}