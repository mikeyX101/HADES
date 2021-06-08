using HADES.Services;
using HADES.Util.Exceptions;
using HADES.Util.ModelAD;
using Novell.Directory.Ldap;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace HADES.Util
{
    public enum Action
    {
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
        private static readonly string GenericErrorLogTemplate = "An unexepected error occured while doing an operation with the Active Directory in function {Function}";
        private static readonly string DataFetchErrorLogTemplate = "An unexpected error occured while fetching data in the Active Directory in function {Function}";

        public ADManager()
        {
            if (ADSettingsCache.Ad == null)
            {
                ADSettingsCache.Refresh();
            }
        }

        public ADManager(Data.ApplicationDbContext db)
        {
            if (ADSettingsCache.Ad == null)
            {
                ADSettingsCache.Refresh(db);
            }
        }

        /*****************************************************
         GETATTRIBUTE  AD
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
                Log.Warning(e, GenericErrorLogTemplate, "getAttributeValue()");
                return null;
            }
        }


        /*****************************************************
         GETATTRIBUTE  AD
         ******************************************************/
        private DateTime getDateExp(LdapEntry entry)
        {

            try
            {
                string dateExp = entry.GetAttribute("expirationDateHades").StringValue;
                string format = "yyMMddHHmmss'Z'";
                DateTime d = DateTime.ParseExact(dateExp, format, CultureInfo.InvariantCulture);
                return d;
            }
            catch (KeyNotFoundException)
            {
                // The key is not set 
                return DateTime.Now;
            }
            catch (Exception e)
            {
                Log.Warning(e, GenericErrorLogTemplate, "getAttributeValue()");
                return DateTime.Now;
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
                Log.Warning(e, GenericErrorLogTemplate, "getObjectGUID()");
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
                connection.ConnectionTimeout = 1000 * 5;
                //Connect function will create a socket connection to the server
                connection.Connect(ADSettingsCache.Ad.ServerAddress, ADSettingsCache.Ad.PortNumber);
                Log.Verbose("isConnected : {Connected}", connection.Connected);

                //Bind function will Bind the user object Credentials to the Server
                if (userDN != null && password != null)
                {
                    Log.Verbose("userCredential");
                    connection.Bind(userDN, password);
                }
                else
                {
                    Log.Verbose("serverCredential");
                    string p = EncryptionUtil.Decrypt(ADSettingsCache.Ad.PasswordDN);
                    connection.Bind(ADSettingsCache.Ad.AccountDN, p);
                }

                Log.Verbose("isAuthenticated : {Bound}", connection.Bound);

                return connection;

            }
            catch (Exception ex)
            {
                Log.Warning(ex, GenericErrorLogTemplate, "createConnection()");
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
                    }
                    catch (Exception e)
                    {
                        Log.Warning(e, DataFetchErrorLogTemplate, "authenticate()");
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
                Log.Warning(ex, GenericErrorLogTemplate, "authenticate()");
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

                    users.Add(u);
                }
                catch (Exception e)
                {

                    Log.Warning(e, DataFetchErrorLogTemplate, "getAllUsers()");
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
            else
            {
                lsc = (LdapSearchResults)connection.Search(ADSettingsCache.Ad.BaseDN, LdapConnection.ScopeSub, "(objectGUID=" + usernameOrGUID + ")", null, false);
            }



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
            }
            catch (LdapReferralException)
            {
                LoginException exception = new();
                Log.Warning(exception, "AD User not found in {Function}", "getUserAD()");
                throw exception;
            }
            catch (Exception e)
            {
                Log.Warning(e, DataFetchErrorLogTemplate, "getUserAD()");
                throw;
            }

            if (u == null)
            {
                LoginException exception = new();
                Log.Warning(exception, "AD User not found in {Function}", "getUserAD()");
                throw exception;
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

                    // PATH OF THE OBJECT
                    if (nextEntry.Dn != ADSettingsCache.Ad.RootOu)
                    {
                        string[] rootOuName = ADSettingsCache.Ad.RootOu.Split(',');

                        data.Path += "/" + rootOuName[0].Split("=")[1];
                    }

                    string[] path = nextEntry.Dn.Split(',');
                    
                    // We stop when 1 >= because we don't when the OUname or the GroupName in the path
                    for (int i = path.Length - 1; i >= 1; i--)
                    {
                        if (path[i].Contains("DC=") || ADSettingsCache.Ad.RootOu.Contains(path[i]))
                        {
                            path[i] = "";
                        }

                        if (path[i] != "")
                        {
                            data.Path += "/" + path[i].Split("=")[1];
                        }
                    }


                    //DN 
                    data.Dn = nextEntry.Dn;

                    //GUID
                    data.ObjectGUID = getObjectGUID(nextEntry);

                    root.Add(data);
                }
                catch (KeyNotFoundException e)
                {
                    Log.Error(e, "Fetched objectClass in LDAP Entry when it should exist");
                }
                catch (Exception e)
                {
                    Log.Warning(e, GenericErrorLogTemplate, "getRoot()");
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
                    group.ExpirationDate = getDateExp(nextEntry);

                    root.Add(group);

                }
                catch (Exception e)
                {
                    Log.Warning(e, DataFetchErrorLogTemplate, "getGroupsInRoot()");
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
            if (lsc.HasMore())
            {

                LdapEntry nextEntry = null;
                try
                {
                    nextEntry = lsc.Next();
                    group.SamAccountName = getAttributeValue(nextEntry, "sAMAccountName");
                    group.Email = getAttributeValue(nextEntry, "mail");
                    group.Notes = getAttributeValue(nextEntry, "info");
                    group.Description = getAttributeValue(nextEntry, "description");
                    group.Members = GetMembersOfGroup(groupDN, connection);
                    group.ObjectGUID = getObjectGUID(nextEntry);
                    group.ExpirationDate = getDateExp(nextEntry);
                }
                catch (LdapException e)
                {
                    connection.Disconnect();
                    Log.Warning(e, DataFetchErrorLogTemplate, "getGroupInformation()");
                    //Exception is thrown, go for next entry

                }
                catch (Exception e)
                {
                    connection.Disconnect();
                    Log.Warning(e, GenericErrorLogTemplate, "getGroupInformation()");
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
                Log.Warning(e, GenericErrorLogTemplate, "createOU()");
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
                Log.Warning(e, GenericErrorLogTemplate, "renameOU()");
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
                Log.Warning(e, GenericErrorLogTemplate, "deleteOU()");
                return false;
            }
        }

        /*****************************************************
         GROUP
         ******************************************************/
        public bool createGroup(string ouName, GroupAD group, List<UserAD> members)
        {
            if (string.IsNullOrWhiteSpace(group.Description))
            {
                group.Description = " ";
            }
            if (string.IsNullOrWhiteSpace(group.Email))
            {
                group.Email = " ";
            }
            if (string.IsNullOrWhiteSpace(group.Notes))
            {
                group.Notes = " ";
            }

            LdapConnection connection = createConnection();
            try
            {
                //Creates the List attributes of the entry and add them to attribute
                LdapAttributeSet attributeSet = new LdapAttributeSet();
                attributeSet.Add(new LdapAttribute("objectclass", "group"));
                attributeSet.Add(new LdapAttribute("CN", group.SamAccountName));
                attributeSet.Add(new LdapAttribute("samaccountname", group.SamAccountName));
                attributeSet.Add(new LdapAttribute("name", group.SamAccountName));
                attributeSet.Add(new LdapAttribute("description", group.Description));
                attributeSet.Add(new LdapAttribute("mail", group.Email));
                attributeSet.Add(new LdapAttribute("info", group.Notes));
                string format = "yyMMddHHmmss'Z'";
                attributeSet.Add(new LdapAttribute("expirationDateHades", group.ExpirationDate.ToString(format)));
                // DN of the entry to be added
                string dn = "CN=" + group.SamAccountName + "," + "OU=" + ouName + "," + ADSettingsCache.Ad.RootOu;
                LdapEntry newEntry = new LdapEntry(dn, attributeSet);
                //Add the entry to the directory
                connection.Add(newEntry);
                connection.Disconnect();

                EmailHelper.SendEmail(NotificationType.GroupCreate, this.getGroupInformation(dn));

                //Add members
                addMemberToGroup(dn, members);

                return true;
            }
            catch (Exception e)
            {
                Log.Warning(e, GenericErrorLogTemplate, "createGroup()");
                connection.Disconnect();
                return false;
            }
        }

        public bool modifyGroup(string dnGroupToModify, GroupAD group, string ouGroup, Dictionary<UserAD, Action> members)
        {
            if (string.IsNullOrWhiteSpace(group.Description))
            {
                group.Description = " ";
            }
            if (string.IsNullOrWhiteSpace(group.Email))
            {
                group.Email = " ";
            }
            if (string.IsNullOrWhiteSpace(group.Notes))
            {
                group.Notes = " ";
            }
            try
            {
                LdapConnection connection = createConnection();

                //Rename 
                string newRdn = "CN=" + group.SamAccountName;
                connection.Rename(dnGroupToModify, newRdn, true);

                dnGroupToModify = newRdn + ",OU=" + ouGroup + "," + ADSettingsCache.Ad.RootOu;

                //Modify Attribute
                List<LdapModification> modList = new List<LdapModification>();

                //Description
                LdapAttribute attribute = new LdapAttribute("description", group.Description);
                modList.Add(new LdapModification(LdapModification.Replace, attribute));

                //Email

                attribute = new LdapAttribute("mail", group.Email);
                modList.Add(new LdapModification(LdapModification.Replace, attribute));

                //Notes
                attribute = new LdapAttribute("info", group.Notes);
                modList.Add(new LdapModification(LdapModification.Replace, attribute));

                //SamAccountName 
                attribute = new LdapAttribute("samaccountname", group.SamAccountName);
                modList.Add(new LdapModification(LdapModification.Replace, attribute));

                //DateExp 
                string format = "yyMMddHHmmss'Z'";
                attribute = new LdapAttribute("expirationDateHades", group.ExpirationDate.ToString(format));
                modList.Add(new LdapModification(LdapModification.Replace, attribute));

                LdapModification[] mods = new LdapModification[modList.Count];
                mods = modList.ToArray();
                connection.Modify(dnGroupToModify, mods);

                connection.Disconnect();

                List<UserAD> add = new List<UserAD>();
                List<UserAD> delete = new List<UserAD>();
                //Modify members
                foreach (KeyValuePair<UserAD, Action> entry in members)
                {
                    if (entry.Value == Action.ADD)
                    {
                        add.Add(entry.Key);

                    }
                    else if (entry.Value == Action.DELETE)
                    {
                        delete.Add(entry.Key);
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
            catch (Exception e)
            {
                Log.Warning(e, GenericErrorLogTemplate, "modifyGroup()");
                return false;
            }
        }

        public bool deleteGroup(string dnGroupToDelete)
        {
            LdapConnection connection = createConnection();
            try
            {
                EmailHelper.SendEmail(NotificationType.GroupDelete, this.getGroupInformation(dnGroupToDelete));
                connection.Delete(dnGroupToDelete);
                connection.Disconnect();
                return true;
            }
            catch (Exception e)
            {
                connection.Disconnect();
                Log.Warning(e, GenericErrorLogTemplate, "deleteGroup()");
                return false;
            }
        }

        public bool doesGroupExist(string GUID)
        {
            LdapConnection connection = createConnection();
            bool wasFound = false;

            try
            {
                LdapSearchResults lsc = (LdapSearchResults)connection.Search(getBaseAd(), LdapConnection.ScopeSub, "(objectGUID =" + GUID + ")", null, false);

                if (lsc.HasMore())
                {
                    wasFound = true;
                }
            }
            catch (Exception e)
            {
                connection.Disconnect();
                Log.Warning(e, GenericErrorLogTemplate, "doesGroupExist()");
                throw new ADException();
            }

            connection.Disconnect();
            return wasFound;
        }

        public string getBaseAd()
        {
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
            string dn = "";

            try
            {
                LdapSearchResults lsc = (LdapSearchResults)connection.Search(getBaseAd(), LdapConnection.ScopeSub, "(objectGUID =" + GUID + ")", null, false);

                LdapEntry nextEntry = lsc.Next();
                dn = nextEntry.Dn;

            }
            catch (Exception e)
            {
                connection.Disconnect();
                Log.Warning(e, DataFetchErrorLogTemplate, "getGroupDnByGUID()");
                return dn;
            }

            connection.Disconnect();
            return dn;

        }

        public string getGroupSamAccountNameByGUID(string GUID)
        {
            LdapConnection connection = createConnection();
            string samAccountName = "";

            try
            {
                LdapSearchResults lsc = (LdapSearchResults)connection.Search(getBaseAd(), LdapConnection.ScopeSub, "(objectGUID =" + GUID + ")", null, false);
                LdapEntry nextEntry = null;
                while (lsc.HasMore())
                {

                    nextEntry = lsc.Next();
                    samAccountName = getAttributeValue(nextEntry, "sAMAccountName"); ;
                }
            }
            catch (Exception e)
            {
                connection.Disconnect();
                Log.Warning(e, DataFetchErrorLogTemplate, "getGroupSamAccountNameByGUID()");
                return samAccountName;
            }

            connection.Disconnect();
            return samAccountName;

        }

        public string getGroupGUIDByDn(string Dn)
        {
            LdapConnection connection = createConnection();
            string GUID = "";

            try
            {
                LdapSearchResults lsc = (LdapSearchResults)connection.Search(getBaseAd(), LdapConnection.ScopeSub, "(&(objectClass=group)(distinguishedName=" + Dn + "))", null, false);
                LdapEntry nextEntry = lsc.Next();
                GUID = getObjectGUID(nextEntry);

            }
            catch (Exception e)
            {

                connection.Disconnect();
                Log.Warning(e, DataFetchErrorLogTemplate, "getGroupGUIDByDn()");
                return GUID;
            }

            connection.Disconnect();
            return GUID;

        }

        public List<string> GetGroupsNameforUser(string userDn, LdapConnection connectionAlreadyOpen)
        {
            LdapConnection connection;
            if (connectionAlreadyOpen == null)
            {
                connection = createConnection();
            }
            else
            {
                connection = connectionAlreadyOpen;
            }

            LdapSearchResults lsc = (LdapSearchResults)connection.Search(ADSettingsCache.Ad.RootOu, LdapConnection.ScopeSub, "(&(objectClass=group)(member=" + userDn + "))", null, false);
            List<string> groupsname = new List<string>();

            while (lsc.HasMore())
            {
                LdapEntry nextEntry = null;
                try
                {
                    nextEntry = lsc.Next();
                    groupsname.Add(getAttributeValue(nextEntry, "sAMAccountName"));

                }
                catch (LdapException e)
                {
                    connection.Disconnect();
                    Log.Warning(e, DataFetchErrorLogTemplate, "GetGroupsNameforUser()");
                    continue;
                }
                catch (Exception e)
                {
                    Log.Warning(e, GenericErrorLogTemplate, "GetGroupsNameforUser()");
                    connection.Disconnect();
                }
            }

            if (connectionAlreadyOpen == null)
            {
                connection.Disconnect();
            }

            return groupsname;
        }

        public List<String> GetGroupsDNforUser(string userDn, LdapConnection connectionAlreadyOpen)
        {
            LdapConnection connection;
            if (connectionAlreadyOpen == null)
            {
                connection = createConnection();
            }
            else
            {
                connection = connectionAlreadyOpen;
            }

            LdapSearchResults lsc = (LdapSearchResults)connection.Search(ADSettingsCache.Ad.RootOu, LdapConnection.ScopeSub, "(&(objectClass=group)(member=" + userDn + "))", null, false);
            List<string> groupsname = new List<string>();

            while (lsc.HasMore())
            {
                LdapEntry nextEntry = null;
                try
                {
                    nextEntry = lsc.Next();
                    groupsname.Add(nextEntry.Dn);

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

            if (connectionAlreadyOpen == null)
            {
                connection.Disconnect();
            }

            return groupsname;
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
            else
            {
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
                    u.ObjectGUID = getObjectGUID(nextEntry);
                    users.Add(u);

                }
                catch (LdapException e)
                {
                    connection.Disconnect();
                    Log.Warning(e, DataFetchErrorLogTemplate, "GetMembersOfGroup()");
                    continue;
                }
                catch (Exception e)
                {
                    Log.Warning(e, GenericErrorLogTemplate, "GetMembersOfGroup()");
                    connection.Disconnect();
                }
            }

            if (connectionAlreadyOpen == null)
            {
                connection.Disconnect();
            }

            return users;
        }

        public bool addMemberToGroup(string groupDn, List<UserAD> users)
        {
            LdapConnection connection = createConnection();
            try
            {
                string usersAdded = "";

                List<LdapModification> modList = new List<LdapModification>();

                foreach (UserAD u in users)
                {

                    if (u != null)
                    {
                        usersAdded += u.FirstName + " " + u.LastName + ", ";
                    }
                    LdapAttribute attribute = new LdapAttribute("member", u.Dn);
                    modList.Add(new LdapModification(LdapModification.Add, attribute));
                }

                LdapModification[] mods = new LdapModification[modList.Count];
                mods = modList.ToArray();

                //Modify the entry in the directory
                connection.Modify(groupDn, mods);

                connection.Disconnect();

                EmailHelper.SendEmail(NotificationType.MemberAdd, this.getGroupInformation(groupDn), usersAdded);
                return true;
            }
            catch (Exception e)
            {
                Log.Warning(e, GenericErrorLogTemplate, "addMemberToGroup()");
                connection.Disconnect();
                return false;
            }
        }

        public bool deleteMemberToGroup(string groupDn, List<UserAD> users)
        {
            LdapConnection connection = createConnection();
            try
            {
                List<LdapModification> modList = new List<LdapModification>();
                string usersDeleted = "";
                foreach (UserAD u in users)
                {

                    if (u != null)
                    {
                        usersDeleted += u.FirstName + " " + u.LastName + ", ";
                    }

                    LdapAttribute attribute = new LdapAttribute("member", u.Dn);
                    modList.Add(new LdapModification(LdapModification.Delete, attribute));
                }

                LdapModification[] mods = new LdapModification[modList.Count];
                mods = modList.ToArray();

                //Modify the entry in the directory
                connection.Modify(groupDn, mods);

                connection.Disconnect();
                EmailHelper.SendEmail(NotificationType.MemberRemoval, this.getGroupInformation(groupDn), usersDeleted);
                return true;
            }
            catch (Exception e)
            {
                Log.Warning(e, GenericErrorLogTemplate, "deleteMemberToGroup()");
                connection.Disconnect();
                return false;
            }
        }


    }
}
