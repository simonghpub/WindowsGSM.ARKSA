using System;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using WindowsGSM.Functions;
using WindowsGSM.GameServer.Engine;
using WindowsGSM.GameServer.Query;

namespace WindowsGSM.Plugins
{
    public class ARKSA : SteamCMDAgent
    {
        // - Plugin Details
        public Plugin Plugin = new Plugin
        {
            name = "WindowsGSM.ARKSA",
            author = "Simonth",
            description = "WindowsGSM plugin for a ARK Survival Ascended Dedicated server",
            version = "0.2",
            url = "https://github.com/simonghpub/WindowsGSM.ARKSA",
            color = "#9eff99"
        };


        // - Standard Constructor and properties
        public ARKSA(ServerConfig serverData) : base(serverData) => base.serverData = _serverData = serverData;
        private readonly ServerConfig _serverData;


        // - Settings properties for SteamCMD installer
        public override bool loginAnonymous => true;
        public override string AppId => "2430930"; // Appid for server


        // - Game server Fixed variables
        public override string StartPath => @"ShooterGame\Binaries\Win64\ArkAscendedServer.exe"; // Game server start path
        public string FullName = "ARK: Survival Ascended Dedicated Server";
        public bool AllowsEmbedConsole = false;  // Does this server support output redirect?
        public int PortIncrements = 2; // This tells WindowsGSM how many ports should skip after installation
        public object QueryMethod = null; // Query method, for now null. Accepted value: null or new A2S() or new FIVEM() or new UT3()


        // - Game server default values
        public string Port = "7820"; // Default port
        public string QueryPort = "";
        public string Defaultmap = "TheIsland_WP"; // Default map
        public string Maxplayers = "64"; // Default maxplayers
        public string Additional = ""; // Additional server start parameter


        // - Create a default cfg for the game server after installation
        public async void CreateServerCFG() { }


        // - Start server function, return its Process to WindowsGSM
        public async Task<Process> Start()
        {
            // Prepare start parameter
            var param = new StringBuilder();

            param.Append(string.IsNullOrWhiteSpace(_serverData.ServerMap) ? string.Empty : _serverData.ServerMap);

            param.Append(string.IsNullOrWhiteSpace(_serverData.ServerName) ? string.Empty : $"?SessionName=\"\"\"{_serverData.ServerName}\"\"\"");

            param.Append(string.IsNullOrWhiteSpace(_serverData.ServerName) ? string.Empty : $"?Port={_serverData.ServerPort}");
            param.Append(string.IsNullOrWhiteSpace(_serverData.ServerName) ? string.Empty : $"?MaxPlayers={_serverData.ServerMaxPlayer}");

            param.Append($"{_serverData.ServerParam}");
 
            // Prepare process
            var p = new Process
            {
                StartInfo =
                {
                    WindowStyle = ProcessWindowStyle.Minimized,
                    UseShellExecute = false,
                    WorkingDirectory = ServerPath.GetServersServerFiles(_serverData.ServerID),
                    FileName = ServerPath.GetServersServerFiles(_serverData.ServerID, StartPath),
                    Arguments = param.ToString()
                },

                EnableRaisingEvents = true
            };


            // Start process
            try
            {
                p.Start();
                return p;
            }
            catch (Exception e)
            {
                base.Error = e.Message;
                return null;
            }
        }


        // Stop server
        public async Task Stop(Process p)
        {
            await Task.Run(() =>
            {
                p.Kill();
            });
        }

        public string GetLocalBuild()
        {
            var steamCMD = new Installer.SteamCMD();
            return steamCMD.GetLocalBuild(_serverData.ServerID, AppId);
        }

        public async Task<string> GetRemoteBuild()
        {
            var steamCMD = new Installer.SteamCMD();
            return await steamCMD.GetRemoteBuild(AppId);
        }

    }
}
