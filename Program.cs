using System;
using System.IO;
using BusinessLogicLayer;

namespace ImpersonatorExample
{
	/// <summary>
	/// This as a sample to using impersonate
   /// </summary>
	class Program
	{
		/// <summary>
		/// The main entry point.
		/// </summary>
		[STAThread]
		static void Main( string[] args )
		{
			// Impersonate, automatically release the impersonation.
			using ( new Impersonator( "Username", "Domain", "Password" ) )
			{
				// Example of code using impersonate
				string[] files = Directory.GetFiles( "c:\\windows" );
			}
		}
	}
}