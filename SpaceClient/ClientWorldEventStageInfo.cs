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
    public class ClientWorldEventStageInfo : IInfoParser
    {
        public int StageId { get; private set; }
        public string StartTextId { get; private set; }
        public string TaskTextId { get; private set; }
        public bool IsFinal { get; private set; }
        public bool IsSuccess { get; private set; }
        public int Timeout { get; private set; }

        public ClientWorldEventStageInfo()
        {

        }


        public void ParseInfo(Hashtable info)
        {
            this.StageId = info.GetValue<int>((int)SPC.StageId, 0);
            this.StartTextId = info.GetValue<string>((int)SPC.StartText, string.Empty);
            this.TaskTextId = info.GetValue<string>((int)SPC.TaskText, string.Empty);
            this.IsFinal = info.GetValue<bool>((int)SPC.IsFinal, false);
            this.IsSuccess = info.GetValue<bool>((int)SPC.IsSuccess, false);
            this.Timeout = info.GetValue<int>((int)SPC.Timeout, -1);
        }
    }
}
*/