using System;
using System.IO;

namespace BellyRub.UI
{
	class BrowserLocator
	{
        public BrowserInstance Find() {
            if (OS.IsPosix)
                return findInPosix();
            else
                return findInWindows();
        }

        private BrowserInstance findInWindows() {
            return null;
        }

        private BrowserInstance findInPosix() {
            if (File.Exists("/opt/google/chrome/chrome")) {
                return 
                    new BrowserInstance(
                        "/opt/google/chrome/chrome",
                        "--app={{url}}",
                        "--window-position={{x}},{{y}}",
                        "--window-size={{width}},{{height}}");
            } else if (File.Exists("/usr/bin/firefox")) {
                return new BrowserInstance("/usr/bin/firefox", "{{url}}", "", "");
            }
            return null;
        }
	}
}