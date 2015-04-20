using System;
using System.IO;
using System.Threading;
using System.Reflection;
using BellyRub;
using BellyRub.UI;

namespace BellyRub.TestClient
{
	class Program
	{
		static void Main(string[] args)
		{
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "site");
            var engine = new BellyEngine(path);
            Console.CancelKeyPress += (sender, e) => {
                engine.Stop();
                while (engine.HasConnectedClients) {
                    Thread.Sleep(50);
                }
            };
            var browser = engine 
                .OnConnected(() => Console.WriteLine("Client connected"))
                .OnDisconnected(() => Console.WriteLine("Client disconnected"))
                .OnSendException((ex) => Console.WriteLine(ex.ToString()))
                .On("hello-server", (m) => Console.WriteLine(m))
                .RespondTo("ping-server", (m, with) => with("pong from server"))
                .Start(new Point(100, 100));

            engine.WaitForFirstClientToConnect();
            engine.Send("hello-client", "hello world from server");
            Console.WriteLine(engine.Request("ping-client"));

            browser.BringToFront(); 

            while (engine.HasConnectedClients) {
                Thread.Sleep(50);
            }
		}
	}
}
