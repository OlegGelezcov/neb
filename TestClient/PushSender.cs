using Common;
using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient {
    public class PushSender : IPhotonPeerListener  {
#if LOCAL
        private const string IPADDRESS = "192.168.1.102";
#else
        private const string IPADDRESS = "45.63.0.198";
#endif
        private const string PORT = "5108";
        private const string APPLICATION = "SelectCharacter";

        private PhotonPeer mPeer;
        private readonly StringBuilder mCommandBuilder = new StringBuilder();

        public bool connected { get; private set; }

        private bool mLoopStarted = true;

        public void Setup() {
            mPeer = new PhotonPeer(this, ConnectionProtocol.Udp);
            mPeer.Connect(IPADDRESS + ":" + PORT, APPLICATION);
        }

        public void Run() {
            mCommandBuilder.Clear();
            while(mLoopStarted) {
                if(mPeer != null ) {
                    mPeer.Service();
                }

                if(Console.KeyAvailable) {
                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleKeyInfo key = Console.ReadKey();
                    if(key.Key != ConsoleKey.Enter) {
                        mCommandBuilder.Append(key.KeyChar);
                    } else {
                        Console.WriteLine();
                        string command = mCommandBuilder.ToString();
                        ExecuteCommand(command);
                        mCommandBuilder.Clear();
                    }
                }
            }
        }

        public void DebugReturn(DebugLevel level, string message) {
            
        }

        public void OnOperationResponse(OperationResponse operationResponse) {
            
        }

        public void OnStatusChanged(StatusCode statusCode) {
            if(statusCode == StatusCode.Connect) {
                connected = true;
                Console.WriteLine("change to connected....");
            } else {
                Console.WriteLine("change to not connected...");
                connected = false;
            }
        }

        public void OnEvent(EventData eventData) {
            
        }

        private void ExecuteCommand(string cmd) {
            if(!connected) {
                return;
            }

            switch(cmd.Trim().ToLower()) {
                case "push":
                    {
                        Dictionary<byte, object> parameters = new Dictionary<byte, object> {
                            { (byte)ParameterCode.Type, (byte)ClientSettings.Default.PushType },
                            { (byte)ParameterCode.Body, ClientSettings.Default.Body },
                            { (byte)ParameterCode.Title, ClientSettings.Default.Title }
                        };
                        mPeer.OpCustom((byte)SelectCharacterOperationCode.SendPushToPlayers, parameters, true);
                        Console.WriteLine("push sended");
                    }
                    break;
                case "exit":
                    {
                        mPeer.Disconnect();
                        mLoopStarted = false;
                    }
                    break;
            }
        }
    }
}
