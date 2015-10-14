using UnityEngine;

public class MmoConnectionParameters {
    public MmoConnectionFlags connectionFlags { get; private set; }

    public MmoConnectionParameters() {
        connectionFlags = MmoConnectionFlags.None;
    }

    public void SetConnectionFlags(MmoConnectionFlags flags) {
        connectionFlags = flags;
    }

    public void SetPosition(Vector3 pos) {
        savedPosition = pos;
    }

    public void SetWorld(string w) {
        world = w;
    }

    public Vector3 savedPosition { get; private set; }
    public string world { get; private set; }
}