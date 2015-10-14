namespace Nebula.Client.Res
{
    using Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    public class ResWeapons
    {
        private Dictionary<string, ResWeaponTemplate> weapons;

        public ResWeapons()
        {
            this.weapons = new Dictionary<string, ResWeaponTemplate>();
        }

        public void Load(string xml)
        {
            XDocument document = XDocument.Parse(xml);
            this.weapons = document.Element("weapons").Elements("weapon").Select(e =>
                {
                    string id = e.Attribute("id").Value;
                    Workshop workshop = (Workshop)Enum.Parse(typeof(Workshop), e.Attribute("workshop").Value);
                    string name = e.Attribute("name").Value;
                    string desc = e.Attribute("description").Value;
                    ResWeaponTemplate weapon = new ResWeaponTemplate(id, workshop, name, desc);
                    return new { ID = id, VALUE = weapon };
                }).ToDictionary(w => w.ID, w => w.VALUE);
        }

        public ResWeaponTemplate Weapon(string id)
        {
            if (this.weapons.ContainsKey(id))
                return this.weapons[id];
            return null;
        }
    }
}
