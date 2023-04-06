using System;
using System.Diagnostics;
using System.Text;

namespace SIL.LCModel.Utils
{
	public static class Watch
	{
		private static int indent = 0;
		private static readonly long[] recordedTime = new long[20];
		private static StringBuilder sb;
		private const bool printAfter = true;
		private const bool printAfterReverse = true;
		public static bool Enabled { get; set; }

		public static T Time<T>(string name, Func<T> action)
		{
#if DEBUG
			var sw = OnStart();
#endif
			var result = action();
#if DEBUG
			OnFinished(name, sw);
#endif
			return result;
		}

		public static void Time(string name, Action action)
		{
			Time(name, () =>
			{
				action();
				return 0;
			});
		}

		private static Stopwatch OnStart()
		{
			if (indent == 0)
			{
				sb = new StringBuilder();
			}
			indent++;
			return Stopwatch.StartNew();
		}

		private static void OnFinished(string name, Stopwatch sw)
		{
			sw.Stop();
			var time = sw.ElapsedMilliseconds;
			indent--;
			var done = indent == 0;
			recordedTime[indent] += time;
			var innerRecordedTime = recordedTime[indent + 1];
			recordedTime[indent + 1] = 0;
			var missingTime = time - innerRecordedTime;
			if (Enabled)
			{
				if (innerRecordedTime > 0 && missingTime > 0)
					WriteLine($"{time + "ms",7} [{name}] (Missing: {missingTime}ms)");
				else
					WriteLine($"{time + "ms",7} [{name}]");
			}
			if (Enabled && done)
			{
				if (printAfter)
				{
					if (!printAfterReverse)
					{
						Console.WriteLine(sb.ToString());
					}
					else
					{
						var lines = sb.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None);
						for (int i = lines.Length - 1; i >= 0; i--)
						{
							Console.WriteLine(lines[i]);
						}
					}
				}
				sb = null;
			}
		}

		private static void WriteLine(string value)
		{
			if (!printAfter)
				Console.Write(new StringBuilder().Insert(0, "    ", indent).AppendLine(value));

			sb.Insert(sb.Length, "    ", indent);
			sb.AppendLine(value);
		}
	}
}
