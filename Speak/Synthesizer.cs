using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Threading;

using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace SterlingDigital.CommandLineSpeech.Speak
{
	public class Synthesizer
	{
		private readonly int latency = 100;
		public Role Device { get; set; }
		public bool RandomVoice { get; set; }
		public string Voice { get; set; }
		private SpeechAudioFormatInfo Format => new SpeechAudioFormatInfo(EncodingFormat.Pcm, waveFormat.SampleRate, waveFormat.BitsPerSample, waveFormat.Channels, waveFormat.AverageBytesPerSecond, waveFormat.BlockAlign, null);
		private WaveFormat waveFormat => new WaveFormat();

		public Synthesizer(Role device, string voice, bool randomVoice)
		{
			Device = device;
			Voice = voice;
			RandomVoice = randomVoice;
		}

		public void Speak(string textToSpeak)
		{
			using (var stream = new MemoryStream())
			{
				using (var synth = new SpeechSynthesizer())
				{
					SetVoice(synth);
					synth.SetOutputToAudioStream(stream, Format);
					synth.Speak(textToSpeak);

					stream.Position = 0;

					var device = GetDevice();

					using (var player = new WasapiOut(device, AudioClientShareMode.Shared, false, latency))
					{
						player.PlaybackStopped += PlayerOnPlaybackStopped;
						using (var wavStream = new RawSourceWaveStream(stream, waveFormat))
						{
							player.Init(wavStream);
							player.Play();
							while (player.PlaybackState == PlaybackState.Playing)
							{
								Thread.Sleep(latency);
							}
						}
					}
				}
			}
		}

		private MMDevice GetDevice()
		{
			var enumerator = new MMDeviceEnumerator();
			return enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Device);
		}

		private void PlayerOnPlaybackStopped(object sender, StoppedEventArgs stoppedEventArgs)
		{
			if (stoppedEventArgs.Exception != null)
			{
				Console.WriteLine(stoppedEventArgs.Exception);
				Console.ReadKey();
			}
		}

		private void SetVoice(SpeechSynthesizer synth)
		{
			var voices = synth.GetInstalledVoices();

			InstalledVoice voice;
			if (RandomVoice)
			{
				voice = voices.OrderBy(x => Guid.NewGuid()).First();
			}
			else
			{
				voice = voices.FirstOrDefault(v => CultureInfo.CurrentCulture.CompareInfo.IndexOf(v.VoiceInfo.Name, Voice ?? string.Empty, CompareOptions.IgnoreCase) >= 0) ?? voices.First();
			}

			synth.SelectVoice(voice.VoiceInfo.Name);
		}
	}
}
