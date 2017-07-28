using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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

            commands.Add("update", new Action<string>(Update));
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

        public async void Update(string path)
        {
            Console.WriteLine("INFO: FxServer update running");

            path = path.TrimEnd('/').TrimEnd('\\');

            using (WebClient client = new WebClient())
            {
                // Reading html
                string htmlFile = await client.DownloadStringTaskAsync("https://runtime.fivem.net/artifacts/fivem/build_server_windows/master/");
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(htmlFile);
                HtmlNode aElement = htmlDocument.DocumentNode.Descendants().Where(o => o.Name == "a").Last();
                string onlineVersion = aElement.InnerText.TrimEnd('/');

                // Creating directory & .version file
                Directory.CreateDirectory(path);

                Console.WriteLine($"INFO: Latest version detected is {onlineVersion}");
                if (File.Exists($"{path}/version"))
                {
                    string localVersion = File.ReadAllText($"{path}/version");
                    Console.WriteLine($"INFO: Local version {localVersion} detected");
                    if (localVersion == onlineVersion)
                    {
                        Console.WriteLine("ERROR: You already have the latest version");
                        return;
                    }
                    else
                    {
                        Directory.CreateDirectory($"{path}/{localVersion}");

                        string[] files = Directory.GetFiles(path);

                        foreach (string file in files)
                        {
                            string fileName = file.Split('\\').Last();
                            File.Move($"{path}/{fileName}", $"{path}/{localVersion}/{fileName}");
                        }

                        Directory.Move($"{path}/citizen", $"{path}/{localVersion}/citizen");
                    }
                }

                File.Create($"{path}/version").Close();
                File.WriteAllText($"{path}/version", onlineVersion);
                
                // Downloading file
                Console.WriteLine("INFO: Downloading file");
                client.DownloadProgressChanged += OnDownloadProgressChanged;

                await client.DownloadFileTaskAsync($"https://runtime.fivem.net/artifacts/fivem/build_server_windows/master/{aElement.InnerText}/server.zip", $"{path}/server.zip");
            }

            // TODO: create a backup with a versionning system
            ZipFile.ExtractToDirectory($"{path}/server.zip", path);
            File.Delete($"{path}/server.zip");

            Console.WriteLine("INFO: Update finished");
        }

        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Console.Write($"{((double)e.BytesReceived).ToMo("N2")} / {((double)e.TotalBytesToReceive).ToMo("N2")} Mo -> {e.ProgressPercentage}% downloaded\r");

            if (e.BytesReceived == e.TotalBytesToReceive)
                Console.WriteLine($"{((double)e.BytesReceived).ToMo("N2")} / {((double)e.TotalBytesToReceive).ToMo("N2")} Mo -> {e.ProgressPercentage}% downloaded");
        }
    }
}
