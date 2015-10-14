//#define FILTERS
//#define BALANCE
//#define SCRIPT
using Common;
using ServerClientCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Client.Servers;
using TestClient.SelectCharacterTests;
using TestClient.Scripts;
using System.Text.RegularExpressions;
using Space.Game;
using Space.Game.Drop;
using Space.Game.Ship;
using TestClient.TestDatabase;
using TestClient.PlayMarketReviewReader;
using TestClient.TestDropping;

namespace TestClient {
    public class ClientApplication {

        public readonly static ClientServerCollection Servers = new ClientServerCollection();
        public readonly static Client client = new Client();

        private static DisconnectAction disconnectAction = DisconnectAction.None;
        private static string gameRefId = null;
        private static GameState gameState = TestClient.GameState.Null;
        private static Hashtable playerCharacters = null;
        private static string selectedCharacterId = null;



        static void Main(string[] args)
        {
            //var reader = ReviewReader.Create();
            //foreach(var review in reader.reviews) {
            //    Console.WriteLine("{0}-{1}-{2}", review.rating, review.title, review.text);
            //}

            TestDroppingProbs testDroppingProbs = new TestDroppingProbs();
            testDroppingProbs.TestModifyWeights();
        }

        private static void OldTests() {
#if CONCURRENT_BAG
            ConcurrentTests.TestConcurrentBag();
#endif
#if FILTERS
            TestFilters.run();
#endif
#if BALANCE
            TestBalance.TestBalance.Speed();
            TestBalance.TestBalance.Distance();
#endif
#if SCRIPT
            TestDSL.Test();
#endif
            //DropSetsModules testModules = new DropSetsModules();
            //testModules.Test();
            //TestExpr();

            /*
            Res res = new Res(@"C:\development\Nebula\TestClient\bin\Debug");
            res.Load();
            var dropManager = DropManager.Get(res);
            for (int i = 0; i < 20; i++) {
                ModuleDropper moduleDropper = null;
                ShipModule prevModule = null;
                var CB = res.ModuleTemplates.RandomModule(Workshop.Yshuar, ShipModelSlotType.ES);
                var CBParams = new ModuleDropper.ModuleDropParams(
                    res,
                    CB.Id,
                    1,
                    Difficulty.none,
                    new Dictionary<string, int>(),
                    ObjectColor.white,
                    string.Empty
                    );
                moduleDropper = dropManager.GetModuleDropper(CBParams);
                ShipModule module = dropManager.GetModuleDropper(CBParams).Drop() as ShipModule;
                Console.WriteLine("template: " + module.TemplateModuleId);



            }*/


        }

        private static void TestExpr() {
            string str = "Englishcharacters";
            if (Regex.IsMatch(str, "^[a-zA-Z0-9]*$")) {
                Console.WriteLine("all ok");
            } else {
                Console.WriteLine("all bad");
            }
        }


        public static void SetSelectedCharacterId(string cid) {
            selectedCharacterId = cid;
        }

        public static string SelectedCharacterId {
            get {
                return selectedCharacterId;
            }
        }

        public static string GameRefId {
            get {
                return gameRefId;
            }
        }

        public static GameState GameState {
            get {
                return gameState;
            }
        }

        public static void SetGameState(TestClient.GameState state) {
            gameState = state;
        }

        public static DisconnectAction DisconnectAction {
            get {
                return disconnectAction;
            }
        }

        public static void SetDisconnectAction(DisconnectAction onDisconnect ) {
            disconnectAction = onDisconnect;
        }

        public static void ResetDisconnectAction() {
            disconnectAction = DisconnectAction.None;
        }

        private static Random rand = new Random();


        private static T GetTemplatedString<T>() {
            string s = null;
            return (T)(object)s;
        }

        private static void RunServerClient() {

            Events.ServersReceived += Client_ServersReceived;
            Events.GameRefIdReceived += Events_GameRefIdReceived;
            Events.PlayerCharactersReceived += Events_PlayerCharactersReceived;
            Events.CharacterCreated += Events_CharacterCreated;
            Events.CharacterSelected += Events_CharacterSelected;
            client.Setup();
            client.Run();
        }

        private static void Events_CharacterSelected(string obj) {
            SetSelectedCharacterId(obj);
            Console.WriteLine("Character selected {0}", obj);
        }

        private static void Events_CharacterCreated(string obj) {
            selectedCharacterId = obj;
            Console.WriteLine("Character created {0}", selectedCharacterId);
        }

        private static void Events_PlayerCharactersReceived(Hashtable obj) {
            playerCharacters = obj;
            if(playerCharacters == null ) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Player characters null");
            } else {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Player characters:");
                StringBuilder builder = new StringBuilder();
                CommonUtils.ConstructHashString(playerCharacters, 1, ref builder);
                Console.WriteLine(builder.ToString());
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void Events_GameRefIdReceived(string obj) {
            Console.WriteLine("Received game ref id: " + obj);
            gameRefId = obj;
        }

        private static void Client_ServersReceived(List<ServerInfo> servers) {
            foreach(var server in servers) {
                Console.WriteLine(server.ToString());
            }
            Servers.Clear();
            Servers.AddRange(servers);
        }

        public static void SendLogin() {
            client.Peer.OpCustom((byte)OperationCode.Login, new Dictionary<byte, object> {
                { (byte)ParameterCode.LoginId, "123456789" },
                { (byte)ParameterCode.AccessToken, "abrakadabra" },
                { (byte)ParameterCode.DisplayName, "Oleg" }
            }, true);
        }

        public static void OnCompleteDisconnectAction() {
            client.ExecuteDisconnectAction(disconnectAction);
        }

        public static string[] CharacterIds() {
            if(playerCharacters == null) {
                return new string[] { };
            }
            Hashtable chs = playerCharacters.Value<Hashtable>((int)SPC.Characters, new Hashtable());
            if(chs.Count == 0) {
                return new string[] { };
            }

            List<string> ids = new List<string>();
            foreach(var cid in chs.Keys) {
                ids.Add(cid.ToString());
            }
            return ids.OrderBy(i => i).ToArray();
        }

        public static int CharacterCount() {
            if (playerCharacters == null) {
                return 0;
            }
            Hashtable chs = playerCharacters.Value<Hashtable>((int)SPC.Characters, new Hashtable());
            return chs.Count;
        }
    }
}
