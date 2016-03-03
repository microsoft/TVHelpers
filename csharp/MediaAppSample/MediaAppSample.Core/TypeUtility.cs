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
