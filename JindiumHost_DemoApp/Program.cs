using JindiumHost;

class Program
{
    static void Main()
    {
        JinHost host = new JinHost();
        var id = host.AddJinServer("Jindium2_DemoApp.exe", "Demo");
        host.StartServer(id);

        while (true)
        {
            Console.ReadKey();

            foreach (var sever in host.OnlineServers)
            {
                Console.WriteLine(sever.IsRunning);
            }
        }
    }
}