using Autofac;
using MyNoSqlServer.DataReader;
using Service.AssetsDictionary.MyNoSql;

namespace Service.AssetsDictionary.Client
{
    public static class AssetDictionaryAutofacHelper
    {
        /// <summary>
        /// Register interfaces:
        ///   * IAssetsDictionaryClient
        ///   * ISpotInstrumentDictionaryClient
        /// </summary>
        public static void RegisterAssetsDictionaryClients(this ContainerBuilder builder, IMyNoSqlSubscriber myNoSqlSubscriber)
        {
            var assetSubs = new MyNoSqlReadRepository<AssetNoSqlEntity>(myNoSqlSubscriber, AssetNoSqlEntity.TableName);
            var brandSubs = new MyNoSqlReadRepository<BrandAssetsAndInstrumentsNoSqlEntity>(myNoSqlSubscriber, BrandAssetsAndInstrumentsNoSqlEntity.TableName);
            var instrumentSubs = new MyNoSqlReadRepository<SpotInstrumentNoSqlEntity>(myNoSqlSubscriber, SpotInstrumentNoSqlEntity.TableName);
            
            builder
                .RegisterInstance(new AssetsDictionaryClient(assetSubs, brandSubs))
                .As<IAssetsDictionaryClient>()
                .AutoActivate()
                .SingleInstance();

            builder
                .RegisterInstance(new SpotInstrumentDictionaryClient(instrumentSubs, brandSubs))
                .As<ISpotInstrumentDictionaryClient>()
                .AutoActivate()
                .SingleInstance();
        }
    }
}