using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace LittleConsoleHelper.Display.Images
{
	internal static class ImageRenderer
	{
		static Rectangle _latestImageRect;
		public static void RenderImageBottom(string imagePath)
		{
			
			var fontSize = GetConsoleFontSize();
			var consoleWidth = Console.WindowWidth * fontSize.Width;
			var consoleHeight = Console.WindowHeight * fontSize.Height;
			var imageWidth = consoleWidth;

			if (_latestImageRect != null)
			{
				using (Graphics g = Graphics.FromHwnd(GetConsoleWindow()))
				{
					g.Clip = new Region(_latestImageRect);
					g.Clear(Color.Black);
				}
			}

			using (Graphics g = Graphics.FromHwnd(GetConsoleWindow()))
			{
				using (Image image = Image.FromFile(imagePath))
				{
					var scale = (float)imageWidth / (float)image.Width;
					var imageHeight = (int)((float)image.Height * scale);
					var x = 0;
					var y = consoleHeight - imageHeight;
					Rectangle imagePlacement = new Rectangle(x, y, imageWidth, imageHeight);
					g.DrawImage(image, imagePlacement);
					_latestImageRect = imagePlacement;
				}
			}
		}
		public static void Foo()
		{

		}

		public static void RenderImage(string imagePath, int x, int y, int width = 0, int height = 0)
		{
			Point location = new Point(x, y);
			Size imageSize;

			using (Graphics g = Graphics.FromHwnd(GetConsoleWindow()))
			{
				using (Image image = Image.FromFile(imagePath))
				{
					if (width == 0 && height == 0)
					{
						width = image.Width;
						height = image.Height;
					}
					else if (width == 0)
					{
						var scale = (float)height / (float)image.Height;
						width = (int)((float)image.Width * scale);
					}
					else if (height == 0)
					{
						var scale = (float)width / (float)image.Width;
						height = (int)((float)image.Height * scale);
					}
					imageSize = new Size(width, height);

					Rectangle imageRect = new Rectangle(location.X, location.Y, imageSize.Width, imageSize.Height);
					g.DrawImage(image, imageRect);
				}
			}
		}
		public static Size GetConsoleFontSize()
		{
			// getting the console out buffer handle
			IntPtr outHandle = CreateFile("CONOUT$", GENERIC_READ | GENERIC_WRITE,
				FILE_SHARE_READ | FILE_SHARE_WRITE,
				IntPtr.Zero,
				OPEN_EXISTING,
				0,
				IntPtr.Zero);
			int errorCode = Marshal.GetLastWin32Error();
			if (outHandle.ToInt32() == INVALID_HANDLE_VALUE)
			{
				throw new IOException("Unable to open CONOUT$", errorCode);
			}

			ConsoleFontInfo cfi = new ConsoleFontInfo();
			if (!GetCurrentConsoleFont(outHandle, false, cfi))
			{
				throw new InvalidOperationException("Unable to get font information.");
			}

			return new Size(cfi.dwFontSize.X, cfi.dwFontSize.Y);
		}
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr GetConsoleWindow();

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr CreateFile(
			string lpFileName,
			int dwDesiredAccess,
			int dwShareMode,
			IntPtr lpSecurityAttributes,
			int dwCreationDisposition,
			int dwFlagsAndAttributes,
			IntPtr hTemplateFile);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool GetCurrentConsoleFont(
			IntPtr hConsoleOutput,
			bool bMaximumWindow,
			[Out][MarshalAs(UnmanagedType.LPStruct)] ConsoleFontInfo lpConsoleCurrentFont);

		[StructLayout(LayoutKind.Sequential)]
		internal class ConsoleFontInfo
		{
			internal int nFont;
			internal Coord dwFontSize;
		}

		[StructLayout(LayoutKind.Explicit)]
		internal struct Coord
		{
			[FieldOffset(0)]
			internal short X;
			[FieldOffset(2)]
			internal short Y;
		}

		private const int GENERIC_READ = unchecked((int)0x80000000);
		private const int GENERIC_WRITE = 0x40000000;
		private const int FILE_SHARE_READ = 1;
		private const int FILE_SHARE_WRITE = 2;
		private const int INVALID_HANDLE_VALUE = -1;
		private const int OPEN_EXISTING = 3;
	}
}
