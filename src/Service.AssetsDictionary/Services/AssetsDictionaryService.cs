using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyJetWallet.Domain;
using MyJetWallet.Domain.Assets;
using MyNoSqlServer.Abstractions;
using Newtonsoft.Json;
using Service.AssetsDictionary.Domain.Models;
using Service.AssetsDictionary.Grpc;
using Service.AssetsDictionary.Grpc.Models;
using Service.AssetsDictionary.MyNoSql;

namespace Service.AssetsDictionary.Services
{
    public class AssetsDictionaryService: IAssetsDictionaryService
    {
        private readonly IMyNoSqlServerDataWriter<AssetNoSqlEntity> _writer;
        private readonly ILogger<AssetsDictionaryService> _logger;

        public AssetsDictionaryService(
            IMyNoSqlServerDataWriter<AssetNoSqlEntity> writer,
            ILogger<AssetsDictionaryService> logger)
        {
            _writer = writer;
            _logger = logger;
        }

        public async ValueTask<Asset> CreateAssetAsync(Asset asset)
        {
            if (string.IsNullOrEmpty(asset.BrokerId)) _logger.ThrowValidationError("Cannot create asset. BrokerId cannot be empty");
            if (string.IsNullOrEmpty(asset.Symbol)) _logger.ThrowValidationError("Cannot create asset. Symbol cannot be empty");

            var entity = AssetNoSqlEntity.Create(asset);
            entity.MatchingEngineId = $"{asset.BrokerId}::{asset.Symbol}";

            var existingItem = await ReadAsset(entity.PartitionKey, entity.PartitionKey);
            if (existingItem != null)
            {
                _logger.ThrowValidationError("Cannot create asset. Symbol already exist");
            }

            await _writer.InsertAsync(entity);

            _logger.LogInformation("Asset is created. BrokerId: {brokerId}, Symbol: {symbol}", entity.BrokerId, entity.Symbol);

            return Asset.Create(entity);
        }

        public async ValueTask<Asset> UpdateAssetAsync(Asset asset)
        {
            _logger.LogInformation("Receive UpdateAsset request: {jsonText}", JsonConvert.SerializeObject(asset));

            if (string.IsNullOrEmpty(asset.BrokerId)) _logger.ThrowValidationError("Cannot update asset. BrokerId cannot be empty");
            if (string.IsNullOrEmpty(asset.Symbol)) _logger.ThrowValidationError("Cannot update asset. Symbol cannot be empty");

            var entity = await ReadAsset(AssetNoSqlEntity.GeneratePartitionKey(asset.BrokerId), AssetNoSqlEntity.GenerateRowKey(asset.Symbol));
            if (entity == null)
            {
                _logger.ThrowValidationError("Cannot update asset. Asset do not found");
                throw new Exception();
            }

            if (!asset.IsEnabled && entity.IsEnabled)
            {
                //todo: validate what asset can be disabled ... do not used in active pairs
            }

            entity.Apply(asset);

            await _writer.InsertOrReplaceAsync(entity);

            _logger.LogInformation("Asset is updated. BrokerId: {brokerId}, Symbol: {symbol}", entity.BrokerId, entity.Symbol);

            return Asset.Create(entity);
        }

        public async ValueTask<NullableValue<Asset>> GetAssetByIdAsync(AssetIdentity assetId)
        {
            var entity = await ReadAsset(AssetNoSqlEntity.GeneratePartitionKey(assetId.BrokerId),
                AssetNoSqlEntity.GenerateRowKey(assetId.Symbol));

            if (entity == null)
                return new NullableValue<Asset>();

            return new NullableValue<Asset>(Asset.Create(entity));
        }

        private async ValueTask<AssetNoSqlEntity> ReadAsset(string partitionKey, string rowKey)
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

        public async ValueTask<AssetsListResponse> GetAssetsByBrokerAsync(JetBrokerIdentity brokerId)
        {
            var entities = await _writer.GetAsync(AssetNoSqlEntity.GeneratePartitionKey(brokerId.BrokerId));
            
            var response = new AssetsListResponse();
            response.Assets = entities.Select(Asset.Create).ToArray();

            return response;
        }

        public async ValueTask<AssetsListResponse> GetAllAssetsAsync()
        {
            var entities = await _writer.GetAsync();

            var response = new AssetsListResponse();
            response.Assets = entities.Select(Asset.Create).ToArray();

            return response;
        }
    }
}