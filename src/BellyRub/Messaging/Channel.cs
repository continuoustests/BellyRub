using System;
using System.Threading;
using System.Dynamic;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Newtonsoft.Json.Linq;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace BellyRub.Messaging
{
	class Channel
	{
        private WebSocketServer _server;
        private WebSocketServiceManager _serverServiceManager;
        private Chat _serverChatService;
        private ConcurrentDictionary<string, Action<object>> _responseHandlers = new ConcurrentDictionary<string, Action<object>>();

        private Action<Exception> _onSendException = (ex) => {};

        public string Url { get; private set; }
        public bool HasConnectedClients { get { return _serverServiceManager.SessionCount > 0; } }

        public Channel() {
            _serverChatService = new Chat();
        }

        public Channel OnReceive(Action<BellyEngine.Message> action) {
            _serverChatService.OnReceive((msg) => {
                if (_responseHandlers.ContainsKey(msg.Subject)) {
                    _responseHandlers[msg.Subject](msg.Body);
                    return;
                }
                action(msg);
            });
            return this;
        }

        public Channel OnConnected(Action action) {
            _serverChatService.OnConnected(action);
            return this;
        }

        public Channel OnDisconnected(Action action) {
            _serverChatService.OnDisconnected(action);
            return this;
        }

        public Channel OnSendException(Action<Exception> action) {
            _onSendException = action;
            return this;
        }

        public void Start() {
            while (true) {
                if (start())
                    break;
            }
        }

        public void Stop() {
            if (_server != null) {
                Send("shutdown", new object());
                while (HasConnectedClients)
                    Thread.Sleep(10);
                _server.Stop();
            }
        }

        public void WaitForFirstClientToConnect() {
            waitForFirstClient();
        }

        public void Send(string subject, object body) {
            try {
                dynamic payload = new ExpandoObject();
                payload.subject = subject;
                payload.body = body;
                payload.token = "";
                var json = LowerCaseSerializer.SerializeObject(payload);
                _serverChatService.SendMessage(json);
            } catch (Exception ex) {
                _onSendException(ex);
            }
        }

        public dynamic Request(string subject, object body) {
            object response = null;
            try {
                var token = Guid.NewGuid().ToString();
                dynamic payload = new ExpandoObject();
                payload.subject = subject;
                payload.body = body;
                payload.token = token;
                var json = LowerCaseSerializer.SerializeObject(payload);
                _responseHandlers.TryAdd(token, (msg) => {
                    response = msg;
                });
                _serverChatService.SendMessage(json);
                while (response == null) {
                    Thread.Sleep(200);
                }
                Action<object> removedItem;
                _responseHandlers.TryRemove(token, out removedItem);
                return (dynamic)response;
            } catch (Exception ex) {
                _onSendException(ex);
            }
            return null;
        }

        private bool start() {
            var port = new Random().Next(1025, 65535);
            try {
                var url = "ws://localhost:" + port.ToString();
                _server = new WebSocketServer(url);
                _serverServiceManager = _server.WebSocketServices;
                _server.AddWebSocketService<Chat>("/chat", () => _serverChatService);
                _server.Start();
                Url = url;
                return true;
            } catch {
            }
            return false;
        }

        private void waitForFirstClient() {
            var timeout = DateTime.Now.AddSeconds(10);
            while (!HasConnectedClients && DateTime.Now < timeout) {
                Thread.Sleep(10);
            }
        }
	}

    class Chat : WebSocketService
    {
        private Action<BellyEngine.Message> _onReceive = (msg) => {};
        private Action _onConnected = () => {};
        private Action _onDisconnected = () => {};

        public void OnReceive(Action<BellyEngine.Message> action) {
            _onReceive = action;
        }

        public void OnConnected(Action action) {
            _onConnected = action;
        }

        public void OnDisconnected(Action action) {
            _onDisconnected = action;
        }

        public void SendMessage(string json) {
            Sessions.Broadcast(json);
        }

        protected override void OnOpen() {
            _onConnected();
        }

        protected override void OnClose(CloseEventArgs e) {
            _onDisconnected();
        }

        protected override void OnMessage (MessageEventArgs e)
        {
            dynamic msg = JObject.Parse(e.Data);
            _onReceive(new BellyEngine.Message(msg.subject.ToString(), msg.token.ToString(), msg.body));
        }
    }
}