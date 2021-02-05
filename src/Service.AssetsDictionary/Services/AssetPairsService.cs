using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MyJetWallet.Domain.Assets;
using Service.AssetsDictionary.Grpc;
using Service.AssetsDictionary.Services.Proto;

namespace Service.AssetsDictionary.Services
{
    public class AssetPairsService: AssetPairs.AssetPairsBase
    {
        private readonly ISpotInstrumentsDictionaryService _service;

        public AssetPairsService(ISpotInstrumentsDictionaryService service)
        {
            _service = service;
        }

        public override async Task<GetAllAssetPairsResponse> GetAll(Empty request, ServerCallContext context)
        {
            var instruments = await _service.GetAllSpotInstrumentsAsync();

            var response = new GetAllAssetPairsResponse();
            response.AssetPairs.AddRange(instruments.SpotInstruments.Select(e => new AssetPair()
            {
                BrokerId = e.BrokerId,
                Symbol = e.Symbol,
                Accuracy = e.Accuracy,
                BaseAsset = e.BaseAsset,
                QuotingAsset = e.QuoteAsset,
                MaxOppositeVolume = e.MaxOppositeVolume.ToString(CultureInfo.InvariantCulture),
                MaxVolume = e.MaxVolume.ToString(CultureInfo.InvariantCulture),
                MinVolume = e.MinVolume.ToString(CultureInfo.InvariantCulture),
                MarketOrderPriceThreshold = e.MarketOrderPriceThreshold.ToString(CultureInfo.InvariantCulture)
            }));

            return response;
        }

        public override async Task<GetAssetPairBySymbolResponse> GetBySymbol(GetAssetPairBySymbolRequest request, ServerCallContext context)
        {
            var instruments = await _service.GetSpotInstrumentByIdAsync(new SpotInstrumentIdentity()
            {
                BrokerId = request.BrokerId,
                Symbol = request.Symbol
            });

            if (!instruments.HasValue())
                return new GetAssetPairBySymbolResponse();

            return new GetAssetPairBySymbolResponse()
            {
                AssetPair = new AssetPair()
                {
                    BrokerId = instruments.Value.BrokerId,
                    Symbol = instruments.Value.Symbol,
                    Accuracy = instruments.Value.Accuracy,
                    BaseAsset = instruments.Value.BaseAsset,
                    QuotingAsset = instruments.Value.QuoteAsset,
                    MaxOppositeVolume = instruments.Value.MaxOppositeVolume.ToString(CultureInfo.InvariantCulture),
                    MaxVolume = instruments.Value.MaxVolume.ToString(CultureInfo.InvariantCulture),
                    MinVolume = instruments.Value.MinVolume.ToString(CultureInfo.InvariantCulture),
                    MarketOrderPriceThreshold = instruments.Value.MarketOrderPriceThreshold.ToString(CultureInfo.InvariantCulture)
                }
            };
        }
    }
}