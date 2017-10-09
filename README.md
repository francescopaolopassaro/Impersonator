# Impersonator
Thislibrary make a impersonation of a user. Allows to execute code under another credential
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
