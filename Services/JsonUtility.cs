using System;
using System.Globalization;
using System.Collections.Generic;
using System.Web;

namespace RentIt
{
    using AccountType = AccountTypes.AccountType;

    /// <summary>
    /// A small utility to handle data transmitted by JSON via the REST API
    /// </summary>
    public static class JsonUtility
    {

        private static CultureInfo provider = CultureInfo.InvariantCulture;

        #region Date/time handling

        private static string dateTimeFormat = "yyyy-MM-dd HH:mm:ss zzz";

        /// <summary>
        /// Converts a DateTime date into the string date/time format specified by the REST API
        /// </summary>
        /// <param name="date">The date/time value to convert</param>
        /// <returns>The date/time value formatted as the REST API dictates</returns>
        public static string dateTimeToString(DateTime date){
        
            return date.ToString(dateTimeFormat);
        }

        /// <summary>
        /// Converts a string in the date/time format specified by the REST API into a DateTime object
        /// </summary>
        /// <param name="str">The string formatted as a date/time string, which should be parsed</param>
        /// <returns>The DateTime object representing the passed date/time string</returns>
        /// <exception cref="FormatException">If the passed string does not match the REST API format</exception>
        public static DateTime stringToDateTime(string str){
        
            return DateTime.ParseExact(str, dateTimeFormat, provider);
        }

        #endregion
        
        #region Date handling

        private static string dateFormat = "yyyy-MM-dd";

        /// <summary>
        /// Converts a DateTime date into the string date format specified by the REST API
        /// </summary>
        /// <param name="date">The date value to convert</param>
        /// <returns>The date value formatted as the REST API dictates</returns>
        public static string dateToString(DateTime date){
        
            return date.ToString(dateFormat);
        }

        /// <summary>
        /// Converts a string in the date format specified by the REST API into a DateTime object
        /// </summary>
        /// <param name="str">The string formatted as a date string, which should be parsed</param>
        /// <returns>The DateTime object representing the passed date string</returns>
        /// <exception cref="FormatException">If the passed string does not match the REST API format</exception>
        public static DateTime stringToDate(string str){
        
            return DateTime.ParseExact(str, dateFormat, provider);
        }

        #endregion

        #region AccountType handling   MAYBE NOT NEEDED

        /// <summary>
        /// Converts an account type identifier into the string format used by the REST API
        /// </summary>
        /// <param name="accType">The account type to convert to string</param>
        /// <returns>The string representation of the account type according to the REST API</returns>
        public static string AccountTypeToString(AccountType accType){
        
            if(accType.Equals(AccountType.Admin))           return "Admin";
            if(accType.Equals(AccountType.ContentProvider)) return "Content Provider";
            if (accType.Equals(AccountType.Customer))       return "Customer";

            throw new NotImplementedException("No implementation is given for the type: " + accType);
        }

        /// <summary>
        /// Converts the string form of an account type identifier (from the REST API) into an actual type
        /// </summary>
        /// <param name="str">The string representation of an account type identifier</param>
        /// <returns>The actual account type identifier represented by the string</returns>
        public static AccountType StringToAccountType(string str){
        
            if("Admin".Equals(str))             return AccountType.Admin;
            if("Content Provider".Equals(str))  return AccountType.ContentProvider;
            if("Customer".Equals(str))          return AccountType.Customer;

            throw new NotImplementedException("No implementation is given for the type: " + str);
        }

        #endregion

        #region Other

        /// <summary>
        /// Validates an email address
        /// </summary>
        /// <param name="email">The email address to validate</param>
        /// <returns>True if the email address probably is valid, false if it by guarantee is not valid</returns>
        public static bool ValidateEmail(string email){
        
            throw new NotImplementedException("Yet to find a good implementation....");
        }

        #endregion
    }
}