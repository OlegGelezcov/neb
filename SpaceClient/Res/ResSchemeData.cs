using Common;

namespace Nebula.Client.Res {
    public class ResSchemeData {
        public readonly Workshop Workshop;
        public readonly string Icon;

        public ResSchemeData(Workshop workshop, string icon) {
            this.Workshop = workshop;
            this.Icon = icon;
        }
    }
}
