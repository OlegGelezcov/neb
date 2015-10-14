using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ServerClientCommon;

namespace Nebula.Client.Notifications {
    public class Notification : IInfoParser {

        public NotficationRespondAction respondAction { get; private set; }
        public NotificationSourceServiceType sourceServiceType { get; private set; }
        public string id { get; private set; }
        public string text { get; private set; }
        public Hashtable data { get; private set; }
        public bool handled { get; private set; }
        public NotificationSubType subType { get; private set; }

        public Notification(Hashtable info) { ParseInfo(info); }

        public void ParseInfo(Hashtable info) {
            id                  = info.Value<string>((int)SPC.Id);
            respondAction       = (NotficationRespondAction)info.Value<int>((int)SPC.RespondAction);
            sourceServiceType   = (NotificationSourceServiceType)info.Value<int>((int)SPC.SourceServiceType);
            text                = info.Value<string>((int)SPC.Text);
            data                = info.Value<Hashtable>((int)SPC.Data);
            handled             = info.Value<bool>((int)SPC.Handled);
            subType             = (NotificationSubType)(int)info.GetValue<int>((int)SPC.SubType, (int)NotificationSubType.Unknown);
        }


        public FormatParameters formatParameters {
            get {
                if(subType == NotificationSubType.InviteToGroup) {
                    string login = data.GetValue<string>((int)SPC.Login, string.Empty);
                    return new FormatParameters { key = text, argumentParameters = new string[] { login } };
                }
                return new FormatParameters { key = text, argumentParameters = new string[] { } };
            }
        }
    }
}
