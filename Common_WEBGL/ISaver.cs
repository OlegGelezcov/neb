
namespace Common {
    public interface IBaseSaver {
        void SaveModified();
    }

    public interface ISaver<T> {
        void Update(T obj);

    }
}
