using System;
using System.Runtime.Serialization;

namespace Service.AssetsDictionary.Grpc.Models
{
    [DataContract]
    public class NullableValue<T>
    {
        public NullableValue()
        {
        }

        public NullableValue(T value)
        {
            Value = value;
        }

        [DataMember(Order = 1)] public T Value { get; set; }

        public bool HasValue() => Value != null;
    }
}