using Common;
using GameMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Quests.Markers {
    public class MarkerParser {

        private StageMarker ParseMarker(UniXMLElement element ) {
            MarkerType type = (MarkerType)Enum.Parse(typeof(MarkerType), element.GetString("type"));

            switch(type) {
                case MarkerType.point: {
                        string world = element.GetString("world");
                        Vector3 point = element.GetFloatArray("point").ToVector3();
                        return new PointMarker(world, point);
                    }
                default:
                    return null;
            } 
        }

        public List<StageMarker> ParseMarkerList(UniXMLElement parent) {
            List<StageMarker> markers = new List<StageMarker>();
            if(parent == null ) {
                return markers;
            }

            var dump = parent.Elements("marker").Select(me => {
                StageMarker marker = ParseMarker(me);
                if (marker != null) {
                    markers.Add(marker);
                }
                return marker;
            }).ToList();
            return markers;
        }
    }
}
