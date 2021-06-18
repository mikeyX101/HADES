using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HADES.Extensions
{
    public static class LdapResponseQueueExtensions
    {
        public static void WaitForEmpty(this Novell.Directory.Ldap.LdapResponseQueue queue)
		{
            while (queue.MessageIDs.Length > 0)
            {
                queue.GetResponse();
            }
        }
    }
}
