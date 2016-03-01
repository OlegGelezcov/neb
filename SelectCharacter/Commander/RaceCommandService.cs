using Common;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ServerClientCommon;
using SelectCharacter.Events;
using ExitGames.Logging;

namespace SelectCharacter.Commander {
    public class RaceCommandService : IInfoSource {

        private SelectCharacterApplication application { get; set; }
        private RaceCommandCache cache { get; set; }

        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();

        public RaceCommandService(SelectCharacterApplication inApplication ) {
            application = inApplication;
            cache = new RaceCommandCache();
        }

        public void Save() {
            cache.Save(application.DB.RaceCommands);
        }

        public RaceCommand GetCommand(int race) {
            var command = cache.GetCommand(race);
            if(command != null ) {
                return command;
            }

            command = application.DB.RaceCommands.FindOne(Query<RaceCommand>.EQ(rc => rc.race, race));
            if(command != null ) {
                if( cache.AddCommand(command) ) {
                    s_Log.InfoFormat("new command with commander character = {0} added to cache", command.commander.characterID);
                } else {
                    s_Log.InfoFormat("error of adding command to cache");
                }
                return command;
            } else {
                command = new RaceCommand {
                    race = race,
                    commander = new RaceCommander(),
                    firstAdmiral = new RaceAdmiral(),
                    secondAdmiral = new RaceAdmiral()
                };
                cache.AddCommand(command);
                cache.SetChanged(true);
                Save();
                return command;
            }
        }

        public void Clear() {
            
            var hummans = GetCommand((int)(byte)Race.Humans);
            var borguzands = GetCommand((int)(byte)Race.Borguzands);
            var criptizids = GetCommand((int)(byte)Race.Criptizoids);
            ClearCommandPlayers(hummans);
            ClearCommandPlayers(borguzands);
            ClearCommandPlayers(criptizids);
            hummans.Clear();
            borguzands.Clear();
            criptizids.Clear();
            cache.SetChanged(true);
        }

        private void ClearCommandPlayers(RaceCommand command) {
            if (command.commander.has) {
                application.Players.SetRaceStatus(command.commander.gameRefID, command.commander.characterID, (int)RaceStatus.None);
            }
            if (command.firstAdmiral.has) {
                application.Players.SetRaceStatus(command.firstAdmiral.gameRefID, command.firstAdmiral.characterID, (int)RaceStatus.None);
            }
            if (command.secondAdmiral.has) {
                application.Players.SetRaceStatus(command.secondAdmiral.gameRefID, command.secondAdmiral.characterID, (int)RaceStatus.None);
            }
        }

        private bool SetCommander(int race, string login, string gameRefID, string characterID ) {
            bool result =  GetCommand(race).SetCommander(login, gameRefID, characterID);
            cache.SetChanged(true);
            //send achievment commander variable
            if (result) {
                application.AddAchievmentVariable(gameRefID, "commander_count", 1);
                Save();
            }
            s_Log.InfoFormat("player = {0} setted as commander of race = {1}, character = {2}", login, (Race)(byte)race, characterID);
            return result;
        }

        private bool SetFirstAdmiral(int race, string login, string gameRefID, string characterID ) {
            bool result =  GetCommand(race).SetFirstAdmiral(login, gameRefID, characterID);
            cache.SetChanged(true);
            //send achievment admiral variable
            if (result) {
                application.AddAchievmentVariable(gameRefID, "admiral_count", 1);
                Save();
            }
            s_Log.InfoFormat("player = {0} setted as admiral of race = {1}", login, (Race)(byte)race);
            return result;
        }

        private bool SetSecondAdmiral(int race, string login, string gameRefID, string characterID) {
            bool result = GetCommand(race).SetSecondAdmiral(login, gameRefID, characterID);
            cache.SetChanged(true);
            //send achievment admiral variable
            if (result) {
                application.AddAchievmentVariable(gameRefID, "admiral_count", 1);
                Save();
            }
            s_Log.InfoFormat("player = {0} setted as admiral of race = {1}", login, (Race)(byte)race);
            return result;
        }

        public bool SetRaceStatus(int race, RaceStatus raceStatus, string login, string gamerefID, string characterID) {
            switch(raceStatus) {
                case RaceStatus.Commander:
                    return SetCommander(race, login, gamerefID, characterID);
                case RaceStatus.Admiral: {
                        var command = GetCommand(race);
                        if(!command.firstAdmiral.has) {
                            return SetFirstAdmiral(race, login, gamerefID, characterID);
                        } else {
                            return SetSecondAdmiral(race, login, gamerefID, characterID);
                        }
                    }
                case RaceStatus.None:
                    {
                        var command = GetCommand(race);
                        if(command.commander.IsCommander(login, gamerefID, characterID)) {
                            command.Clear();
                            cache.SetChanged(true);
                            Save();
                            return true;
                        } else if(command.firstAdmiral.IsAdmiral(login, gamerefID, characterID)) {
                            command.firstAdmiral.Clear();
                            cache.SetChanged(true);
                            Save();
                            return true;
                        } else if(command.secondAdmiral.IsAdmiral(login, gamerefID, characterID)) {
                            command.secondAdmiral.Clear();
                            cache.SetChanged(true);
                            Save();
                            return true;
                        }
                    }
                    return false;
                default:
                    return false;
            }
            
        }

        public bool MakeAdmiral(int race, string sourceLogin, string sourceGameRefID, string sourceCharacterID, string targetLogin, string targetGameRefID, string targetCharacterID ) {
            bool setted = false;
            var command = GetCommand(race);
            if(command != null ) {

                if(command.commander != null ) {
                    if(command.commander.characterID == sourceCharacterID ) {

                        if(command.IsAdmiral(targetCharacterID)) {
                            command.ClearAdmiral(targetCharacterID);
                        }

                        if( (!command.firstAdmiral.has) || (!command.secondAdmiral.has) ) {
                            application.Players.SetRaceStatus(targetGameRefID, targetCharacterID, (int)RaceStatus.Admiral);
                            cache.SetChanged(true);
                            setted = true;
                        }

                    } else {
                        s_Log.InfoFormat("Make admiral error: invalid commander character Id");
                    }
                } else {
                    s_Log.InfoFormat("Make admiral error: commander is null");
                }
            } else {
                s_Log.InfoFormat("Make admiral error: command is null");
            }
            if(setted ) {
                GenericEvent genEvent = new GenericEvent {
                    data = GetInfo(),
                    subCode = (byte)SelectCharacterGenericEventSubCode.CommandsUpdate
                };
                application.Clients.SendGenericEventToGameref(sourceGameRefID, genEvent);
                application.Clients.SendGenericEventToGameref(targetGameRefID, genEvent);
                
            }
            return setted;
        }

        //Get info for all commands
        public Hashtable GetInfo() {
            Hashtable hash = new Hashtable();
            hash.Add((int)(byte)Race.Humans, GetCommandInfo(GetCommand((int)(byte)Race.Humans)));
            hash.Add((int)(byte)Race.Borguzands, GetCommandInfo(GetCommand((int)(byte)Race.Borguzands)));
            hash.Add((int)(byte)Race.Criptizoids, GetCommandInfo(GetCommand((int)(byte)Race.Criptizoids)));
            return hash;
        }

        private Hashtable GetCommandInfo(RaceCommand command) {
            Hashtable hash = new Hashtable();
            hash.Add(RaceCommandKey.COMMANDER, GetCommandMemberInfo(command.commander));
            hash.Add(RaceCommandKey.ADMIRAL1, GetCommandMemberInfo(command.firstAdmiral));
            hash.Add(RaceCommandKey.ADMIRAL2, GetCommandMemberInfo(command.secondAdmiral));
            return hash;
        }

        private Hashtable GetCommandMemberInfo(ICommandMember member) {
            Hashtable baseInfo = member.GetInfo();
            if(member.exists) {
                var data = application.Players.GetExistingPlayer(member.gameRefID);
                if(data != null ) {
                    var character = data.Data.GetCharacter(member.characterID);
                    if(character != null ) {
                        baseInfo.Add((int)SPC.Exp, character.Exp);
                        baseInfo.Add((int)SPC.Workshop, character.Workshop);
                        baseInfo.Add((int)SPC.CharacterName, character.Name);
                        return baseInfo;
                    }
                }
            }

            baseInfo.Add((int)SPC.Exp, 0);
            baseInfo.Add((int)SPC.Workshop, (int)(byte)Workshop.Arlen);
            baseInfo.Add((int)SPC.CharacterName, string.Empty);
            return baseInfo;
        }
    }
}
