using System;
using System.Diagnostics;

namespace BellyRub.UI.WindowingHandlers
{
	class Posix : WindowingHandler
	{
        public void BringToFront(int pid, string title) {
            string windowId = null;
            new System.Diagnostics.Process().Query(
                "/usr/bin/wmctrl",
                "-l",
                false,
                Environment.CurrentDirectory,
                (err, msg) => {
                    if (err) {
                        return;
                    }
                    var chunks = msg.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
                    if (chunks.Length >= 4 && chunks[3] == title)
                        windowId = chunks[0];
                }
            );
            if (windowId == null)
                return;

            var args = "-ia \"" + windowId + "\"";
            new System.Diagnostics.Process().Query(
                "/usr/bin/wmctrl",
                args,
                false,
                Environment.CurrentDirectory,
                (err, msg) => {});
        }
	}
}