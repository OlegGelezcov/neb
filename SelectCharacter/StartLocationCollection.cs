using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Common;
using ExitGames.Logging;

namespace SelectCharacter {
    public class StartLocationCollection : List<StartLocation> {

        private static ILogger log = LogManager.GetCurrentClassLogger();

        public void LoadFromFile(string file) {
            try {
                XDocument document = XDocument.Load(file);
                var dumpArray = document.Element("start_locations").Elements("location").Select(e => {
                    Race race = (Race)Enum.Parse(typeof(Race), e.GetString("race"));
                    Workshop workshop = (Workshop)Enum.Parse(typeof(Workshop), e.GetString("workshop"));
                    string worldID = e.GetString("world");
                    return new StartLocation {
                        Race = race,
                        Workshop = workshop,
                        WorldId = worldID
                    };
                }).ToArray();
                Clear();
                AddRange(dumpArray);
            } catch(Exception exception) {
                log.Error("exception", exception);
            }
        }

        public string GetWorld(Race race, Workshop workshop ) {
            foreach(var startLocation in this ) {
                if(startLocation.Race == race && startLocation.Workshop == workshop) {
                    return startLocation.WorldId;
                }
            }
            return string.Empty;
        }

    }
}
