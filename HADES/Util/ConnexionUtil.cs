using HADES.Data;
using HADES.Models;
using HADES.Util.Exceptions;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HADES.Util
{
    // Handles Connexion to Hades and refers to the ADManager if the user is not a default user.
    // Updates the model to match the Active Directory User
    public abstract class ConnexionUtil
    {

        static readonly byte[] salt = { 80, 232, 103, 125, 189, 33, 51, 46, 132, 179, 77, 146, 140, 164, 204, 227, 60, 147, 126, 173, 123, 7, 180, 183, 38, 78, 40, 105, 74, 105, 39, 30 };
        static ApplicationDbContext db = new ApplicationDbContext();

        static ADManager aDManager = new ADManager();

        // Attempts to login
        // DefaultUser, ActiveDirectory => Check if allowed to Hades
        // Returns true if the user connecting is a default user, false if the user connecting is an ADUser
        // Throws ForbiddenException or LoginException
        public static IUser Login(string user, string password)
        {
            // Check Default User in BD

            if (db.DefaultUser.SingleOrDefault((a) => a.UserName.ToLower().Equals(user.ToLower()) && a.Password.Equals(HashPassword(password))) != null)
            {
                return db.DefaultUser.SingleOrDefault((a) => a.UserName.ToLower().Equals(user.ToLower()) && a.Password.Equals(HashPassword(password)));
            }
            else if (aDManager.authenticate(user, password))
            {
                // Check Active Directory

                if (true)
                {
                    //Check Allowed in HADES
                    return null;
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

        // Returns the Hashed password for Default User (Other login is handled by Active Directory)
        private static string HashPassword(string password)
        {
            // Generate Salt
            char[] passArray = password.ToCharArray();

            // Calculate Hash
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));
            return hashed;
        }

        // Returns the Current User or null if the Current user is not in the database/no cookie found
        public static IUser CurrentUser(Controller controller)
        {
            if (controller.User.Identity.IsAuthenticated)
            {
                int id = int.Parse(controller.User.Claims.Where(c => c.Type == "id").FirstOrDefault().Value);

                if (bool.Parse(controller.User.Claims.Where(c => c.Type == "isDefault").FirstOrDefault().Value))
                {
                    return db.DefaultUser.Where((a) => a.Id == id).FirstOrDefault();
                }
                else
                {
                    return db.User.Where((a) => a.Id == id).FirstOrDefault();
                }

            }
            else
            {
                return null;
            }
        }


    }

}
