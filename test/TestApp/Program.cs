using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyJetWallet.Domain.Assets;
using ProtoBuf.Grpc.Client;
using ProtoBuf.Grpc.Reflection;
using Service.AssetsDictionary.Client;
using Service.AssetsDictionary.Domain.Models;
using Service.AssetsDictionary.Grpc;
using Service.AssetsDictionary.Grpc.Models;

namespace TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            Console.Write("Press enter to start");
            Console.ReadLine();


            var factory = new AssetsDictionaryClientFactory("http://localhost:5001");
            var client = factory.GetAssetsDictionaryService();

            var btc = await  client.GetAssetByIdAsync(new AssetIdentity(){BrokerId = "test", Symbol = "BTC"});
            Console.WriteLine($"Existing: {btc.Value?.BrokerId}: {btc.Value?.Symbol} [{btc.Value?.IsEnabled}]");

            if (!btc.HasValue())
            {
                var newbtc = await client.CreateAssetAsync(new Asset()
                {
                    BrokerId = "test",
                    Symbol = "BTC",
                    Accuracy = 8,
                    Description = "Bitcoin",
                    IsEnabled = true,
                    MatchingEngineId = "test-BTC"
                });
                Console.WriteLine($"New asset: {newbtc.BrokerId}: {newbtc.Symbol} [{newbtc.IsEnabled}]");
            }

            btc = await client.GetAssetByIdAsync(new AssetIdentity() { BrokerId = "test", Symbol = "BTC" });
            btc.Value.Description = "Bitcoin";
            btc.Value.IsEnabled = !btc.Value.IsEnabled;
            var updatedBtc = await client.UpdateAssetAsync(btc.Value);
            Console.WriteLine($"updated asset: {updatedBtc.BrokerId}: {updatedBtc.Symbol} [{updatedBtc.IsEnabled}]");
            
            Console.WriteLine();
            Console.WriteLine("All assets:");
            var resp = await client.GetAllAssetsAsync();
            foreach (var asset in resp.Assets)
            {
                Console.WriteLine($"{asset.BrokerId}: {asset.Symbol} [{asset.IsEnabled}]");
            }

            Console.WriteLine();
            Console.WriteLine("All spot instruments:");
            var instruments = await client.GetAllAssetsAsync();
            foreach (var asset in instruments.Assets)
            {
                Console.WriteLine($"{asset.BrokerId}: {asset.Symbol} [{asset.IsEnabled}]");
            }

            var clientInstrument = factory.GetSpotInstrumentsDictionaryService();
            //var res = await clientInstrument.CreateSpotInstrumentAsync(new SpotInstrument()
            //{
            //    BrokerId = "test",
            //    Symbol = "BTC",
            //    Accuracy = 5,
            //    MatchingEngineId = "",
            //    MaxOppositeVolume = 10.45m,
            //    MaxVolume = 10.45m,
            //    MinVolume = 0.0001m,
            //    BaseAsset = "BTC",
            //    QuoteAsset = "USD",
            //    MarketOrderPriceThreshold = 0.2m
            //});

            var res = await clientInstrument.GetAllSpotInstrumentsAsync();
            Console.WriteLine();
            Console.WriteLine("Instruments:");
            foreach (var instrument in res.SpotInstruments)
            {
                Console.WriteLine($"{instrument.BrokerId}: {instrument.Symbol} [{instrument.IsEnabled}] MaxVolume: {instrument.MaxVolume}");
            }


            Console.WriteLine();
            var generator = new SchemaGenerator();
            var schema = generator.GetSchema<ISpotInstrumentsDictionaryService>();
            Console.WriteLine(schema);
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
