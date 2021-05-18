using HADES.Services;
using HADES.Util.Exceptions;
using HADES.Util.ModelAD;
using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;

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

    //https://www.novell.com/documentation/developer/ldapcsharp/?page=/documentation/developer/ldapcsharp/cnet/data/bovumfi.html
    public class ADManager
    {
        public ADManager()
        {
            if (ADSettingsCache.Ad == null) {
                ADSettingsCache.Refresh();
            }         
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
                // The key is not set 
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("LOG: " + e.Message);
                return null;
            }
        }

        /*****************************************************
        GETObjectGUID Hexadecimal value in AD
        ******************************************************/
        private string getObjectGUID(LdapEntry entry)
        {
            try
            {
                string guid = System.BitConverter.ToString(entry.GetAttribute("ObjectGUID").ByteValue);
                // Format for Search filter
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
                //Put a timeout (instead of using the default one) on the connection to reduce the time waiting AND avoiding Nginx to timeout with 504 Gateway Timeout
                connection.ConnectionTimeout = 1000 * 15;
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

        //Authenticate the user in the Active Directory 
        public bool authenticate(string username, string password)
        {
            //Creating an LdapConnection instance
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
                        Console.WriteLine("Error: " + e.Message);
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
                Console.WriteLine("LOG " + ex.Message);
                return false;
            }
        }

        /*****************************************************
         USER 
         ******************************************************/
        public List<UserAD> getAllUsers()
        {
            List<UserAD> users = new List<UserAD>();

            //Creating an LdapConnection instance
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

                    Console.WriteLine("Error: " + e.Message);
                    //Exception is thrown, go for next entry
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
                    Console.WriteLine("Error: " + e.Message);
                    //Exception is thrown, go for next entry
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
                    /*  string[] rootOuName = ADSettingsCache.Ad.RootOu.Split(',');
                      string[] splitRoutOU = nextEntry.Dn.Split(rootOuName[0]);
                      Console.WriteLine("splitResult " + splitRoutOU[0] + " /////" + splitRoutOU[1]);*/
                
                    if (nextEntry.Dn != ADSettingsCache.Ad.RootOu) {
                        string[] rootOuName = ADSettingsCache.Ad.RootOu.Split(',');
                        
                        data.Path += "/" + rootOuName[0].Split("=")[1];
                    }
                    string[] path = nextEntry.Dn.Split(',');

                    for (int i = path.Length - 1; i >= 0; i--)
                       {
                           if (path[i].Contains("DC=") || path[i].Contains("OU=" + data.SamAccountName) || path[i].Contains("CN=" + data.SamAccountName) || ADSettingsCache.Ad.RootOu.Contains(path[i]))
                           {
                               path[i] = null;
                           }

                           if (path[i] != null)
                           {
                               data.Path += "/" + path[i].Split("=")[1];
                           }
                       }

                    //   Console.WriteLine("path: " + data.Path);

                  
                    //DN 
                    data.Dn = nextEntry.Dn;

                    root.Add(data);

                    Console.WriteLine(data);

                }
                catch (Exception e)
                {
                    Console.WriteLine("Log: " + e.Message);
                    continue;
                }
            }

            connection.Disconnect();
            return root;
        }

        public List<GroupAD> getGroupsInRoot()
        {
            List<GroupAD> root = new List<GroupAD>();

            //Creating an LdapConnection instance
            LdapConnection connection = createConnection();
            LdapSearchResults lsc = (LdapSearchResults)connection.Search(ADSettingsCache.Ad.RootOu, LdapConnection.ScopeSub, "(|(objectClass=group))", null, false);

            while (lsc.HasMore())
            {
                GroupAD group = new GroupAD();
                LdapEntry nextEntry = null;
                try
                {
                    nextEntry = lsc.Next();
                    group.SamAccountName = getAttributeValue(nextEntry, "sAMAccountName");
                    group.Email = getAttributeValue(nextEntry, "mail");
                    group.Notes = getAttributeValue(nextEntry, "info");
                    group.Description = getAttributeValue(nextEntry, "description");
                    group.Members = GetMembersOfGroup(nextEntry.Dn, connection);
                    group.ObjectGUID = getObjectGUID(nextEntry);
                    root.Add(group);
                    Console.WriteLine(group);
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
                    group.Members = GetMembersOfGroup(groupDN,connection);
                    group.ObjectGUID = getObjectGUID(nextEntry);
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
                Console.WriteLine("Cannot create the folder: " + e.Message);
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
                Console.WriteLine("Cannot delete the folder: " + e.Message);
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
                Console.WriteLine("Cannot create the group:" + e.Message);
                connection.Disconnect();
                return false;
            }
        }


        public bool modifyGroup(string dnGroupToModify, string name, string ouGroup, string description, string email, string notes, Dictionary<UserAD, Action> members)

        {
            LdapConnection connection = createConnection();
           
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
                    if (entry.Value == Action.ADD)
                    {
                        add.Add(entry.Key.Dn);

                    }
                    else if (entry.Value == Action.DELETE)
                    {
                        delete.Add(entry.Key.Dn);
                    }
                }

                if (add.Count > 0)
                {
                    addMemberToGroup(dnGroupToModify, add);
                }
                if (delete.Count > 0)
                {
                    deleteMemberToGroup(dnGroupToModify, delete);
                }
                return true;
            
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
                Console.WriteLine("Cannot delete the group: " + e.Message);
                return false;
            }
        }

        public bool doesGroupExist(string GUID)
        {
            LdapConnection connection = createConnection();
            bool wasFound = false;

            try
            {
                LdapSearchResults lsc = (LdapSearchResults)connection.Search(ADSettingsCache.Ad.RootOu, LdapConnection.ScopeSub, "(objectGUID =" + GUID + ")", null, false);

                if (lsc.HasMore())
                {
                    Console.WriteLine("The group was found");
                    wasFound = true;
                }
                else
                {
                    Console.WriteLine("The group was NOT found");
                }
            }
            catch (Exception) 
            {
                connection.Disconnect();
                throw new ADException();
            }

            connection.Disconnect();
            return wasFound;
        }

        private string getBaseAd() {
            string[] rootTab = ADSettingsCache.Ad.RootOu.Split(",");
            string b = "";
            for (int i = 0; i < rootTab.Length; i++)
            {
                if (rootTab[i].Contains("DC="))
                {
                    b += rootTab[i] + ",";
                }
            }
            if (b.Length > 0)
            {
                b = b.Remove(b.Length - 1);
            }
            return b;
        }
        public string getGroupDnByGUID(string GUID)
        {
            LdapConnection connection = createConnection();
            String dn = "";

            try
            {
                LdapSearchResults lsc = (LdapSearchResults)connection.Search(getBaseAd(), LdapConnection.ScopeSub, "(objectGUID =" + GUID + ")", null, false);
                LdapEntry nextEntry = null;
                while (lsc.HasMore())
                {

                    nextEntry = lsc.Next();
                    dn = nextEntry.Dn;
                }
            }
            catch (Exception)
            {
                connection.Disconnect();
                return dn;
            }

            connection.Disconnect();
            return dn;

        }

        public string getGroupGUIDByDn(string Dn)
        {
            LdapConnection connection = createConnection();
            String GUID = "";

            try
            {
                LdapSearchResults lsc = (LdapSearchResults)connection.Search(getBaseAd(), LdapConnection.ScopeSub, "(&(objectClass=group)(distinguishedName=" + Dn + "))", null, false);
                LdapEntry nextEntry = null;
                while (lsc.HasMore())
                {

                    nextEntry = lsc.Next();
                    GUID = getObjectGUID(nextEntry);
                }
            }
            catch (Exception)
            {
                connection.Disconnect();
                return GUID;
            }

            connection.Disconnect();
            return GUID;

        }

        /*****************************************************
         MEMBER
         ******************************************************/
        public List<UserAD> GetMembersOfGroup(string groupDN, LdapConnection connectionAlreadyOpen)
        {
            LdapConnection connection;
            if (connectionAlreadyOpen == null)
            {
                connection = createConnection();
            }
            else {
                connection = connectionAlreadyOpen;
            }

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
                    connection.Disconnect();
                    Console.WriteLine("LOG: " + e.Message);
                    continue;
                }
                catch (Exception e)
                {
                    Console.WriteLine("LOG: " + e.Message);
                    connection.Disconnect();
                }
            }
            
            if (connectionAlreadyOpen == null) {  
                connection.Disconnect();
            }
           
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

                //Modify the entry in the directory
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

                //Modify the entry in the directory
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
