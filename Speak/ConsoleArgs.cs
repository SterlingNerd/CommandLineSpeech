using System;

using NAudio.CoreAudioApi;

namespace SterlingDigital.CommandLineSpeech.Speak
{
	public class ConsoleArgs
	{
		public Role Device { get; set; }
		public string TextToSpeak { get; set; }
		public string Voice { get; set; }
		public bool RandomVoice { get; set; }
	}
}
