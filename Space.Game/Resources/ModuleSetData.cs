// ModuleSetData.cs
// Nebula
// 
// Created by Oleg Zheleztsov on Monday, January 26, 2015 11:29:41 AM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using System.Collections.Generic;

namespace Space.Game.Resources
{
    public class ModuleSetData
    {
        private readonly string id;
        private readonly string name;
        private readonly int unlockLevel;
        private readonly float dropProb;
        private readonly bool isDefault;
        private int skillID;
        private readonly Workshop workshop;


        public ModuleSetData(string id, string name, int unlockLevel, float dropProb, bool isDefault, int skillID, Workshop w)
        {
            this.id = id;
            this.name = name;
            this.unlockLevel = unlockLevel;
            this.dropProb = dropProb;
            this.isDefault = isDefault;
            this.skillID = skillID;
            this.workshop = w;
        }

        public string Id
        {
            get
            {
                return this.id;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public int UnlockLevel
        {
            get
            {
                return this.unlockLevel;
            }
        }

        public float DropProb
        {
            get
            {
                return this.dropProb;
            }
        }
        
        public bool IsDefault {
            get {
                return isDefault;
            }
        } 
        
        public int Skill {
            get {
                return skillID;
            }
        }   
        
        public Workshop Workshop {
            get {
                return workshop;
            }
        }    
    }
}
