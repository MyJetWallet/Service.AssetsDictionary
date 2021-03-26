using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Service.AssetsDictionary.Grpc.Models
{
    [DataContract]
    public class AllBrandMappingResponse
    {
        [DataMember(Order = 1)] public List<Brand> Brands { get; set; }

        [DataContract]
        public class Brand
        {
            [DataMember(Order = 1)] public string BrokerId { get; set; }
            [DataMember(Order = 2)] public string BrandId { get; set; }
            [DataMember(Order = 3)] public List<string> AssetSymbolsList { get; set; }
            [DataMember(Order = 4)] public List<string> SpotInstrumentSymbolsList { get; set; }
        }
    }
}