using System;
using BellyRub.UI.WindowingHandlers;

namespace BellyRub.UI
{
	public static class Windowing
	{
        public static void BringToFront(int pid, string title) {
            var handler = getHandler();
            if (handler != null)
                handler.BringToFront(pid, title);
        }

        private static WindowingHandler getHandler() {
            if (OS.IsPosix)
                return new Posix();
            // Add for windows
            return null;
        }
	}
}
