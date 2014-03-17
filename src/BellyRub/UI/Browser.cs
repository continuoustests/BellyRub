using System;
using System.IO;
using System.Diagnostics;

namespace BellyRub.UI
{
	public class Browser
	{
        private Process _process;
        private Func<string> _windowNameFetcher;

        internal Browser(Func<string> windowNameFetcher) {
            _windowNameFetcher = windowNameFetcher;
        }

        public void BringToFront() {
            var title = _windowNameFetcher();
            Windowing.BringToFront(_process.Id, title);
        }

        internal void Launch(string url, string ws, Point position, Size size) {
            var locator = new BrowserLocator();
            var browser = locator.Find();
            if (browser != null) {
                _process = new Process();
                var info = new ProcessStartInfo(
                    browser.Executable, 
                    browser.GetArguments(url + "/site/index.html?channel="+ws, position, size)
                );
                info.WindowStyle = ProcessWindowStyle.Minimized;
                info.UseShellExecute = false;
                info.RedirectStandardOutput = true;
                info.RedirectStandardError = true;
                _process.StartInfo = info;
                _process.Start();
            }
        }

        internal void Kill() {
            if (_process == null)
                return;
            if (_process.HasExited)
                return;
            _process.Kill();
        }
	}
}