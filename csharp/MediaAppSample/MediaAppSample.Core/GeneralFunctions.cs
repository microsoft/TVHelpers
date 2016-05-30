// The MIT License (MIT)
//
// Copyright (c) 2016 Microsoft. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Globalization;
using Windows.Foundation;
using Windows.UI;

namespace MediaAppSample.Core
{
    public static partial class GeneralFunctions
    {
        /// <summary>
        /// Converts a string hex color value to a Windows.UI.Color object instance.
        /// </summary>
        /// <param name="hexColor">Hex value of the color.</param>
        /// <returns>Windows.UI.Color instance for the specified hex color value.</returns>
        public static Color HexToColor(string hexColor)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hexColor))
                    return Colors.Transparent;

                // Remove # if present
                if (hexColor.IndexOf('#') != -1)
                    hexColor = hexColor.Replace("#", "");

                hexColor = hexColor.ToLower();

                int red = 0;
                int green = 0;
                int blue = 0;

                if (hexColor.Length == 6)
                {
                    // #RRGGBB
                    red = int.Parse(hexColor.Substring(0, 2), NumberStyles.AllowHexSpecifier, System.Globalization.CultureInfo.InvariantCulture);
                    green = int.Parse(hexColor.Substring(2, 2), NumberStyles.AllowHexSpecifier, System.Globalization.CultureInfo.InvariantCulture);
                    blue = int.Parse(hexColor.Substring(4, 2), NumberStyles.AllowHexSpecifier, System.Globalization.CultureInfo.InvariantCulture);
                }
                else if (hexColor.Length == 3)
                {
                    // #RGB
                    red = int.Parse(hexColor[0].ToString() + hexColor[0].ToString(), NumberStyles.AllowHexSpecifier, System.Globalization.CultureInfo.InvariantCulture);
                    green = int.Parse(hexColor[1].ToString() + hexColor[1].ToString(), NumberStyles.AllowHexSpecifier, System.Globalization.CultureInfo.InvariantCulture);
                    blue = int.Parse(hexColor[2].ToString() + hexColor[2].ToString(), NumberStyles.AllowHexSpecifier, System.Globalization.CultureInfo.InvariantCulture);
                }

                return Color.FromArgb(255, (byte)red, (byte)green, (byte)blue);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("Could not convert hex value '{0}' to a Color.", hexColor), ex);
            }
        }

        /// <summary>
        /// Parses a query string into a dictionary of key/value pairs of each parameter in the string.
        /// </summary>
        /// <param name="querystring">Query string to parse.</param>
        /// <returns>Dictionary of key value pairs found within the query string.</returns>
        public static IDictionary<string, string> ParseQuerystring(string querystring)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(querystring))
            {
                WwwFormUrlDecoder decoder = new WwwFormUrlDecoder(querystring);
                foreach (var entry in decoder)
                    dic.Add(entry.Name, entry.Value);
            }
            return dic;
        }

        /// <summary>
        /// Converts a dictionary of key/value pairs into a query string.
        /// </summary>
        /// <param name="parameters">Key/value pairs of parameters.</param>
        /// <returns>Query string from all the key/value pair data supplied in the dictionary.</returns>
        public static string CreateQuerystring(IDictionary<string, string> parameters)
        {
            if (parameters == null || parameters.Count == 0)
                return null;
            else
            {
                var contents = new Windows.Web.Http.HttpFormUrlEncodedContent(parameters);
                return contents.ReadAsStringAsync().AsTask().Result;
            }
        }
    }
}
