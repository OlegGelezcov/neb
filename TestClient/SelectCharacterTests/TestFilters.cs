using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Collections.Concurrent;

namespace TestClient.SelectCharacterTests {
    public class TestFilters {
        public static void run() {
            Nebula.Client.Auction.AuctionRequest clientRequest = new Nebula.Client.Auction.AuctionRequest();
            clientRequest.SetFilter(new Nebula.Client.Auction.ObjectTypeFilter { objectType = Common.AuctionObjectType.Module });
            clientRequest.SetFilter(new Nebula.Client.Auction.ObjectTypeFilter { objectType = Common.AuctionObjectType.Ore });
            clientRequest.SetFilter(new Nebula.Client.Auction.ColorFilter { color = Common.ObjectColor.blue });
            clientRequest.SetFilter(new Nebula.Client.Auction.ColorFilter { color = Common.ObjectColor.green });
            clientRequest.SetFilter(new Nebula.Client.Auction.WorkshopFilter { workshop = Common.Workshop.Arlen });
            clientRequest.SetFilter(new Nebula.Client.Auction.WorkshopFilter { workshop = Common.Workshop.BigBang });
            Console.WriteLine("===========Client filters:");
            printClientFilters(clientRequest.filters);

            Console.WriteLine("===========Client filters hash:");
            Console.WriteLine(clientRequest.FilterHash().ToStringBuilder().ToString());

            SelectCharacter.Auction.PlayerAuctionRequest serverRequest = new SelectCharacter.Auction.PlayerAuctionRequest();
            var newFilters = SelectCharacter.Auction.AuctionFilter.GetList(clientRequest.FilterHash());
            serverRequest.SetFilters(newFilters);
            Console.WriteLine("===========Server filters:");
            printServerFilters(serverRequest.filters);

            Console.WriteLine("Adding 2 filters");
            clientRequest.SetFilter(new Nebula.Client.Auction.WorkshopFilter { workshop = Common.Workshop.DarthTribe });
            clientRequest.SetFilter(new Nebula.Client.Auction.WorkshopFilter { workshop = Common.Workshop.Dyneira });

            Console.WriteLine("===========Client filters:");
            printClientFilters(clientRequest.filters);

            newFilters = SelectCharacter.Auction.AuctionFilter.GetList(clientRequest.FilterHash());
            serverRequest.SetFilters(newFilters);
            Console.WriteLine("===========Server filters:");
            printServerFilters(serverRequest.filters);

            Console.WriteLine("Removing 2 filters");
            clientRequest.RemoveFilter(new Nebula.Client.Auction.ColorFilter { color = ObjectColor.blue });
            clientRequest.RemoveFilter(new Nebula.Client.Auction.ColorFilter { color = ObjectColor.green });
            Console.WriteLine("===========Client filters:");
            printClientFilters(clientRequest.filters);

            newFilters = SelectCharacter.Auction.AuctionFilter.GetList(clientRequest.FilterHash());
            serverRequest.SetFilters(newFilters);
            Console.WriteLine("===========Server filters:");
            printServerFilters(serverRequest.filters);

        }

        private static void printClientFilters(Dictionary<byte, List<Nebula.Client.Auction.AuctionFilter>> filters )  {
            StringBuilder builder = new StringBuilder();
            foreach(var pair in filters ) {
                builder.Append(string.Format("{0}: ", (Common.AuctionFilterType)pair.Key));
                foreach(var filter in pair.Value) {
                    builder.Append(filter.ToString() + ", ");
                }
                builder.AppendLine();
            }
            Console.WriteLine(builder.ToString());
        }

        private static void printServerFilters(ConcurrentDictionary<AuctionFilterType, ConcurrentBag<SelectCharacter.Auction.AuctionFilter>> filters ) {
            StringBuilder builder = new StringBuilder();
            foreach(var pair in filters ) {
                builder.Append(string.Format("{0}: ", pair.Key));
                foreach (var filter in pair.Value) {
                    builder.Append(filter.ToString() + ", ");
                }
                builder.AppendLine();
            }
            Console.WriteLine(builder.ToString());
        }
    }
}
