using Common;
using System.Collections.Generic;
using System.Linq;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace ServerClientCommon {
    public class BankSlotPriceCollection {

        public List<BankSlotPrice> prices { get; private set; }
        public int maxCount { get; private set; }

        public BankSlotPriceCollection() {
            prices = new List<BankSlotPrice>();
        }
#if UP
        public void LoadFromFile(string path) {
            UPXDocument document = new UPXDocument();
            document.LoadFromFile(path);
            LoadDocument(document);
        }

        public void LoadFromText(string text) {
            UPXDocument document = new UPXDocument(text);
            LoadDocument(document);
        }

        private void LoadDocument(UPXDocument document) {
            maxCount = document.Element("slots").GetAttributeInt("max_count");
            prices = document.Element("slots").Elements("slot").Select(slotElement => {
                return new BankSlotPrice(slotElement);
            }).ToList();
        }
#else
        public void LoadFromFile(string path) {
            XDocument document = XDocument.Load(path);
            LoadDocument(document);
        }

        public void LoadFromText(string text) {
            XDocument document = XDocument.Parse(text);
            LoadDocument(document);
        }

        private void LoadDocument(XDocument document) {
            maxCount = document.Element("slots").GetInt("max_count");
            prices = document.Element("slots").Elements("slot").Select(slotElement => {
                return new BankSlotPrice(slotElement);
            }).ToList();
        }
#endif

        public BankSlotPrice GetPriceForSlots(int slots ) {
            if(slots >= maxCount ) {
                return null;
            }

            foreach(var p in prices ) {
                if(p.slots == slots ) {
                    return p;
                }
            }
            return null;
        }
    }
}
