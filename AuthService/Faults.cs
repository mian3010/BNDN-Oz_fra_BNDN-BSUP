using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;

namespace RentIt.AuthService
{
    /// <summary>
    /// Contains the various fault codes the RentIt services may raise along with fault exceptions
    /// </summary>
    public static class Faults
    {
        readonly public static FaultCode INVALID_CREDENTIALS = new FaultCode("INVALID_CREDENTIALS");
    }
}