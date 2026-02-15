using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace CheatEngineExternalGui;

internal sealed class MemoryPatcher
{
    private const uint ProcessVmWrite = 0x0020;
    private const uint ProcessVmOperation = 0x0008;

    public void ApplyPatch(string processName, ToggleDefinition toggle, bool enabled)
    {
        var processes = Process.GetProcessesByName(processName);
        if (processes.Length == 0)
        {
            throw new InvalidOperationException($"Process '{processName}' is not running.");
        }

        using var process = processes[0];
        var address = ParseAddress(toggle.Address);
        var bytes = ParseHexBytes(enabled ? toggle.OnBytes : toggle.OffBytes);

        var handle = OpenProcess(ProcessVmWrite | ProcessVmOperation, false, process.Id);
        if (handle == IntPtr.Zero)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        try
        {
            if (!WriteProcessMemory(handle, address, bytes, bytes.Length, out var written))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            if ((nint)written != bytes.Length)
            {
                throw new InvalidOperationException($"Partial write: expected {bytes.Length}, wrote {(nint)written}.");
            }
        }
        finally
        {
            CloseHandle(handle);
        }
    }

    private static IntPtr ParseAddress(string raw)
    {
        var cleaned = raw.Trim().Replace("0x", string.Empty, StringComparison.OrdinalIgnoreCase);
        var value = long.Parse(cleaned, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        return new IntPtr(value);
    }

    private static byte[] ParseHexBytes(string raw)
    {
        var parts = raw.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return parts.Select(part => byte.Parse(part, NumberStyles.HexNumber, CultureInfo.InvariantCulture)).ToArray();
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr OpenProcess(uint desiredAccess, bool inheritHandle, int processId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool WriteProcessMemory(
        IntPtr process,
        IntPtr baseAddress,
        byte[] buffer,
        int size,
        out UIntPtr bytesWritten);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr handle);
}
