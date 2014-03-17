using System;

namespace BellyRub.UI.WindowingHandlers
{
	interface WindowingHandler
	{
        void BringToFront(int pid, string title);
	}
}