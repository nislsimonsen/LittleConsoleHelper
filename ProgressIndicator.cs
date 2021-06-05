using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace LittleConsoleHelper
{
	public abstract class ProgressIndicator
	{
		public static T Start<T>(int current, int max, string initialMessage = "", int updateTimeout = 100)
			where T : ProgressIndicator, new()
		{
			if (updateTimeout < 50)
				updateTimeout = 50;
			var r = new T();
			r.Current = current;
			r.Max = max;
			r.UpdateTimeout = updateTimeout;

			r.Init(initialMessage);
			return r;
		}

		public void SetProgress(int value, string additionalMessage = "")
		{
			Current = value;
			AdditionalMessage = additionalMessage;
			if (Current == Max)
				StopAfterNextRendering = true;
		}
		public void Stop()
		{
			Timer.Stop();
		}
		public int Current { get; set; }
		public int Max { get; set; }
		private bool StopAfterNextRendering = false;
		private string AdditionalMessage { get; set; }
		private Timer Timer;
		private int UpdateTimeout;
		
		private void Init(string initialMessage)
		{
			AdditionalMessage = initialMessage;
			Render(true, AdditionalMessage);
			Timer = new Timer(UpdateTimeout) { AutoReset = true };
			Timer.Elapsed += Timer_Elapsed;
			Timer.Start();
		}

		private void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			Render(false, AdditionalMessage);
			if (StopAfterNextRendering)
				Stop();
		}

		protected abstract void Render(bool first, string additionalMessage);
		
		
	}

	public class ProgressBar : ProgressIndicator
	{
		protected override void Render(bool first, string additionalMessage)
		{
			var width = Console.WindowWidth - 1;
			var progress = (float)Current / (float)Max;

			var numberOfCharsProgress = Math.Max((int)(progress * width), 3);
			if (Current == Max)
			{
				animIndex = -1;
			}
			var bar = "{white}" + (GetAnimChar().PadRight(numberOfCharsProgress - 2, '-') + ">").PadRight(width - 1, ' ') + "|";

			TemporaryMessage.WriteLine(bar, !first, first);
			if (!string.IsNullOrEmpty(additionalMessage))
				TemporaryMessage.WriteLine(additionalMessage);
		}

		protected int animIndex = -1;
		protected string[] animChars = new string[] { "|", "/", "-", "\\", "|", "/", "-", "\\" };
		protected string GetAnimChar()
		{
			if (++animIndex >= animChars.Length)
				animIndex = 0;
			return animChars[animIndex];
		}
	}

	public class ProgressBarWithPercentageIndication : ProgressBar
	{
		protected override void Render(bool first, string additionalMessage)
		{
			var width = Console.WindowWidth - 1;
			var progress = (float)Current / (float)Max;

			var numberOfCharsProgress = Math.Max((int)(progress * width), 8);
			if (Current == Max)
			{
				animIndex = -1;
			}
			var bar = "{white}" + (GetAnimChar().PadRight(numberOfCharsProgress - 7, '-') + ">").PadRight(width - 6, ' ')
				//+ progress.ToString("p0");
				+ (int)(progress * 100) + "%";

			TemporaryMessage.WriteLine(bar, !first, first);
			if (!string.IsNullOrEmpty(additionalMessage))
				TemporaryMessage.WriteLine(additionalMessage);
		}
	}

}
