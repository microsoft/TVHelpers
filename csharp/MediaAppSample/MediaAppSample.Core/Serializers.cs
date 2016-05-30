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
using System.IO;
using System.Runtime.Serialization;

namespace MediaAppSample.Core
{
    #region Enums

    /// <summary>
    /// Serialization methods the application supports.
    /// </summary>
    public enum SerializerTypes
    {
        /// <summary>
        /// Use the default serializer for the application.
        /// </summary>
        Default,

        /// <summary>
        /// Use the JSON serializer.
        /// </summary>
        Json,

        /// <summary>
        /// Use the XML serializer.
        /// </summary>
        Xml
    }

    #endregion

    /// <summary>
    /// Serialize and deserialize objects.
    /// </summary>
    public static class Serializer
    {
        #region Properties

        private static SerializerTypes _defaultSerializerType = SerializerTypes.Json;

        /// <summary>
        /// Get or set the default serialization method for the application. If an object that needs serialization/deserialization does not explictly
        /// request a particular serializer, the default serializer will be used.
        /// </summary>
        public static SerializerTypes DefaultSerializerType
        {
            get { return _defaultSerializerType; }
            set
            {
                if (value == SerializerTypes.Default)
                    throw new System.ArgumentOutOfRangeException("Default serialization type cannot be SerializerTypes.Default");
                _defaultSerializerType = value;
            }
        }

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Serialize an object to string.
        /// </summary>
        /// <param name="obj">Object to serialize.</param>
        /// <param name="serializerType">Use the default serializer or specific a particular serializer.</param>
        /// <returns>String returned from the serializer.</returns>
        public static string Serialize(object obj, SerializerTypes serializerType = SerializerTypes.Default)
        {
            if (serializerType == SerializerTypes.Default)
                serializerType = DefaultSerializerType;

            switch (serializerType)
            {
                case SerializerTypes.Xml:
                    return SerializeToXml(obj);
                case SerializerTypes.Json:
                    return SerializeToJson(obj);
                default:
                    throw new NotImplementedException("Unrecognized file type " + serializerType);
            }
        }

        /// <summary>
        /// Deserializes a string back into an instance of an object.
        /// </summary>
        /// <typeparam name="T">Type of the object to be returned.</typeparam>
        /// <param name="data">String data to be deserialized.</param>
        /// <param name="serializerType">Use the default serializer or specific a particular serializer.</param>
        /// <returns>Object instance from the deserialized data string.</returns>
        public static T Deserialize<T>(string data, SerializerTypes serializerType = SerializerTypes.Default)
        {
            if (serializerType == SerializerTypes.Default)
                serializerType = DefaultSerializerType;

            switch (serializerType)
            {
                case SerializerTypes.Xml:
                    return DeserializeFromXml<T>(data);
                case SerializerTypes.Json:
                    return DeserializeFromJson<T>(data);
                default:
                    throw new NotSupportedException("Unrecognized file type " + serializerType);
            }
        }

        #endregion Public Methods

        #region XML Serialization

        /// <summary>
        /// Deserializes an XML string back into an object instance.
        /// </summary>
        /// <typeparam name="T">Type of the object to be returned.</typeparam>
        /// <param name="xml">XML string to be deserialized.</param>
        /// <returns>Object instance deserialized from the string data.</returns>
        private static T DeserializeFromXml<T>(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
                return default(T);

            using (Stream stream = new MemoryStream())
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(xml);
                stream.Write(data, 0, data.Length);
                stream.Position = 0;
                DataContractSerializer deserializer = new DataContractSerializer(typeof(T));
                return (T)deserializer.ReadObject(stream);
            }
        }

        /// <summary>
        /// Serializes an object to XML.
        /// </summary>
        /// <param name="obj">Object to be serialized.</param>
        /// <returns>String representing the serialized object.</returns>
        private static string SerializeToXml(object obj)
        {
            if (obj == null)
                return null;

            MemoryStream stream = new MemoryStream();
            DataContractSerializer serializer = new DataContractSerializer(obj.GetType());
            serializer.WriteObject(stream, obj);
            stream.Position = 0;
            string result = null;
            using (StreamReader sr = new StreamReader(stream))
            {
                result = sr.ReadToEnd();
            }
            return result;
        }

        #endregion XML Serialization

        #region JSON Serialization

        /// <summary>
        /// Deserializes a JSON string into an object instance.
        /// </summary>
        /// <typeparam name="T">Type of the object to be returned.</typeparam>
        /// <param name="json">JSON string representing an object.</param>
        /// <returns>Instance of an object from the JSON string.</returns>
        private static T DeserializeFromJson<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return default(T);
            
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Serializes an object into a JSON string.
        /// </summary>
        /// <param name="obj">Object instance to serialize.</param>
        /// <returns>JSON string representing the object instance.</returns>
        private static string SerializeToJson(object obj)
        {
            if (obj == null)
                return null;

            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

        #endregion JSON Serialization

        #endregion
    }
}