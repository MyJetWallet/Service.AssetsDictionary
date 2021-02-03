using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.AssetsDictionary.Domain.Models;

namespace Service.AssetsDictionary.Grpc.Models
{
    [DataContract]
    public class AssetsListResponse
    {
        [DataMember(Order = 1)] public Asset[] Assets { get; set; } = {};
    }
}