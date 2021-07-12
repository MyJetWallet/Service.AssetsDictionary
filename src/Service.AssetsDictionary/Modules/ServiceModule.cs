using Autofac;
using Service.AssetsDictionary.Grpc;
using Service.AssetsDictionary.Services;

namespace Service.AssetsDictionary.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AssetsDictionaryService>()
                .As<IAssetsDictionaryService>();

            builder.RegisterType<SpotInstrumentsDictionaryService>()
                .As<ISpotInstrumentsDictionaryService>();

            builder.RegisterType<BrandAssetsAndInstrumentsService>()
                .As<IBrandAssetsAndInstrumentsService>();

            builder.RegisterType<MarketReferencesDictionaryService>()
                .As<IMarketReferencesDictionaryService>();
        }
    }
}