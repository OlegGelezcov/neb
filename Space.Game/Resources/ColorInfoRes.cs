using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using Space.Game.Resources;
using GameMath;

namespace Space.Game.Resources
{
    public class ColorInfoRes
    {
        private Dictionary<ColoredObjectType, List<ColorInfo>> colors;


        public void Load(string basePath )
        {
            this.colors = new Dictionary<ColoredObjectType,List<ColorInfo>>();

            string fullPath = Path.Combine(basePath, "Data/Drop/colors.xml");
            XDocument document = XDocument.Load(fullPath);

            XElement colorsElement = document.Element("colors");

            this.colors.Add(ColoredObjectType.Weapon, GetColorInfoList(colorsElement.Element("weapon")));
            this.colors.Add(ColoredObjectType.Module, GetColorInfoList(colorsElement.Element("module")));
            this.colors.Add(ColoredObjectType.Scheme, GetColorInfoList(colorsElement.Element("scheme")));

        }

        private List<ColorInfo> GetColorInfoList(XElement parent)
        {
            return parent.Elements("color").Select(e =>
                {
                    return new ColorInfo
                    {
                        color = (ObjectColor)Enum.Parse(typeof(ObjectColor), e.Attribute("name").Value),
                        factor = e.GetFloat("factor"),
                        prob = e.GetFloat("prob")
                    };
                }).ToList();
        }



        public ColorInfo Color(ColoredObjectType type, ObjectColor color) {
            List<ColorInfo> list;
            if (this.colors.TryGetValue(type, out list))
                return list.FirstOrDefault(c => c.color == color);
            return null;
        }

        public ColorInfo White(ColoredObjectType type) {
            return this.Color(type, ObjectColor.white);
        }

        public ColorInfo Blue(ColoredObjectType type) {
            return this.Color(type, ObjectColor.blue);
        }

        public ColorInfo Yellow(ColoredObjectType type) {
            return this.Color(type, ObjectColor.yellow);
        }

        public ColorInfo Green(ColoredObjectType type) {
            return this.Color(type, ObjectColor.green);
        }

        public ColorInfo Orange(ColoredObjectType type) {
            return this.Color(type, ObjectColor.orange);
        }

        private float[] RemapWeights(float[] sourceWeights, float remapWeight) {
            remapWeight = Mathf.Clamp01(remapWeight / 2.0f);
            float val = sourceWeights[sourceWeights.Length - 1] * remapWeight;
            sourceWeights[sourceWeights.Length - 1] -= val;
            float unitVal = val / (sourceWeights.Length - 1);
            for(int i = 0; i < sourceWeights.Length - 1; i++) {
                sourceWeights[i] += unitVal;
            }
            return sourceWeights;
        }

        public ColorInfo GenColor(ColoredObjectType type, float remapWeight = 0f)
        {
            float accum = 0f;
            float[] weights = new float[5];
            weights[0] = Orange(type).prob;
            accum += weights[0];
            weights[1] = Green(type).prob;
            accum += weights[1];
            weights[2] = Yellow(type).prob;
            accum += weights[2];
            weights[3] = Blue(type).prob;
            accum += weights[3];
            weights[4] = 1f - accum;

            weights = RemapWeights(weights, remapWeight);

            int index = Rand.RandomIndex(weights);
            switch(index) {
                case 0:
                    return Orange(type);
                case 1:
                    return Green(type);
                case 2:
                    return Yellow(type);
                case 3:
                    return Blue(type);
                default:
                    return White(type);
            }
        }

        public bool TryGetColorList(ColoredObjectType type, out List<ColorInfo> oColors) {
            return colors.TryGetValue(type, out oColors);
        }
    }
}
