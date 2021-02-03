using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.AssetsDictionary.Domain.Models;

namespace Service.AssetsDictionary.Grpc.Models
{
    [DataContract]
    public class SpotInstrumentsListResponse
    {
        [DataMember(Order = 1)] public SpotInstrument[] SpotInstruments { get; set; } = { };
    }
}