namespace BusinessLogicLayer
{
	#region Using directives.
	// ----------------------------------------------------------------------

	using System;
	using System.Security.Principal;
	using System.Runtime.InteropServices;
	using System.ComponentModel;

	// ----------------------------------------------------------------------
	#endregion

	/////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Impersonation of a user. Allows to execute code under another
	/// user context.
    /// Is based on the information in the Microsoft knowledge base
    /// article http://support.microsoft.com/default.aspx?scid=kb;en-us;Q306158
	/// </summary>
	/// <remarks>	
    /// Example
	/// 
	///		...
	///		using ( new Impersonator( "myUsername", "myDomainname", "myPassword" ) )
	///		{
	///			...
    ///          [yourcode]
	///		}
	///		...
	/// 
	/// </remarks>
	public class Impersonator :
		IDisposable
	{
		#region Public methods.
		// ------------------------------------------------------------------

		public Impersonator(
			string userName,
			string domainName,
			string password )
		{
			ImpersonateValidUser( userName, domainName, password );
		}

		#endregion

		#region IDisposable member.
	
		public void Dispose()
		{
			UndoImpersonation();
		}

		// ------------------------------------------------------------------
		#endregion

		#region P/Invoke.
		// ------------------------------------------------------------------

		[DllImport("advapi32.dll", SetLastError=true)]
		private static extern int LogonUser(
			string lpszUserName,
			string lpszDomain,
			string lpszPassword,
			int dwLogonType,
			int dwLogonProvider,
			ref IntPtr phToken);
		
		[DllImport("advapi32.dll", CharSet=CharSet.Auto, SetLastError=true)]
		private static extern int DuplicateToken(
			IntPtr hToken,
			int impersonationLevel,
			ref IntPtr hNewToken);

		[DllImport("advapi32.dll", CharSet=CharSet.Auto, SetLastError=true)]
		private static extern bool RevertToSelf();

		[DllImport("kernel32.dll", CharSet=CharSet.Auto)]
		private static extern  bool CloseHandle(
			IntPtr handle);

		private const int LOGON32_LOGON_INTERACTIVE = 2;
		private const int LOGON32_PROVIDER_DEFAULT = 0;

		// ------------------------------------------------------------------
		#endregion

		#region Private member.
		// ------------------------------------------------------------------

        private void ImpersonateValidUser(
			string userName, 
			string domain, 
			string password )
		{
			WindowsIdentity tempWindowsIdentity = null;
			IntPtr token = IntPtr.Zero;
			IntPtr tokenDuplicate = IntPtr.Zero;

			try
			{
				if ( RevertToSelf() )
				{
					if ( LogonUser(
						userName, 
						domain, 
						password, 
						LOGON32_LOGON_INTERACTIVE,
						LOGON32_PROVIDER_DEFAULT, 
						ref token ) != 0 )
					{
						if ( DuplicateToken( token, 2, ref tokenDuplicate ) != 0 )
						{
							tempWindowsIdentity = new WindowsIdentity( tokenDuplicate );
							impersonationContext = tempWindowsIdentity.Impersonate();
						}
						else
						{
							throw new Win32Exception( Marshal.GetLastWin32Error() );
						}
					}
					else
					{
						throw new Win32Exception( Marshal.GetLastWin32Error() );
					}
				}
				else
				{
					throw new Win32Exception( Marshal.GetLastWin32Error() );
				}
			}
			finally
			{
				if ( token!= IntPtr.Zero )
				{
					CloseHandle( token );
				}
				if ( tokenDuplicate!=IntPtr.Zero )
				{
					CloseHandle( tokenDuplicate );
				}
			}
		}

		
		private void UndoImpersonation()
		{
			if ( impersonationContext!=null )
			{
				impersonationContext.Undo();
			}	
		}

		private WindowsImpersonationContext impersonationContext = null;

		// ------------------------------------------------------------------
		#endregion
	}


}