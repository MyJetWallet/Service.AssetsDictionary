using System;
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
    public class SpotInstrumentsDictionaryService: ISpotInstrumentsDictionaryService
    {
        private readonly IMyNoSqlServerDataWriter<SpotInstrumentNoSqlEntity> _writer;
        private readonly ILogger<SpotInstrumentsDictionaryService> _logger;

        public SpotInstrumentsDictionaryService(
            IMyNoSqlServerDataWriter<SpotInstrumentNoSqlEntity> writer,
            ILogger<SpotInstrumentsDictionaryService> logger)
        {
            _writer = writer;
            _logger = logger;
        }

        public async Task<SpotInstrument> CreateSpotInstrumentAsync(SpotInstrument instrument)
        {
            if (string.IsNullOrEmpty(instrument.BrokerId)) _logger.ThrowValidationError("Cannot create instrument. BrokerId cannot be empty");
            if (string.IsNullOrEmpty(instrument.Symbol)) _logger.ThrowValidationError("Cannot create instrument. Symbol cannot be empty");

            var entity = SpotInstrumentNoSqlEntity.Create(instrument);
            entity.MatchingEngineId = $"{instrument.BrokerId}::{instrument.Symbol}";

            var existingItem = await ReadInstrument(entity.PartitionKey, entity.PartitionKey);
            if (existingItem != null)
            {
                _logger.ThrowValidationError("Cannot create instrument. Symbol already exist");
            }

            await _writer.InsertAsync(entity);

            return SpotInstrument.Create(entity);
        }

        public async Task<SpotInstrument> UpdateSpotInstrumentAsync(SpotInstrument instrument)
        {
            if (string.IsNullOrEmpty(instrument.BrokerId)) _logger.ThrowValidationError("Cannot update instrument. BrokerId cannot be empty");
            if (string.IsNullOrEmpty(instrument.Symbol)) _logger.ThrowValidationError("Cannot update instrument. Symbol cannot be empty");

            var entity = await ReadInstrument(SpotInstrumentNoSqlEntity.GeneratePartitionKey(instrument.BrokerId), SpotInstrumentNoSqlEntity.GenerateRowKey(instrument.Symbol));
            if (entity == null)
            {
                _logger.ThrowValidationError("Cannot update instrument. Asset do not found");
                throw new Exception();
            }

            if (!instrument.IsEnabled && entity.IsEnabled)
            {
                //todo: validate what asset can be disabled ... do not used in active pairs
            }

            entity.Apply(instrument);

            await _writer.InsertOrReplaceAsync(entity);

            return SpotInstrument.Create(entity);
        }

        public Task<SpotInstrument> GetSpotInstrumentByIdAsync(SpotInstrumentIdentity assetId)
        {
            throw new System.NotImplementedException();
        }

        public Task<SpotInstrumentsListResponse> GetSpotInstrumentsByBrokerAsync(JetBrokerIdentity brokerId)
        {
            throw new System.NotImplementedException();
        }

        public Task<SpotInstrumentsListResponse> GetSpotInstrumentsByBrandAsync(JetBrandIdentity brandId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<SpotInstrumentsListResponse> GetAllSpotInstrumentsAsync()
        {
            var entities = await _writer.GetAsync();

            var response = new SpotInstrumentsListResponse();
            response.SpotInstruments = entities.Select(SpotInstrument.Create).ToArray();

            return response;
        }

        private async ValueTask<SpotInstrumentNoSqlEntity> ReadInstrument(string partitionKey, string rowKey)
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