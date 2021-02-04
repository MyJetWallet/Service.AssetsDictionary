using System;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using JetBrains.Annotations;
using MyJetWallet.Sdk.GrpcMetrics;
using ProtoBuf.Grpc.Client;
using Service.AssetsDictionary.Grpc;

namespace Service.AssetsDictionary.Client.Grpc
{
    [UsedImplicitly]
    public class AssetsDictionaryClientFactory
    {
        private readonly CallInvoker _channel;

        public AssetsDictionaryClientFactory(string assetsDictionaryGrpcServiceUrl)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress(assetsDictionaryGrpcServiceUrl);
            _channel = channel.Intercept(new PrometheusMetricsInterceptor());
        }

        public IAssetsDictionaryService GetAssetsDictionaryService() => _channel.CreateGrpcService<IAssetsDictionaryService>();
        public ISpotInstrumentsDictionaryService GetSpotInstrumentsDictionaryService() => _channel.CreateGrpcService<ISpotInstrumentsDictionaryService>();
        public IBrandAssetsAndInstrumentsService GetBrandAssetsAndInstrumentsService() => _channel.CreateGrpcService<IBrandAssetsAndInstrumentsService>();
    }
}