using Nebula.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ntool.Commands {
    public class GetWebCharacterObjectCommand : BaseCommand {

        public GetWebCharacterObjectCommand(string source, Application context)
            : base(source, context) { }

        public override void Execute() {

            Thread thread = new Thread(() => {
                string url = "http://localhost/get_characters.php?game_ref=ef0f0497-01f2-4037-a1d0-f1a49b2e8ce4";
                WebRequest request = WebRequest.Create(url);
                Stream objStream = request.GetResponse().GetResponseStream();
                StreamReader objReader = new StreamReader(objStream);
                string str = objReader.ReadToEnd();
                objReader.Close();
                objStream.Close();
                app.logger.PushColor(ConsoleColor.DarkYellow);
                app.logger.Log(str);
                app.logger.PopColor();

                try {
                    ClientPlayerCharactersContainer characters = new ClientPlayerCharactersContainer(str);
                    app.logger.PushColor(ConsoleColor.Magenta);
                    app.logger.Log("parsed game ref: {0}", characters.GameRefId);
                    app.logger.Log("parsed selected character id: {0}", characters.SelectedCharacterId);
                    app.logger.Log("parsed characters count: {0}", characters.Characters.Count);

                    foreach(var playerCharacter in characters.Characters) {
                        app.logger.PushColor(ConsoleColor.Gray);
                        app.logger.Log(playerCharacter.ToString());
                        app.logger.PopColor();
                    }
                    app.logger.PopColor();
                } catch(Exception exception ) {
                    app.logger.PushColor(ConsoleColor.Red);
                    app.logger.Log(exception.Message);
                    app.logger.Log(exception.StackTrace);
                    app.logger.PopColor();
                }
            });
            thread.Start();
        }

    }
}
