using System;
using System.IO;
using Newtonsoft;
using Newtonsoft.Json;
namespace RemoteReupload.Core
{
    public class Config
    {
        public string API_Key { get; set; }
        public string Group { get; set; }
        public string API_URL { get; set; }

        public static Config Load()
        {
            if (!File.Exists("UserData\\RemoteReuploadConfig.json")) 
            {
                File.AppendAllText("UserData\\RemoteReuploadConfig.json", JsonConvert.SerializeObject(new Config() { API_Key = "Replace Me if provided with one", API_URL = "Replace me", Group = "This can be anything you want :)"}, Formatting.Indented));
                return new Config() { API_Key = "Replace me if provided with one", API_URL = "Replace me", Group = "This can be anything you want :)" };
            }
            return JsonConvert.DeserializeObject<Config>(File.ReadAllText("UserData\\RemoteReuploadConfig.json"));
        }

        internal void Save() 
        {
            File.WriteAllText("UserData\\RemoteReuploadConfig.json", JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }
}