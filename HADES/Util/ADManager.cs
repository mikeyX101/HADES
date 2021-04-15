using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Novell.Directory.Ldap;

namespace HADES.Util
{
    public class ADManager
    {
        private const string server = "sv01-bdinfo01.cegeplimoilou.lan";
        private const int PortNumber = 50003;
        private const string baseDN = "CN=Users,CN=ADLDS,DC=H21-420-6D9-EQ1,DC=lan";
        private const string accountDn = "CN=0765353,CN=Users,CN=ADLDS,DC=H21-420-6D9-EQ1,DC=lan";
        private const string passwordDn = "Toto1234!";
        private string connectionFilter = "(&(objectClass=user)(objectCategory=person))";
        private string rootOU = "CN=rootHades,CN=ADLDS,DC=H21-420-6D9-EQ1,DC=lan";

        //Client Side
        //private const string server = "bkomstudios.com";
        //private const int PortNumber = 389;
        //private const string accessPoint = "OU=BkomUsers,DC=bkomstudios,DC=com";

        //https://www.novell.com/documentation/developer/ldapcsharp/?page=/documentation/developer/ldapcsharp/cnet/data/bovumfi.html
        public ADManager()
        {
            // Console.WriteLine(authenticate("CN=Etu001,CN=Users,CN=ADLDS,DC=H21-420-6D9-EQ1,DC=lan", "Toto1234!"));
            //Console.WriteLine(getAllUsers());
            Console.WriteLine(getRoot());
        }

        // Champ dans le formulaire
        // PORT
        // SERVER
        // Filtre de connection
        // baseDN (Ou sont les users) 
        // DN du compte (Quelle user connect CRUD AD)
        // Mot de passe du compte DN
        // Champ de synchronisation EX: SamAccountName

        // Create a connection with de DN account
        private LdapConnection createConnection()
        {
            //Creating an LdapConnection instance
            LdapConnection connection = new LdapConnection();
            try
            {
                //Connect function will create a socket connection to the server
                connection.Connect(server, PortNumber);
                Console.WriteLine("isConnected : " + connection.Connected);
                //Bind function will Bind the user object  Credentials to the Server
                connection.Bind(accountDn, passwordDn);
                Console.WriteLine("isAuthenticated : " + connection.Bound);

                return connection;
            }
            catch (LdapException ex)
            {
                Console.WriteLine(ex.LdapErrorMessage);
                return null;
            }
        }

        //Dicconnect the socket in parameter
        private void disconnect(LdapConnection connection)
        {
            connection.Disconnect();
        }


        //Authenticate the user in the Active Directory
        public bool authenticate(string username, string password) {
            //Creating an LdapConnection instance
            LdapConnection connection = new LdapConnection();
            try
            {
                //Connect function will create a socket connection to the server
                connection.Connect(server, PortNumber);
                Console.WriteLine("isConnected : " + connection.Connected);
                //Bind function will Bind the user object  Credentials to the Server
                connection.Bind(username, password);

                return connection.Bound;
            }
            catch (LdapException ex)
            {
                Console.WriteLine(ex.LdapErrorMessage);
                return false;
            }
        }


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
          
                // This part is to get ALL Attributes of the user
               /* // Get the attribute set of the entry
                LdapAttributeSet attributeSet = nextEntry.GetAttributeSet();
                System.Collections.IEnumerator ienum = attributeSet.GetEnumerator();

                // Parse through the attribute set to get the attributes and the corresponding values

                while (ienum.MoveNext())
                {
                    LdapAttribute attribute = (LdapAttribute)ienum.Current;
                    string attributeName = attribute.Name;
                    string attributeVal = attribute.StringValue;
                    Console.WriteLine(attributeName + "value:" + attributeVal);
                }*/
            }

            return users;
        }

        // Get all the OU and the Groups from the root 
        public List<string> getRoot()
        {
            List<string> root = new List<string>();

            //Creating an LdapConnection instance
            LdapConnection connection = createConnection();
            LdapSearchResults lsc = (LdapSearchResults)connection.Search(rootOU, LdapConnection.ScopeSub, "(&(objectClass=*))", null, false);

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

                Console.WriteLine("\n" + nextEntry.Dn);
                root.Add(nextEntry.Dn);

                // This part is to get ALL Attributes of the user
                // Get the attribute set of the entry
                /* LdapAttributeSet attributeSet = nextEntry.GetAttributeSet();
                 System.Collections.IEnumerator ienum = attributeSet.GetEnumerator();

                 //Parse through the attribute set to get the attributes and the corresponding values

                 while (ienum.MoveNext())
                 {
                     LdapAttribute attribute = (LdapAttribute)ienum.Current;
                     string attributeName = attribute.Name;
                     string attributeVal = attribute.StringValue;
                     Console.WriteLine(attributeName + "value:" + attributeVal);
                 }*/
            }
            return root;
        }
    }
}
