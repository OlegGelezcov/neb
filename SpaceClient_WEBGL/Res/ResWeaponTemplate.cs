namespace Nebula.Client.Res
{
    using Common;
    using global::Common;

    public class ResWeaponTemplate
    {
        private readonly string id;
        private readonly Workshop workshop;
        private readonly string name;
        private readonly string description;

        public ResWeaponTemplate(string id, Workshop workshop, string name, string description )
        {
            this.id = id;
            this.workshop = workshop;
            this.name = name;
            this.description = description;
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

        public Workshop Workshop
        {
            get
            {
                return this.workshop;
            }
        }

        public string Description {
            get {
                return this.description;
            }
        }
    }
}
