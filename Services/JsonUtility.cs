using System;
using System.Globalization;
using System.Collections.Generic;
using System.Web;

namespace RentIt.Services
{
    /// <summary>
    /// A small utility to handle data transmitted by JSON via the REST API
    /// </summary>
    public static class JsonUtility
    {

        private static readonly CultureInfo provider = CultureInfo.InvariantCulture;

        #region Date/time handling

        private const string dateTimeFormat = "yyyy-MM-dd HH:mm:ss zzz";

        /// <summary>
        /// Converts a DateTime date into the string date/time format specified by the REST API
        /// </summary>
        /// <param name="date">The date/time value to convert</param>
        /// <returns>The date/time value formatted as the REST API dictates</returns>
        public static string DateTimeToString(DateTime date){
        
            return date.ToString(dateTimeFormat);
        }

        /// <summary>
        /// Converts a string in the date/time format specified by the REST API into a DateTime object
        /// </summary>
        /// <param name="str">The string formatted as a date/time string, which should be parsed</param>
        /// <returns>The DateTime object representing the passed date/time string</returns>
        /// <exception cref="FormatException">If the passed string does not match the REST API format</exception>
        public static DateTime StringToDateTime(string str){
        
            return DateTime.ParseExact(str, dateTimeFormat, provider);
        }

        #endregion
        
        #region Date handling

        private const string DateFormat = "yyyy-MM-dd";

        /// <summary>
        /// Converts a DateTime date into the string date format specified by the REST API
        /// </summary>
        /// <param name="date">The date value to convert</param>
        /// <returns>The date value formatted as the REST API dictates</returns>
        public static string DateToString(DateTime date){
        
            return date.ToString(DateFormat);
        }

        /// <summary>
        /// Converts a string in the date format specified by the REST API into a DateTime object
        /// </summary>
        /// <param name="str">The string formatted as a date string, which should be parsed</param>
        /// <returns>The DateTime object representing the passed date string</returns>
        /// <exception cref="FormatException">If the passed string does not match the REST API format</exception>
        public static DateTime StringToDate(string str){
        
            return DateTime.ParseExact(str, DateFormat, provider);
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