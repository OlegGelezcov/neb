using Common;
using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient.Listeners {
    public class LoginListener : IPhotonPeerListener {
        public void DebugReturn(DebugLevel level, string message) {
            Console.WriteLine("LoginListener.DebugReturn() {0}:{1}", level, message);
        }

        public void OnEvent(EventData eventData) {
            
        }

        public void OnOperationResponse(OperationResponse operationResponse) {
            if(operationResponse.ReturnCode != (short)ReturnCode.Ok) {
                Console.WriteLine("LoginListener response error {0}:{1}", (ReturnCode)operationResponse.ReturnCode, operationResponse.DebugMessage);
                return;
            }
            switch((OperationCode)operationResponse.OperationCode) {
                case OperationCode.Login:
                    {
                        HandleLoginOperationResponse(operationResponse);
                        break;
                    }
                default:
                    {
                        Console.WriteLine("LoginListener unknown operation response {0}", (OperationCode)operationResponse.OperationCode);
                        break;
                    }
            }
        }



        public void OnStatusChanged(StatusCode statusCode) {
            Console.WriteLine("Login status changed: " + statusCode);
            if(statusCode == StatusCode.Connect) {
                Console.WriteLine("Connected to login server");
                ClientApplication.SendLogin();
            }
            if (statusCode == StatusCode.Disconnect || statusCode == StatusCode.DisconnectByServer ||
    statusCode == StatusCode.DisconnectByServerLogic || statusCode == StatusCode.DisconnectByServerUserLimit) {
                ClientApplication.OnCompleteDisconnectAction();
            }
        }

        private void HandleLoginOperationResponse(OperationResponse response) {
            string gameRefId = (string)response.Parameters[(byte)ParameterCode.GameRefId];
            Events.EvtGameRefIdReceived(gameRefId);
        }
    }
}
