﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace BoonsUp.Services;


[Serializable]
public class Persistance
{
    [JsonIgnore]
    public static string FILENAME = "resets.json";

    [JsonProperty("version")]
    public string Version { get; set; } = "1.0.0";

    [JsonProperty("lastKnownReset")]
    public Dictionary<string, DateTime> LastKnownReset { get; set; } = new();

    public DateTime GetEmpty() => new();

    public void SaveClear(string account)
    {
        LastKnownReset[account] = DateTime.UtcNow;
        Save();

    }

    public DateTime GetLastSave(string account)
    {
        DateTime date;
        if (!LastKnownReset.TryGetValue(account, out date))
        {
            // the key isn't in the dictionary.
            date = new();
        }

        return date;
    }


    public void Save()
    {
        var configFileInfo = GetConfigFileInfo();
        var serializedContents = JsonConvert.SerializeObject(this, Formatting.Indented);

        using var writer = new StreamWriter(configFileInfo.FullName);
        writer.Write(serializedContents);
        writer.Close();
    }

    private static FileInfo GetConfigFileInfo()
    {
        var pluginConfigDirectory = Service.DirectoriesManager.GetFullDirectoryPath(Module.DIRECTORY_PATH);

        return new FileInfo($@"{pluginConfigDirectory}\{FILENAME}");
    }

    public static Persistance Load()
    {
        if (GetConfigFileInfo() is { Exists: true } configFileInfo)
        {
            using var reader = new StreamReader(configFileInfo.FullName);
            var fileText = reader.ReadToEnd();
            reader.Close();

            return LoadExistingCharacterConfiguration(fileText);
        }
        else
        {
            return CreateNewCharacterConfiguration();
        }
    }

    private static Persistance LoadExistingCharacterConfiguration(string fileText)
    {
        var loadedCharacterConfiguration = JsonConvert.DeserializeObject<Persistance>(fileText);

        if (loadedCharacterConfiguration == null)
        {
            loadedCharacterConfiguration = new Persistance();
        }

        return loadedCharacterConfiguration;
    }

    private static Persistance CreateNewCharacterConfiguration()
    {
        var newCharacterConfiguration = new Persistance();
        newCharacterConfiguration.LastKnownReset.Add("default", newCharacterConfiguration.GetEmpty());
        newCharacterConfiguration.Save();
        return newCharacterConfiguration;
    }
}