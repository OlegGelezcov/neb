// ServerInfoCollection.cs
// Nebula
//
// Created by Oleg Zheleztsov on Sunday, September 13, 2015 5:43:45 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
namespace Master {
    using Common;
    using ExitGames.Logging;
    using ServerClientCommon;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    public class ServerInfoCollection : List<ServerInfo>, IInfoSource {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger(); 

        private readonly object syncRoot = new object();

        public void LoadFrom(string file) {
            try {
                XDocument document = XDocument.Load(file);
                lock(syncRoot) {
                    var dumpArray = document.Element("servers").Elements("server").Select(server => {
                        ServerType serverType = (ServerType)Enum.Parse(typeof(ServerType), server.GetString("type"));
                        string ip = server.GetString("ip");
                        string protocol = server.GetString("protocol");
                        int port = server.GetInt("port");
                        int index = server.GetInt("index");
                        string app = server.GetString("application");
                        string[] locations = new string[] { };
                        string ipv6Address = server.GetString("ipv6");

                        if(server.Element("locations") != null ) {
                            locations = server.Element("locations").Value.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        }

                        ServerInfo serverInfo = new ServerInfo {
                            Index = index,
                            IpAddress = ip,
                            Port = port,
                            Protocol = protocol,
                            ServerType = serverType,
                            Application = app,
                            Locations = locations,
                            Ipv6Address = ipv6Address
                        };
                        this.Add(serverInfo);
                        return serverInfo;
                    }).ToArray();
                }
            }catch(Exception ex) {
                log.Error(ex);
            }
        }

        public Hashtable GetInfo() {
            Hashtable result = new Hashtable();
            lock(syncRoot) {
                foreach(var serverInfo in this) {
                    result.Add(serverInfo.Key(), serverInfo.GetInfo());
                }
            }
            return result;
        }
    }
}
