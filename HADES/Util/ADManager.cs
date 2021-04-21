using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HADES.Util.Exceptions;
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

        //Client Side
        //private const string server = "bkomstudios.com";
        //private const int PortNumber = 389;
        //private const string accessPoint = "OU=BkomUsers,DC=bkomstudios,DC=com";
        //private const string accountDn = "CN=hades,OU=ServiceAccounts,OU=BkomUsers,DC=bkomstudios,DC=com";

        //https://www.novell.com/documentation/developer/ldapcsharp/?page=/documentation/developer/ldapcsharp/cnet/data/bovumfi.html
        public ADManager()
        {
            //Console.WriteLine(authenticate("hades", "Toto123!"));
            //Console.WriteLine(createConnection());
            //Console.WriteLine(getAllUsers());

            List<string[]> root = getRoot();
            Console.WriteLine(root.Count);
            for (int i = 0; i < root.Count; i++)
            {
                Console.WriteLine(root[i][0] + "      " + root[i][1] + "      " + root[i][2]);
            }
        }

        // Champ dans le formulaire
        // PORT
        // SERVER
        // Filtre de connection
        // baseDN (Ou sont les users) 
        // DN du compte (Quelle user connect CRUD AD)
        // Mot de passe du compte DN
        // Champ de synchronisation pour l'authentification EX: SamAccountName

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
            catch (LdapException ex)
            {
                Console.WriteLine("LOG: " + ex.Message);
                throw new ADException();
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
                    catch (LdapException e)
                    {

                        Console.WriteLine("Error: " + e.LdapErrorMessage);
                        //Exception is thrown, go for next entry
                        continue;
                    }

                    if (nextEntry.GetAttribute("sAMAccountName").StringValue == username)
                    {
                        Console.WriteLine(nextEntry.GetAttribute("sAMAccountName").StringValue);
                        Console.WriteLine("\n" + nextEntry.Dn);
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
            catch (LdapException ex)
            {
                Console.WriteLine(ex.LdapErrorMessage);
                return false;
            }
        }

        // Get all User from the baseDN in the Active directory
        public List<string> getAllUsers()
        {
            List<string> users = new List<string>();

            //Creating an LdapConnection instance
            LdapConnection connection = createConnection();
            LdapSearchResults lsc = (LdapSearchResults)connection.Search(baseDN, LdapConnection.ScopeSub, connectionFilter, null, false);

            while (lsc.HasMore())
            {
                LdapEntry nextEntry = null;
                try
                {
                    nextEntry = lsc.Next();
                }
                catch (LdapException e)
                {

                    Console.WriteLine("Error: " + e.LdapErrorMessage);
                    //Exception is thrown, go for next entry
                    continue;
                }

                users.Add(nextEntry.Dn);
                Console.WriteLine("\n" + nextEntry.Dn);

            }

            return users;
        }

        // Get all the OU and the Groups from the root 
        public List<string[]> getRoot()
        {
            List<string[]> root = new List<string[]>();

            //Creating an LdapConnection instance
            LdapConnection connection = createConnection();
            LdapSearchResults lsc = (LdapSearchResults)connection.Search(rootOU, LdapConnection.ScopeSub, "(&(objectClass=*))", null, false);

            while (lsc.HasMore())
            {
                string[] data = new string[3];

                LdapEntry nextEntry = null;
                try
                {
                    nextEntry = lsc.Next();
                }
                catch (LdapException e)
                {
                    Console.WriteLine("Error: " + e.LdapErrorMessage);
                    //Exception is thrown, go for next entry
                    continue;
                }

                // TYPE
                LdapAttribute att = nextEntry.GetAttribute("objectClass");
                if (att.ToString().Contains("group"))
                {
                    data[0] = "group";

                }
                else if (att.ToString().Contains("organizationalUnit"))
                {
                    data[0] = "ou";
                }

                // PATH OF THE OBJECT
                string[] path = nextEntry.Dn.Split(',');
                for (int i = path.Length - 1; i >= 0; i--)
                {
                    if (path[i].Contains("DC=") || path[i].Contains("OU=" + nextEntry.GetAttribute("name").StringValue) || path[i].Contains("CN=" + nextEntry.GetAttribute("name").StringValue))
                    {
                        path[i] = null;
                    }

                    if (path[i] != null)
                    {
                        data[1] += "/" + path[i].Split("=")[1];
                    }
                }

                //NAME
                data[2] = nextEntry.GetAttribute("name").StringValue;

               
                root.Add(data);
            }

            connection.Disconnect();
            return root;
        }


        public string createOU(string name)
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
                return "Folder created !";
            }
            catch (Exception e)
            {
                return " Cannot create the folder: " + e.Message;
            }
        }

        public void modifyOU(string name, string newName)
        {
            LdapConnection connection = createConnection();

            List<LdapModification> modList = new List<LdapModification>();

            // Add a new value to the description attribute
            LdapAttribute attribute = new LdapAttribute("ou", newName);
            modList.Add(new LdapModification(LdapModification.Add, attribute));


            LdapModification[] mods = new LdapModification[modList.Count];
            Type mtype = Type.GetType("Novell.Directory.LdapModification");
            mods = (LdapModification[])modList.ToArray();

            string dn = "OU=" + name + "," + rootOU;
            //Modify the entry in the directory
            connection.Modify(dn, mods);
        }

        public void deleteOU()
        {

        }
        public string createGroup(string name, string ouName)
        {
            LdapConnection connection = createConnection();
            try
            {
                //Creates the List attributes of the entry and add them to attribute
                LdapAttributeSet attributeSet = new LdapAttributeSet();
                attributeSet.Add(new LdapAttribute("objectclass", "group"));
                attributeSet.Add(new LdapAttribute("CN", name));
                // DN of the entry to be added
                string dn = "CN=" + name + "," + "OU=" + ouName + "," + rootOU;
                LdapEntry newEntry = new LdapEntry(dn, attributeSet);
                //Add the entry to the directory
                connection.Add(newEntry);
                return "Group created !";
            }
            catch (Exception e)
            {

                return "Cannot create the group:" + e.Message;
            }
        }

        public void modifyGroup()
        {

        }

        public void deleteGroup()
        {

        }




    }
}
