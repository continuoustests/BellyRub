using System;
using System.IO;
using System.Reflection;
using Nancy;
using Nancy.TinyIoc;
using Nancy.Conventions;
using Nancy.Bootstrapper;
using Nancy.Hosting.Self;

namespace BellyRub.WebServer
{
	class Server
	{
        private NancyHost _host;

        public string Url { get; private set; }

        public void Start(int port) {
            generateDefaultSite();
            while (true) {
                if (start(port))
                    break;
            }
        }

        public void Stop() {
            if (_host != null)
                _host.Stop();
        }

        private bool start(int port) {
            try {
                var url = "http://localhost:" + port.ToString();
                var config = new HostConfiguration();
                config.RewriteLocalhost = false;
                _host = new NancyHost(config, new Uri(url));
                _host.Start();
                Url = url;
                return true;
            } catch {
            }
            return false;
        }

        private void generateDefaultSite() {
            if (!Directory.Exists(RESTBootstrapper.RootDir()))
                Directory.CreateDirectory(RESTBootstrapper.RootDir());
            writefile("index.html", RESTBootstrapper.RootDir());
        }

        private void writefile(string resourceFileName, string writeDirectory) {
            var file = Path.Combine(writeDirectory, resourceFileName);
            if (File.Exists(file))
                return;
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "BellyRub.WebServer.site." + resourceFileName;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName)) {
                using (StreamReader reader = new StreamReader(stream)) {
                    File.WriteAllText(file, reader.ReadToEnd());
                }
            }
        }
	}

    public class ClientModule : NancyModule
    {
        public ClientModule()
        {
            Get["/js/bellyrub-client.js"] = _ =>
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "BellyRub.WebServer.site.bellyrub-client.js";
                using (Stream stream = assembly.GetManifestResourceStream(resourceName)) {
                    using (StreamReader reader = new StreamReader(stream)) {
                        return reader.ReadToEnd();
                    }
                }
            };
        }
    }

    public class RESTBootstrapper : DefaultNancyBootstrapper
    {
        private static string _rootDir = null;
        public static string RootDir() {
            if (_rootDir == null) {
                var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()).Replace(".", "");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                _rootDir = path;
            }
            return _rootDir;
        }

        public static void SetRootDir(string dir) {
            _rootDir = dir;
            if (!Directory.Exists(_rootDir))
                Directory.CreateDirectory(_rootDir);
        }

        protected override IRootPathProvider RootPathProvider {
            get { return new CustomRootPathProvider(); }
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines) {
        }
    
        protected override void ConfigureConventions(NancyConventions nancyConventions) {
            Conventions.StaticContentsConventions.Add(
                StaticContentConventionBuilder.AddDirectory("site", @".")
            );
            base.ConfigureConventions(nancyConventions);
        }
    }

    public class CustomRootPathProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
            return RESTBootstrapper.RootDir();
        }
    }
}
