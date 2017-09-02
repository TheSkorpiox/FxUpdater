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
        public bool activeCommand;

        public CommandManager(string _prefix)
        {
            prefix = _prefix;
            activeCommand = false;

            commands.Add("update", new Action<string, string>(Update));
            commands.Add("clear", new Action(Clear));
        }

        public void ParseCommand(string command)
        {
            if (command.StartsWith(prefix))
            {
                command = command.TrimStart(prefix.ToCharArray());
                List<string> commandArgs = command.Split(' ').ToList();
                ExecuteCommand(commandArgs);
                /*
                Delegate method = commands.FirstOrDefault(o => o.Key == commandArgs.First()).Value;
                if (method == null)
                {
                    Console.WriteLine("ERROR: Command does not exist");
                    return;
                }
                commandArgs.RemoveAt(0);

                try
                {
                    activeCommand = true;
                    method.DynamicInvoke(commandArgs.ToArray());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("ERROR: Command does not exist or have wrong parameters");

                    activeCommand = false;
                }
                */
            }
            else
                Console.WriteLine("ERROR: You should begin a command with '/'");
        }

        public void ExecuteCommand(List<string> commandArgs)
        {
            Delegate method = commands.FirstOrDefault(o => o.Key == commandArgs.First()).Value;
            if (method == null)
            {
                Console.WriteLine("ERROR: Command does not exist");
                return;
            }
            commandArgs.RemoveAt(0);

            try
            {
                activeCommand = true;
                method.DynamicInvoke(commandArgs.ToArray());

                while (activeCommand) ;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("ERROR: Command does not exist or have wrong parameters");

                activeCommand = false;
            }
        }

        public void Clear()
        {
            Console.Clear();
            activeCommand = false;
        }

        public async void Update(string distrib, string path)
        {
            // SUCH UGLY CODE
            if (distrib == string.Empty || (distrib != "linux" && distrib != "windows"))
            {
                Console.WriteLine("ERROR: Please choose \"linux\" or \"windows\" as first parameter");
                activeCommand = false;
                return;
            }
            if (path == string.Empty)
            {
                Console.WriteLine("ERROR: No path specified");
                activeCommand = false;
                return;
            }

            if (distrib == "linux")
                distrib = "build_proot_linux";
            else if (distrib == "windows")
                distrib = "build_server_windows";

            path = path.TrimEnd('/').TrimEnd('\\');

            Console.WriteLine("INFO: FxServer update running");

            using (WebClient client = new WebClient())
            {
                // Reading html
                string htmlFile = await client.DownloadStringTaskAsync($"https://runtime.fivem.net/artifacts/fivem/{distrib}/master/");
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(htmlFile);
                HtmlNode aElement = htmlDocument.DocumentNode.Descendants().Where(o => o.Name == "a").Last();
                string onlineVersion = aElement.InnerText.TrimEnd('/');

                // Creating directory & .version file
                Directory.CreateDirectory(path);

                Console.WriteLine($"INFO: Online version {onlineVersion} detected");
                if (File.Exists($"{path}/version"))
                {
                    string localVersion = File.ReadAllText($"{path}/version");
                    Console.WriteLine($"INFO: Local version {localVersion} detected");
                    if (localVersion == onlineVersion)
                    {
                        Console.WriteLine("ERROR: You already have the latest version");
                        activeCommand = false;
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
                else
                {
                    if (Directory.GetDirectories(path).Count() != 0 || Directory.GetFiles(path).Count() != 0)
                        path.CopyServerFiles("versionless");
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
            activeCommand = false;
        }

        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Console.Write($"{((double)e.BytesReceived).ToMo("N2")} / {((double)e.TotalBytesToReceive).ToMo("N2")} Mo -> {e.ProgressPercentage}% downloaded\r");

            if (e.BytesReceived == e.TotalBytesToReceive)
                Console.WriteLine($"{((double)e.BytesReceived).ToMo("N2")} / {((double)e.TotalBytesToReceive).ToMo("N2")} Mo -> {e.ProgressPercentage}% downloaded");
        }
    }
}
