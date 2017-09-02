using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FxUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandManager cmdManager = new CommandManager("/");

            if (args.Count() != 0)
                cmdManager.ExecuteCommand(args.ToList());
            else
            {
                while (true)
                {
                    if (cmdManager.activeCommand == false)
                        Console.Write("> ");

                    cmdManager.ParseCommand(Console.ReadLine());
                }
            }
        }
    }
}
