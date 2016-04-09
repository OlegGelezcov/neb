using Common;
using ServerClientCommon;
using System.Collections;

namespace SelectCharacter.Guilds {
    public abstract class CoalitionTransaction : IInfoSource {
        public CoalitionTransactionType type { get; private set; }
        public string characterName { get; private set; }
        public string characterId { get; private set; }
        
        public int time { get; private set; }

        protected CoalitionTransaction(CoalitionTransactionType inType, string inCharacterName, string inChracterId ) {
            this.type = inType;
            this.characterName = inCharacterName;
            this.characterId = inChracterId;
            time = CommonUtils.SecondsFrom1970();
        }

        public virtual Hashtable GetInfo() {
            return new Hashtable {
                { (int)SPC.Type, (int)type },
                { (int)SPC.CharacterName, characterName },
                { (int)SPC.CharacterId, characterId },
                { (int)SPC.Time, time }
            };
        }

        public static CoalitionTransaction MakeTransaction(CoalitionTransactionType inType, string inCharacterName, string inChracterId, int inCount) {
            switch(inType) {
                case CoalitionTransactionType.deposit:
                case CoalitionTransactionType.withdraw:
                case CoalitionTransactionType.set_poster:
                    return new CountCoalitionTransaction(inType, inCharacterName, inChracterId, inCount);
                default:
                    return null;
            }
        }

        public static CoalitionTransaction MakeTransaction(CoalitionTransactionType inType, string inCharacterName, string inChracterId, string inTargetCharacterName, string inTargetCharacterId ) {
            switch(inType) {
                case CoalitionTransactionType.make_officier:
                case CoalitionTransactionType.member_added:
                case CoalitionTransactionType.member_removed:
                    return new MemberActionCoalitionTransaction(inType, inCharacterName, inChracterId, inTargetCharacterName, inTargetCharacterId);
                default:
                    return null;
            }
        }
    }

    public class CountCoalitionTransaction : CoalitionTransaction {

        public int count { get; private set; }

        public CountCoalitionTransaction(CoalitionTransactionType inType, string inCharacterName, string inChracterId, int inCount )
            : base(inType, inCharacterName, inChracterId) {
            count = inCount;
        }

        public override Hashtable GetInfo() {
            var hash = base.GetInfo();
            hash.Add((int)SPC.Count, count);
            return hash;
        }

    }

    public class MemberActionCoalitionTransaction : CoalitionTransaction {
        public string targetCharacterId { get; private set; }
        public string targetCharacterName { get; private set; }

        public MemberActionCoalitionTransaction(CoalitionTransactionType inType, string inCharacterName, string inCharacterId, string inTargetCharacterName, string inTargetCharacterId )
            : base(inType, inCharacterName, inCharacterId) {
            targetCharacterId = inTargetCharacterId;
            targetCharacterName = inTargetCharacterName;
        }

        public override Hashtable GetInfo() {
            var hash = base.GetInfo();
            hash.Add((int)SPC.TargetCharacterId, targetCharacterId);
            hash.Add((int)SPC.TargetCharacterName, targetCharacterName);
            return hash;
        }
    }
}
