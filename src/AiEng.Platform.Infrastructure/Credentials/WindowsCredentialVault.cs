using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using AiEng.Platform.Application.Infrastructure;

namespace AiEng.Platform.Infrastructure.Credentials;

public sealed class WindowsCredentialVault : ICredentialVault
{
    public Task<string?> GetAsync(string name, CancellationToken cancellationToken = default)
    {
        EnsureSupportedPlatform();
        cancellationToken.ThrowIfCancellationRequested();
        ValidateName(name);

        if (!NativeMethods.CredRead(name, NativeMethods.CRED_TYPE_GENERIC, 0, out var handle))
        {
            var error = Marshal.GetLastWin32Error();
            if (error == NativeMethods.ERROR_NOT_FOUND)
            {
                return Task.FromResult<string?>(null);
            }
            throw new Win32Exception(error, $"CredRead failed for '{name}'.");
        }

        try
        {
            var credential = Marshal.PtrToStructure<NativeMethods.CREDENTIAL>(handle);
            if (credential.CredentialBlobSize == 0 || credential.CredentialBlob == IntPtr.Zero)
            {
                return Task.FromResult<string?>(string.Empty);
            }
            var bytes = new byte[credential.CredentialBlobSize];
            Marshal.Copy(credential.CredentialBlob, bytes, 0, bytes.Length);
            return Task.FromResult<string?>(Encoding.Unicode.GetString(bytes));
        }
        finally
        {
            NativeMethods.CredFree(handle);
        }
    }

    public Task SetAsync(string name, string secret, CancellationToken cancellationToken = default)
    {
        EnsureSupportedPlatform();
        cancellationToken.ThrowIfCancellationRequested();
        ValidateName(name);
        if (secret is null)
        {
            throw new ArgumentNullException(nameof(secret));
        }

        var bytes = Encoding.Unicode.GetBytes(secret);
        var blob = Marshal.AllocCoTaskMem(bytes.Length);
        try
        {
            Marshal.Copy(bytes, 0, blob, bytes.Length);
            var credential = new NativeMethods.CREDENTIAL
            {
                Type = NativeMethods.CRED_TYPE_GENERIC,
                TargetName = name,
                CredentialBlobSize = (uint)bytes.Length,
                CredentialBlob = blob,
                Persist = NativeMethods.CRED_PERSIST_LOCAL_MACHINE,
            };
            if (!NativeMethods.CredWrite(ref credential, 0))
            {
                var error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error, $"CredWrite failed for '{name}'.");
            }
        }
        finally
        {
            Marshal.FreeCoTaskMem(blob);
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string name, CancellationToken cancellationToken = default)
    {
        EnsureSupportedPlatform();
        cancellationToken.ThrowIfCancellationRequested();
        ValidateName(name);

        if (!NativeMethods.CredDelete(name, NativeMethods.CRED_TYPE_GENERIC, 0))
        {
            var error = Marshal.GetLastWin32Error();
            if (error == NativeMethods.ERROR_NOT_FOUND)
            {
                return Task.CompletedTask;
            }
            throw new Win32Exception(error, $"CredDelete failed for '{name}'.");
        }
        return Task.CompletedTask;
    }

    private static void EnsureSupportedPlatform()
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException(
                "WindowsCredentialVault is only supported on Windows.");
        }
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Credential name is required.", nameof(name));
        }
    }

    private static class NativeMethods
    {
        public const uint CRED_TYPE_GENERIC = 1;
        public const uint CRED_PERSIST_LOCAL_MACHINE = 2;
        public const int ERROR_NOT_FOUND = 1168;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct CREDENTIAL
        {
            public uint Flags;
            public uint Type;
            [MarshalAs(UnmanagedType.LPWStr)] public string TargetName;
            [MarshalAs(UnmanagedType.LPWStr)] public string? Comment;
            public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
            public uint CredentialBlobSize;
            public IntPtr CredentialBlob;
            public uint Persist;
            public uint AttributeCount;
            public IntPtr Attributes;
            [MarshalAs(UnmanagedType.LPWStr)] public string? TargetAlias;
            [MarshalAs(UnmanagedType.LPWStr)] public string? UserName;
        }

        [DllImport("advapi32.dll", EntryPoint = "CredReadW", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CredRead(string target, uint type, uint reservedFlag, out IntPtr credentialPtr);

        [DllImport("advapi32.dll", EntryPoint = "CredWriteW", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CredWrite([In] ref CREDENTIAL credential, [In] uint flags);

        [DllImport("advapi32.dll", EntryPoint = "CredDeleteW", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CredDelete(string target, uint type, uint flags);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern void CredFree([In] IntPtr cred);
    }
}
