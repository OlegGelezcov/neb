﻿using Common;
using ExitGames.Logging;
using GameMath;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Pets;
using Nebula.Game.Utils;
using Nebula.Server.Components;
using NebulaCommon.Group;
using ServerClientCommon;
using Space.Game;
using System.Collections;
using System.Collections.Generic;

namespace Nebula.Game.Components {

    [REQUIRE_COMPONENT(typeof(MmoActor))]
    [REQUIRE_COMPONENT(typeof(PlayerShip))]
    public class PlayerCharacterObject : CharacterObject {

        private const float RACE_STATUS_BONUS_UPDATE_INTERVAL = 60;

        private MmoActor mPlayer;
        private PlayerShip mShip;

        public Group group { get; private set; }

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private bool mRaceStatusRequested = false;
        private bool mGuildRequested = false;

        private PlayerLoaderObject mLoader;
        private MmoMessageComponent mMessage;
        private float mUpdateRaceStatusBonusTimer = 0;
        private PlayerBonuses mBonuses;
        private RaceableObject mRace;
        private PetManager m_PetManager;


        public void Init(PlayerCharacterComponentData data) {
            SetWorkshop(data.workshop);
            SetLevel(data.level);
            SetFraction(data.fraction);
        }

        public override void Start() {
            mPlayer = RequireComponent<MmoActor>();
            mShip = RequireComponent<PlayerShip>();
            mLoader = GetComponent<PlayerLoaderObject>();
            mMessage = GetComponent<MmoMessageComponent>();
            mBonuses = GetComponent<PlayerBonuses>();
            mRace = GetComponent<RaceableObject>();
            m_PetManager = GetComponent<PetManager>();
        }

        public string characterId { get; private set; }

        public int exp { get; private set; }

        public string login { get; private set; }

        public int raceStatus { get; private set; }

        public string characterName { get; private set; }

        public string guildId { get; private set; } = string.Empty;

        public string guildName { get; private set; } = string.Empty;

        public void SetGuildInfo(Hashtable hash) {
            guildId = hash.GetValue<string>((int)SPC.Guild, string.Empty);
            guildName = hash.GetValue<string>((int)SPC.GuildName, string.Empty);
            nebulaObject.properties.SetProperty((byte)PS.GuildName, guildName);
            log.InfoFormat("guild name setted = {0}", guildName);
        }

        public void AddExp(int e) {
            float bonusAddition = 0f;
            if(mBonuses != null && e > 0) {
                bonusAddition = (float)System.Math.Ceiling( e * mBonuses.expPcBonus );
            }
            int additionalExp = (int)bonusAddition;

            e += additionalExp;
            exp += (e);
            log.InfoFormat("added exp = {0}, additional exp = {1}".Color(LogColor.orange), e,  e - additionalExp);
            mPlayer.UpdateCharacterOnMaster();
            mPlayer.EventOnPlayerInfoUpdated();
            //mMessage.ReceiveServiceMessage(ServiceMessageType.Info, string.Format("exp received = {0}", e));
            mMessage.ReceiveExp(e);

            if(m_PetManager) {
                m_PetManager.AddExp((int)System.Math.Round((float)(e + additionalExp) * 0.5f));
            }
        }

        public override void SetFraction(FractionType inFraction) {
            base.SetFraction(inFraction);
            log.InfoFormat("player character = {0} set fraction = {1}", login, (FractionType)(byte)fraction);
        }

        public void SetExp(int e) {
            exp = e;
        }

        public void SetCharacterId(string cID) {
            characterId = cID;
            props.SetProperty((byte)PS.CharacterID, characterId);
        }

        public void SetCharacterName(string cName) {
            characterName = cName;
            props.SetProperty((byte)PS.CharacterName, characterName);
        }

        public void SetLogin(string inLogin) {
            login = inLogin;
            props.SetProperty((byte)PS.Login, login);
            log.InfoFormat("set login = {0}", login);
        }

        public void SetRaceStatus(int inRaceStatus) {
            raceStatus = inRaceStatus;
            props.SetProperty((byte)PS.RaceStatus, raceStatus);
        }

        public override void Update(float deltaTime) {
            base.Update(deltaTime);

            if(!mRaceStatusRequested) {
                if(mLoader.loaded) {
                    mRaceStatusRequested = true;
                    mPlayer.application.updater.CallS2SMethod(NebulaCommon.ServerType.SelectCharacter, "RequestRaceStatus", new object[] { nebulaObject.Id, characterId });
                    log.InfoFormat("requested race status of player {0}:{1}", nebulaObject.Id, characterId);
                }
            }

            if(!mGuildRequested) {
                if(mLoader.loaded) {
                    mGuildRequested = true;
                    mPlayer.application.updater.CallS2SMethod(NebulaCommon.ServerType.SelectCharacter, "RequestGuildInfo", new object[] { nebulaObject.Id, characterId });
                }
            }

            mUpdateRaceStatusBonusTimer -= deltaTime;
            if(mUpdateRaceStatusBonusTimer <= 0f) {
                mUpdateRaceStatusBonusTimer = RACE_STATUS_BONUS_UPDATE_INTERVAL;
                if (isCommanderOrAdmiral()) {
                    mBonuses.SetBuff(new Buff("race_status", null, BonusType.increase_resist_on_pc, RACE_STATUS_BONUS_UPDATE_INTERVAL * 2, 0.5f));
                }
            }
        }

        private bool isCommanderOrAdmiral() {
            return ((RaceStatus)raceStatus == RaceStatus.Commander || (RaceStatus)raceStatus == RaceStatus.Admiral);
        }

        public override int level {
            get {
                return resource.Leveling.LevelForExp(exp);
            }
        }

        public void SetGroup(Group grp) {
            group = grp;
        }

        public bool hasGroup {
            get {
                return (group != null) && (false == string.IsNullOrEmpty(group.groupID));
            }
        }

        public List<NebulaObject> GroupMemberPlayers(float radius) {
            List<NebulaObject> result = new List<NebulaObject>();

            if (group.members != null) {
                MmoWorld mmoWorld = nebulaObject.world as MmoWorld;

                List<string> gameRefs = new List<string>();
                foreach (var groupMember in group.members) {
                    gameRefs.Add(groupMember.Value.gameRefID);
                }

                var actors = mmoWorld.GetMmoActorsConcurrent((player) => {
                    return gameRefs.Contains(player.nebulaObject.Id) && (transform.DistanceTo(player.transform) < radius);
                });

                foreach (var pActor in actors) {
                    result.Add(pActor.Value.nebulaObject);
                }
            }
            return result;
        }

        /// <summary>
        /// Called by server 
        /// </summary>
        /// <param name="groupID"></param>
        public void OnGroupRemoved(object groupID) {
            if(groupID == null ) { return;  }
            if(group == null ) { return; }
            if(group.groupID == groupID.ToString() ) {
                group.Clear();
            }
        }

        public void AddPvpPoints(int points) {
            log.InfoFormat("give pvp points => {0}:{1} [red]", login, points);
            if(mRace == null ) {
                mRace = GetComponent<RaceableObject>();
            }


            points = ApplyPvpPointsBonus(points);
            mPlayer.application.updater.GivePvpPoints(login, nebulaObject.Id, characterId, guildId, mRace.race , points);
        }

        private int ApplyPvpPointsBonus(int points) {
            float bonus = points * (1 + mBonuses.pvpPointsPcBonus);
            int iBonus = Mathf.RoundToInt(bonus);
            return iBonus;
        }
    }
}
