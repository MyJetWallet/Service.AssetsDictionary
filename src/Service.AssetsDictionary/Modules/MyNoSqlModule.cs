using Autofac;
using MyJetWallet.Sdk.NoSql;
using Service.AssetsDictionary.MyNoSql;

namespace Service.AssetsDictionary.Modules
{
    public class MyNoSqlModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            builder.RegisterMyNoSqlWriter<AssetNoSqlEntity>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), AssetNoSqlEntity.TableName);
            builder.RegisterMyNoSqlWriter<SpotInstrumentNoSqlEntity>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), SpotInstrumentNoSqlEntity.TableName);
            builder.RegisterMyNoSqlWriter<BrandAssetsAndInstrumentsNoSqlEntity>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), BrandAssetsAndInstrumentsNoSqlEntity.TableName);
            builder.RegisterMyNoSqlWriter<MarketReferenceNoSqlEntity>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), MarketReferenceNoSqlEntity.TableName);
        }

    }
}