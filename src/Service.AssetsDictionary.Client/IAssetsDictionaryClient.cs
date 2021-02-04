using System.Collections.Generic;
using MyJetWallet.Domain;
using MyJetWallet.Domain.Assets;
using Service.AssetsDictionary.Domain.Models;

namespace Service.AssetsDictionary.Client
{
    public interface IAssetsDictionaryClient
    {
        IAsset GetAssetById(IAssetIdentity assetId);
        IReadOnlyList<IAsset> GetAssetsByBroker(IJetBrokerIdentity brokerId);
        IReadOnlyList<IAsset> GetAssetsByBrand(IJetBrandIdentity brandId);
        IReadOnlyList<IAsset> GetAllAssets();
    }
}