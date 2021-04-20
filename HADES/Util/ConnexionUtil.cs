using HADES.Util.Exceptions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HADES.Util
{
    // Handles Connexion to Hades and refers to the ADManager if the user is not a default user.
    // Updates the model to match the Active Directory User
    public class ConnexionUtil
    {
        // SignInManager for Identity
        private SignInManager<IdentityUser> signInMngr;

        public ConnexionUtil(SignInManager<IdentityUser> signInMngr)
        {
            this.signInMngr = signInMngr;
        }

        // Attempts to login
        // Returns true if the user connecting is a default user, false if the user connecting is an ADUser
        // Throws ForbiddenException or LoginException
        public bool Login(string user, string password)
        {
            throw new ForbiddenException();
            throw new LoginException();
            return true;
        }
    }

}
