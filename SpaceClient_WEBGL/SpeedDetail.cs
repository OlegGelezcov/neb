using Common;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;

namespace Nebula.Client {
    public class SpeedDetail : IInfoParser {
        public int controlMult { get; private set; }
        public int stopMult { get; private set; }
        public float modelSpeed { get; private set; }
        public float bonusAdd { get; private set; }
        public float passiveAbilitiesAdd { get; private set; }
        public float accelerationAdd { get; private set; }

        public void ParseInfo(Hashtable info) {
            controlMult = info.GetValueInt((int)SPC.SPEED_ControlIsMoving);
            stopMult = info.GetValueInt((int)SPC.SPEED_IsStopped);
            modelSpeed = info.GetValueFloat((int)SPC.SPEED_ModelSpeed);
            bonusAdd = info.GetValueFloat((int)SPC.SPEED_BonusesAdd);
            passiveAbilitiesAdd = info.GetValueFloat((int)SPC.SPEED_PassiveAbilitiesAdd);
            accelerationAdd = info.GetValueFloat((int)SPC.SPEED_AccelerationAdd);
        }

        public SpeedDetail(Hashtable hash) {
            ParseInfo(hash);
        }

        public SpeedDetail() { }

        public override string ToString() {
            return string.Format("control mult: {0}, stopMult: {1}, modelSpeed: {2}, bonusAdd: {3}, passive abil speed: {4}, acceleration Add: {5}",
                controlMult, stopMult, modelSpeed, bonusAdd, passiveAbilitiesAdd, accelerationAdd);
        }
    }
}
