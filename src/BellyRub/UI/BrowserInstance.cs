using System;

namespace BellyRub.UI
{
	class BrowserInstance
	{
        private string _arguments;
        private string _positionArgs;
        private string _sizeArgs;

        public string Executable { get; private set; }

        public BrowserInstance(string executable, string arguments, string positionArgs, string sizeArgs) {
            Executable = executable;
            _arguments = arguments;
            _positionArgs = positionArgs;
            _sizeArgs = sizeArgs;
        }

        public string GetArguments(string url, Point position, Size size) {
            var args = _arguments.Replace("{{url}}", url);
            if (position != null) {
                args += " " + _positionArgs
                    .Replace("{{x}}", position.X.ToString())
                    .Replace("{{y}}", position.Y.ToString());
            }
            if (size != null) {
                args += " " + _sizeArgs
                    .Replace("{{width}}", size.Width.ToString())
                    .Replace("{{height}}", size.Height.ToString());
            }
            return args;
        }
	}
}