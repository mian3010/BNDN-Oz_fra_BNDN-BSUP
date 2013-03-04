using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

using F = RentIt.AuthService.Faults;

namespace RentIt.AuthService
{
    // Implementation of IAuthenticationServiceContract
    // See interface for documentation
    public class AuthenticationService : IAuthenticationServiceContract
    {

        public string Authenticate(string user, string password)
        {

            try
            {
                Tuple<string, string> tuple = Auth.authenticate(user, password);
                return tuple.Item1 + "\n" + tuple.Item2;
            }
            catch (Auth.AuthenticationFailed)
            {
                throw new FaultException("No account exists with the passed username/password pair.", F.INVALID_CREDENTIALS);
            }
        }
    }
}
