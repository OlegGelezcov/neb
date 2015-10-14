/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Collections;
using ServerClientCommon;

namespace Nebula.Client
{
    public class ClientWorldEventInfo : IInfoParser
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public float Cooldown { get; private set; }
        public int Exp { get; private set; }
        public int Coins { get; private set; }
        public string WorldId { get; private set; }

        public bool Active { get; private set; }

        public Hashtable Specific { get; private set; }

        public Hashtable Inputs { get; private set; }

        public float[] Position { get; private set; }

        public ClientWorldEventStageInfo Stage { get; private set; }

        public Hashtable VariablesInfo { get; private set; }

        public ClientWorldEventInfo(Hashtable info )
        {
            this.Stage = new ClientWorldEventStageInfo();
            this.ParseInfo(info);
        }

        public void ParseInfo(Hashtable info)
        {
            this.Id = info.GetValue<string>((int)SPC.Id, string.Empty);
            this.Name = info.GetValue<string>((int)SPC.Name, string.Empty);
            this.Description = info.GetValue<string>((int)SPC.Description, string.Empty);
            this.Cooldown = info.GetValue<float>((int)SPC.Cooldown, 0.0f);
            this.Exp = info.GetValue<int>((int)SPC.Exp, 0);
            this.Coins = info.GetValue<int>((int)SPC.Credits, 0);
            this.WorldId = info.GetValue<string>((int)SPC.WorldId, string.Empty);
            this.Specific = info.GetValue<Hashtable>((int)SPC.SpecificInfo, new Hashtable());
            this.Active = info.GetValue<bool>((int)SPC.Active, false);
            this.Inputs = info.GetValue<Hashtable>((int)SPC.Inputs, new Hashtable());
            this.Position = info.GetValue<float[]>((int)SPC.Position, new float[] { 0f, 0f, 0f });

            Hashtable currentStageInfo = info.GetValue<Hashtable>((int)SPC.CurrentStage, new Hashtable());
            this.Stage.ParseInfo(currentStageInfo);

            this.VariablesInfo = info.GetValue<Hashtable>((int)SPC.Variables, new Hashtable());
        }

        public bool IsFinalStage()
        {
            if (this.Stage == null)
                return false;
            return this.Stage.IsFinal;
        }

        public string StageTaskTextId()
        {
            if (this.Stage == null)
                return string.Empty;
            return this.Stage.TaskTextId;
        }

        public string ReplaceTextWithVAriables(string sourceText)
        {
            if (string.IsNullOrEmpty(sourceText))
                return string.Empty;
            if (this.VariablesInfo == null)
                return sourceText;
            if (this.VariablesInfo.Count == 0)
                return sourceText;
            return sourceText.ReplaceVariables(this.VariablesInfo);
        }
    }
}
*/