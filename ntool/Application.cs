/*
send operation -> Login
operation code: 126
reliable: True
parameters ->
 {
 144 : oleg11
 145 : oleg11
 18 : oleg11@mail.ru
 19 : 1234
 20 : 56789
 }

Operation: RegisterUser
ReturnCode: Ok
DebugMessage:
Parameters ->
 {
 Login : oleg11
 GameRefId : 0792d4b8-af59-44bd-b1ab-60dcafd1c284
 Status : 0
 }

*/
using Common;
using ExitGames.Client.Photon;
using NebulaCommon;
using ntool.Commands;
using ntool.Listeners;
using ServerClientCommon;
using System;
using System.Collections.Generic;

namespace ntool {
    public class Application {

        private static Application s_Instance = null;
        private readonly ILogger m_Logger;
        private readonly List<Listeners.BaseListener> m_Listeners = new List<Listeners.BaseListener>();
        private System.Threading.Thread m_RunThread;
        private bool m_Running;
        private readonly object m_Sync = new object();
        private readonly CommandManager m_CommandManager;
        private readonly Player m_Player = new Player();


        private Application() {
            m_Logger = new ConsoleLogger();
            m_CommandManager = new CommandManager(this);
            Events.e_LoginFailed += Events_e_LoginFailed;
            Events.e_LoginSuccess += Events_e_LoginSuccess;
        }

        private void Events_e_LoginSuccess(string login, string gameRef) {
            player.SetLogin(login);
            player.SetGameRef(gameRef);
        }

        private void Events_e_LoginFailed(LoginReturnCode code) {
            if(code == LoginReturnCode.UserNotFound ) {
                Command("register buratino 87e898AA vasvav@gmail.com");
            }
        }

        public Player player {
            get {
                return m_Player;
            }
        }

        public static Application instance {
            get {
                if(s_Instance == null ) {
                    s_Instance = new Application();
                }
                return s_Instance;
            }
        }

        public ILogger logger {
            get {
                return m_Logger;
            }
        }


        public void Run() {
            m_Running = true;

            m_RunThread = new System.Threading.Thread(() => {
                while(m_Running) {
                    try {
                        foreach (var listener in m_Listeners) {
                            listener.peer.Service();
                        }
                        System.Threading.Thread.Sleep(17);
                    } catch(Exception ex) {
                        logger.Log(ex.Message);
                        Stop();
                    } 
                }
            });

            m_RunThread.Start();
            m_CommandManager.Run();
        }

        public void Stop() {
            lock(m_Sync) {
                m_Running = false;
            }
        }

        public void Command(string command) {
            if(command.ToLower() == "exit" ) {
                m_CommandManager.Stop();
                Stop();
                foreach(var listener in m_Listeners) {
                    listener.peer.Disconnect();
                }
                m_Listeners.Clear();

                logger.PushColor(ConsoleColor.Cyan);
                logger.Log("buy.");
                logger.PopColor();
            } else {
                var cmd = BaseCommand.CreateCommand(command, this);
                if(cmd != null ) {
                    cmd.Execute();
                } else {
                    logger.PushColor(ConsoleColor.Red);
                    logger.Log("not found command for input: {0}", command);
                    logger.PopColor();
                }
            }
        }

        public void Connect(List<ServerInfo> servers) {
            m_Listeners.Clear();


            foreach(var server in servers ) {
                switch(server.type) {
                    case NebulaCommon.ServerType.Game:
                        m_Listeners.Add(ConnectPeer(new GameListener(server.server, this, server), server));
                        break;
                    case NebulaCommon.ServerType.Master:
                        m_Listeners.Add(ConnectPeer(new MasterListener(server.server, this, server), server));
                        break;
                    case NebulaCommon.ServerType.Login:
                        m_Listeners.Add(ConnectPeer(new LoginListener(server.server, this, server), server));
                        break;
                    case NebulaCommon.ServerType.SelectCharacter:
                        m_Listeners.Add(ConnectPeer(new SelectCharacterListener(server.server, this, server), server));
                        break;

                }
            }
        }

        private BaseListener ConnectPeer(BaseListener listener, ServerInfo server) {
            var peer = new PhotonPeer(listener, ConnectionProtocol.Udp);
            peer.Connect(string.Format("{0}:{1}", server.ip, server.port), server.appName);
            listener.SetPeer(peer);
            return listener;
        }


        public void Operation(NebulaCommon.ServerType serverType, byte code, Dictionary<byte, object> parameters, bool reliable = true) {
            var peer = FindPeer(serverType);
            if(peer != null ) {

                logger.PushColor(ConsoleColor.Blue);
                logger.Log("send operation -> {0}", serverType);
                logger.Log("operation code: {0}", code);
                logger.Log("reliable: {0}", reliable);
                logger.Log("parameters -> ");
                if(parameters != null ) {
                    logger.Log("{0}", parameters.toHash().ToStringBuilder().ToString());
                } else {
                    logger.Log("(none)");
                }

                logger.PopColor();
                peer.OpCustom(code, parameters, reliable);
            } else {
                logger.PushColor(ConsoleColor.Red);
                logger.Log("Peer not found for server type = {0}", serverType);
                logger.PopColor();
            }
        }

        private PhotonPeer FindPeer(NebulaCommon.ServerType serverType) {
            var lst =  m_Listeners.Find(listener => listener.serverInfo.type == serverType);
            if(lst != null ) {
                return lst.peer;
            }
            return null;
        }
    }
}
