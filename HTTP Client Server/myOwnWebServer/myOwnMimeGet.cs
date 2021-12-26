using System;
using System.Collections.Generic;
using System.Linq;

namespace myOwnWebServer
{
    class MyOwnMimeGet
    {

        public static Dictionary<string, string> MimeTypeDictionary = new Dictionary<string, string>
        {
            {".html", "text/html"},
            {".jpg", "image/jpeg"},
            {".txt", "text/plain"},
            {".gif", "image/gif"},
          
        };

        /// <summary>
        /// Returns the Dictionary entry that matches the Extension.
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        private static KeyValuePair<string, string> FindByExtension(string extension)
        {
            return MimeTypeDictionary.SingleOrDefault(oo => oo.Key.ToLowerInvariant() == extension.ToLowerInvariant());
        }

        /// <summary>
        /// Returns the MimeType that matches the Extension. If no match is found an error is thrown.
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static string GetMimeType(string extension)
        {
            var result = FindByExtension(extension);
            if (!string.IsNullOrWhiteSpace(result.Value))
                return result.Value;
            else
                throw new ApplicationException("Unknown Extension.");
        }

    }
}
