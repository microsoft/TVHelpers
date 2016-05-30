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
using System.Linq;
using System.Reflection;

namespace MediaAppSample.Core
{
    /// <summary>
    /// TypeUtility tests types to see if they are primitive types.
    /// </summary>
    public static class TypeUtility
    {
        readonly static Type[] Primitives;
        readonly static Type[] NullablePrimitives;
        readonly static Type[] AllPrimitives;

        static TypeUtility()
        {
            Primitives = new Type[]
            {
                typeof (Enum),
                typeof (String),
                typeof (Char),
                typeof (Guid),

                typeof (Boolean),
                typeof (Byte),
                typeof (Int16),
                typeof (Int32),
                typeof (Int64),
                typeof (Single),
                typeof (Double),
                typeof (Decimal),

                typeof (SByte),
                typeof (UInt16),
                typeof (UInt32),
                typeof (UInt64),

                //typeof (DateTime),
                //typeof (DateTimeOffset),
                //typeof (TimeSpan),
            };

            NullablePrimitives = (from t in Primitives
                                  where t.GetTypeInfo().IsValueType
                                  select typeof(Nullable<>).MakeGenericType(t)).ToArray();

            AllPrimitives = Primitives.Concat(NullablePrimitives).ToArray();
        }

        /// <summary>
        /// Tests a type to see if its a primitive type.
        /// </summary>
        /// <param name="type">Type to check if primitive.</param>
        /// <returns>True if the specified type is a primitive else false.</returns>
        public static bool IsPrimitive(Type type)
        {
            if (AllPrimitives.Any(x => x.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo())))
                return true;

            var nullable = Nullable.GetUnderlyingType(type);
            return nullable != null && nullable.GetTypeInfo().IsEnum;
        }
    }
}
