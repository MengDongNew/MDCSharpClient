using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClientConsoleApplication.Controller;

namespace ClientConsoleApplication
{
    class Program
    {
        private static ClientEngine engine;
        static void Main(string[] args)
        {
            
            engine = new ClientEngine();
            engine.OnConnectToServer += Engine_OnConnectToServer;
            (new Thread(ThreadTimer)).Start();

            while (true)
            {
                Console.ReadLine();
            }
        }

        private static void Engine_OnConnectToServer()
        {
            ServerListController.Instance.RequestServerList();
        }

        static void ThreadTimer()
        {
            while (true)
            {
                Thread.Sleep(16);
                engine.Update();
            }
        }
    }
}
