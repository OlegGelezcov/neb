using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ntool.Commands
{
    public class HelpCommand : BaseCommand
    {
        public HelpCommand(string source, Application app)
            : base(source, app) { }


        public override void Execute()
        {
            app.logger.PushColor(ConsoleColor.Green);
            app.logger.Log("ucount - number of users on server", ConsoleColor.Green);
            app.logger.Log("exit - safe exit from program", ConsoleColor.Green);
            app.logger.PopColor();
        }
    }
}
