using Common;
using ServerClientCommon;
using System.Collections;

namespace Nebula.Client
{
    public class ActivatorData : IInfo
    {
        public string Id { get; set; }
        public float[] Position { get; set; }

        public float Radius { get; set; }

        /// <summary>
        /// Type of activator Common.ActivatorType
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Name of action when activator toched
        /// </summary>
        public string Action { get; set; }

        public Hashtable GetInfo()
        {
            Hashtable info = new Hashtable();
            info.Add((int)SPC.Id, this.Id);
            info.Add((int)SPC.Position, this.Position);
            info.Add((int)SPC.Radius, this.Radius);
            info.Add((int)SPC.Type, this.Type);
            info.Add((int)SPC.Action, this.Action);
            return info;
        }

        public void ParseInfo(Hashtable info)
        {
            this.Id = info.GetValue<string>((int)SPC.Id, string.Empty);
            this.Position = info.GetValue<float[]>((int)SPC.Position, new float[] { });
            this.Radius = info.GetValue<float>((int)SPC.Radius, 0.0f);
            //this.Type = info.GetValue<int>((int)SPC.Type, ActivatorType.EVENT);
            this.Action = info.GetValue<string>((int)SPC.Action, string.Empty);
        }
    }
}
