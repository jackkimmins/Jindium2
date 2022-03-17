using System.Diagnostics;
using Jindium;
using System.Linq;

namespace JindiumHost
{
    public class JinServerHost
    {
        public Process Process { get; set; }
        public string ID { get; set; }
        public bool IsRunning { get; set; } = false;

        public void Start()
        {
            ProcessStartInfo sinfo = new ProcessStartInfo();
            sinfo.UseShellExecute = true;
            sinfo.FileName = Process.StartInfo.FileName;
            sinfo.RedirectStandardInput = false;
            sinfo.RedirectStandardOutput = false;

            Process.Start(sinfo);
            IsRunning = true;
        }

        public void Stop()
        {
            Process.Kill();
            
            IsRunning = false;
        }
    }

    public class JinHost
    {
        private List<JinServerHost> _servers = new List<JinServerHost>();
        private string WorkingDirectory { get; } = "";

        private string RndID()
        {
            return System.Guid.NewGuid().ToString();
        }

        public void StartServer(string id)
        {
            //Use Linq to select the server with the given id
            var server = _servers.Where(s => s.ID == id).FirstOrDefault();
            server.Start();
        }

        public List<JinServerHost> OnlineServers
        {
            get
            {
                List<JinServerHost> servers = new List<JinServerHost>();

                foreach (JinServerHost server in _servers)
                {
                    if (server.IsRunning)
                    {
                        servers.Add(server);
                    }
                }

                return servers;
            }
        }

        public string AddJinServer(string jinServer, string jinServerPath)
        {
            var server = new Process();

            server.StartInfo.FileName = System.IO.Path.Combine(WorkingDirectory, jinServerPath, jinServer);

            server.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(server.StartInfo.FileName);

            server.StartInfo.Arguments = "--port=0";
            server.StartInfo.UseShellExecute = true;
            server.StartInfo.RedirectStandardOutput = true;
            server.StartInfo.RedirectStandardError = true;

            var ID = RndID();

            _servers.Add(new JinServerHost()
            {
                Process = server,
                ID = ID
            });

            return ID;
        }

        public JinHost(string workingDirectory = "_JinHosts")
        {
            WorkingDirectory = workingDirectory;

            if (!System.IO.Directory.Exists(WorkingDirectory))
            {
                System.IO.Directory.CreateDirectory(WorkingDirectory);
            }
        }

    }
}