using System;
using System.Collections.Generic;
using MyJetWallet.Domain;
using MyJetWallet.Domain.Assets;
using Service.AssetsDictionary.Domain.Models;

namespace Service.AssetsDictionary.Client
{
    public interface ISpotInstrumentDictionaryClient
    {
        ISpotInstrument GetSpotInstrumentById(ISpotInstrumentIdentity spotInstrumentId);
        IReadOnlyList<ISpotInstrument> GetSpotInstrumentByBroker(IJetBrokerIdentity brokerId);
        IReadOnlyList<ISpotInstrument> GetSpotInstrumentByBrand(IJetBrandIdentity brandId);
        IReadOnlyList<ISpotInstrument> GetAllSpotInstruments();

        event Action OnChanged;
    }
}