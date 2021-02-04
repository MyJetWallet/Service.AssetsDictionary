using Autofac;
using MyNoSqlServer.DataReader;

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
            builder
                .RegisterInstance(new AssetsDictionaryClient(myNoSqlSubscriber))
                .As<IAssetsDictionaryClient>()
                .AutoActivate()
                .SingleInstance();

            builder
                .RegisterInstance(new AssetsDictionaryClient(myNoSqlSubscriber))
                .As<ISpotInstrumentDictionaryClient>()
                .AutoActivate()
                .SingleInstance();
        }


        
    }
}