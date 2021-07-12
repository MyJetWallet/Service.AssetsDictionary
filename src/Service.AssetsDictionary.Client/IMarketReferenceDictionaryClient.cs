using System;
using System.Collections.Generic;
using MyJetWallet.Domain;
using MyJetWallet.Domain.Assets;
using Service.AssetsDictionary.Domain.Models;

namespace Service.AssetsDictionary.Client
{
    public interface IMarketReferenceDictionaryClient
    {
        IMarketReference GetMarketReferenceById(IMarketReferenceIdentity spotInstrumentId);
        IReadOnlyList<IMarketReference> GetMarketReferencesByBroker(IJetBrokerIdentity brokerId);
        IReadOnlyList<IMarketReference> GetMarketReferencesByBrand(IJetBrandIdentity brandId);
        IReadOnlyList<IMarketReference> GetAllMarketReferences();

        event Action OnChanged;
    }
}