using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Collections;


namespace Space.Game.Resources.Zones
{
    //public class WeaponSettingsRes : IResourceLoader
    //{
    //    private bool loaded = false;
    //    private Dictionary<WeaponDamageType, Dictionary<Workshop, WorkshopWeaponSettings>> settings;
    //    private Dictionary<Difficulty, float> difficultyTable;

    //    public int TotalSettingsCount()
    //    {
    //        int count = 0;
    //        foreach(var p1 in settings)
    //        {
    //            foreach(var p2 in p1.Value)
    //            {
    //                count++;
    //            }
    //        }
    //        return count;
    //    }

    //    public int DifficultyTableCount()
    //    {
    //        int count = 0;
    //        foreach(var p in difficultyTable)
    //        {
    //            count++;
    //        }
    //        return count;
    //    }

    //    public bool Load(string basePath)
    //    {

    //        settings = new Dictionary<WeaponDamageType, Dictionary<Workshop, WorkshopWeaponSettings>>();
    //        string fullPath = Path.Combine(basePath, "Data/Drop/weapon_settings.xml");
    //        XDocument document = XDocument.Load(fullPath);
    //        var dumpList = document.Element("settings").Elements("inputs").Select(e =>
    //        {
    //            WeaponDamageType damageType = (WeaponDamageType)Enum.Parse(typeof(WeaponDamageType), e.Attribute("weapon_type").Value);
    //            Workshop workshop = (Workshop)Enum.Parse(typeof(Workshop), e.Attribute("workshop").Value);
    //            Hashtable settingsObject = new Hashtable();
    //            var dumpList2 = e.Elements("input").Select(e2 =>
    //            {
    //                string key = e2.Attribute("key").Value;
    //                settingsObject.Add(key, CommonUtils.ParseValue(e2.Attribute("value").Value, e2.Attribute("type").Value));
    //                return key;
    //            }).ToList();

    //            WorkshopWeaponSettings sObj = new WorkshopWeaponSettings(settingsObject);
    //            AddSettings(damageType, workshop, sObj);
    //            return sObj;
    //        }).ToList();

    //        this.difficultyTable = document.Element("settings").Element("difficulty_table").Elements("difficulty").Select(e =>
    //        {
    //            Difficulty key = (Difficulty)Enum.Parse(typeof(Difficulty), e.Attribute("name").Value);
    //            float val = e.Attribute("value").Value.ToFloat();
    //            return new { KEY = key, VALUE = val };
    //        }).ToDictionary(obj => obj.KEY, obj => obj.VALUE);

    //        loaded = true;

    //        return loaded;
    //    }

    //    private void AddSettings(WeaponDamageType damageType, Workshop workshop, WorkshopWeaponSettings settingsObject )
    //    {
    //        Dictionary<Workshop, WorkshopWeaponSettings> filteredSettingsBytType = null;
    //        if(!this.settings.TryGetValue(damageType, out filteredSettingsBytType))
    //        {
    //            filteredSettingsBytType = new Dictionary<Workshop, WorkshopWeaponSettings>();
    //            this.settings.Add(damageType, filteredSettingsBytType);
    //        }
    //        filteredSettingsBytType.Add(workshop, settingsObject);
    //    }

    //    public bool Loaded
    //    {
    //        get { return this.loaded; }
    //    }

    //    public bool TryGetSettings(WeaponDamageType damageType, Workshop workshop, out WorkshopWeaponSettings settingsObject)
    //    {
    //        settingsObject = null;
    //        Dictionary<Workshop, WorkshopWeaponSettings> filteredSettings = null;
    //        if(this.settings.TryGetValue(damageType, out filteredSettings))
    //        {
    //            return filteredSettings.TryGetValue(workshop, out settingsObject);
    //        }
    //        return false;
    //    }

    //    public bool TryGetDifficultyMult(Difficulty difficulty, out float result)
    //    {
    //        result = 1.0f;
    //        return this.difficultyTable.TryGetValue(difficulty, out result);
    //    }
    //}
}
