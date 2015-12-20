using ExitGames.Client.Photon;

namespace ntool.Handlers {
    public class RegisterHandler : BaseHandler  {

        public RegisterHandler(byte code, Application context)
            : base(code, context ) { }

        public override void Handle(OperationResponse response) {
            LogResponse(response);
        }
    }
}
