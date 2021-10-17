using System;

namespace Furball.Engine.Engine.Helpers {
    public static class StringExtensions {
        /// <summary>
        ///     takes a substring between two anchor strings (or the end of the string if that anchor is null)
        /// </summary>
        /// <param name="this">a string</param>
        /// <param name="from">an optional string to search after</param>
        /// <param name="until">an optional string to search before</param>
        /// <param name="comparison">an optional comparison for the search</param>
        /// <returns>a substring based on the search</returns>
        public static string Substring(this string @this, string from = null, string until = null, StringComparison comparison = StringComparison.InvariantCulture) {
            int fromLength = (from ?? string.Empty).Length;
            int startIndex = !string.IsNullOrEmpty(from) ? @this.IndexOf(from, comparison) + fromLength : 0;

            if (startIndex < fromLength)
                throw new ArgumentException("from: Failed to find an instance of the first anchor");

            int endIndex = !string.IsNullOrEmpty(until) ? @this.IndexOf(until, startIndex, comparison) : @this.Length;

            if (endIndex < 0)
                throw new ArgumentException("until: Failed to find an instance of the last anchor");

            string subString = @this.Substring(startIndex, endIndex - startIndex);
            return subString;
        }

        /// <summary>
        ///     takes a substring between two anchor strings (or the end of the string if that anchor is null)
        /// </summary>
        /// <param name="this">a string</param>
        /// <param name="from">an optional string to search after</param>
        /// <param name="until">an optional string to search before</param>
        /// <param name="comparison">an optional comparison for the search</param>
        /// <returns>a substring based on the search</returns>
        public static string SubstringWithEnds(this string @this, string from, string until, StringComparison comparison = StringComparison.InvariantCulture) {
            int fromLength = 0;
            int startIndex = !string.IsNullOrEmpty(from) ? @this.IndexOf(from, comparison) + fromLength : 0;

            if (startIndex < fromLength)
                throw new ArgumentException("from: Failed to find an instance of the first anchor");

            int endIndex = !string.IsNullOrEmpty(until) ? @this.IndexOf(until, startIndex, comparison) : @this.Length;

            if (endIndex < 0)
                throw new ArgumentException("until: Failed to find an instance of the last anchor");

            string subString = @this.Substring(startIndex, (endIndex - startIndex) + until.Length);
            return subString;
        }
    }
}
