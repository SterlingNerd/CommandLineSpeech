using System;
using System.Linq;

using NAudio.CoreAudioApi;

namespace SterlingDigital.CommandLineSpeech.Speak
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var consoleArgs = ParseArgs(args);

			if (string.IsNullOrWhiteSpace(consoleArgs?.TextToSpeak))
			{
				ShowHelp();
				return;
			}

			var Synthesizer = new Synthesizer(consoleArgs.Device, consoleArgs.Voice);
			Synthesizer.Speak(consoleArgs.TextToSpeak);

		}

		private static ConsoleArgs ParseArgs(string[] args)
		{
			if (args == null)
			{
				return null;
			}

			var consoleArgs = new ConsoleArgs();
			var arg = args.AsEnumerable().GetEnumerator();

			while (arg.MoveNext())
			{
				switch (arg.Current.ToUpperInvariant())
				{
					case "-D":
					case "--DEVICE":
						arg.MoveNext();
						consoleArgs.Device = ParseDevice(arg.Current);
						break;
					case "-V":
					case "--VOICE":
						arg.MoveNext();
						consoleArgs.Voice = arg.Current;
						break;
					default:
						consoleArgs.TextToSpeak = arg.Current;
						break;
				}
			}

			return consoleArgs;
		}

		private static Role ParseDevice(string roleString)
		{
			Role role;
			if (!Enum.TryParse(roleString, out role))
			{
				role = Role.Console;

			}
			return role;
		}

		private static void ShowHelp()
		{
			Console.WriteLine("Syntax: Speak [options] \"Text To Speak\"");
			Console.WriteLine();
			Console.WriteLine("Options:");
			Console.WriteLine("   -d|--device\tThe device to use (\"{0}\")", string.Join("\", \"", Enum.GetNames(typeof(Role))));
			Console.WriteLine("   -v|--voice\tThe voice to use. (Defaults to the first one found)");
			Console.WriteLine("Example:");
			Console.WriteLine("Speak -d Communications -v eva \"I can't let you do that chief.\"");
		}
	}
}
