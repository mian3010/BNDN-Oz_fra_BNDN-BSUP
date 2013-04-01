using System;
using System.IO;

namespace RestClient
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