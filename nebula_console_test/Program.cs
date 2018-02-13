
using nebula_console_test.Contracts;
using Space.Game.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Console;

namespace nebula_console_test {
    class Program {
        static void Main(string[] args) {
            ParseQuestFile();
        }

        static void PrintModuleSettings() {
            Console.WriteLine("START");
            ModuleSettingsRes res = new ModuleSettingsRes();
            res.Load(string.Empty);
            Console.WriteLine(res.ToString());
        }

        private static void ParseQuestFile() {
            string path = @"C:\Users\olegg\Documents\Nebula_PC\Assets\Resources\Data\quests.xml";
            XDocument document = XDocument.Load(path);
            List<Nebula.Quests.QuestData> quests = new List<Nebula.Quests.QuestData>();
            foreach(var questElement in document.Element("quests").Elements("quest")) {
                Nebula.Quests.QuestData quest = new Nebula.Quests.QuestData();
                quest.Load(questElement);
                quests.Add(quest);
            }

            quests.ForEach(q => WriteLine(q));

        }
    }
}
