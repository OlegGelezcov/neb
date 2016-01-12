using Common;
using ExitGames.Logging;
using Nebula.Game.Components;
using Nebula.Game.Pets;
using Nebula.Game.Utils;
using Nebula.Pets;
using ServerClientCommon;
using Space.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game{

    public class PetOperations {
        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();

        private MmoActor player { get; set; }

        public PetOperations(MmoActor player) {
            this.player = player;
        }

        public Hashtable GetPets() {
            return player.GetComponent<PetManager>().GetInfo();
        }

        public Hashtable AddRandomPet() {

            PetDropper.PetDropSettings settings = new PetDropper.PetDropSettings();
            settings.OnGenerateColor();

            settings.OffGeneratePassiveSkill();
            settings.SetPassiveSkill(-1);

            settings.OnGenerateModel();
            settings.OnGenerateActiveSkills();
            settings.OnGenerateDamageType();

            settings.OffSetMastery();
            settings.SetMastery(0);

            settings.OffGenerateRace();
            settings.SetRace((Race)player.GetComponent<RaceableObject>().race);

            PetDropper dropper = new PetDropper();
            PetInfo info = dropper.Drop(player.resource.petParameters, settings, player.resource.petSkills);
            player.GetComponent<PetManager>().AddPet(info);
            return new Hashtable {
                {(int)SPC.ReturnCode, (int)RPCErrorCode.Ok },
                {(int)SPC.Info, info.GetInfo(player.resource) }
            };
        }

        /*
        public Hashtable ActivatePet(string id) {
            bool status = player.GetComponent<PetManager>().ActivatePet(id);
            return new Hashtable {
                {(int)SPC.ReturnCode, (int)RPCErrorCode.Ok },
                {(int)SPC.Status, status }
            };
        }


        public Hashtable DeactivatePet(string id) {
            bool status = player.GetComponent<PetManager>().DeactivatePet(id);
            return new Hashtable {
                {(int)SPC.ReturnCode, (int)RPCErrorCode.Ok },
                {(int)SPC.Status, status }
            };
        }*/

        public Hashtable ActivatePet(string deactivatePetId, string activatePetId ) {
            var petManager = player.GetComponent<PetManager>();
            bool deactivateSuccess = true;
            if(!string.IsNullOrEmpty(deactivatePetId)) {
                deactivateSuccess = petManager.DeactivatePet(deactivatePetId);
            }

            bool activateSuccess = true;
            if(!string.IsNullOrEmpty(activatePetId)) {
                activateSuccess = petManager.ActivatePet(activatePetId);
            }

            return new Hashtable {
                {(int)SPC.ReturnCode, (int)RPCErrorCode.Ok },
                {(int)SPC.Status, (deactivateSuccess && activateSuccess )  }
            };
        }

        /// <summary>
        /// Add or replace skill
        /// </summary>
        /// <param name="petId">Id of pet</param>
        /// <param name="oldSkill">old skill id, set zero if we only add new skill</param>
        /// <param name="newSkill">new skill will be setted</param>
        /// <returns></returns>
        public Hashtable AddOrReplaceActiveSkill(string petId, int oldSkill, int newSkill ) {
            var petManager = player.GetComponent<PetManager>();
            bool success = petManager.AddOrReplaceActiveSkill(petId, oldSkill, newSkill);
            s_Log.InfoFormat("changing pet skill {0} => {1} status: {2}".Color(LogColor.white), oldSkill, newSkill, success);
            return new Hashtable {
                { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok },
                { (int)SPC.Status, success }
            };
        }

        public Hashtable SetPassiveSkill(string petId, int skillId ) {
            var petManager = player.GetComponent<PetManager>();
            bool success = petManager.SetPassiveSkill(petId, skillId);
            s_Log.InfoFormat("set passive skill to {0} => {1}".Color(LogColor.white), skillId, success);
            return new Hashtable {
                {(int)SPC.ReturnCode, (int)RPCErrorCode.Ok },
                {(int)SPC.Status, success }
            };
        }
    }
}
