using System;
using System.Dynamic;
using System.Threading;
using BellyRub.UI;

namespace BellyRub.TestClient
{
	class Program
	{
		static void Main(string[] args)
		{
            var engine = new BellyEngine();
            Console.CancelKeyPress += (sender, e) => {
                engine.Stop();
                while (engine.HasConnectedClients) {
                    Thread.Sleep(50);
                }
            };
            var browser = engine
                .OnConnected(() =>  {
                    Console.WriteLine("Client connected");
                })
                .OnDisconnected(() => {
                    Console.WriteLine("Client disconnected");
                })
                .OnSendException((ex) => {
                    Console.WriteLine(ex.ToString());
                })
                .Start(new Point(100, 100));

            engine.WaitForFirstClientToConnect();
            browser.BringToFront();

            while (engine.HasConnectedClients) {
                Thread.Sleep(50);
            }
		}
	}
}
