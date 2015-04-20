using System;
using System.IO;
using System.Reflection;

namespace BellyRub.UI
{
	class BrowserLocator
	{
        public BrowserInstance Find() {
            var configuredInstance = getConfiguredInstance();
            if (configuredInstance != null)
                return configuredInstance;

            if (OS.IsPosix)
                return findInPosix();
            else
                return findInWindows();
        }

        private BrowserInstance findInWindows() {
            var pf = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            var pf86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            var appDataLocal = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (File.Exists(Path.Combine(appDataLocal, "Google", "Chrome", "Application", "Chrome.exe"))) {
                return 
                    new BrowserInstance(
                        Path.Combine(appDataLocal, "Google", "Chrome", "Application", "Chrome.exe"),
                        "--app={{url}} --kiosk",
                        "--window-position={{x}},{{y}}",
                        "--window-size={{width}},{{height}}");
            } else if (File.Exists(Path.Combine(pf, "Google", "Chrome", "Application", "Chrome.exe"))) {
                return 
                    new BrowserInstance(
                        Path.Combine(pf, "Google", "Chrome", "Application", "Chrome.exe"),
                        "--app={{url}} --kiosk",
                        "--window-position={{x}},{{y}}",
                        "--window-size={{width}},{{height}}");
            } else if (File.Exists(Path.Combine(pf86, "Google", "Chrome", "Application", "Chrome.exe"))) {
                return 
                    new BrowserInstance(
                        Path.Combine(pf86, "Google", "Chrome", "Application", "Chrome.exe"),
                        "--app={{url}} --kiosk",
                        "--window-position={{x}},{{y}}",
                        "--window-size={{width}},{{height}}");
            }
            return null;
        }

        private BrowserInstance findInPosix() {
            if (File.Exists("/opt/google/chrome/chrome")) {
                return 
                    new BrowserInstance(
                        "/opt/google/chrome/chrome",
                        "--app={{url}} --kiosk",
                        "--window-position={{x}},{{y}}",
                        "--window-size={{width}},{{height}}");
            } else if (File.Exists("/usr/bin/chromium-browser")) {
                return 
                    new BrowserInstance(
                        "/usr/bin/chromium-browser",
                        "--app={{url}} --kiosk",
                        "--window-position={{x}},{{y}}",
                        "--window-size={{width}},{{height}}");
            } else if (File.Exists("/usr/bin/firefox")) {
                return new BrowserInstance("/usr/bin/firefox", "{{url}}", "", "");
            } else if (File.Exists("/usr/bin/opera")) {
                return new BrowserInstance("/usr/bin/opera", "{{url}}", "", "");
            } else if (File.Exists("/Applications/Google Chrome.app/Contents/MacOS/Google Chrome")) {
                new BrowserInstance(
                        "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome",
                        "--app={{url}} --kiosk",
                        "--window-position={{x}},{{y}}",
                        "--window-size={{width}},{{height}}"); 
            } else if (File.Exists("/Applications/Firefox.app/Contents/MacOS/firefox-bin")) {
                return new BrowserInstance("/Applications/Firefox.app/Contents/MacOS/firefox-bin", "{{url}}", "", "");
            }
            return null;
        }

        private BrowserInstance getConfiguredInstance() {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var file = Path.Combine(dir, "BellyRub.browser");
            if (!File.Exists(file))
                return null;
            var chunks = File.ReadAllText(file).Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            if (chunks.Length == 0)
                return null;
            var executable = chunks[0];

            var urlargs = "{{url}}";
            if (chunks.Length > 1)
                urlargs = chunks[1];

            var positionargs = "";
            if (chunks.Length > 2)
                positionargs = chunks[2];

            var dimensionargs = "";
            if (chunks.Length > 3)
                dimensionargs = chunks[3];

            return new BrowserInstance(executable, urlargs, positionargs, dimensionargs);
        }
	}
}
