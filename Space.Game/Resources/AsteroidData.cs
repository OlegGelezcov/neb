using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Space.Game.Resources
{
    public class AsteroidData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Quality { get; set; }
        public List<AsteroidMaterialData> ContentData { get; set; }


        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(this.Id); }
        }

        public static AsteroidData Empty
        {
            get
            {
                return new AsteroidData
                {
                    ContentData = new List<AsteroidMaterialData>(),
                    Id = string.Empty,
                    Name = string.Empty,
                    Quality = 0
                };
            }
        }
    }
}
