using HADES.Data;
using HADES.Models;
using HADES.Util.Exceptions;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Linq;

namespace HADES.Util
{
	// Handles Connexion to Hades and refers to the ADManager if the user is not a default user.
	// Updates the model to match the Active Directory User
	public abstract class ConnexionUtil
    {

        static readonly byte[] salt = { 80, 232, 103, 125, 189, 33, 51, 46, 132, 179, 77, 146, 140, 164, 204, 227, 60, 147, 126, 173, 123, 7, 180, 183, 38, 78, 40, 105, 74, 105, 39, 30 };

        static ADManager aDManager = new ADManager();

        // Attempts to login
        // DefaultUser, ActiveDirectory => Check if allowed to Hades
        // Returns true if the user connecting is a default user, false if the user connecting is an ADUser
        // Throws ForbiddenException or LoginException
        public static IUser Login(string user, string password)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            // Check Default User in BD
            if (ValidateAttempts(user))
            {
                if (db.DefaultUser.SingleOrDefault((a) => a.UserName.ToLower().Equals(user.ToLower()) && a.Password.Equals(HashPassword(password))) != null)
                { 
                    DefaultUser u = db.DefaultUser.SingleOrDefault((a) => a.UserName.ToLower().Equals(user.ToLower()) && a.Password.Equals(HashPassword(password)));
                    db.Update(u);
                    u.Attempts = 0;
                    db.SaveChanges();
                    return u;
                }
                else if (aDManager.authenticate(user, password))
                {
                    // Check Active Directory

                    // TODO Update Admin/SuperAdmin User in DB

                    if (db.User.Include(u => u.Role).SingleOrDefault((u) => u.GUID.ToLower().Equals(aDManager.getUserAD(user, false).ObjectGUID) && u.Role.HadesAccess) != null)
                    {
                        //Check Allowed in HADES (is in DB as User && Role.HadesAccess)
                        User u = db.User.SingleOrDefault((u) => u.GUID.ToLower().Equals(aDManager.getUserAD(user,false).ObjectGUID));
                        db.Update(u);
                        u.Attempts = 0;
                        db.SaveChanges();
                        return u;
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
            else
            {
                throw new LoginException();
            }
            
        }

        // Returns true if the specified user exists and is allowed to Login
        // False otherwise
        // Also increases the number of attempts
        private static bool ValidateAttempts(string user)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            try
            {
                DefaultUser u = db.DefaultUser.SingleOrDefault((a) => a.UserName.ToLower().Equals(user.ToLower()));
                db.Update(u);
                u.Attempts++;
                if (u.Attempts > 5)
                {
                    u.Attempts = 0;
                    u.Date = DateTime.UtcNow.AddMinutes(10);
                    Log.Information("{User} has been locked from login attempts. The user will unlock at {Date}", u.GetName(), u.Date.ToString());
                }
                db.SaveChanges();
                return u.Date < DateTime.UtcNow;
            }
            catch (Exception)
            {
                // If anything Wrong happens then try User
                try
                {
                    string guid = aDManager.getUserAD(user, false).ObjectGUID;
                    User u = db.User.SingleOrDefault((a) => a.GUID.ToLower().Equals(guid));
                    if (u == null)
                    {
                        throw new ForbiddenException();
                    }
                    db.Update(u);
                    u.Attempts++;
                    if (u.Attempts > 5)
                    {
                        u.Date = DateTime.UtcNow.AddMinutes(10);
                        Log.Information("{User} has been locked from login attempts. The user will unlock at {Date}", u.GetName(), u.Date.ToString());
                    }
                    db.SaveChanges();
                    return u.Date < DateTime.UtcNow;
                }
                catch (ADException)
                {
                    throw; // Logged in login
                }
                catch (ForbiddenException)
                {
                    return true; // Pass handling to next function
                }
                catch (Exception e)
                {
                    Log.Warning(e, "An unexepected error occured while validating login attempts");
                    // If anything Wrong happens then this User must not exist
                    return false;
                }
            }
        }

        // Returns the Hashed password for Default User (Other login is handled by Active Directory)
        public static string HashPassword(string password)
        {
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
        public static IUser CurrentUser(System.Security.Claims.ClaimsPrincipal user)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            if (user.Identity.IsAuthenticated)
            {
                int id = int.Parse(user.Claims.Where(c => c.Type == "id").FirstOrDefault().Value);

                if (bool.Parse(user.Claims.Where(c => c.Type == "isDefault").FirstOrDefault().Value))
                {
                    return db.DefaultUser.Include(a=>a.Role).Include(a=> a.UserConfig).ThenInclude(a=>a.Emails).Where((a) => a.Id == id).FirstOrDefault();
                }
                else
                {
                    return db.User.Include(a => a.Role).Include(a => a.UserConfig).ThenInclude(a => a.Emails).Include(a=>a.OwnerGroupUsers).ThenInclude(a=>a.OwnerGroup).Where((a) => a.Id == id).FirstOrDefault();
                }

            }
            else
            {
                return null;
            }
        }


    }

}
