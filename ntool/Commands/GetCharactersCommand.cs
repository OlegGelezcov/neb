using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ntool.Commands {
    public class GetCharactersCommand : BaseCommand{

        public GetCharactersCommand(string source, Application app)
            : base(source, app) { }


        public override void Execute() {
            string login = string.Empty;
            string gameRef = string.Empty;
            GetCommandArguments<string, string>(out login, out gameRef);

            Dictionary<byte, object> parameters = new Dictionary<byte, object> {
                { (byte)ParameterCode.GameRefId, gameRef },
                { (byte)ParameterCode.Login, login }
            };

            app.Operation(
                NebulaCommon.ServerType.SelectCharacter,
                (byte)SelectCharacterOperationCode.GetCharacters,
                parameters);

        }


    }
}
