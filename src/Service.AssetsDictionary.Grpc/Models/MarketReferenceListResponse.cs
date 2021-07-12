using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.AssetsDictionary.Domain.Models;

namespace Service.AssetsDictionary.Grpc.Models
{
    [DataContract]
    public class MarketReferenceListResponse
    {
        [DataMember(Order = 1)] public List<MarketReference> References { get; set; } = new List<MarketReference>();
    }
}