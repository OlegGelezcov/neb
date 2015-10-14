namespace Nebula.UI {
    public interface IObjectInfo {

        ObjectInfoType InfoType { get; }

        string Name { get; }

        string Description { get; }
    }
}
