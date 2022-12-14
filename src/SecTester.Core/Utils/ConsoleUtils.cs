using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace SecTester.Core.Utils;

// This is from https://github.com/silkfire/Pastel/blob/master/src/ConsoleExtensions.cs
[ExcludeFromCodeCoverage]
internal static class ConsoleUtils
{
  private const string Kernel32 = "kernel32";

  private const int StdOutHandle = -11;
  private const int StdErrHandle = -12;

  private const uint EnableProcessedOutput = 0x0001;
  private const uint EnableVirtualTerminalProcessing = 0x0004;
  private const uint AnsiColorRequiredMode = EnableProcessedOutput | EnableVirtualTerminalProcessing;

  [DllImport(Kernel32)]
  private static extern bool GetConsoleMode(SafeFileHandle hConsoleHandle, out uint lpMode);

  [DllImport(Kernel32)]
  private static extern bool SetConsoleMode(SafeFileHandle hConsoleHandle, uint dwMode);

  [DllImport(Kernel32, SetLastError = true)]
  private static extern SafeFileHandle GetStdHandle(int nStdHandle);

  public static bool IsColored { get; }

  static ConsoleUtils()
  {
    IsColored = Environment.GetEnvironmentVariable("NO_COLOR") == null;
    IsColored = IsColored && EnableAnsiColors();
  }

  private static bool EnableAnsiColors()
  {
    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
      return true;
    }

    return EnableWindowsAnsiColors(StdOutHandle) && EnableWindowsAnsiColors(StdErrHandle);
  }

  private static bool EnableWindowsAnsiColors(int consoleHandle)
  {
    var handle = GetStdHandle(consoleHandle);

    if (handle.IsInvalid || !GetConsoleMode(handle, out var outConsoleMode))
    {
      return false;
    }

    return AnsiColorRequiredMode == (outConsoleMode & AnsiColorRequiredMode) ||
           SetConsoleMode(handle, outConsoleMode | AnsiColorRequiredMode);
  }
}

