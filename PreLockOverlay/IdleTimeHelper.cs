using System;
using System.Runtime.InteropServices;

public static class IdleTimeHelper
{
    [DllImport("user32.dll")]
    private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

    [StructLayout(LayoutKind.Sequential)]
    private struct LASTINPUTINFO
    {
        public uint cbSize;
        public uint dwTime;
    }

    public static TimeSpan GetIdleTime()
    {
        var lastInputInfo = new LASTINPUTINFO();
        lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
        GetLastInputInfo(ref lastInputInfo);
        return TimeSpan.FromMilliseconds(Environment.TickCount - lastInputInfo.dwTime);
    }
}
