namespace Nebula.Client {
    using Common;

    public interface IServiceMessage {
        ServiceMessageType Type { get; }
        string Message { get; }
    }
}
