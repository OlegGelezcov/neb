namespace Nebula.Client.Res
{
    public class ResZoneInfo
    {
        private readonly string id;
        private readonly string scene;
        private readonly string displayName;

        private readonly bool isNull;

        public ResZoneInfo(string id, string scene, string displayName)
        {
            this.id = id;
            this.scene = scene;
            this.displayName = displayName;
            this.isNull = (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(scene));
        }

        public string Id()
        {
            return this.id;
        }

        public string Scene()
        {
            return this.scene;
        }

        public string DisplayName()
        {
            return this.displayName;
        }

        public bool IsNull()
        {
            return this.isNull;
        }

        public static ResZoneInfo Null()
        {
            return new ResZoneInfo(string.Empty, string.Empty, string.Empty);
        }
    }
}
