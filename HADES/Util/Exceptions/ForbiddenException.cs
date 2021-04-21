using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HADES.Util.Exceptions
{
    // Thrown if the user should not have access to this page.
    // When catching this exception you should redirect to 403 Forbidden => Account/AccessDenied.cshtml
    public class ForbiddenException : Exception { }
}
