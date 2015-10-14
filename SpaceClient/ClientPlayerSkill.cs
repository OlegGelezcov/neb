// ClientPlayerSkill.cs
// Nebula
// 
// Created by Oleg Zhelestcov on Monday, November 10, 2014 2:16:53 PM
// Copyright (c) 2014 KomarGames. All rights reserved.
//


namespace Nebula.Client
{
    using Common;
    using ServerClientCommon;
    using System;
    using System.Collections;

    public class ClientPlayerSkill : IInfo
    {
        public int Id { get; private set; }
        public bool HasSkill { get; private set; }
        public float UseTime { get; private set; }
        public bool IsOn { get; private set; }
        public float Timer { get; private set; }
        public float Cooldown { get; private set; }
        public SkillType Type { get; private set; }

        public ClientPlayerSkill()
        {
            this.Id = -1;
            this.HasSkill = false;
            this.UseTime = 0f;
            this.IsOn = false;
            this.Timer = 0f;
            this.Cooldown = float.MaxValue;
            this.Type = SkillType.OneUse;
        }

        public ClientPlayerSkill(Hashtable info)
        {
            this.ParseInfo(info);
        }


        public Hashtable GetInfo()
        {
            throw new NotImplementedException();
        }

        public bool IsEmpty
        {
            get
            {
                return (Id == -1);
            }
        }

        public bool Ready
        {
            get {
                return this.Timer < 0f;
            }
        }

        public void ParseInfo(Hashtable info)
        {
            this.Id = info.GetValue<int>((int)SPC.Id, -1);
            this.HasSkill = info.GetValue<bool>((int)SPC.HasSkill, false);
            this.UseTime = info.GetValue<float>((int)SPC.UseTime, 0f);
            this.IsOn = info.GetValue<bool>((int)SPC.IsOn, false);
            this.Timer = info.GetValue<float>((int)SPC.Timer, 0f);
            this.Cooldown = info.GetValue<float>((int)SPC.Cooldown, 0f);
            this.Type = (SkillType)info.GetValue<byte>((int)SPC.SkillType, (byte)SkillType.OneUse);
        }

        public static ClientPlayerSkill Empty
        {
            get
            {
                return new ClientPlayerSkill();
            }
        }

        public float Progress()
        {
            if (this.Cooldown == 0f)
                return 0f;
            return this.Clamp01(this.Timer / this.Cooldown);
        }

        private float Clamp01(float t)
        {
            if (t < 0f)
                t = 0f;
            if (t > 1f)
                t = 1f;
            return t;
        }
    }
}
