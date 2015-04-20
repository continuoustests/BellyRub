# BellyRub

BellyRub is a library for writing cross platform desktop applications using .NET and HTML. when you start bellyrub it will scan the machine for any known browsers and start it up in application mode for your application.

Just pull the source code and run deploy.sh/deploy.bat to compile it. You will find the complied library in the ReleaseBinaries folder. If you want to see how it works you can try the BellyRub.TestClient

Just create any console/windowed .Net application and put this in your main method
```C#
using System;
using System.IO;
using System.Threading;
using System.Reflection;
using BellyRub;
using BellyRub.UI;

namespace MyApplication
{
	class Program
	{
		static void Main(string[] args)
		{
            // Set a path where we keep our html code and create the engine
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "site");
            var engine = new BellyEngine(path);
            // Set up messaging to talk to your UI code and start the engine
            var browser = engine
                .OnConnected(() => Console.WriteLine("Client connected"))
                .OnDisconnected(() => Console.WriteLine("Client disconnected"))
                .OnSendException((ex) => Console.WriteLine(ex.ToString()))
                .On("hello-server", (m) => Console.WriteLine(m))
                .RespondTo("ping-server", (m, with) => with("pong from server"))
                .Start();

            // Wait for the browser to connect then communicate
            engine.WaitForFirstClientToConnect();
            engine.Send("hello-client", "hello world from server");
            Console.WriteLine(engine.Request("ping-client"));

            // Make the browser the topmost window
            browser.BringToFront(); 

            // Run for as long as the browser is connected
            while (engine.HasConnectedClients) {
                Thread.Sleep(50);
            }
		}
	}
}
```

Put an index html file in the directory specified in your main method with something like this.
```html
<!DOCTYPE html>
<html>
<head>
    <meta http-equiv="Cache-Control" content="no-cache, no-store, must-revalidate" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="Expires" content="0" />
	<title>MyApplication</title>

    <script type="text/javascript" src="/js/bellyrub-client.js"></script>

     <script type="text/javascript">
        // Createthe bellyrub client
        var client = createBellyRubClient(); 
        console.log('connecting..');
        // When connected communicate a bit
        client.onconnected = function () {
            client.send('hello-server', 'hello world from client');
            client.request('ping-server', '', function (m) { console.log(m); });
        };
        // Setup message handlers
        client.handlers['hello-client'] = function (msg, respondWith) {
            console.log(msg);
        };
        client.handlers['ping-client'] = function (msg, respondWith) {
            respondWith('pong from client');
        }; 
        // Connect to bellyrub engine
        client.connect(); 
    </script>   
</head>
<body>
Hello Bellyrub!
</body>
</html>
```

