using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Common.Space.Game.Resources {
    public class FractionResolver {

        public Dictionary<FractionType, Dictionary<FractionType, FractionRelation>> relations { get; private set; }

        private void LoadFromDocument(XDocument document) {
            relations = new Dictionary<FractionType, Dictionary<FractionType, FractionRelation>>();

            relations = document.Element("fractions").Elements("fraction").Select(e => {
                var relations = e.Element("relations").Elements("relation").Select(e2 => {
                    FractionType toFraction = (FractionType)Enum.Parse(typeof(FractionType), e2.GetString("to"));
                    FractionRelation rel = (FractionRelation)Enum.Parse(typeof(FractionRelation), e2.GetString("value"));
                    return new { FRACTION = toFraction, RELATION = rel };
                }).ToDictionary(obj => obj.FRACTION, obj => obj.RELATION);

                FractionType sourceFraction = (FractionType)Enum.Parse(typeof(FractionType), e.GetString("name"));
                return new { SOURCEFRACTION = sourceFraction, SOURCERELATIONS = relations };
            }).ToDictionary(obj => obj.SOURCEFRACTION, obj => obj.SOURCERELATIONS);
        }

        public void Load(string basePath) {
            string fullPath = Path.Combine(basePath, "Data/fractions.xml");
            XDocument document = XDocument.Load(fullPath);
            LoadFromDocument(document);
        }

        public void LoadFromXmlText(string xml) {
            XDocument document = XDocument.Parse(xml);
            LoadFromDocument(document);
        }

        //public FractionRelation Relation(FractionType first, FractionType second) {
        //    return relations[first][second];
        //}

        public Dictionary<FractionType, FractionRelation> RelationFor(FractionType type) {
            return relations[type];
        }

        public Dictionary<FractionType, FractionRelation> RelationFor(int type) {
            return RelationFor((FractionType)type);
        }
    }


}

namespace Common {
    public static class FractionExtensions {
        public static FractionRelation RelationTo(this Dictionary<FractionType, FractionRelation> source, FractionType type) {
            return source[type];
        }
        public static FractionRelation RelationTo(this Dictionary<FractionType, FractionRelation> source, int type) {
            return RelationTo(source, (FractionType)type);
        }

    }
}
