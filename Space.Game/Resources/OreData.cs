using Common;

namespace Space.Game.Resources
{
    public class OreData : MaterialData
    {
        public override string Id
        {
            get;
            set;
        }

        public override string Name
        {
            get;
            set;
        }

        public override MaterialType Type
        {
            get 
            {
                return MaterialType.ore;
            }
        }

        public static OreData Empty
        {
            get
            {
                return new OreData { Id = string.Empty, Name = string.Empty };
            }
        }

        public bool IsEmpty
        {
            get
            {
                return string.IsNullOrEmpty(this.Id);
            }
        }
    }
}
