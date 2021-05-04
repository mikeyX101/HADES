using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HADES.Models;
using HADES.Services;
using HADES.Util.Exceptions;
using HADES.Util.ModelAD;
using Novell.Directory.Ldap;

namespace HADES.Util
{
    public enum Action {
        ADD,
        DELETE
    }

    public enum SyncField
    {
        samaccountname,
        mail,
        userprincipalname
    }
    public class ADManager
    {
        public object DatabaseSyncService { get; private set; }


        //   private string server = "172.20.48.10";
        //  private int PortNumber = 389;
        // private string accountDn = "CN=hades,CN=Users,DC=R991-AD,DC=lan";
        //  private string passwordDn = "Toto123!";
        // private string baseDN = "CN=Users,DC=R991-AD,DC=lan";
        // private string rootOU = "OU=hades_root,DC=R991-AD,DC=lan";
        // private string connectionFilter = "(&(objectClass=user)(objectCategory=person))";
        //private SyncField syncField = SyncField.samaccountname;


        //Client Side information for testing
        //private const string server = "bkomstudios.com";
        //private const int PortNumber = 389;
        //private const string accountDn = "CN=hades,OU=ServiceAccounts,OU=BkomUsers,DC=bkomstudios,DC=com";
        //private const string passwordDn = "";
        //private const string baseDN = "OU=BkomUsers,DC=bkomstudios,DC=com";
        //private string rootOU = "OU=BkomGroups,DC=bkomstudios,DC=com";

        //https://www.novell.com/documentation/developer/ldapcsharp/?page=/documentation/developer/ldapcsharp/cnet/data/bovumfi.html
        public ADManager()
        {
            if (ADSettingsCache.Ad == null) {
                ADSettingsCache.Refresh();
            }
        }
       
        /*****************************************************
            For testing in the dev Build
         ******************************************************/
        public void test() {
            /*  UserAD u1 = getUserAD("hades");
              UserAD u2 = getUserAD("Administrator");
              UserAD u3 = getUserAD("Guest");
              Dictionary<UserAD,Action> list = new Dictionary<UserAD, Action>();
              list.Add(u1, Action.DELETE);
              list.Add(u2, Action.DELETE);
              list.Add(u3, Action.DELETE);

              modifyGroup("CN=Group11,OU=Dossier22,OU=hades_root,DC=R991-AD,DC=lan", "Group11", "Dossier22", "Une fgdsgfsdescriptionfsd", "emddddailfds", "notessssdsadsassssssssssssssssssdfs", list);
          */
            // getUserAD("hades@R991-AD.lan");
            // getUserAD("hades@hades.com");
            //  Console.WriteLine( getUserAD("hades"));

            //  authenticate("hades", "Toto123!");
            //authenticate("hades@hades.com", "Toto123!");
            // authenticate("hades@R991-AD.lan", "Toto123!");

            // getAllUsers();
            //  Console.WriteLine(getGroupInformation("CN=Group11,OU=Dossier1,OU=hades_root,DC=R991-AD,DC=lan"));
            // UserAD hades = getUserAD("hades",false);

            //  Console.WriteLine(hades.ObjectGUID);

            // Console.WriteLine(getUserAD(hades.ObjectGUID, true));
        }

       

        /*****************************************************
         GETATTRIBUTE in AD
         ******************************************************/
        private string getAttributeValue(LdapEntry entry, string attribute)
        {
            try
            {
                return entry.GetAttribute(attribute).StringValue;
            }
            catch (KeyNotFoundException)
            {
                //The key is not set or empty
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("LOG: " + e.Message);
                return null;
            }
        }

        private string getObjectGUID(LdapEntry entry)
        {
            try
            {
                string guid = System.BitConverter.ToString(entry.GetAttribute("ObjectGUID").ByteValue);
                guid = "\\" + guid.Replace("-", "\\").ToLower();

                return guid;

            }
            catch (KeyNotFoundException)
            {
                //The key is not set or empty
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("LOG: " + e.Message);
                return null;
            }
        }

        /*****************************************************
         CONNECTION
         ******************************************************/

        private LdapConnection createConnection(string userDN = null, string password = null)
        {
            DatabaseSyncService.ExecUpdate();
            //Creating an LdapConnection instance
            LdapConnection connection = new LdapConnection();
            try
            {
                //Connect function will create a socket connection to the server
                connection.Connect(ADSettingsCache.Ad.ServerAddress, ADSettingsCache.Ad.PortNumber);
                Console.WriteLine("isConnected : " + connection.Connected);

                //Bind function will Bind the user object Credentials to the Server
                if (userDN != null && password != null)
                {
                    Console.WriteLine("userCredential");
                    connection.Bind(userDN, password);
                }
                else
                {
                    Console.WriteLine("serverCredential");
                    connection.Bind(ADSettingsCache.Ad.AccountDN, ADSettingsCache.Ad.PasswordDN);
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

        //Authenticate the user in the Active Directory for the login
        public bool authenticate(string username, string password)
        {
            LdapConnection connection = createConnection();

            try
            {
                LdapSearchResults lsc = (LdapSearchResults)connection.Search(ADSettingsCache.Ad.BaseDN, LdapConnection.ScopeSub, ADSettingsCache.Ad.ConnectionFilter.Remove(ADSettingsCache.Ad.ConnectionFilter.Length - 1) + "(" + ADSettingsCache.Ad.SyncField + "=" + username + "))", null, false);

                string userDN = null;
                bool userIsAuthenticate = false;

                while (lsc.HasMore())
                {
                    LdapEntry nextEntry = null;
                    try
                    {
                        nextEntry = lsc.Next();
                        Console.WriteLine(nextEntry.Dn);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("LOG: " + e.Message);
                        continue;
                    }


                    if (getAttributeValue(nextEntry, ADSettingsCache.Ad.SyncField) == username)
                    {
                        userDN = nextEntry.Dn;
                    }
                }

                connection.Disconnect();

                if (userDN != null)
                {

                    connection = createConnection(userDN, password);
                    if (connection != null)
                    {
                        userIsAuthenticate = connection.Bound;
                    }
                }

                return userIsAuthenticate;
            }
            catch (Exception ex)
            {
                Console.WriteLine("LOG: " + ex.Message);
                return false;
            }
        }

        /*****************************************************
         USER 
         ******************************************************/
        public List<UserAD> getAllUsers()
        {
            List<UserAD> users = new List<UserAD>();

            LdapConnection connection = createConnection();
            LdapSearchResults lsc = (LdapSearchResults)connection.Search(ADSettingsCache.Ad.BaseDN, LdapConnection.ScopeSub, ADSettingsCache.Ad.ConnectionFilter, null, false);

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
                    u.ObjectGUID = getObjectGUID(nextEntry);

                    Console.WriteLine(u);
                    users.Add(u);
                }
                catch (Exception e)
                {

                    Console.WriteLine("LOG: " + e.Message);
                    continue;
                }

            }

            return users;
        }

        public UserAD getUserAD(string usernameOrGUID, bool fetchByGUID = false)
        {
            UserAD u = null;
            LdapConnection connection = createConnection();
            LdapSearchResults lsc = null;
            if (fetchByGUID == false)
            {
                lsc = (LdapSearchResults)connection.Search(ADSettingsCache.Ad.BaseDN, LdapConnection.ScopeSub, ADSettingsCache.Ad.ConnectionFilter.Remove(ADSettingsCache.Ad.ConnectionFilter.Length - 1) + "(" + ADSettingsCache.Ad.SyncField + "=" + usernameOrGUID + "))", null, false);
            }
            else {
                lsc = (LdapSearchResults)connection.Search(ADSettingsCache.Ad.BaseDN, LdapConnection.ScopeSub, "(objectGUID=" + usernameOrGUID + ")", null, false);
            }



            while (lsc.HasMore())
            {
                LdapEntry nextEntry = null;
                try
                {
                    nextEntry = lsc.Next();

                    u = new UserAD();
                    u.SamAccountName = getAttributeValue(nextEntry, "samaccountName");
                    u.FirstName = getAttributeValue(nextEntry, "givenName");
                    u.LastName = getAttributeValue(nextEntry, "sn");
                    u.Dn = nextEntry.Dn;
                    u.ObjectGUID = getObjectGUID(nextEntry);
                    Console.WriteLine(u);

                }
                catch (Exception e)
                {
                    Console.WriteLine("LOG: " + e.Message);
                    continue;
                }

            }

            if (u == null)
            {
                Console.WriteLine("LOG : USER WAS NOT FOUND ");
                throw new LoginException();
            }

            return u;
        }

        /*****************************************************
         ROOT
         ******************************************************/
        public List<RootDataInformation> getRoot()
        {
            List<RootDataInformation> root = new List<RootDataInformation>();

            //Creating an LdapConnection instance
            LdapConnection connection = createConnection();
            LdapSearchResults lsc = (LdapSearchResults)connection.Search(ADSettingsCache.Ad.RootOu, LdapConnection.ScopeSub, "(|(objectClass=group)(objectClass=organizationalUnit))", null, false);

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
                    catch (KeyNotFoundException)
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
                    Console.WriteLine("LOG: " + e.Message);
                    continue;
                }
            }

            connection.Disconnect();
            return root;
        }

        public GroupAD getGroupInformation(string groupDN)
        {
            //Creating an LdapConnection instance
            LdapConnection connection = createConnection();

            LdapSearchResults lsc = (LdapSearchResults)connection.Search(ADSettingsCache.Ad.RootOu, LdapConnection.ScopeSub, "(&(objectClass=group)(distinguishedName=" + groupDN + "))", null, false);
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
                    group.ObjectGUID = getObjectGUID(nextEntry);
                }
                catch (LdapException e)
                {
                    Console.WriteLine("LOG: " + e.LdapErrorMessage);
                    continue;
                }
                catch (Exception e)
                {
                    Console.WriteLine("LOG: " + e.Message);
                }
            }

            connection.Disconnect();
            return group;
        }

        /*****************************************************
         OU
         ******************************************************/

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
                string dn = "OU=" + name + "," + ADSettingsCache.Ad.RootOu;
                LdapEntry newEntry = new LdapEntry(dn, attributeSet);
                //Add the entry to the directory
                connection.Add(newEntry);
                connection.Disconnect();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("LOG : Cannot create the folder: " + e.Message);
                connection.Disconnect();
                return false;
            }
        }

        public bool renameOU(string dnOUToRename, string newName)
        {
            LdapConnection connection = createConnection();
            try
            {
                string newRdn = "OU=" + newName;

                connection.Rename(dnOUToRename, newRdn, true);
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

        public bool deleteOU(string dnOUToDelete)
        {
            LdapConnection connection = createConnection();
            try
            {
                connection.Delete(dnOUToDelete);
                connection.Disconnect();
                return true;
            }
            catch (Exception e)
            {
                connection.Disconnect();
                Console.WriteLine("LOG : Cannot delete the folder: " + e.Message);
                return false;
            }
        }

        /*****************************************************
         GROUP
         ******************************************************/
        public bool createGroup(string name, string ouName, string description, string email, string notes, List<UserAD> members)
        {

            LdapConnection connection = createConnection();
            try
            {
                //Creates the List attributes of the entry and add them to attribute
                LdapAttributeSet attributeSet = new LdapAttributeSet();
                attributeSet.Add(new LdapAttribute("objectclass", "group"));
                attributeSet.Add(new LdapAttribute("CN", name));
                attributeSet.Add(new LdapAttribute("samaccountname", name));
                attributeSet.Add(new LdapAttribute("name", name));
                attributeSet.Add(new LdapAttribute("description", description));
                attributeSet.Add(new LdapAttribute("mail", email));
                attributeSet.Add(new LdapAttribute("info", notes));

                // DN of the entry to be added
                string dn = "CN=" + name + "," + "OU=" + ouName + "," + ADSettingsCache.Ad.RootOu;
                LdapEntry newEntry = new LdapEntry(dn, attributeSet);

                //Add the entry to the directory
                connection.Add(newEntry);
                connection.Disconnect();

                //Add members
                List<string> add = new List<string>();
                foreach (UserAD m in members) {
                    add.Add(m.Dn);
                }
                addMemberToGroup(dn, add);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("LOG : Cannot create the group:" + e.Message);
                connection.Disconnect();
                return false;
            }
        }


        public bool modifyGroup(string dnGroupToModify, string name, string ouGroup, string description, string email, string notes, Dictionary<UserAD, Action> members)

        {
            LdapConnection connection = createConnection();
            try
            {

                //Rename 
                string newRdn = "CN=" + name;
                connection.Rename(dnGroupToModify, newRdn, true);

                dnGroupToModify = newRdn + ",OU=" + ouGroup + "," + ADSettingsCache.Ad.RootOu;

                //Modify Attribute
                List<LdapModification> modList = new List<LdapModification>();

                //Description
                LdapAttribute attribute = new LdapAttribute("description", description);
                modList.Add(new LdapModification(LdapModification.Replace, attribute));

                //Email
                attribute = new LdapAttribute("mail", email);
                modList.Add(new LdapModification(LdapModification.Replace, attribute));

                //Notes
                attribute = new LdapAttribute("info", notes);
                modList.Add(new LdapModification(LdapModification.Replace, attribute));

                //SamAccountName 
                attribute = new LdapAttribute("samaccountname", name);
                modList.Add(new LdapModification(LdapModification.Replace, attribute));

                LdapModification[] mods = new LdapModification[modList.Count];
                mods = modList.ToArray();
                connection.Modify(dnGroupToModify, mods);

                connection.Disconnect();

                List<string> add = new List<string>();
                List<string> delete = new List<string>();
                //Modify members
                foreach (KeyValuePair<UserAD, Action> entry in members)
                {
                    if (entry.Value == Action.ADD) {
                        add.Add(entry.Key.Dn);

                    } else if (entry.Value == Action.DELETE) {
                        delete.Add(entry.Key.Dn);
                    }
                }

                if (add.Count > 0) {
                    addMemberToGroup(dnGroupToModify, add);
                }
                if (delete.Count > 0)
                {
                    deleteMemberToGroup(dnGroupToModify, delete);
                }
                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine("LOG : Cannot modify the group: " + e.Message);
                return false;
            }
        }

        public bool deleteGroup(string dnGroupToDelete)
        {
            LdapConnection connection = createConnection();
            try
            {
                connection.Delete(dnGroupToDelete);
                connection.Disconnect();
                return true;
            }
            catch (Exception e)
            {
                connection.Disconnect();
                Console.WriteLine("LOG : Cannot delete the group: " + e.Message);
                return false;
            }
        }

        /*****************************************************
         MEMBER
         ******************************************************/
        public List<UserAD> GetMembersOfGroup(string groupDN)
        {
            LdapConnection connection = createConnection();

            LdapSearchResults lsc = (LdapSearchResults)connection.Search(ADSettingsCache.Ad.BaseDN, LdapConnection.ScopeSub, "(&(objectClass=user)(memberOf=" + groupDN + "))", null, false);
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
                    Console.WriteLine("LOG: " + e.Message);
                    continue;
                }
                catch (Exception e)
                {
                    Console.WriteLine("LOG: " + e.Message);
                }
            }

            connection.Disconnect();
            return users;
        }

        public bool addMemberToGroup(string groupDn, List<string> usersDn)
        {
            LdapConnection connection = createConnection();
            try
            {
                List<LdapModification> modList = new List<LdapModification>();


                foreach (string dn in usersDn)
                {
                    LdapAttribute attribute = new LdapAttribute("member", dn);
                    modList.Add(new LdapModification(LdapModification.Add, attribute));
                }


                LdapModification[] mods = new LdapModification[modList.Count];
                mods = modList.ToArray();

                connection.Modify(groupDn, mods);

                connection.Disconnect();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot add user in " + groupDn + " : " + e.Message);
                connection.Disconnect();
                return false;
            }
        }

        public bool deleteMemberToGroup(string groupDn, List<string> usersDn)
        {
            LdapConnection connection = createConnection();
            try
            {
                List<LdapModification> modList = new List<LdapModification>();

                foreach (string dn in usersDn)
                {
                    LdapAttribute attribute = new LdapAttribute("member", dn);
                    modList.Add(new LdapModification(LdapModification.Delete, attribute));
                }

                LdapModification[] mods = new LdapModification[modList.Count];
                mods = modList.ToArray();

                connection.Modify(groupDn, mods);

                connection.Disconnect();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("LOG : Cannot delete user in "+ groupDn+" : " + e.Message);
                connection.Disconnect();
                return false;
            }
        }

    }
}
