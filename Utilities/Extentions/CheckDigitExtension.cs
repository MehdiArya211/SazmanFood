using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;


namespace Utilities.Extentions
{
    /// <summary>
    /// مدیریت ایجاد و بررسی چک دیجیت برای شماره بارنامه
    /// </summary>
    public static class CheckDigitExtension
    {
        static readonly int[] Results = { 0, 2, 4, 6, 8, 1, 3, 5, 7, 9 };

        /// <summary>
        /// چک دیجیت چند رقمی باشد؟
        /// </summary>
        public static readonly int NumberOfDigits = 2;
        public static readonly int CriticalNum = (int)Math.Pow(10, NumberOfDigits);




        #region extension methods for IList<int>

        ///// <summary>
        ///// For a list of digits, compute the ending checkdigit 
        ///// </summary>
        ///// <param name="digits">The list of digits for which to compute the check digit</param>
        ///// <returns>the check digit</returns>
        //public static int CheckDigit(this IList<int> digits)
        //{
        //    var i = 0;
        //    var lengthMod = digits.Count % 2;

        //    return (digits.Sum(d => i++ % 2 == lengthMod ? d : Results[d]) * 9) % CriticalNum;
        //}


        /// <summary>
        /// گرفتن چک دیجیت با تعداد دلخواه
        /// </summary>
        /// <param name="digits">عددی که میخواهیم برای آن چک دیجیت بگیریم به صورت لیستی از اعداد.</param>
        /// <param name="numberOfCheckDigit">تعداد چک دیجیت</param>
        /// <returns></returns>
        public static int CheckDigit(this IList<int> digits, int? numberOfCheckDigit = null)
        {
            var i = 0;
            var lengthMod = digits.Count % 2;
            int _CriticalNum = CriticalNum;
            if (numberOfCheckDigit != null)
                _CriticalNum = (int)Math.Pow(10, (int)numberOfCheckDigit);
            return (digits.Sum(d => i++ % 2 == lengthMod ? d : Results[d]) * 9) % _CriticalNum;
        }




        /// <summary>
        /// Return a list of digits including the checkdigit
        /// </summary>
        /// <param name="digits">The original list of digits</param>
        /// <returns>the new list of digits including checkdigit</returns>
        public static IList<int> AppendCheckDigit(this IList<int> digits, int? numberOfCheckDigit = null)
        {
            var result = digits;
            result.Add(digits.CheckDigit(numberOfCheckDigit));
            return result;
        }




        /// <summary>
        /// Returns true when a list of digits has a valid checkdigit
        /// </summary>
        /// <param name="digits">The list of digits to check</param>
        /// <returns>true/false depending on valid checkdigit</returns>
        public static bool HasValidCheckDigit(this IList<int> digits, int? numberOfCheckDigit = null)
        {
            if (numberOfCheckDigit == null)
                numberOfCheckDigit = NumberOfDigits;
            var LastNums = digits.Skip(Math.Max(0, digits.Count() - (int)numberOfCheckDigit));
            int checkdigit = 0;
            foreach (var item in LastNums)
                checkdigit = checkdigit * 10 + item;
            return checkdigit == CheckDigit(digits.Take(digits.Count - (int)numberOfCheckDigit).ToList(), numberOfCheckDigit);
        }

        #endregion extension methods for IList<int>




        #region extension methods for strings
        /// <summary>
        /// Internal conversion function to convert string into a list of ints
        /// </summary>
        /// <param name="digits">the original string</param>
        /// <returns>the list of ints</returns>
        private static IList<int> ToDigitList(this string digits)
        {
            return digits.Select(d => d - 48).ToList();
        }



        /// <summary>
        /// For a string of digits, compute the ending checkdigit 
        /// </summary>
        /// <param name="digits">The string of digits for which to compute the check digit</param>
        /// <returns>the check digit</returns>
        public static string CheckDigit(this string digits, int? numberOfCheckDigit = null)
        {
            return digits.ToDigitList().CheckDigit(numberOfCheckDigit).ToString(CultureInfo.InvariantCulture);
        }



        /// <summary>
        /// Return a string of digits including the checkdigit
        /// </summary>
        /// <param name="digits">The original string of digits</param>
        /// <returns>the new string of digits including checkdigit</returns>
        public static string AppendCheckDigit(this string digits, int? numberOfCheckDigit = null)
        {
            return digits + digits.CheckDigit(numberOfCheckDigit);
        }



        /// <summary>
        /// Returns true when a string of digits has a valid checkdigit
        /// </summary>
        /// <param name="digits">The string of digits to check</param>
        /// <returns>true/false depending on valid checkdigit</returns>
        public static bool HasValidCheckDigit(this string digits, int? numberOfCheckDigit = null)
        {
            var _list = digits.ToDigitList();
            return _list.HasValidCheckDigit(numberOfCheckDigit);
        }

        #endregion extension methods for strings






        #region extension methods for integers

        /// <summary>
        /// Internal conversion function to convert int into a list of ints, one for each digit
        /// </summary>
        /// <param name="digits">the original int</param>
        /// <returns>the list of ints</returns>
        private static IList<int> ToDigitList(this int digits)
        {
            return digits.ToString(CultureInfo.InvariantCulture).Select(d => d - 48).ToList();
        }



        /// <summary>
        /// For an integer, compute the ending checkdigit 
        /// </summary>
        /// <param name="digits">The integer for which to compute the check digit</param>
        /// <returns>the check digit</returns>
        public static int CheckDigit(this int digits, int? numberOfCheckDigit = null)
        {
            return digits.ToDigitList().CheckDigit(numberOfCheckDigit);
        }




        /// <summary>
        /// Return an integer including the checkdigit
        /// </summary>
        /// <param name="digits">The original integer</param>
        /// <returns>the new integer including checkdigit</returns>
        public static int AppendCheckDigit(this int digits, int? numberOfCheckDigit = null)
        {
            int _CriticalNum = CriticalNum;
            if (numberOfCheckDigit != null)
                _CriticalNum = (int)Math.Pow(10, (int)numberOfCheckDigit);
            return digits * _CriticalNum + digits.CheckDigit(numberOfCheckDigit);
        }



        /// <summary>
        /// Returns true when an integer has a valid checkdigit
        /// </summary>
        /// <param name="digits">The integer to check</param>
        /// <returns>true/false depending on valid checkdigit</returns>
        public static bool HasValidCheckDigit(this int digits, int? numberOfCheckDigit = null)
        {
            return digits.ToDigitList().HasValidCheckDigit(numberOfCheckDigit);
        }

        #endregion extension methods for integers




        #region extension methods for int64s

        /// <summary>
        /// Internal conversion function to convert int into a list of ints, one for each digit
        /// </summary>
        /// <param name="digits">the original int</param>
        /// <returns>the list of ints</returns>
        private static IList<int> ToDigitList(this Int64 digits)
        {
            return digits.ToString(CultureInfo.InvariantCulture).Select(d => d - 48).ToList();
        }



        /// <summary>
        /// For an integer, compute the ending checkdigit 
        /// </summary>
        /// <param name="digits">The integer for which to compute the check digit</param>
        /// <returns>the check digit</returns>
        public static int CheckDigit(this Int64 digits, int? numberOfCheckDigit = null)
        {
            return digits.ToDigitList().CheckDigit(numberOfCheckDigit);
        }

        /// <summary>
        /// Return an integer including the checkdigit
        /// </summary>
        /// <param name="digits">The original integer</param>
        /// <returns>the new integer including checkdigit</returns>
        public static Int64 AppendCheckDigit(this Int64 digits, int? numberOfCheckDigit = null)
        {
            int _CriticalNum = CriticalNum;
            if (numberOfCheckDigit != null)
                _CriticalNum = (int)Math.Pow(10, (int)numberOfCheckDigit);
            return digits * _CriticalNum + digits.CheckDigit(numberOfCheckDigit);
        }

        /// <summary>
        /// Returns true when an integer has a valid checkdigit
        /// </summary>
        /// <param name="digits">The integer to check</param>
        /// <returns>true/false depending on valid checkdigit</returns>
        public static bool HasValidCheckDigit(this Int64 digits, int? numberOfCheckDigit = null)
        {
            return digits.ToDigitList().HasValidCheckDigit(numberOfCheckDigit);
        }

        #endregion extension methods for int64s
    }
}
