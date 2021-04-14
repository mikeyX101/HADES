using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Novell.Directory.Ldap;

namespace HADES.Util
{
    public class ADManager
    {
        private const string connectionPoint = "CN=ADLDS,DC=H21-420-6D9-EQ1,DC=lan";
        private const string server = "sv01-bdinfo01.cegeplimoilou.lan";
        private const int PortNumber = 50003;
        private const string DomainName = "H21-420-6D9-EQ1.lan";

        //https://www.novell.com/documentation/developer/ldapcsharp/?page=/documentation/developer/ldapcsharp/cnet/data/bovumfi.html
        public ADManager()
        {

        }

        // Create a Authenticated connection 
        public LdapConnection createConnection()
        {
            //Creating an LdapConnection instance
            LdapConnection connection = new LdapConnection();
            try
            {
                string username = "CN=0765353,CN=Users,CN=ADLDS,DC=H21-420-6D9-EQ1,DC=lan";
                string password = "Toto1234!";


                //Connect function will create a socket connection to the server
                connection.Connect(server, PortNumber);
                Console.WriteLine("isConnected : " + connection.Connected);
                //Bind function will Bind the user object  Credentials to the Server
                connection.Bind(username, password);
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
        public void disconnect(LdapConnection connection)
        {
            connection.Disconnect();
        }


        // NOT WORKING
        public String getAllUsers()
        {
            //Creating an LdapConnection instance
            LdapConnection connection = createConnection();

            LdapSearchResults lsc = (LdapSearchResults)connection.Search("CN=ADLDS,DC=H21-420-6D9-EQ1,DC=lan", LdapConnection.ScopeOne, "(objectClass = user)", null, false);

            Console.WriteLine(lsc.Count);
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

                // Get the attribute set of the entry
                LdapAttributeSet attributeSet = nextEntry.GetAttributeSet();
                System.Collections.IEnumerator ienum = attributeSet.GetEnumerator();

                // Parse through the attribute set to get the attributes and the corresponding values

                while (ienum.MoveNext())
                {
                    LdapAttribute attribute = (LdapAttribute)ienum.Current;
                    string attributeName = attribute.Name;
                    string attributeVal = attribute.StringValue;
                    Console.WriteLine(attributeName + "value:" + attributeVal);
                }
            }


            return connection.AuthenticationDn;


        }
    }
}
