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

            while (true)
            {
                cmdManager.ExecuteCommand(Console.ReadLine());
            }
        }
    }
}
