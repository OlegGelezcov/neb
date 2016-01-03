using Common;
using ExitGames.Logging;
using Nebula.Database;
using Nebula.Engine;
using Nebula.Game.Components;
using Nebula.Game.Utils;
using Nebula.Pets;
using Space.Game;
using System.Collections.Concurrent;

namespace Nebula.Game.Pets {
    public class PetManager : NebulaBehaviour {

        private static ILogger s_Log = LogManager.GetCurrentClassLogger();

        private PlayerCharacterObject m_Character;
        private readonly ConcurrentDictionary<string, PetObject> m_Pets = new ConcurrentDictionary<string, PetObject>();
        private PetCollection m_PetInfoCollection = new PetCollection();
        private bool m_Initialized = false;

        public void Load() {
            if(m_Character == null ) {
                m_Character = GetComponent<PlayerCharacterObject>();
            }
            bool isNew = false;
            m_PetInfoCollection = PetDatabase.instance.LoadPets(m_Character.characterId, nebulaObject.resource as Res, out isNew);
            CreatePets();
        }

        public override void Start() {
            base.Start();
            m_Character = GetComponent<PlayerCharacterObject>();
        }

        public override void Update(float deltaTime) {
            if(!m_Initialized) {
                m_Initialized = true;
                CreatePets();
            }
        }


        public override int behaviourId {
            get {
                return (int)ComponentID.PetManager;
            }
        }

        public void Reinitialize() {
            m_Initialized = false;
        }

        public void CreatePets() {
            /*
            s_Log.InfoFormat("try create pets on pet manager [red]");
            if(m_Pets.Count == 0 ) {
                s_Log.InfoFormat("YES: pet is created... ".Color(LogColor.red));
                NebulaObject petObject = ObjectCreate.CreatePet(nebulaObject.world as MmoWorld, nebulaObject);
                (petObject as GameObject).AddToWorld();

                m_Pets.TryAdd(petObject.Id, petObject.GetComponent<PetObject>());

                petObject = ObjectCreate.CreatePet(nebulaObject.world as MmoWorld, nebulaObject);
                (petObject as GameObject).AddToWorld();
                m_Pets.TryAdd(petObject.Id, petObject.GetComponent<PetObject>());
            }*/
        }

        public void DestroyPets() {
            foreach (var kvp in m_Pets) {
                if (kvp.Value) {
                    (kvp.Value.nebulaObject as GameObject).Destroy();
                }
            }
        }

        public void RemovePet(string id) {
            PetObject pet;
            if(m_Pets.TryRemove(id, out pet)) {
                s_Log.InfoFormat("successfully removed pet = {0} [red]", id);
            }

            if(m_Pets.Count == 0 ) {
                s_Log.InfoFormat("PetManager pets now is empty [red]");
            }
        }

        public PetCollection pets {
            get {
                return m_PetInfoCollection;
            }
        }
    }
}
