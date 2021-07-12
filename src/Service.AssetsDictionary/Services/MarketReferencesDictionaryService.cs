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
    public class MarketReferencesDictionaryService : IMarketReferencesDictionaryService
    {
        private readonly IMyNoSqlServerDataWriter<MarketReferenceNoSqlEntity> _writer;
        private readonly ILogger<MarketReferencesDictionaryService> _logger;

        public MarketReferencesDictionaryService(ILogger<MarketReferencesDictionaryService> logger, IMyNoSqlServerDataWriter<MarketReferenceNoSqlEntity> writer)
        {
            _logger = logger;
            _writer = writer;
        }

        public async Task<AssetDictionaryResponse<MarketReference>> CreateMarketReferenceAsync(MarketReference reference)
        {
            _logger.LogInformation("Receive UpdateMarketReference request: {jsonText}", JsonConvert.SerializeObject(reference));

            if (string.IsNullOrEmpty(reference.BrokerId)) return AssetDictionaryResponse<MarketReference>.Error("Cannot update reference. BrokerId cannot be empty");
            if (string.IsNullOrEmpty(reference.Id)) return AssetDictionaryResponse<MarketReference>.Error("Cannot update reference. Symbol cannot be empty");

            var entity = MarketReferenceNoSqlEntity.Create(reference);

            var existingItem = await ReadInstrument(MarketReferenceNoSqlEntity.GeneratePartitionKey(reference.BrokerId), MarketReferenceNoSqlEntity.GenerateRowKey(reference.Id));
            if (existingItem != null)
            {
                return AssetDictionaryResponse<MarketReference>.Error("Cannot create reference. Asset already exists");
            }

            await _writer.InsertAsync(entity);

            _logger.LogInformation("Market reference is created. BrokerId: {brokerId}, Symbol: {symbol}", entity.BrokerId, entity.Id);

            return AssetDictionaryResponse<MarketReference>.Success(MarketReference.Create(entity));
            
        }

        public async Task<AssetDictionaryResponse<MarketReference>> UpdateMarketReferenceAsync(MarketReference reference)
        {
            _logger.LogInformation("Receive UpdateMarketReference request: {jsonText}", JsonConvert.SerializeObject(reference));

            if (string.IsNullOrEmpty(reference.BrokerId)) return AssetDictionaryResponse<MarketReference>.Error("Cannot update reference. BrokerId cannot be empty");
            if (string.IsNullOrEmpty(reference.Id)) return AssetDictionaryResponse<MarketReference>.Error("Cannot update reference. Symbol cannot be empty");

            var entity = await ReadInstrument(MarketReferenceNoSqlEntity.GeneratePartitionKey(reference.BrokerId), MarketReferenceNoSqlEntity.GenerateRowKey(reference.Id));
            if (entity == null)
            {
                return AssetDictionaryResponse<MarketReference>.Error("Cannot update reference. Asset do not found");
                throw new Exception();
            }
            
            entity.Apply(reference);

            await _writer.InsertOrReplaceAsync(entity);

            _logger.LogInformation("Market reference is updated. BrokerId: {brokerId}, Symbol: {symbol}", entity.BrokerId, entity.Id);

            return AssetDictionaryResponse<MarketReference>.Success(MarketReference.Create(entity));        
        }

        public async Task<AssetDictionaryResponse<MarketReference>> DeleteMarketReferenceAsync(MarketReference reference)
        {
            _logger.LogInformation("Receive DeleteMarketReference request: {jsonText}", JsonConvert.SerializeObject(reference));

            var entity = await ReadInstrument(MarketReferenceNoSqlEntity.GeneratePartitionKey(reference.BrokerId), MarketReferenceNoSqlEntity.GenerateRowKey(reference.Id));
            if (entity != null)
            {
                _logger.LogWarning("Deleting reference: {jsonText}", JsonConvert.SerializeObject(entity));
                await _writer.DeleteAsync(MarketReferenceNoSqlEntity.GeneratePartitionKey(reference.BrokerId), MarketReferenceNoSqlEntity.GenerateRowKey(reference.Id));
            }
            return AssetDictionaryResponse<MarketReference>.Success(MarketReference.Create(reference));
        }

        public async Task<NullableValue<MarketReference>> GetMarketReferenceByIdAsync(MarketReferenceIdentity identity)
        {
            var entity = await ReadInstrument(MarketReferenceNoSqlEntity.GeneratePartitionKey(identity.BrokerId), MarketReferenceNoSqlEntity.GenerateRowKey(identity.Id));

            if (entity == null)
                return new NullableValue<MarketReference>();

            return new NullableValue<MarketReference>(MarketReference.Create(entity));        
        }

        public async Task<MarketReferenceListResponse> GetMarketReferencesByBrokerAsync(JetBrokerIdentity brokerId)
        {
            var entities = await _writer.GetAsync(MarketReferenceNoSqlEntity.GeneratePartitionKey(brokerId.BrokerId));
            return new MarketReferenceListResponse()
            {
                References = entities.Select(MarketReference.Create).ToList()
            };
        }

        public async Task<MarketReferenceListResponse> GetAllMarketReferencesAsync()
        {
            var entities = await _writer.GetAsync();
            return new MarketReferenceListResponse()
            {
                References = entities.Select(MarketReference.Create).ToList()
            };        
        }
        
        private async ValueTask<MarketReferenceNoSqlEntity> ReadInstrument(string partitionKey, string rowKey)
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