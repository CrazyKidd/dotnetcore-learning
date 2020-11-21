using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace NetNote.Utils
{
    public class ConfigHelper
    {
        private static IConfiguration Config = null; //ConfigurationRoot
        public static void _Init(IConfiguration Configuration)
        {
            Config = Configuration;
        }

        public static string Get(string Key)
        {
            IConfigurationSection ISection = Config.GetSection(Key);
            return ISection.Value;
        }

        public static IConfigurationSection GetSection(string Key)
        {
            return Config.GetSection(Key);
        }

        public static void Set(string Key, string Value)
        {
            IConfigurationSection ISection = Config.GetSection(Key);
            ISection.Value = Value;
        }
    }
}
