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
        private readonly IAssetsDictionaryService _assetsDictionary;

        public SpotInstrumentsDictionaryService(
            IMyNoSqlServerDataWriter<SpotInstrumentNoSqlEntity> writer,
            ILogger<SpotInstrumentsDictionaryService> logger,
            IAssetsDictionaryService assetsDictionary)
        {
            _writer = writer;
            _logger = logger;
            _assetsDictionary = assetsDictionary;
        }

        public async Task<SpotInstrument> CreateSpotInstrumentAsync(SpotInstrument instrument)
        {
            if (string.IsNullOrEmpty(instrument.BrokerId)) _logger.ThrowValidationError("Cannot create instrument. BrokerId cannot be empty");
            if (string.IsNullOrEmpty(instrument.Symbol)) _logger.ThrowValidationError("Cannot create instrument. Symbol cannot be empty");

            var baseAsset = await _assetsDictionary.GetAssetByIdAsync(new AssetIdentity() { BrokerId = instrument.BrokerId, Symbol = instrument.BaseAsset });
            var quoteAsset = await _assetsDictionary.GetAssetByIdAsync(new AssetIdentity() { BrokerId = instrument.BrokerId, Symbol = instrument.QuoteAsset });

            if (!baseAsset.HasValue()) _logger.ThrowValidationError("Cannot create instrument. BaseAsset do not found");
            if (!quoteAsset.HasValue()) _logger.ThrowValidationError("Cannot create instrument. QuoteAsset do not found");

            if (instrument.IsEnabled)
            {
                if (!baseAsset.Value.IsEnabled) _logger.ThrowValidationError("Cannot create instrument. BaseAsset is disabled");
                if (!quoteAsset.Value.IsEnabled) _logger.ThrowValidationError("Cannot create instrument. QuoteAsset is disabled");
            }

            var entity = SpotInstrumentNoSqlEntity.Create(instrument);
            entity.MatchingEngineId = $"{instrument.BrokerId}::{instrument.Symbol}";

            var existingItem = await ReadInstrument(entity.PartitionKey, entity.PartitionKey);
            if (existingItem != null)
            {
                _logger.ThrowValidationError("Cannot create instrument. Symbol already exist");
            }

            await _writer.InsertAsync(entity);

            _logger.LogInformation("Spot instrument is created. BrokerId: {brokerId}, Symbol: {symbol}", entity.BrokerId, entity.Symbol);

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

            var baseAsset = await _assetsDictionary.GetAssetByIdAsync(new AssetIdentity() { BrokerId = instrument.BrokerId, Symbol = instrument.BaseAsset });
            var quoteAsset = await _assetsDictionary.GetAssetByIdAsync(new AssetIdentity() { BrokerId = instrument.BrokerId, Symbol = instrument.QuoteAsset });

            if (!baseAsset.HasValue()) _logger.ThrowValidationError("Cannot create instrument. BaseAsset do not found");
            if (!quoteAsset.HasValue()) _logger.ThrowValidationError("Cannot create instrument. QuoteAsset do not found");

            if (instrument.IsEnabled)
            {
                if (!baseAsset.Value.IsEnabled) _logger.ThrowValidationError("Cannot create instrument. BaseAsset is disabled");
                if (!quoteAsset.Value.IsEnabled) _logger.ThrowValidationError("Cannot create instrument. QuoteAsset is disabled");
            }

            entity.Apply(instrument);

            await _writer.InsertOrReplaceAsync(entity);

            _logger.LogInformation("Spot instrument is updated. BrokerId: {brokerId}, Symbol: {symbol}", entity.BrokerId, entity.Symbol);

            return SpotInstrument.Create(entity);
        }

        public async Task<NullableValue<SpotInstrument>> GetSpotInstrumentByIdAsync(SpotInstrumentIdentity assetId)
        {
            var entity = await ReadInstrument(SpotInstrumentNoSqlEntity.GeneratePartitionKey(assetId.BrokerId), SpotInstrumentNoSqlEntity.GenerateRowKey(assetId.Symbol));

            if (entity == null)
                return new NullableValue<SpotInstrument>();

            return new NullableValue<SpotInstrument>(SpotInstrument.Create(entity));
        }

        public async Task<SpotInstrumentsListResponse> GetSpotInstrumentsByBrokerAsync(JetBrokerIdentity brokerId)
        {
            var list = await _writer.GetAsync(SpotInstrumentNoSqlEntity.GeneratePartitionKey(brokerId.BrokerId));
            return new SpotInstrumentsListResponse()
            {
                SpotInstruments = list.Select(SpotInstrument.Create).ToArray()
            };
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