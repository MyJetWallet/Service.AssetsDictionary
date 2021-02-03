﻿using Grpc.Net.Client;
using JetBrains.Annotations;
using ProtoBuf.Grpc.Client;
using Service.AssetsDictionary.Grpc;

namespace Service.AssetsDictionary.Client
{
    [UsedImplicitly]
    public class AssetsDictionaryClientFactory
    {
        private readonly GrpcChannel _channel;

        public AssetsDictionaryClientFactory(string assetsDictionaryGrpcServiceUrl)
        {
            _channel = GrpcChannel.ForAddress("http://localhost:5001");
        }

        public IAssetsDictionaryService GetAssetsDictionaryService() => _channel.CreateGrpcService<IAssetsDictionaryService>();
        public ISpotInstrumentsDictionaryService GetSpotInstrumentsDictionaryService() => _channel.CreateGrpcService<ISpotInstrumentsDictionaryService>();
    }
}