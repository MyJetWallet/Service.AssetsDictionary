using System.Collections.Generic;
using System.Runtime.Serialization;
using MyJetWallet.Domain;

namespace Service.AssetsDictionary.Domain.Models
{
    public interface IBrandAssetsAndInstruments: IJetBrandIdentity
    {
        IReadOnlyList<string> AssetSymbolsList { get; }

        IReadOnlyList<string> SpotInstrumentSymbolsList { get; }
    }

    [DataContract]
    public class BrandAssetsAndInstruments : IBrandAssetsAndInstruments
    {
        [DataMember(Order = 1)] public string BrokerId { get; set; }
        [DataMember(Order = 2)] public string BrandId { get; set; }
        [DataMember(Order = 3)] public List<string> AssetSymbolsList { get; set; }
        [DataMember(Order = 4)] public List<string> SpotInstrumentSymbolsList { get; set; }

        IReadOnlyList<string> IBrandAssetsAndInstruments.AssetSymbolsList => AssetSymbolsList;
        IReadOnlyList<string> IBrandAssetsAndInstruments.SpotInstrumentSymbolsList => SpotInstrumentSymbolsList;
    }
}