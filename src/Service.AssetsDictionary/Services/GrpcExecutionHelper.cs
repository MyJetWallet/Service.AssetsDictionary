using System;
using Microsoft.Extensions.Logging;

namespace Service.AssetsDictionary.Services
{
    public static class GrpcExecutionHelper
    {
        public static void ThrowValidationError(this ILogger logger, string message)
        {
            logger.LogError(message);
            throw new Exception(message);
        }
    }
}