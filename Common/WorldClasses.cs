using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Common
{
    public abstract class WorldObject
    {

        public string Id { get; set; }
        public string Name { get; set; }

        public byte Type { get; set; }

        public virtual Hashtable GetContent()
        {
            Hashtable result = new Hashtable();
            if (Id != null)
                result.Add("id", Id);
            if (Name != null)
                result.Add("name", Name);
            result.Add("type", Type);
            return result;
        }
    }

    public class StarWorldObject : WorldObject
    {

        public float Radius { get; set; }
        public float[] Position { get; set; }
        public string Prefab { get; set; }

        public override Hashtable GetContent()
        {
            Hashtable baseContent = base.GetContent();
            baseContent.Add("radius", Radius);
            if (Position != null)
                baseContent.Add("position", Position);
            if (Prefab != null)
                baseContent.Add("prefab", Prefab);
            return baseContent;
        }
    }

    public class PlanetWorldObject : WorldObject
    {
        public float Radius { get; set; }
        public float[] Position { get; set; }
        public string Prefab { get; set; }

        public override Hashtable GetContent()
        {
            Hashtable baseContent = base.GetContent();
            baseContent.Add("radius", Radius);
            if (Position != null)
                baseContent.Add("position", Position);
            if (Prefab != null)
                baseContent.Add("prefab", Prefab);
            return baseContent;
        }
    }

    public class PortalWorldObject : WorldObject
    {
        public float[] Position { get; set; }
        public string Prefab { get; set; }

        public override Hashtable GetContent()
        {
            Hashtable baseContent = base.GetContent();
            if (Position != null)
                baseContent.Add("position", Position);
            if (Prefab != null)
                baseContent.Add("prefab", Prefab);
            return baseContent;
        }
    }

    public class WorldMetrics
    {
        public float DistanceUnit { get; set; }
        public float ObjectRadiusUnit { get; set; }
    }
}
