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
        // DefaultUser, ActiveDirectory => Check if allowed to Hades
        // Returns true if the user connecting is a default user, false if the user connecting is an ADUser
        // Throws ForbiddenException or LoginException
        public bool Login(string user, string password)
        {
            // Check Default User in BD

            if (true)
            {

            }
            else if (true)
            {
                // Check Active Directory

                if (true)
                {
                    //Check Allowed in HADES
                }
                else
                {
                    throw new ForbiddenException();
                }
            }
            else
            {
                throw new LoginException();
            }

            return true;
        }
    }

}
