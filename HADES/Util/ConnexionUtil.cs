using HADES.Data;
using HADES.Models;
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
        ApplicationDbContext db;

        ADManager aDManager;
        public ConnexionUtil()
        {
            db = new ApplicationDbContext();
        }

        // Attempts to login
        // DefaultUser, ActiveDirectory => Check if allowed to Hades
        // Returns true if the user connecting is a default user, false if the user connecting is an ADUser
        // Throws ForbiddenException or LoginException
        public async Task<bool> Login(string user, string password)
        {
            // Check Default User in BD

            if (true)
            {
                return false;
            }
            else if (this.aDManager.authenticate(user,password))
            {
                // Check Active Directory

                if (true)
                {
                    //Check Allowed in HADES
                    return true;
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
        }
    }

}
