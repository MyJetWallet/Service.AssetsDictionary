using System;
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
    public class MarketReferenceDictionaryClient : IMarketReferenceDictionaryClient
    {
        private readonly MyNoSqlReadRepository<MarketReferenceNoSqlEntity> _readerAssets;
        private readonly MyNoSqlReadRepository<BrandAssetsAndInstrumentsNoSqlEntity> _readerInstrumentBrand;

        public event Action OnChanged;

        public MarketReferenceDictionaryClient(MyNoSqlReadRepository<MarketReferenceNoSqlEntity> readerAssets, MyNoSqlReadRepository<BrandAssetsAndInstrumentsNoSqlEntity> readerInstrumentBrand)
        {
            _readerAssets = readerAssets;
            _readerInstrumentBrand = readerInstrumentBrand;

            _readerAssets.SubscribeToUpdateEvents(list => Changed(), list => Changed());
            _readerInstrumentBrand.SubscribeToUpdateEvents(list => Changed(), list => Changed());
        }

        public IMarketReference GetMarketReferenceById(IMarketReferenceIdentity marketReferenceId)
        {
            return _readerAssets.Get(MarketReferenceNoSqlEntity.GeneratePartitionKey(marketReferenceId.BrokerId),
                MarketReferenceNoSqlEntity.GenerateRowKey(marketReferenceId.Id));
        }

        public IReadOnlyList<IMarketReference> GetMarketReferencesByBroker(IJetBrokerIdentity brokerId)
        {
            return _readerAssets.Get(MarketReferenceNoSqlEntity.GeneratePartitionKey(brokerId.BrokerId));
        }

        public IReadOnlyList<IMarketReference> GetMarketReferencesByBrand(IJetBrandIdentity brandId)
        {
            var brand = _readerInstrumentBrand.Get(
                BrandAssetsAndInstrumentsNoSqlEntity.GeneratePartitionKey(brandId.BrokerId),
                BrandAssetsAndInstrumentsNoSqlEntity.GenerateRowKey(brandId.BrandId));

            if (brand == null)
                return new List<IMarketReference>();

            var references = GetMarketReferencesByBroker(brandId);

            return references.Where(a => brand.MarketReferenceIdsList.Contains(a.Id)).ToList();        
        }


        public IReadOnlyList<IMarketReference> GetAllMarketReferences()
        {
            return _readerAssets.Get();
        }

        private void Changed()
        {
            OnChanged?.Invoke();
        }
    }
}