using Common;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
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
            id = info.GetValueString((int)SPC.Id);
            respondAction = (NotficationRespondAction)info.GetValueInt((int)SPC.RespondAction);
            sourceServiceType = (NotificationSourceServiceType)info.GetValueInt((int)SPC.SourceServiceType);
            text = info.GetValueString((int)SPC.Text);
            data = info.GetValueHash((int)SPC.Data);
            handled = info.GetValueBool((int)SPC.Handled);
            subType = (NotificationSubType)(int)info.GetValueInt((int)SPC.SubType, (int)NotificationSubType.Unknown);
        }


        public FormatParameters formatParameters {
            get {
                if (subType == NotificationSubType.InviteToGroup) {
                    string login = data.GetValue<string>((int)SPC.Login, string.Empty);
                    return new FormatParameters { key = text, argumentParameters = new string[] { login } };
                }
                return new FormatParameters { key = text, argumentParameters = new string[] { } };
            }
        }
    }
}
