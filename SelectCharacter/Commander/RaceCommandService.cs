using Common;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ServerClientCommon;

namespace SelectCharacter.Commander {
    public class RaceCommandService : IInfoSource {

        private SelectCharacterApplication application { get; set; }
        private RaceCommandCache cache { get; set; }

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
                cache.AddCommand(command);
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
            return result;
        }

        private bool SetAdmiral(int race, string login, string gameRefID, string characterID ) {
            bool result =  GetCommand(race).SetAdmiral(login, gameRefID, characterID);
            cache.SetChanged(true);
            return result;
        }

        public bool SetRaceStatus(int race, RaceStatus raceStatus, string login, string gamerefID, string characterID) {
            switch(raceStatus) {
                case RaceStatus.Commander:
                    return SetCommander(race, login, gamerefID, characterID);
                case RaceStatus.Admiral:
                    return SetAdmiral(race, login, gamerefID, characterID);
                case RaceStatus.None:
                    {
                        var command = GetCommand(race);
                        if(command.commander.IsCommander(login, gamerefID, characterID)) {
                            command.Clear();
                            cache.SetChanged(true);
                            return true;
                        } else if(command.firstAdmiral.IsAdmiral(login, gamerefID, characterID)) {
                            command.firstAdmiral.Clear();
                            cache.SetChanged(true);
                            return true;
                        } else if(command.secondAdmiral.IsAdmiral(login, gamerefID, characterID)) {
                            command.secondAdmiral.Clear();
                            cache.SetChanged(true);
                            return true;
                        }
                    }
                    return false;
                default:
                    return false;
            }
            
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
