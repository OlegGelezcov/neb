namespace Nebula.UI {
    public enum ChangeType : byte { ADD, UPDATE, REMOVE }

    public class ObjectChange<T> {
        public T TargetObject;
        public ChangeType ChangeType;
    }
}
