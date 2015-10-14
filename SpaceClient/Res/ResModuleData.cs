

namespace Nebula.Client.Res
{
    using Common;
    using System.Collections.Generic;

    public class ResModuleData
    {
        public string Id { get; set; }
        public Workshop Workshop { get; set; }
        public ShipModelSlotType SlotType { get; set; }
        public string SetId { get; set; }
        public string Model { get; set; }

        public string NameId { get; set; }

        public string DescriptionId { get; set; }
    }
}
