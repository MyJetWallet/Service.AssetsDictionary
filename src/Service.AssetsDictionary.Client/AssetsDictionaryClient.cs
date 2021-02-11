using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Autofac;
using JetBrains.Annotations;
using MyJetWallet.Domain;
using MyJetWallet.Domain.Assets;
using MyNoSqlServer.DataReader;
using Service.AssetsDictionary.Domain.Models;
using Service.AssetsDictionary.Grpc;
using Service.AssetsDictionary.MyNoSql;

namespace Service.AssetsDictionary.Client
{
    [UsedImplicitly]
    public class AssetsDictionaryClient: IAssetsDictionaryClient, IStartable
    {
        private readonly MyNoSqlReadRepository<AssetNoSqlEntity> _readerAssets;
        private readonly MyNoSqlReadRepository<BrandAssetsAndInstrumentsNoSqlEntity> _readerAssetsBrand;

        public AssetsDictionaryClient(MyNoSqlReadRepository<AssetNoSqlEntity> readerAssets, MyNoSqlReadRepository<BrandAssetsAndInstrumentsNoSqlEntity> readerAssetsBrand)
        {
            _readerAssets = readerAssets;
            _readerAssetsBrand = readerAssetsBrand;

            _readerAssets.SubscribeToUpdateEvents(list => Changed(), list => Changed());
            _readerAssetsBrand.SubscribeToUpdateEvents(list => Changed(), list => Changed());
        }

        public event Action OnChanged;

        public IAsset GetAssetById(IAssetIdentity assetId)
        {
            var entity = _readerAssets.Get(AssetNoSqlEntity.GeneratePartitionKey(assetId.BrokerId), AssetNoSqlEntity.GenerateRowKey(assetId.Symbol));
            return entity;
        }

        public IReadOnlyList<IAsset> GetAssetsByBroker(IJetBrokerIdentity brokerId)
        {
            var list = _readerAssets.Get(AssetNoSqlEntity.GeneratePartitionKey(brokerId.BrokerId));
            return list;
        }

        public IReadOnlyList<IAsset> GetAssetsByBrand(IJetBrandIdentity brandId)
        {
            var brand = _readerAssetsBrand.Get(BrandAssetsAndInstrumentsNoSqlEntity.GeneratePartitionKey(brandId.BrokerId), BrandAssetsAndInstrumentsNoSqlEntity.GenerateRowKey(brandId.BrandId));

            if (brand == null)
                return new List<IAsset>();

            var assets = GetAssetsByBroker(brandId);

            return assets.Where(a => brand.AssetSymbolsList.Contains(a.Symbol)).ToList();
        }

        public IReadOnlyList<IAsset> GetAllAssets()
        {
            var list = _readerAssets.Get();
            return list;
        }

        public void Start()
        {
            var sw = new Stopwatch();
            sw.Start();
            var iteration = 0;
            while (iteration < 100)
            {
                iteration++;
                if (GetAllAssets().Count > 0)
                    break;

                Thread.Sleep(100);
            }
            sw.Stop();
            Console.WriteLine($"AssetNoSqlEntity client is started. Wait time: {sw.ElapsedMilliseconds} ms. Counts: {GetAllAssets().Count}");
        }

        private void Changed()
        {
            OnChanged?.Invoke();
        }
    }
}