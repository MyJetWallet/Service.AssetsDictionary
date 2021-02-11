using System;
using Autofac;
using MyNoSqlServer.Abstractions;
using Service.AssetsDictionary.MyNoSql;

namespace Service.AssetsDictionary.Modules
{
    public class MyNoSqlModule : Module
    {
        private readonly Func<string> _myNoSqlServerWriterUrl;

        public MyNoSqlModule(Func<string> myNoSqlServerWriterUrl)
        {
            _myNoSqlServerWriterUrl = myNoSqlServerWriterUrl;
        }

        protected override void Load(ContainerBuilder builder)
        {
            RegisterMyNoSqlWriter<AssetNoSqlEntity>(builder, AssetNoSqlEntity.TableName);
            RegisterMyNoSqlWriter<SpotInstrumentNoSqlEntity>(builder, SpotInstrumentNoSqlEntity.TableName);
            RegisterMyNoSqlWriter<BrandAssetsAndInstrumentsNoSqlEntity>(builder, BrandAssetsAndInstrumentsNoSqlEntity.TableName);
        }

        private void RegisterMyNoSqlWriter<TEntity>(ContainerBuilder builder, string table)
            where TEntity : IMyNoSqlDbEntity, new()
        {
            builder.Register(ctx => new MyNoSqlServer.DataWriter.MyNoSqlServerDataWriter<TEntity>(_myNoSqlServerWriterUrl, table,true))
                .As<IMyNoSqlServerDataWriter<TEntity>>()
                .SingleInstance();

        }
    }
}