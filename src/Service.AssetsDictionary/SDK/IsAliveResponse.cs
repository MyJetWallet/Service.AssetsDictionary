using Newtonsoft.Json;

namespace Service.AssetsDictionary.SDK
{
    public static class IsAliveResponse
    {
        public static string IsAlive()
        {
            return JsonConvert.SerializeObject(new
            {
                ApplicationEnvironment.AppName,
                ApplicationEnvironment.AppVersion,
                ApplicationEnvironment.Environment,
                ApplicationEnvironment.HostName,
                ApplicationEnvironment.UserName,
                ApplicationEnvironment.StartedAt
            });
        }
    }
}