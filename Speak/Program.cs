using System;
using System.Linq;
using System.Speech.Synthesis;

namespace SterlingDigital.CommandLineSpeech.Speak
{
	public class Program
	{
		public static void Main(string[] args)
		{
			if (args == null || !args.Any())
			{
				// Bail, nothing to say.
				return;
			}

			using (var synth = new SpeechSynthesizer())
			{
				synth.Speak(args.First());
			}
		}
	}
}
