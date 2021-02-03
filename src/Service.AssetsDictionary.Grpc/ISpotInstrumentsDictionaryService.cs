using System.ServiceModel;
using System.Threading.Tasks;
using MyJetWallet.Domain;
using MyJetWallet.Domain.Assets;
using Service.AssetsDictionary.Domain.Models;
using Service.AssetsDictionary.Grpc.Models;

namespace Service.AssetsDictionary.Grpc
{
    [ServiceContract]
    public interface ISpotInstrumentsDictionaryService
    {
        [OperationContract] Task<SpotInstrument> CreateSpotInstrumentAsync(SpotInstrument instrument);
        [OperationContract] Task<SpotInstrument> UpdateSpotInstrumentAsync(SpotInstrument instrument);

        [OperationContract] Task<NullableValue<SpotInstrument>> GetSpotInstrumentByIdAsync(SpotInstrumentIdentity assetId);
        [OperationContract] Task<SpotInstrumentsListResponse> GetSpotInstrumentsByBrokerAsync(JetBrokerIdentity brokerId);
        [OperationContract] Task<SpotInstrumentsListResponse> GetSpotInstrumentsByBrandAsync(JetBrandIdentity brandId);
        [OperationContract] Task<SpotInstrumentsListResponse> GetAllSpotInstrumentsAsync();
    }
}