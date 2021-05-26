using System;

namespace HADES.Util.Exceptions
{
	// Thrown if the user has entered a wrong password.
	// When catching this exception you should display an error and update the account lock system
	public class LoginException : Exception { }
}
