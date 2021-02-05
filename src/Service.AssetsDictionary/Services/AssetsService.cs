using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MyJetWallet.Domain.Assets;
using Service.AssetsDictionary.Grpc;
using Service.AssetsDictionary.Services.Proto;

namespace Service.AssetsDictionary.Services
{
    public class AssetsService: Assets.AssetsBase
    {
        private readonly IAssetsDictionaryService _service;

        public AssetsService(IAssetsDictionaryService service)
        {
            _service = service;
        }

        public override async Task<GetAllAssetsResponse> GetAll(Empty request, ServerCallContext context)
        {
            var assets = await _service.GetAllAssetsAsync();

            var response = new GetAllAssetsResponse();

            response.Assets.AddRange(assets.Assets.Select(e => new Asset()
            {
                BrokerId = e.BrokerId,
                Symbol = e.Symbol,
                Accuracy = e.Accuracy,
                Description = e.Description
            }));
            return response;
        }

        public override async Task<GetAssetBySymbolResponse> GetBySymbol(GetAssetBySymbolRequest request, ServerCallContext context)
        {
            var asset = await _service.GetAssetByIdAsync(new AssetIdentity()
            {
                BrokerId = request.BrokerId,
                Symbol = request.Symbol
            });

            if (!asset.HasValue())
                return new GetAssetBySymbolResponse();

            return new GetAssetBySymbolResponse()
            {
                Asset = new Asset()
                {
                    BrokerId = asset.Value.BrokerId,
                    Symbol = asset.Value.Symbol,
                    Accuracy = asset.Value.Accuracy,
                    Description = asset.Value.Description
                }
            };
        }
    }
}
