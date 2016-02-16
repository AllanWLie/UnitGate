/*
  Application Name:     Unitgate  
  Created by:           Allan Wamming Lie
  Created:              2014-2016
  Company:              Lie Development
  E-mail:               unitgate@allanlie.dk 
  Web:                  unitgate.eu
 */

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnitGate.Enums;

namespace UnitGate.Service
{
    internal class ConfigService
    {
        private string _pathBase; 
        private const string _pathFolderName = "UnitGate"; 
        private const string _generalConfigFile = "generalConfig.json";
        private const string _dataConfigFile = "dataConfig.json";
        //Singleton
        private static ConfigService instance;
        private readonly Dictionary<string, string> _dataConfig;
        private readonly Dictionary<string, string> _generalConfig;

        private ConfigService()
        {
            _pathBase = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            _generalConfig = new Dictionary<string, string>();
            _dataConfig = new Dictionary<string, string>();
           _generalConfig = ReadConfig(_generalConfigFile);

            _dataConfig = ReadConfig(_dataConfigFile);
            
        }

        public static ConfigService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ConfigService();
                }
                return instance;
            }
        }

        //Update event
        public event Action<KeyValuePair<string, string>> OnDataConfigUpdated;
        public event Action OnGeneralConfigUpdated;

        public Dictionary<string, string> GetConfigDictionary(ConfigTypes type)
        {
            var result = new Dictionary<string, string>();
            switch (type)
            {
                case ConfigTypes.General:
                    result = _generalConfig;
                    break;
                case ConfigTypes.Data:
                    result = _dataConfig;
                    break;
            }
            return result;
        }

        public string GetConfig(string key, ConfigTypes type)
        {
            var result = "";
            switch (type)
            {
                case ConfigTypes.General:
                    if (_generalConfig.ContainsKey(key))
                    {
                        result = _generalConfig[key];
                    }
                    break;
                case ConfigTypes.Data:
                    if (_dataConfig.ContainsKey(key))
                    {
                        result = _dataConfig[key];
                    }
                    break;
            }
            return result;
        }

        public void AppendConfig(string key, string data, ConfigTypes type, bool refresh = false)
        {
            switch (type)
            {
                case ConfigTypes.General:
                    if (_generalConfig.ContainsKey(key))
                    {
                        _generalConfig[key] = data;
                    }
                    else
                    {
                        _generalConfig.Add(key, data);
                    }

                    if (OnGeneralConfigUpdated != null)
                    {
                        OnGeneralConfigUpdated();
                    }
                    break;

                case ConfigTypes.Data:
                    if (_dataConfig.ContainsKey(key))
                    {
                        _dataConfig[key] = data;
                    }
                    else
                    {
                        _dataConfig.Add(key, data);
                        if (OnDataConfigUpdated != null)
                        {
                            OnDataConfigUpdated(new KeyValuePair<string, string>(key, data));
                        }
                    }
                   
                    
                    break;
            }
        }

        public void CommitConfig(ConfigTypes type)
        {
            switch (type)
            {
                case ConfigTypes.General:
                    WriteConfig(_generalConfig, _generalConfigFile);
                    break;
                case ConfigTypes.Data:
                    WriteConfig(_dataConfig, _dataConfigFile);
                    break;
            }
        }

        private Dictionary<string, string> ReadConfig(string file)
        {
            var result = new Dictionary<string, string>();

            try
            {
                string folderPath = Path.Combine(_pathBase, _pathFolderName);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string filePath = Path.Combine(folderPath, file);

                using (FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate))
                using (StreamReader reader = new StreamReader(stream))
                {
                    var json = reader.ReadToEnd();
                    result = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    if (result == null)
                    {
                        result = new Dictionary<string, string>();
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                ErrorHandlingService.PersistError("ConfigService - ReadConfig", ex);
                return result;
            }
        }

        private void WriteConfig(Dictionary<string, string> config, string file)
        {
            try
            {
                string folderPath = Path.Combine(_pathBase, _pathFolderName);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string filePath = Path.Combine(folderPath, file);

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    var json = JsonConvert.SerializeObject(config);
                    writer.Write(json);
                }
            }
            catch (Exception ex)
            {
                ErrorHandlingService.PersistError("ConfigService - WriteConfig", ex);
                
            }
        }
    }
}