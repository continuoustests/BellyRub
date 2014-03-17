using System;

namespace BellyRub
{
	static class OS
	{
        public static bool IsPosix {
            get {
                return 
                    Environment.OSVersion.Platform == PlatformID.Unix || 
                    Environment.OSVersion.Platform == PlatformID.MacOSX;
            }
        }
	}
}