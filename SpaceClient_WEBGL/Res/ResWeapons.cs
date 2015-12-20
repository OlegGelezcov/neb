namespace Nebula.Client.Res
{
    using Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;
#if UP
    using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

    public class ResWeapons
    {
        private Dictionary<string, ResWeaponTemplate> weapons;

        public ResWeapons()
        {
            this.weapons = new Dictionary<string, ResWeaponTemplate>();
        }

        public void Load(string xml)
        {
#if UP
            UPXDocument document = new UPXDocument(xml);
#else
            XDocument document = XDocument.Parse(xml);
#endif
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
