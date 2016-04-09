using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using ServerClientCommon;
using Nebula.Client.Utils;

namespace Nebula.Client.Guilds {
    public abstract class CoalitionTransaction : IInfoParser {
        public CoalitionTransactionType type { get; private set; }
        public string characterName { get; private set; }
        public string characterId { get; private set; }
        public int time { get; private set; }

        public CoalitionTransaction(Hashtable hash) {
            ParseInfo(hash);
        }

        public virtual void ParseInfo(Hashtable info) {
            type = (CoalitionTransactionType)info.GetValueInt((int)SPC.Type);
            characterName = info.GetValueString((int)SPC.CharacterName);
            characterId = info.GetValueString((int)SPC.CharacterId);
            time = info.GetValueInt((int)SPC.Time);

            if(characterName == null ) {
                characterName = string.Empty;
            }
            if(characterId == null ) {
                characterId = string.Empty;
            }
        }

        public static CoalitionTransaction Parse(Hashtable hash) {
            var type = (CoalitionTransactionType)hash.GetValueInt((int)SPC.Type);
            switch(type) {
                case CoalitionTransactionType.deposit:
                case CoalitionTransactionType.withdraw:
                case CoalitionTransactionType.set_poster:
                    return new CountCoalitionTransaction(hash);
                case CoalitionTransactionType.make_officier:
                case CoalitionTransactionType.member_added:
                case CoalitionTransactionType.member_removed:
                    return new MemberActionCoalitionTransaction(hash);
                default:
                    return null;
            }
        }
    }

    public class CountCoalitionTransaction : CoalitionTransaction {

        public int count { get; private set; }

        public CountCoalitionTransaction(Hashtable hash)
            : base(hash) { }

        public override void ParseInfo(Hashtable info) {
            base.ParseInfo(info);
            count = info.GetValueInt((int)SPC.Count);
        }
    }

    public class MemberActionCoalitionTransaction : CoalitionTransaction {
        public string targetCharacterId { get; private set; }
        public string targetCharacterName { get; private set; }

        public MemberActionCoalitionTransaction(Hashtable hash)
            : base(hash) { }

        public override void ParseInfo(Hashtable info) {
            base.ParseInfo(info);
            targetCharacterId = info.GetValueString((int)SPC.TargetCharacterId);
            targetCharacterName = info.GetValueString((int)SPC.TargetCharacterName);
            if(targetCharacterName == null ) {
                targetCharacterName = string.Empty;
            }
            if(targetCharacterId == null ) {
                targetCharacterId = string.Empty;
            }
        }
    }
}
