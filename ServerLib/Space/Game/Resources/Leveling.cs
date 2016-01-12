using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Common;
using System.IO;
using GameMath;
using Nebula.Server.Space.Game.Resources;

namespace Space.Game.Resources
{
    public class Leveling : ILeveling
    {
        private int levelCap;
        private Dictionary<int, int> levelExpDictionary = new Dictionary<int, int>();

        public void Load(string basePath)
        {
            string fullPath = Path.Combine(basePath, "Data/leveling.xml");
            XDocument document = XDocument.Load(fullPath);
            this.LoadFromDocument(document);
        }

        public void LoadFromXmlText(string xml)
        {
            XDocument document = XDocument.Parse(xml);
            this.LoadFromDocument(document);
        }

        private void LoadFromDocument(XDocument document)
        {
            this.levelExpDictionary.Clear();
            this.levelCap = document.Element("leveling").Attribute("level_cap").ToInt();
            this.levelExpDictionary = document.Element("leveling").Elements("data").Select(e =>
                {
                    int level = e.Attribute("level").ToInt();
                    int expForLevel = e.Attribute("exp_for_level").ToInt();
                    return new { Level = level, ExpForLevel = expForLevel };
                }).ToDictionary(obj => obj.Level, obj => obj.ExpForLevel);
        }



        public int LevelForExp(int exp)
        {
            int clampedExp = MathFunc.Clamp(exp, 0, CapExp());
            if (clampedExp == 0)
                return 1;
            if (clampedExp == CapExp())
                return levelCap;
            for(int i = 1; i < levelCap; i++ )
            {
                int curExp = this.levelExpDictionary[i];
                int nextExp = this.levelExpDictionary[i + 1];
                if (clampedExp >= curExp && clampedExp < nextExp)
                    return i;
            }
            throw new Exception("Error in LevelForExp for exp: {0}".f(exp));
        }

        public int ExpToNextLevel(int currentLevel)
        {
            int clampedLevel = MathFunc.Clamp(currentLevel, 1, this.levelCap);
            if (clampedLevel == 1)
                return this.levelExpDictionary[clampedLevel + 1] - this.levelExpDictionary[clampedLevel];
            if (clampedLevel == this.levelCap)
                return 0;
            return this.levelExpDictionary[clampedLevel + 1] - this.levelExpDictionary[clampedLevel];
        }

        public int ExpForLevel(int level)
        {
            int clampedLevel = MathFunc.Clamp(level, 1, levelCap);
            return this.levelExpDictionary[clampedLevel];
        }

        public int RemainedExpForNextLevel(int currentExp)
        {
            int clampedExp = MathFunc.Clamp(currentExp, 0, CapExp());
            int currentLevel = LevelForExp(clampedExp);
            int nextLevelExp = ExpForLevel(currentLevel + 1);
            return nextLevelExp - clampedExp;
        }

        public float LevelProgress(int exp)
        {
            int levelForExp = LevelForExp(exp);
            int levelExp = ExpForLevel(levelForExp);
            float expDiff = exp - levelExp;
            float expDelta = ExpToNextLevel(levelForExp);
            if (expDelta == 0)
                return 1f;
            return MathFunc.Clamp01( (float)expDiff / (float)expDelta );
        }

        private int CapExp()
        {
            return this.levelExpDictionary[this.levelCap];
        }

        public int CapLevel()
        {
            return this.levelCap;
        }

        public Dictionary<int, int> LevelExp
        {
            get { return this.levelExpDictionary; }
        }
    }
}
