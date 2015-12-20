namespace Nebula.Client {
    using Common;
    using global::Common;

    public interface IServiceMessage {
        ServiceMessageType Type { get; }
        string Message { get; }
    }
}
