using Common;
using ExitGames.Client.Photon;
using ServerClientCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestClient.Listeners;

namespace TestClient {

    public class Client  {

        public Dictionary<GameState, IPhotonPeerListener> listeners = new Dictionary<GameState, IPhotonPeerListener> {
            {GameState.Master, new MasterListener() },
            { GameState.Login, new LoginListener() },
            { GameState.SelectCharacter, new SelectCharacterListener() }
        };

        public void ExecuteDisconnectAction(DisconnectAction disconnectAction) {
            try {
                switch (disconnectAction) {
                    case DisconnectAction.None:
                        {
                            this.Peer = new PhotonPeer(listeners[GameState.Master], ConnectionProtocol.Udp);
#if LOCAL
                            this.Peer.Connect("192.168.1.28:5105", "Master");
#else
                            this.Peer.Connect("52.10.78.38:5105", "Master");
#endif
                            ClientApplication.SetGameState(GameState.Master);
                            break;
                        }
                    case DisconnectAction.ConnectToLoginServer:
                        {
                            var server = ClientApplication.Servers.GetServer(ServerType.login);
                            this.Peer = new PhotonPeer(listeners[GameState.Login], ConnectionProtocol.Udp);
                            this.Peer.Connect(server.IpAddress + ":" + server.Port, "Login");
                            ClientApplication.SetGameState(GameState.Login);
                            break;
                        }
                    case DisconnectAction.ConnectToSelectCharacterServer:
                        {
                            var server = ClientApplication.Servers.GetServer(ServerType.character);
                            this.Peer = new PhotonPeer(listeners[GameState.SelectCharacter], ConnectionProtocol.Udp);
                            this.Peer.Connect(server.IpAddress + ":" + server.Port, "SelectCharacter");
                            ClientApplication.SetGameState(GameState.SelectCharacter);
                            break;
                        }
                }
            }catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }
       
        public PhotonPeer Peer { get; private set; }

        public void Setup() {

            //this.Peer = new PhotonPeer(listeners[GameState.Master], ConnectionProtocol.Udp);
            //this.Peer.Connect("192.168.1.28:5105", "Master");
        }

        public void Run() {
            var buffer = new StringBuilder();
            while(true) {
                if(Peer != null)
                    Peer.Service();
                
                if(Console.KeyAvailable) {
                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleKeyInfo key = Console.ReadKey();
                    if (key.Key != ConsoleKey.Enter) {
                        buffer.Append(key.KeyChar);
                    } else {
                        Console.WriteLine();
                        string command = buffer.ToString();
                        if (command.ToLower().Trim() == "exit") {
                            this.Peer.Disconnect();
                            break;
                        } else {
                            this.ExecCommand(buffer.ToString());
                        }
                        buffer.Length = 0;
                    }
                }
            }
        }


        private void ExecCommand(string command) {
            string trimmedCommand = command.ToLower().Trim();
            string[] tokens = trimmedCommand.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if(tokens[0] == "connect" && tokens.Length > 1) {
                switch(tokens[1]) {
                    case "master":
                        ConnectToServer(ServerType.master);
                        break;
                    case "login":
                        ConnectToServer(ServerType.login);
                        break;
                    case "character":
                        ConnectToServer(ServerType.character);
                        break;
                }
            }

            switch(ClientApplication.GameState) {
                case GameState.Master:
                    {
                        MasterCommand(trimmedCommand);
                        break;
                    }
                case GameState.Login:
                    {
                        LoginCommand(trimmedCommand);
                        break;
                    }
                case GameState.SelectCharacter:
                    {
                        SelectCharacterCommand(trimmedCommand);
                        break;
                    }
            }
        }

        private void MasterCommand(string command) {
            switch(command) {
                case "get-servers":
                    {
                        this.Peer.OpCustom((byte)OperationCode.GetServerList, new Dictionary<byte, object>(), true);
                        break;
                    }
                default:
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Undefined command on Master");
                        break;
                    }
            }
        }

        private void LoginCommand(string command) {
            switch(command) {
                case "login":
                    {
                        Peer.OpCustom((byte)OperationCode.Login, new Dictionary<byte, object> {
                            { (byte)ParameterCode.LoginId, "123456789" },
                            { (byte)ParameterCode.AccessToken, "abrakadabra" },
                            { (byte)ParameterCode.DisplayName, "Oleg" }
                        }, true);
                        break;
                    }
                default:
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Undefined command on Login");
                        break;
                    }

            }
        }

        private void SelectCharacterCommand(string command) {

            string[] tokens = command.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            switch (tokens[0]) {
                case "get-characters":
                    {
                        if(string.IsNullOrEmpty(ClientApplication.GameRefId)) {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("GameRefId not exists");
                            Console.ForegroundColor = ConsoleColor.White;
                            return;
                        }
                        Peer.OpCustom((byte)SelectCharacterOperationCode.GetCharacters, new Dictionary<byte, object> {
                            {(byte)ParameterCode.GameRefId, ClientApplication.GameRefId }
                        }, true);
                        break;
                    }
                case "create-character":
                    {
                        Peer.OpCustom((byte)SelectCharacterOperationCode.CreateCharacter, new Dictionary<byte, object> {
                            { (byte)ParameterCode.GameRefId, ClientApplication.GameRefId },
                            { (byte)ParameterCode.Race, (byte)Race.Humans },
                            { (byte)ParameterCode.WorkshopId, (byte)Workshop.DarthTribe },
                            { (byte)ParameterCode.DisplayName, "Oleg(Console)" }
                        }, true);
                        break;
                    }
                case "select-character":
                    {
                        int chIndex = -1;
                        if(tokens.Length > 1) {
                            chIndex = int.Parse(tokens[1]);
                        }

                        if(chIndex >= 0) {
                            string cid = ClientApplication.CharacterIds()[chIndex];
                            Console.WriteLine("Try select character {0}", cid);

                            Peer.OpCustom((byte)SelectCharacterOperationCode.SelectCharacter, new Dictionary<byte, object> {
                                { (byte)ParameterCode.GameRefId, ClientApplication.GameRefId },
                                { (byte)ParameterCode.CharacterId, cid}
                            }, true);
                        }
                        break;
                    }
                case "delete-character":
                    {
                        int chIndex = -1;
                        if (tokens.Length > 1) {
                            chIndex = int.Parse(tokens[1]);
                        }

                        if (chIndex >= 0) {
                            string cid = ClientApplication.CharacterIds()[chIndex];
                            Console.WriteLine("Try delete character {0}", cid);

                            Peer.OpCustom((byte)SelectCharacterOperationCode.DeleteCharacter, new Dictionary<byte, object> {
                                { (byte)ParameterCode.GameRefId, ClientApplication.GameRefId },
                                { (byte)ParameterCode.CharacterId, cid}
                            }, true);
                        } else {
                            Console.WriteLine("Invalid character index {0}", chIndex);
                        }
                        break;
                    }
            }
        }

        private void ConnectToServer(ServerType serverType ) {
            switch(serverType) {
                case ServerType.master:
                    {
                        ClientApplication.SetDisconnectAction(DisconnectAction.None);
                        ClientApplication.OnCompleteDisconnectAction();
                        break;
                    }
                case ServerType.login:
                    {
                        ClientApplication.SetDisconnectAction(DisconnectAction.ConnectToLoginServer);
                        if(this.Peer != null && this.Peer.PeerState == PeerStateValue.Connected) {
                            this.Peer.Disconnect();
                            this.Peer = null;
                        }
                        ClientApplication.OnCompleteDisconnectAction();
                        break;
                    }
                case ServerType.character:
                    {
                        ClientApplication.SetDisconnectAction(DisconnectAction.ConnectToSelectCharacterServer);
                        if(this.Peer != null && this.Peer.PeerState == PeerStateValue.Connected) {
                            this.Peer.Disconnect();
                            this.Peer = null;  
                        } 
                        ClientApplication.OnCompleteDisconnectAction();
                        break;
                    }
            }
        }

        
    }
}
