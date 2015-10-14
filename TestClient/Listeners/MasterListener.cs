using Common;
using ExitGames.Client.Photon;
using ServerClientCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient.Listeners {
    public class MasterListener : IPhotonPeerListener {
        public void DebugReturn(DebugLevel level, string message) {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("DebugReturn(): " + level + ":" + message);
        }

        public void OnEvent(EventData eventData) {
        }

        public void OnOperationResponse(OperationResponse operationResponse) {
            if (operationResponse.ReturnCode != (short)ReturnCode.Ok) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Operation {0} response error {1}", (OperationCode)operationResponse.OperationCode, (ReturnCode)operationResponse.ReturnCode);
                return;
            }

            switch ((OperationCode)operationResponse.OperationCode) {
                default:
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Unknown operation code: {0}", (OperationCode)operationResponse.OperationCode);
                        break;
                    }
                case OperationCode.GetServerList:
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        List<ServerInfo> servers = new List<ServerInfo>();
                        Hashtable hash = operationResponse.Parameters[(byte)ParameterCode.ServerList] as Hashtable;
                        if (hash == null) {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("operation: {0}, parameter {1} null", (OperationCode)operationResponse.OperationCode, ParameterCode.ServerList);
                            return;
                        }
                        foreach (DictionaryEntry entry in hash) {
                            ServerInfo server = new ServerInfo();
                            server.ParseInfo(entry.Value as Hashtable);
                            servers.Add(server);
                        }
                        //fire event servers received
                        Events.EvtServersReceived(servers);
                        break;
                    }
            }
        }

        public void OnStatusChanged(StatusCode statusCode) {
            Console.WriteLine("Master status changed:" + statusCode);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("master status changed: " + statusCode);
            if(statusCode == StatusCode.Disconnect || statusCode == StatusCode.DisconnectByServer || 
                statusCode == StatusCode.DisconnectByServerLogic || statusCode == StatusCode.DisconnectByServerUserLimit) {
                ClientApplication.OnCompleteDisconnectAction();
            }
        }
    }
}
