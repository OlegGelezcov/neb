using System;
using Common;
using Nebula.Engine;
using Space.Game.Resources.Zones;
using System.Collections.Concurrent;
using Nebula.Server.Components;
using System.Collections;
using ServerClientCommon;
using ExitGames.Logging;

namespace Nebula.Game.Components {
    public class PlanetObject : NebulaBehaviour, IInfoSource
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private string mNebulaElementID;
        private byte mMaxSlots;
        private byte mPlanetType;

        private readonly ConcurrentDictionary<byte, PlanetSlot> mSlots = new ConcurrentDictionary<byte, PlanetSlot>();

        public void Init(PlanetObjectComponentData data) {

            mNebulaElementID = data.nebulaElement;
            mMaxSlots = (byte)data.maxSlots;
            mPlanetType = (byte)data.planetType;

            //create empty slots for planet on init
            mSlots.Clear();
            for(int i = 0; i < mMaxSlots; i++) {
                mSlots.TryAdd((byte)i, new PlanetSlot((byte)i));
            }

            props.SetProperty((byte)PS.PlanetType, mPlanetType);
        }

        public string element {
            get {
                return mNebulaElementID;
            }
        }

        public void FreeStationSlot(string miningStationID) {
            foreach(var pSlot in mSlots ) {
                if(pSlot.Value.minigStationID == miningStationID ) {
                    pSlot.Value.FreeSlot();
                    log.InfoFormat("free slot = {0} from station = {1} [red]", pSlot.Key, miningStationID);
                }
            }
        }

        public void SetStation(MiningStation station, int slotNumber) {
            foreach(var pSlot in mSlots ) {
                if(pSlot.Value.slotNumber == slotNumber ) {
                    pSlot.Value.SetStation(station);
                    break;
                }
            }
        }

        public bool IsFreeSlot(int slotNumber) {
            foreach(var pSlot in mSlots ) {
                if(pSlot.Value.slotNumber == slotNumber ) {
                    return pSlot.Value.free;
                }
            }
            return false;
        }

        public Hashtable GetInfo() {
            Hashtable info = new Hashtable();
            info.Add((int)SPC.Template, mNebulaElementID);
            info.Add((int)SPC.MaxSlots, mMaxSlots);
            info.Add((int)SPC.Id, nebulaObject.Id);
            info.Add((int)SPC.Type, nebulaObject.Type);

            Hashtable slotHash = new Hashtable();
            foreach(var slotPair in mSlots) {
                slotHash.Add(slotPair.Key, slotPair.Value.GetInfo());
            }
            info.Add((int)SPC.PlanetSlots, slotHash);
            return info;
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.Planet;
            }
        }

        

    }

    public class PlanetSlot : IInfoSource {
        public byte slotNumber { get; private set; }
        public bool free { get; private set; }
        public string minigStationID { get; private set; }
        public byte miningStationObjectType { get; private set; }
        public byte miningStationRace { get; private set; }
        public string miningStationOwnedPlayerID { get; private set; }


        public void SetStation(MiningStation station) {
            free = false;
            minigStationID = station.nebulaObject.Id;
            miningStationObjectType = station.nebulaObject.Type;
            miningStationRace = station.GetComponent<RaceableObject>().race;
            miningStationOwnedPlayerID = station.ownedPlayer;
        }

        public PlanetSlot(byte number) {
            slotNumber = number;
            ResetSlot();
        }

        private void ResetSlot() {
            free = true;
            minigStationID = string.Empty;
            miningStationObjectType = 0;
            miningStationRace = 0;
            miningStationOwnedPlayerID = string.Empty;
        }

        public void FreeSlot() {
            ResetSlot();
        }

        public Hashtable GetInfo() {
            return new Hashtable {
                { (int)SPC.Index, slotNumber },
                { (int)SPC.Free, free },
                { (int)SPC.AssignedStationID, minigStationID },
                { (int)SPC.AssignedStationType, miningStationObjectType },
                { (int)SPC.AssignedStationRace, miningStationRace },
                { (int)SPC.MiningStationOwnedPlayer, miningStationOwnedPlayerID }
            };
        }
    }
    
}
