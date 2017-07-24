using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FxUpdater
{
    public class CommandManager
    {
        private Dictionary<string, Delegate> commands = new Dictionary<string, Delegate>();
        private string prefix;

        public CommandManager(string _prefix)
        {
            prefix = _prefix;

            commands.Add("update", new Action(Update));
        }

        public void ExecuteCommand(string command)
        {
            if (command.StartsWith(prefix))
            {
                command = command.TrimStart(prefix.ToCharArray());
                List<string> commandArgs = command.Split(' ').ToList();
                Delegate method = commands.FirstOrDefault(o => o.Key == commandArgs.First()).Value;
                if (method == null)
                {
                    Console.WriteLine("ERROR: Command does not exist");
                    return;
                }
                commandArgs.RemoveAt(0);

                try
                {
                    method.DynamicInvoke(commandArgs.ToArray());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("ERROR: Command does not exist or have wrong parameters");
                }
            }
            else
                Console.WriteLine("ERROR: You should begin a command with '/'");
        }

        public async void Update()
        {
            Console.WriteLine("INFO: FxServer update running");

            using (WebClient client = new WebClient())
            {
                string htmlFile = await client.DownloadStringTaskAsync("https://runtime.fivem.net/artifacts/fivem/build_server_windows/master/");
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(htmlFile);
                HtmlNode aElement = htmlDocument.DocumentNode.Descendants().Where(o => o.Name == "a").Last();

                Console.WriteLine("INFO: Downloading file");
                client.DownloadProgressChanged += OnDownloadProgressChanged;
                await client.DownloadFileTaskAsync($"https://runtime.fivem.net/artifacts/fivem/build_server_windows/master/{aElement.InnerText}/server.zip", "C:/Users/Marceau/Desktop/server.zip");
            }
            
            // TODO: Unzip, create a backup, and install new version

            Console.WriteLine("INFO: Update finished");
        }

        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Console.WriteLine(e.ProgressPercentage);
        }
    }
}
