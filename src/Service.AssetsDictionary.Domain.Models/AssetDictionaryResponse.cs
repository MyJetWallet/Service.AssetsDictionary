using System.Runtime.Serialization;

namespace Service.AssetsDictionary.Domain.Models
{
    [DataContract]
    public class AssetDictionaryResponse<T>
    {
        [DataMember(Order = 1)] public T Data { get; set; }
        [DataMember(Order = 2)] public string ErrorMessage { get; set; }
        [DataMember(Order = 3)] public bool IsSuccess { get; set; }

        public static AssetDictionaryResponse<T> Success(T data)
        {
            return new AssetDictionaryResponse<T>()
            {
                Data = data,
                IsSuccess = true
            };
        }

        public static AssetDictionaryResponse<T> Error(string errorMessage)
        {
            return new AssetDictionaryResponse<T>()
            {
                ErrorMessage = errorMessage,
                IsSuccess = false
            };
        }
    }
}