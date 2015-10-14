using Common;
using ExitGames.Client.Photon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient.Listeners {
    public class SelectCharacterListener : IPhotonPeerListener {
        public void DebugReturn(DebugLevel level, string message) {
            Console.WriteLine("SelectCharacterListener.DebugReturn() - {0}:{1}", level, message);
        }

        public void OnEvent(EventData eventData) {
        }

        public void OnOperationResponse(OperationResponse operationResponse) {
            if((ReturnCode)operationResponse.ReturnCode != ReturnCode.Ok) {
                Console.WriteLine("SelectCharacterListener.OnOperationResponse() error - {0}:{1}", 
                    (ReturnCode)operationResponse.ReturnCode,
                    operationResponse.DebugMessage);
                return;
            }
            switch((SelectCharacterOperationCode)operationResponse.OperationCode) {
                case SelectCharacterOperationCode.GetCharacters:
                    {
                        WriteConsole("GetCharacters Response");
                        Hashtable characters = operationResponse.Parameters[(byte)ParameterCode.Characters] as Hashtable;
                        Events.EvtPlayerCharactersReceived(characters);
                        break;
                    }
                case SelectCharacterOperationCode.CreateCharacter:
                    {
                        WriteConsole("CreateCharacter Response");
                        string characterId = (string)operationResponse.Parameters[(byte)ParameterCode.CharacterId];
                        Hashtable characters = (Hashtable)operationResponse.Parameters[(byte)ParameterCode.Characters];
                        Events.EvtCharacterCreated(characterId);
                        Events.EvtPlayerCharactersReceived(characters);
                        break; 
                    }
                case SelectCharacterOperationCode.SelectCharacter:
                    {
                        WriteConsole("SelectCharacter response");
                        string cid = (string)operationResponse.Parameters[(byte)ParameterCode.CharacterId];
                        Hashtable chs = (Hashtable)operationResponse.Parameters[(byte)ParameterCode.Characters];
                        Events.EvtCharacterSelected(cid);
                        Events.EvtPlayerCharactersReceived(chs);
                        break;
                    }
                case SelectCharacterOperationCode.DeleteCharacter:
                    {
                        WriteConsole("DeleteCharacter response");
                        Hashtable chs = (Hashtable)operationResponse.Parameters[(byte)ParameterCode.Characters];
                        Events.EvtPlayerCharactersReceived(chs);
                        break;
                    }
            }
        }

        public void OnStatusChanged(StatusCode statusCode) {
            Console.WriteLine("SelectCharacter status changed: " + statusCode);
            if (statusCode == StatusCode.Disconnect 
                || statusCode == StatusCode.DisconnectByServer 
                || statusCode == StatusCode.DisconnectByServerLogic 
                || statusCode == StatusCode.DisconnectByServerUserLimit) {
                ClientApplication.OnCompleteDisconnectAction();
            }
        }

        private void WriteConsole(string message) {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
