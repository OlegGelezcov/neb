using Common;
using Game.Space;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nebula {
    using Nebula.Resources;
    using UResources = UnityEngine.Resources;

    public class Settings {
        public const float CONTEXT_ACTION_DISTANCE = 70;

        static Settings() {
            ChannelCount = 3;
            DiagnosticsChannel = 0;
            OperationChannel = 0;
            ItemChannel = 0;
        }

        public static Settings GetDefaultSettings() {
            const int IntervalSend = 30;
            const bool SendReliable = false;
            const bool UseTcp = false;


            Vector3 cornerMin = new Vector3(-50000, -50000, -50000);
            Vector3 cornerMax = new Vector3(50000, 50000, 50000);
            Vector3 tileDimensions = new Vector3(25000, 25000, 25000);

            Settings result = new Settings();
            result.UseTcp = UseTcp;

            result.TileDimensions = tileDimensions;
            result.WorldCornerMin = cornerMin;
            result.WorldCornerMax = cornerMax;
            result.SendInterval = IntervalSend;
            result.SendReliable = SendReliable;

            string settingsText = UResources.Load<TextAsset>("settings").text;
            result.ParseSettings(settingsText);

            return result;
        }

        private int sendInterval;
        private bool useTcp;
        private string serverAddress;
        private string applicationName;
        private string worldName;
        private bool sendReliable;
        private Vector3 _tileDimensions;
        private Vector3 _worldCornerMin;
        private Vector3 _worldCornerMax;
        public static byte ChannelCount { get; set; }
        public static byte DiagnosticsChannel { get; set; }
        public static byte ItemChannel { get; set; }
        public static byte OperationChannel { get; set; }
        public static int OutgoingOperationCount = 10;

        public bool UseSpectatorCamera { get; private set; }
        public string[] LogFilters { get; private set; }

        public Hashtable Inputs { get; private set; }

        public Dictionary<Race, string> DefaultZones { get; private set; }

        public void ParseSettings(string text) {
            this.Inputs = new Hashtable();

            sXDocument document = new sXDocument();
            document.ParseXml(text);
            sXElement settingsElement = document.GetElement("settings");
            this.UseSpectatorCamera = settingsElement.GetElement("spectator_camera").GetBool("value");
            this.LogFilters = settingsElement.GetElement("log_filters").GetString("value").Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

            foreach (var e in settingsElement.GetElement("inputs").GetElements("value")) {
                string key = e.GetString("key");
                string value = e.GetString("value");
                string type = e.GetString("type");
                this.Inputs.Add(key, CommonUtils.ParseValue(value, type));
            }

            this.DefaultZones = new Dictionary<Race, string>();
            foreach (var e in settingsElement.GetElement("default_zones").GetElements("zone")) {
                Race race = (Race)System.Enum.Parse(typeof(Race), e.GetString("race"));
                string id = e.GetString("id");
                this.DefaultZones.Add(race, id);
            }
        }



        public float[] ViewDistanceEnter {
            get {
                return new float[] { 1e6f, 1e6f, 1e6f };
            }
        }
        public float[] ViewDistanceExit {
            get {
                return new float[] { 2e6f, 2e6f, 2e6f };
            }
        }

        public Vector3 TileDimensions {
            get { return _tileDimensions; }
            set { _tileDimensions = value; }
        }

        public Vector3 WorldCornerMin {
            get { return _worldCornerMin; }
            set { _worldCornerMin = value; }
        }

        public Vector3 WorldCornerMax {
            get { return _worldCornerMax; }
            set { _worldCornerMax = value; }
        }


        public string WorldName {
            get {
                return worldName;
            }
            set {
                worldName = value;
            }
        }

        public bool SendReliable {
            get {
                return sendReliable;
            }
            set {
                sendReliable = value;
            }
        }

        public int SendInterval {
            get {
                return this.sendInterval;
            }

            set {
                this.sendInterval = value;
            }
        }

        public bool UseTcp {
            get {
                return this.useTcp;
            }

            set {
                this.useTcp = value;
            }
        }

        public float[] DefaultViewDistance {
            get {
                return new float[] {
                TileDimensions[0] * .5f + 1,
                TileDimensions[1] * .5f + 1,
                TileDimensions[2] * .5f + 1
            };
            }
        }

        public const float CHAT_UPDATE_INTERVAL = 1.0f;
        //public const float START_Z = 0;
        public const int MAX_CHAT_MESSAGES_COUNT = 100;
        public const float INVENTORY_UPDATE_INTERVAL = 5.0f;
        public const float EVENTS_UPDATE_INTERVAL = 5.0f;
        public const float BUFFS_UPDATE_INTERVAL = 2.0f;
        public const float STATION_UPDATE_INTERVAL = 5.0f;

        public const float REQUEST_PLAYER_MODEL_INTERVAL_AT_WORLD = 3.0f;
        public const float REQUEST_PLAYER_MODEL_INTERVAL_AT_WORKSHOP = 1.0f;
        public const float REQUEST_WORKSHOP_INTERVAL = 2.5f;
        public const float REQUEST_COMBAT_PARAMS_INTERVAL = 3f;
        public const float REQUEST_WEAPON_INTERVAL = 3f;
        public const float REQUEST_PLAYER_INFO_INTERVAL = 10f;
        public const float REQUEST_SKILLS_INTERVAL = 1f;
        public const float REQUEST_EVENTS_INTERVAL = 2.0f;
        public const float REQUEST_PLAYER_PROPERTIES_INTERVAL = 1.1f;
        public const float REQUEST_PLAYER_BONUSES_INTERVAL = 1.25f;

    }
}