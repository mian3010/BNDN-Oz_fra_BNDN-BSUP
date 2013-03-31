using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace RentIt
{
    public class Utility
    {
        public static String GetTeamPath(String serverPath)
        {
            try
            {
                using (StreamReader sr = new StreamReader(serverPath + "team.txt"))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}