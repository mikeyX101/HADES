using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HADES.Util.Exceptions;
using HADES.Util.ModelAD;
using Novell.Directory.Ldap;

namespace HADES.Util
{
    public class ADManager
    {
        private const string server = "172.20.48.10";
        private const int PortNumber = 389;
        private const string baseDN = "CN=Users,DC=R991-AD,DC=lan";
        private const string accountDn = "CN=hades,CN=Users,DC=R991-AD,DC=lan";
        private const string passwordDn = "Toto123!";
        private string connectionFilter = "(&(objectClass=user)(objectCategory=person))";
        private string rootOU = "OU=hades_root,DC=R991-AD,DC=lan";
        private string syncField = "samaccountName";

        //Client Side
        //private const string server = "bkomstudios.com";
        //private const int PortNumber = 389;
        //private const string accessPoint = "OU=BkomUsers,DC=bkomstudios,DC=com";
        //private const string accountDn = "CN=hades,OU=ServiceAccounts,OU=BkomUsers,DC=bkomstudios,DC=com";

        //https://www.novell.com/documentation/developer/ldapcsharp/?page=/documentation/developer/ldapcsharp/cnet/data/bovumfi.html
        public ADManager()
        {

          //  Console.WriteLine(authenticate("hades", "Toto123!"));


             /* List<RootDataInformation> root = getRoot();
              Console.WriteLine(root.Count);
              for (int i = 0; i < root.Count; i++)
              {
                  Console.WriteLine(root[i]);
              }*/
 
            // Console.WriteLine(getGroupInformation("CN=Group1,OU=Dossier1,OU=hades_root,DC=R991-AD,DC=lan"));

            // Console.WriteLine(getUserAD("hades"));
            //Console.WriteLine(deleteOU("OU=Dossier4,OU=hades_root,DC=R991-AD,DC=lan"));
          /*  Console.WriteLine("---------------------------------------------------");
            List<RootDataInformation> root = getRoot();
            Console.WriteLine(root.Count);
            for (int i = 0; i < root.Count; i++)
            {
                Console.WriteLine(root[i]);
            }*/


            /* List<UserAD> users = getAllUsers();
             Console.WriteLine(users.Count);
             for (int i = 0; i < users.Count; i++)
             {
                 Console.WriteLine(users[i]);
             }*/
        }


        // Create a connection with de DN account
        private LdapConnection createConnection(string userDN = null, string password = null)
        {
            //Creating an LdapConnection instance
            LdapConnection connection = new LdapConnection();
            try
            {
                //Connect function will create a socket connection to the server
                connection.Connect(server, PortNumber);
                Console.WriteLine("isConnected : " + connection.Connected);

                //Bind function will Bind the user object  Credentials to the Server
                if (userDN != null && password != null)
                {
                    Console.WriteLine("userCredential");
                    connection.Bind(userDN, password);
                }
                else
                {
                    Console.WriteLine("serverCredential");
                    connection.Bind(accountDn, passwordDn);
                }

                Console.WriteLine("isAuthenticated : " + connection.Bound);

                return connection;

            }
            catch (Exception ex)
            {
                Console.WriteLine("LOG: " + ex.Message);
                throw new ADException();
            }
        }

        //IF the attribute is not set in the AD it throw KeyNotFoundException
        private string getAttributeValue(LdapEntry entry, string attribute)
        {
            try
            {
                return entry.GetAttribute(attribute).StringValue;
            }
            catch (KeyNotFoundException e)
            {
                // The key is not set 
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("LOG: " + e.Message);
                return null;
            }
        }


        //Authenticate the user in the Active Directory 
        public bool authenticate(string username, string password)
        {
            //Creating an LdapConnection instance
            LdapConnection connection = createConnection();

            try
            {
                LdapSearchResults lsc = (LdapSearchResults)connection.Search(baseDN, LdapConnection.ScopeSub, connectionFilter, null, false);

                string userDN = null;
                bool userWasFound = false;
                bool userIsAuthenticate = false;

                while (lsc.HasMore() && userWasFound == false)
                {
                    LdapEntry nextEntry = null;
                    try
                    {
                        nextEntry = lsc.Next();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: " + e.Message);
                        //Exception is thrown, go for next entry
                        continue;
                    }


                    if (getAttributeValue(nextEntry, syncField) == username)
                    {
                        userDN = nextEntry.Dn;
                        userWasFound = true;
                    }
                }

                connection.Disconnect();

                if (userWasFound)
                {

                    connection = createConnection(username, password);
                    if (connection != null)
                    {
                        userIsAuthenticate = connection.Bound;
                    }
                }

                return userIsAuthenticate;
            }
            catch (Exception ex)
            {
                Console.WriteLine("LOG " + ex.Message);
                return false;
            }
        }

        // Get all User from the baseDN in the Active directory
        public List<UserAD> getAllUsers()
        {
            List<UserAD> users = new List<UserAD>();

            //Creating an LdapConnection instance
            LdapConnection connection = createConnection();
            LdapSearchResults lsc = (LdapSearchResults)connection.Search(baseDN, LdapConnection.ScopeSub, connectionFilter, null, false);

            while (lsc.HasMore())
            {
                LdapEntry nextEntry = null;
                try
                {
                    nextEntry = lsc.Next();

                    UserAD u = new UserAD();
                    u.SamAccountName = getAttributeValue(nextEntry, "samaccountName");
                    u.FirstName = getAttributeValue(nextEntry, "givenName");
                    u.LastName = getAttributeValue(nextEntry, "sn");
                    u.Dn = nextEntry.Dn;
                    users.Add(u);
                }
                catch (Exception e)
                {

                    Console.WriteLine("Error: " + e.Message);
                    //Exception is thrown, go for next entry
                    continue;
                }

            }

            return users;
        }

        // Get the UserAD of the username 
        public UserAD getUserAD(string username)
        {
            UserAD u = null;
            LdapConnection connection = createConnection();
            LdapSearchResults lsc = (LdapSearchResults)connection.Search(baseDN, LdapConnection.ScopeSub, connectionFilter, null, false);
            bool userWasFound = false;

            while (lsc.HasMore() && userWasFound == false)
            {
                LdapEntry nextEntry = null;
                try
                {
                    nextEntry = lsc.Next();

                    if (getAttributeValue(nextEntry, syncField) == username)
                    {
                        userWasFound = true;
                        u = new UserAD();
                        u.SamAccountName = getAttributeValue(nextEntry, "samaccountName");
                        u.FirstName = getAttributeValue(nextEntry, "givenName");
                        u.LastName = getAttributeValue(nextEntry, "sn");
                        u.Dn = nextEntry.Dn;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                    //Exception is thrown, go for next entry
                    continue;
                }

            }

            if (u == null)
            {
                throw new ADException();
            }

            return u;
        }


        // Get all the OU and the Groups from the root 
        public List<RootDataInformation> getRoot()
        {
            List<RootDataInformation> root = new List<RootDataInformation>();

            //Creating an LdapConnection instance
            LdapConnection connection = createConnection();
            LdapSearchResults lsc = (LdapSearchResults)connection.Search(rootOU, LdapConnection.ScopeSub, "(|(objectClass=group)(objectClass=organizationalUnit))", null, false);

            while (lsc.HasMore())
            {
                RootDataInformation data = new RootDataInformation();

                LdapEntry nextEntry = null;
                try
                {
                    nextEntry = lsc.Next();

                    // TYPE
                    string att = "";
                    try
                    {
                        att = nextEntry.GetAttribute("objectClass").ToString();
                        if (att.Contains("group"))
                        {
                            data.Type = "group";
                            data.SamAccountName = getAttributeValue(nextEntry, "sAMAccountName");

                        }
                        else if (att.Contains("organizationalUnit"))
                        {
                            data.Type = "ou";
                            data.SamAccountName = getAttributeValue(nextEntry, "Name");
                        }
                    }
                    catch (KeyNotFoundException e)
                    {
                        Console.WriteLine("LOG: KeyNotFoundExcption");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("LOG: " + e.Message);
                    }


                    // PATH OF THE OBJECT
                    string[] path = nextEntry.Dn.Split(',');
                    for (int i = path.Length - 1; i >= 0; i--)
                    {
                        if (path[i].Contains("DC=") || path[i].Contains("OU=" + data.SamAccountName) || path[i].Contains("CN=" + data.SamAccountName))
                        {
                            path[i] = null;
                        }

                        if (path[i] != null)
                        {
                            data.Path += "/" + path[i].Split("=")[1];
                        }
                    }


                    //DN 
                    data.Dn = nextEntry.Dn;

                    root.Add(data);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                    continue;
                }
            }

            connection.Disconnect();
            return root;
        }

        //Get the information for the group in param in the AD
        public GroupAD getGroupInformation(string groupDN)
        {
            //Creating an LdapConnection instance
            LdapConnection connection = createConnection();

            LdapSearchResults lsc = (LdapSearchResults)connection.Search(rootOU, LdapConnection.ScopeSub, "(&(objectClass=group)(distinguishedName=" + groupDN + "))", null, false);
            GroupAD group = new GroupAD();
            while (lsc.HasMore())
            {

                LdapEntry nextEntry = null;
                try
                {
                    nextEntry = lsc.Next();
                    group.SamAccountName = getAttributeValue(nextEntry, "sAMAccountName");
                    group.Email = getAttributeValue(nextEntry, "mail");
                    group.Notes = getAttributeValue(nextEntry, "info");
                    group.Description = getAttributeValue(nextEntry, "description");
                    group.Members = GetMembersOfGroup(groupDN);
                }
                catch (LdapException e)
                {
                    connection.Disconnect();
                    Console.WriteLine("Error: " + e.LdapErrorMessage);
                    //Exception is thrown, go for next entry
                    continue;
                }
                catch (Exception e)
                {
                    connection.Disconnect();
                    Console.WriteLine("LOG: " + e.Message);
                }
            }

            connection.Disconnect();
            return group;
        }

        //Get all the members of the group in param
        public List<UserAD> GetMembersOfGroup(string groupDN)
        {
            LdapConnection connection = createConnection();

            // Get members
            LdapSearchResults lsc = (LdapSearchResults)connection.Search(baseDN, LdapConnection.ScopeSub, "(&(objectClass=user)(memberOf=" + groupDN + "))", null, false);
            List<UserAD> users = new List<UserAD>();

            while (lsc.HasMore())
            {
                LdapEntry nextEntry = null;
                try
                {
                    nextEntry = lsc.Next();
                    UserAD u = new UserAD();
                    u.SamAccountName = getAttributeValue(nextEntry, "sAMAccountName");
                    u.FirstName = getAttributeValue(nextEntry, "givenName");
                    u.LastName = getAttributeValue(nextEntry, "sn");
                    u.Dn = nextEntry.Dn;
                    users.Add(u);

                }
                catch (LdapException e)
                {
                    connection.Disconnect();
                    Console.WriteLine("LOG: " + e.Message);
                    //Exception is thrown, go for next entry
                    continue;
                }
                catch (Exception e)
                {
                    Console.WriteLine("LOG: " + e.Message);
                    connection.Disconnect();
                }
            }
            connection.Disconnect();
            return users;
        }

        public bool createOU(string name)
        {

            LdapConnection connection = createConnection();
            try
            {
                //Creates the List attributes of the entry and add them to attribute
                LdapAttributeSet attributeSet = new LdapAttributeSet();
                attributeSet.Add(new LdapAttribute("objectclass", "organizationalunit"));
                attributeSet.Add(new LdapAttribute("OU", name));
                // DN of the entry to be added
                string dn = "OU=" + name + "," + rootOU;
                LdapEntry newEntry = new LdapEntry(dn, attributeSet);
                //Add the entry to the directory
                connection.Add(newEntry);
                connection.Disconnect();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot create the folder: " + e.Message);
                connection.Disconnect();
                return false;
            }
        }

        public bool renameOU(string dn, string newName)
        {
            LdapConnection connection = createConnection();
            try
            {
                string newRdn = "OU=" + newName;

                connection.Rename(dn, newRdn, true);
                connection.Disconnect();
                return true;
            }
            catch (Exception e)
            {
                connection.Disconnect();
                Console.WriteLine("LOG : Cannot rename the folder: " + e.Message);
                return false;
            }
        }

        public bool deleteOU(string dn)
        {
            LdapConnection connection = createConnection();
            try
            {
                connection.Delete(dn);
                connection.Disconnect();
                return true;
            }
            catch (Exception e)
            {
                connection.Disconnect();
                Console.WriteLine("Cannot delete the folder: " + e.Message);
                return false;
            }
        }

        public bool createGroup(string name, string ouName)
        {
            /*
              group.SamAccountName = getAttributeValue(nextEntry, "sAMAccountName");
              group.Email = getAttributeValue(nextEntry, "mail");
              group.Notes = getAttributeValue(nextEntry, "info");
              group.Description = getAttributeValue(nextEntry, "description");
             */

            LdapConnection connection = createConnection();
            try
            {
                //Creates the List attributes of the entry and add them to attribute
                LdapAttributeSet attributeSet = new LdapAttributeSet();
                attributeSet.Add(new LdapAttribute("objectclass", "group"));
                attributeSet.Add(new LdapAttribute("CN", name));
                attributeSet.Add(new LdapAttribute("samaccountname", name));
                // DN of the entry to be added
                string dn = "CN=" + name + "," + "OU=" + ouName + "," + rootOU;
                LdapEntry newEntry = new LdapEntry(dn, attributeSet);
                //Add the entry to the directory
                connection.Add(newEntry);
                connection.Disconnect();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot create the group:" + e.Message);
                connection.Disconnect();
                return false;
            }
        }

        public void modifyGroup(string groupDn)
        {

        }

        public bool deleteGroup(string dn)
        {
            LdapConnection connection = createConnection();
            try
            {
                connection.Delete(dn);
                connection.Disconnect();
                return true;
            }
            catch (Exception e)
            {
                connection.Disconnect();
                Console.WriteLine("Cannot delete the group: " + e.Message);
                return false;
            }
        }
    }
}
