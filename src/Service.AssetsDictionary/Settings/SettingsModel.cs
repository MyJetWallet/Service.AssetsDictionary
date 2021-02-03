﻿using SimpleTrading.SettingsReader;

namespace Service.AssetsDictionary.Settings
{
    [YamlAttributesOnly]
    public class SettingsModel
    {
        [YamlProperty("AssetsDictionary.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("AssetsDictionary.MyNoSqlWriterUrl")]
        public string MyNoSqlWriterUrl { get; set; }
    }
}