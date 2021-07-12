using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyJetWallet.Domain;
using MyJetWallet.Domain.Assets;
using MyNoSqlServer.Abstractions;
using Service.AssetsDictionary.Domain.Models;
using Service.AssetsDictionary.Grpc;
using Service.AssetsDictionary.Grpc.Models;
using Service.AssetsDictionary.MyNoSql;

namespace Service.AssetsDictionary.Services
{
    public class BrandAssetsAndInstrumentsService: IBrandAssetsAndInstrumentsService
    {
        private readonly IMyNoSqlServerDataWriter<BrandAssetsAndInstrumentsNoSqlEntity> _writer;
        private readonly IAssetsDictionaryService _assetsDictionaryService;
        private readonly ISpotInstrumentsDictionaryService _spotInstrumentsDictionaryService;
        private readonly IMarketReferencesDictionaryService _marketReferencesDictionaryService;
        private readonly ILogger<BrandAssetsAndInstrumentsService> _logger;

        public BrandAssetsAndInstrumentsService(
            IMyNoSqlServerDataWriter<BrandAssetsAndInstrumentsNoSqlEntity> writer,
            IAssetsDictionaryService assetsDictionaryService,
            ISpotInstrumentsDictionaryService spotInstrumentsDictionaryService,
            IMarketReferencesDictionaryService marketReferencesDictionaryService,
            ILogger<BrandAssetsAndInstrumentsService> logger)
        {
            _writer = writer;
            _assetsDictionaryService = assetsDictionaryService;
            _spotInstrumentsDictionaryService = spotInstrumentsDictionaryService;
            _marketReferencesDictionaryService = marketReferencesDictionaryService;
            _logger = logger;
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public async Task<AssetDictionaryResponse<bool>> AddAssetAsync(AssetIdBrandIdRequest request)
        {
            if (request == null) return AssetDictionaryResponse<bool>.Error($"Request cannot be null");

            if (request.AssetId?.BrokerId != request.BrandId?.BrokerId) return AssetDictionaryResponse<bool>.Error($"Cannot add asset {request.AssetId} to brand {request.BrandId}. Broker should be same");

            var asset = await _assetsDictionaryService.GetAssetByIdAsync(request.AssetId);

            if (asset?.HasValue() != true) return AssetDictionaryResponse<bool>.Error($"Cannot add asset {request.AssetId} to brand {request.BrandId}. Asset not found");

            var entity = await ReadEntity(BrandAssetsAndInstrumentsNoSqlEntity.GeneratePartitionKey(request.BrandId.BrokerId), BrandAssetsAndInstrumentsNoSqlEntity.GenerateRowKey(request.BrandId.BrandId));

            // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
            if (entity == null)
            {
                entity = BrandAssetsAndInstrumentsNoSqlEntity.Create(new BrandAssetsAndInstruments()
                {
                    BrokerId = request.BrandId.BrokerId,
                    BrandId = request.BrandId.BrandId,
                    AssetSymbolsList = new List<string>(),
                    SpotInstrumentSymbolsList = new List<string>()
                });
            }

            if (!entity.AssetSymbolsList.Contains(request.AssetId.Symbol))
            {
                entity.AssetSymbolsList.Add(request.AssetId.Symbol);

                await _writer.InsertOrReplaceAsync(entity);

                _logger.LogInformation("Asset {AssetIdentity} added to brand {JetBrandIdentity}", request.AssetId, request.BrandId);
            }

            return AssetDictionaryResponse<bool>.Success(true);
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public async Task<AssetDictionaryResponse<bool>> RemoveAssetAsync(AssetIdBrandIdRequest request)
        {
            if (request == null) return AssetDictionaryResponse<bool>.Error($"Request cannot be null");

            if (request.AssetId == null) return AssetDictionaryResponse<bool>.Error($"Cannot remove asset from brand {request.BrandId}. Asset cannot be null");

            var entity = await ReadEntity(BrandAssetsAndInstrumentsNoSqlEntity.GeneratePartitionKey(request.BrandId.BrokerId), BrandAssetsAndInstrumentsNoSqlEntity.GenerateRowKey(request.BrandId.BrandId));

            if (entity != null)
            {
                entity.AssetSymbolsList.Remove(request.AssetId.Symbol);

                await _writer.InsertOrReplaceAsync(entity);

                _logger.LogInformation("Asset {AssetIdentity} removed from brand {JetBrandIdentity}", request.AssetId, request.BrandId);
            }

            return AssetDictionaryResponse<bool>.Success(true);
        }

        public async Task<AssetsListResponse> GetAllAssetsByBrandAsync(JetBrandIdentity brandId)
        {
            if (brandId == null) 
                return new AssetsListResponse();

            var entity = await ReadEntity(BrandAssetsAndInstrumentsNoSqlEntity.GeneratePartitionKey(brandId.BrokerId), BrandAssetsAndInstrumentsNoSqlEntity.GenerateRowKey(brandId.BrandId));

            if (entity == null)
                return new AssetsListResponse();

            var assets = await _assetsDictionaryService.GetAssetsByBrokerAsync(new JetBrokerIdentity(){BrokerId = brandId.BrokerId});

            var result =  new AssetsListResponse()
            {
                Assets = assets.Assets.Where(a => entity.AssetSymbolsList.Contains(a.Symbol)).ToArray()
            };

            return result;
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public async Task<AssetDictionaryResponse<bool>> AddSpotInstrumentAsync(SpotInstrumentIdBrandIdRequest request)
        {
            if (request == null) return AssetDictionaryResponse<bool>.Error($"Request cannot be null");

            if (request.SpotInstrumentId.BrokerId != request.BrandId.BrokerId) return AssetDictionaryResponse<bool>.Error($"Cannot add spotInstrument {request.SpotInstrumentId} to brand {request.BrandId}. Broker should be same");

            var spotInstrument = await _spotInstrumentsDictionaryService.GetSpotInstrumentByIdAsync(request.SpotInstrumentId);

            if (spotInstrument?.HasValue() != true) return AssetDictionaryResponse<bool>.Error($"Cannot add spotInstrument {request.SpotInstrumentId} to brand {request.BrandId}. Asset not found");

            var entity = await ReadEntity(BrandAssetsAndInstrumentsNoSqlEntity.GeneratePartitionKey(request.BrandId.BrokerId), BrandAssetsAndInstrumentsNoSqlEntity.GenerateRowKey(request.BrandId.BrandId));

            // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
            if (entity == null)
            {
                entity = BrandAssetsAndInstrumentsNoSqlEntity.Create(new BrandAssetsAndInstruments()
                {
                    BrokerId = request.BrandId.BrokerId,
                    BrandId = request.BrandId.BrandId,
                    AssetSymbolsList = new List<string>(),
                    SpotInstrumentSymbolsList = new List<string>()
                });
            }

            if (!entity.SpotInstrumentSymbolsList.Contains(request.SpotInstrumentId.Symbol))
            {
                entity.SpotInstrumentSymbolsList.Add(request.SpotInstrumentId.Symbol);

                await _writer.InsertOrReplaceAsync(entity);

                _logger.LogInformation("SpotInstrumentId {SpotInstrumentIdentity} added to brand {JetBrandIdentity}", request.SpotInstrumentId, request.SpotInstrumentId);
            }

            return AssetDictionaryResponse<bool>.Success(true);
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public async Task<AssetDictionaryResponse<bool>> RemoveSpotInstrumentAsync(SpotInstrumentIdBrandIdRequest request)
        {
            if (request == null) return AssetDictionaryResponse<bool>.Error($"Request cannot be null");

            if (request.SpotInstrumentId == null) return AssetDictionaryResponse<bool>.Error($"Cannot remove spotInstrument from brand {request.BrandId}. SpotInstrument cannot be null");

            var entity = await ReadEntity(BrandAssetsAndInstrumentsNoSqlEntity.GeneratePartitionKey(request.BrandId.BrokerId), BrandAssetsAndInstrumentsNoSqlEntity.GenerateRowKey(request.BrandId.BrandId));

            if (entity != null)
            {
                entity.SpotInstrumentSymbolsList.Remove(request.SpotInstrumentId.Symbol);

                await _writer.InsertOrReplaceAsync(entity);

                _logger.LogInformation("SpotInstrumentId {SpotInstrumentIdentity} removed from brand {JetBrandIdentity}", request.SpotInstrumentId, request.BrandId);
            }

            return AssetDictionaryResponse<bool>.Success(true);
        }

        public async Task<SpotInstrumentsListResponse> GetAllSpotInstrumentsByBrandAsync(JetBrandIdentity brandId)
        {
            if (brandId == null)
                return new SpotInstrumentsListResponse();

            var entity = await ReadEntity(BrandAssetsAndInstrumentsNoSqlEntity.GeneratePartitionKey(brandId.BrokerId), BrandAssetsAndInstrumentsNoSqlEntity.GenerateRowKey(brandId.BrandId));

            if (entity == null)
                return new SpotInstrumentsListResponse();

            var instruments = await _spotInstrumentsDictionaryService.GetSpotInstrumentsByBrokerAsync(new JetBrokerIdentity(){BrokerId = brandId.BrokerId});

            var result = new SpotInstrumentsListResponse()
            {
                SpotInstruments = instruments.SpotInstruments.Where(i => entity.SpotInstrumentSymbolsList.Contains(i.Symbol)).ToArray()
            };

            return result;
        }

        public async Task<AssetDictionaryResponse<bool>> AddMarketReferenceAsync(MarketReferenceIdBrandIdRequest request)
        {
            if (request == null) return AssetDictionaryResponse<bool>.Error($"Request cannot be null");

            if (request.ReferenceId.BrokerId != request.BrandId.BrokerId) return AssetDictionaryResponse<bool>.Error($"Cannot add spotInstrument {request.ReferenceId} to brand {request.BrandId}. Broker should be same");

            var marketReference = await _marketReferencesDictionaryService.GetMarketReferenceByIdAsync(request.ReferenceId);

            if (marketReference?.HasValue() != true) return AssetDictionaryResponse<bool>.Error($"Cannot add market reference {request.ReferenceId} to brand {request.BrandId}. Asset not found");

            var entity = await ReadEntity(BrandAssetsAndInstrumentsNoSqlEntity.GeneratePartitionKey(request.BrandId.BrokerId), BrandAssetsAndInstrumentsNoSqlEntity.GenerateRowKey(request.BrandId.BrandId));

            // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
            if (entity == null)
            {
                entity = BrandAssetsAndInstrumentsNoSqlEntity.Create(new BrandAssetsAndInstruments()
                {
                    BrokerId = request.BrandId.BrokerId,
                    BrandId = request.BrandId.BrandId,
                    AssetSymbolsList = new List<string>(),
                    SpotInstrumentSymbolsList = new List<string>(),
                    MarketReferenceIdsList = new List<string>()
                });
            }

            if (!entity.MarketReferenceIdsList.Contains(request.ReferenceId.Id))
            {
                entity.MarketReferenceIdsList.Add(request.ReferenceId.Id);

                await _writer.InsertOrReplaceAsync(entity);

                _logger.LogInformation("MarketReferenceId {SpotInstrumentIdentity} added to brand {JetBrandIdentity}", request.ReferenceId, request.ReferenceId);
            }

            return AssetDictionaryResponse<bool>.Success(true);        
        }

        public async Task<AssetDictionaryResponse<bool>> RemoveMarketReferenceAsync(MarketReferenceIdBrandIdRequest request)
        {
            if (request == null) return AssetDictionaryResponse<bool>.Error($"Request cannot be null");

            if (request.ReferenceId == null) return AssetDictionaryResponse<bool>.Error($"Cannot remove market reference from brand {request.BrandId}. MarketReference cannot be null");

            var entity = await ReadEntity(BrandAssetsAndInstrumentsNoSqlEntity.GeneratePartitionKey(request.BrandId.BrokerId), BrandAssetsAndInstrumentsNoSqlEntity.GenerateRowKey(request.BrandId.BrandId));

            if (entity != null)
            {
                entity.SpotInstrumentSymbolsList.Remove(request.ReferenceId.Id);

                await _writer.InsertOrReplaceAsync(entity);

                _logger.LogInformation("MarketReferenceId {SpotInstrumentIdentity} removed from brand {JetBrandIdentity}", request.ReferenceId, request.BrandId);
            }

            return AssetDictionaryResponse<bool>.Success(true);        
        }

        public async Task<MarketReferenceListResponse> GetAllMarketReferencesByBrandAsync(JetBrandIdentity brandId)
        {
            if (brandId == null)
                return new MarketReferenceListResponse();

            var entity = await ReadEntity(BrandAssetsAndInstrumentsNoSqlEntity.GeneratePartitionKey(brandId.BrokerId), BrandAssetsAndInstrumentsNoSqlEntity.GenerateRowKey(brandId.BrandId));

            if (entity == null)
                return new MarketReferenceListResponse();

            var references = await _marketReferencesDictionaryService.GetMarketReferencesByBrokerAsync(new JetBrokerIdentity(){BrokerId = brandId.BrokerId});

            var result = new MarketReferenceListResponse()
            {
                References = references.References.Where(i => entity.SpotInstrumentSymbolsList.Contains(i.Id)).ToList()
            };

            return result;        
        }

        public async Task<AllBrandMappingResponse> GetAllBrandMappingAsync()
        {
            var items = await _writer.GetAsync();

            var response = new AllBrandMappingResponse()
            {
                Brands = items.Select(e => new AllBrandMappingResponse.Brand()
                {
                    BrokerId = e.BrokerId,
                    BrandId = e.BrandId,
                    AssetSymbolsList = e.AssetSymbolsList,
                    SpotInstrumentSymbolsList = e.SpotInstrumentSymbolsList,
                    MarketReferenceIdsList = e.MarketReferenceIdsList
                }).ToList()
            };

            return response;
        }

        private async ValueTask<BrandAssetsAndInstrumentsNoSqlEntity> ReadEntity(string partitionKey, string rowKey)
        {
            try
            {
                var entity = await _writer.GetAsync(partitionKey, rowKey);
                return entity;
            }
            catch (Exception ex)
            {
                if (ex.Message == "Unknown HTTP result CodeNotFound. Message: Row not found")
                    return null;
                throw;
            }
        }
    }
}