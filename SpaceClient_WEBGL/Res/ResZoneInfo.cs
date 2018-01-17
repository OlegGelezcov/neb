namespace Nebula.Client.Res
{
    public class ResZoneInfo
    {
        private readonly string id;
        private readonly string scene;
        private readonly string displayName;
        private readonly bool isAvailable;

        //private float[] m_HumanSpawnPoint;
        //private float[] m_CriptozodSpawnPoint;
        //private float[] m_BorguzandSpawnPoint;

        private readonly bool isNull;

        public ResZoneInfo(string id, string scene, string displayName, bool isAvailable)
        {
            this.id = id;
            this.scene = scene;
            this.displayName = displayName;
            this.isNull = (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(scene));
            this.isAvailable = isAvailable;
            //humanSpawnPoint = humanSP;
            //criptizidSpawnPoint = criptSP;
            //borguzandSpawnPoint = borgSP;
        }

        /*
        public float[] humanSpawnPoint {
            get {
                if(m_HumanSpawnPoint == null ) {
                    m_HumanSpawnPoint = new float[] { 0, 0, 0 };
                }
                return m_HumanSpawnPoint;
            }
            private set {
                if(value == null ) {
                    value = new float[] { 0, 0, 0 };
                }
                m_HumanSpawnPoint = value;
            }
        }

        public float[] criptizidSpawnPoint {
            get {
                if(m_CriptozodSpawnPoint == null ) {
                    m_CriptozodSpawnPoint = new float[] { 0, 0, 0 };
                }
                return m_CriptozodSpawnPoint;
            }
            private set {
                if (value == null) {
                    value = new float[] { 0, 0, 0 };
                }
                m_CriptozodSpawnPoint = value;
            }
        }

        public float[] borguzandSpawnPoint {
            get {
                if(m_BorguzandSpawnPoint == null ) {
                    m_BorguzandSpawnPoint = new float[] { 0, 0, 0 };
                }
                return m_BorguzandSpawnPoint;
            }
            private set {
                if (value == null) {
                    value = new float[] { 0, 0, 0 };
                }
                m_BorguzandSpawnPoint = value;
            }
        }*/

        public string Id()
        {
            return this.id;
        }

        public string Scene()
        {
            return this.scene;
        }

        public string DisplayName()
        {
            return this.displayName;
        }

        public bool IsNull()
        {
            return this.isNull;
        }

        public bool IsAvailable {
            get {
                return isAvailable;
            }
        }

        public override string ToString() {
            return string.Format("Id => {0}, Scene => {1}, Disp. Name => {2}, Is Available => {2}, Is N/A => {4}",
                Id(), Scene(), DisplayName(), IsAvailable, IsNull());
        }

        public static ResZoneInfo Null()
        {
            return new ResZoneInfo(string.Empty, string.Empty, string.Empty, false);
        }
    }
}
