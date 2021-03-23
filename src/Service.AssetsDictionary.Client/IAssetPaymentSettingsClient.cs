using System;
using System.Collections.Generic;
using System.Linq;
using MyJetWallet.Domain;
using MyJetWallet.Domain.Assets;
using MyNoSqlServer.DataReader;
using Service.AssetsDictionary.Domain.Models;
using Service.AssetsDictionary.MyNoSql;
// ReSharper disable UnusedMember.Global

namespace Service.AssetsDictionary.Client
{
    public interface IAssetPaymentSettingsClient
    {
        AssetPaymentSettings GetAssetById(IAssetIdentity assetId);
        IReadOnlyList<AssetPaymentSettings> GetAssetsByBroker(IJetBrokerIdentity brokerId);
        IReadOnlyList<AssetPaymentSettings> GetAllAssets();

        event Action OnChanged;
    }

    public class AssetPaymentSettingsClient : IAssetPaymentSettingsClient
    {
        private readonly MyNoSqlReadRepository<AssetPaymentSettingsNoSqlEntity> _reader;

        public AssetPaymentSettingsClient(MyNoSqlReadRepository<AssetPaymentSettingsNoSqlEntity> reader)
        {
            _reader = reader;
            _reader.SubscribeToUpdateEvents(list => Changed(), list => Changed());
        }

        public AssetPaymentSettings GetAssetById(IAssetIdentity assetId)
        {
            var entity = _reader.Get(AssetPaymentSettingsNoSqlEntity.GeneratePartitionKey(assetId.BrokerId), AssetPaymentSettingsNoSqlEntity.GenerateRowKey(assetId.Symbol));
            return entity?.PaymentSettings;
        }

        public IReadOnlyList<AssetPaymentSettings> GetAssetsByBroker(IJetBrokerIdentity brokerId)
        {
            var list = _reader.Get(AssetPaymentSettingsNoSqlEntity.GeneratePartitionKey(brokerId.BrokerId));
            return list.Select(e => e.PaymentSettings).ToList();
        }

        public IReadOnlyList<AssetPaymentSettings> GetAllAssets()
        {
            var list = _reader.Get();
            return list.Select(e => e.PaymentSettings).ToList();
        }

        public event Action OnChanged;

        private void Changed()
        {
            OnChanged?.Invoke();
        }
    }
}