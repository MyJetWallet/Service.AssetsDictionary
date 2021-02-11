using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using MyJetWallet.Domain;
using MyJetWallet.Domain.Assets;
using MyNoSqlServer.DataReader;
using Service.AssetsDictionary.Domain.Models;
using Service.AssetsDictionary.MyNoSql;

namespace Service.AssetsDictionary.Client
{
    [UsedImplicitly]
    public class SpotInstrumentDictionaryClient : ISpotInstrumentDictionaryClient
    {
        private readonly MyNoSqlReadRepository<SpotInstrumentNoSqlEntity> _readerAssets;
        private readonly MyNoSqlReadRepository<BrandAssetsAndInstrumentsNoSqlEntity> _readerInstrumentBrand;

        public SpotInstrumentDictionaryClient(MyNoSqlReadRepository<SpotInstrumentNoSqlEntity> readerAssets, MyNoSqlReadRepository<BrandAssetsAndInstrumentsNoSqlEntity> readerInstrumentBrand)
        {
            _readerAssets = readerAssets;
            _readerInstrumentBrand = readerInstrumentBrand;
        }

        public ISpotInstrument GetSpotInstrumentById(ISpotInstrumentIdentity spotInstrumentId)
        {
            return _readerAssets.Get(SpotInstrumentNoSqlEntity.GeneratePartitionKey(spotInstrumentId.BrokerId), SpotInstrumentNoSqlEntity.GenerateRowKey(spotInstrumentId.Symbol));
        }

        public IReadOnlyList<ISpotInstrument> GetSpotInstrumentByBroker(IJetBrokerIdentity brokerId)
        {
            return _readerAssets.Get(SpotInstrumentNoSqlEntity.GeneratePartitionKey(brokerId.BrokerId));
        }

        public IReadOnlyList<ISpotInstrument> GetSpotInstrumentByBrand(IJetBrandIdentity brandId)
        {
            var brand = _readerInstrumentBrand.Get(BrandAssetsAndInstrumentsNoSqlEntity.GeneratePartitionKey(brandId.BrokerId), BrandAssetsAndInstrumentsNoSqlEntity.GenerateRowKey(brandId.BrandId));

            if (brand == null)
                return new List<ISpotInstrument>();

            var instruments = GetSpotInstrumentByBroker(brandId);

            return instruments.Where(a => brand.SpotInstrumentSymbolsList.Contains(a.Symbol)).ToList();
        }

        public IReadOnlyList<ISpotInstrument> GetAllSpotInstruments()
        {
            return _readerAssets.Get();
        }
    }
}