using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Metalogix.Permissions
{
    public class ImpersonationContext : IDisposable
    {
        private WindowsImpersonationContext _impersonationContext;

        private bool _disposed;

        internal ImpersonationContext()
        {
            this._impersonationContext = null;
        }

        internal ImpersonationContext(string userName, string domainName, string password)
        {
            this._impersonationContext = ImpersonationContext.ImpersonateValidUser(userName, domainName, password);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed && disposing)
            {
                this.UndoImpersonation();
                this._impersonationContext = null;
            }

            this._disposed = true;
        }

        ~ImpersonationContext()
        {
            this.Dispose(false);
        }

        internal static WindowsImpersonationContext ImpersonateValidUser(string userName, string domain,
            string password)
        {
            IntPtr zero = IntPtr.Zero;
            IntPtr intPtr = IntPtr.Zero;
            WindowsImpersonationContext windowsImpersonationContext = null;
            try
            {
                if (!Metalogix.Permissions.NativeMethods.RevertToSelf())
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                if (Metalogix.Permissions.NativeMethods.LogonUser(userName, domain, password, 2, 0, ref zero) == 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                if (Metalogix.Permissions.NativeMethods.DuplicateToken(zero, 2, ref intPtr) == 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                windowsImpersonationContext = (new WindowsIdentity(intPtr)).Impersonate();
            }
            finally
            {
                if (zero != IntPtr.Zero)
                {
                    Metalogix.Permissions.NativeMethods.CloseHandle(zero);
                }

                if (intPtr != IntPtr.Zero)
                {
                    Metalogix.Permissions.NativeMethods.CloseHandle(intPtr);
                }
            }

            return windowsImpersonationContext;
        }

        private void UndoImpersonation()
        {
            if (this._impersonationContext != null)
            {
                this._impersonationContext.Undo();
            }
        }
    }
}