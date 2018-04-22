using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;

namespace AileronAirwaysWebTests
{
    /// <summary>
    /// Utils used by unit tests
    /// </summary>
    public static class TestUtils
    {
        /// <summary>
        /// Verifies is the specified property exists in the object with the specified value.
        /// </summary>
        public static bool VerifyObject(this object obj, string name, object value)
        {
            var properties = obj.GetType().GetProperties();
            foreach (var prop in properties)
            {
                if (prop.Name == name)
                {
                    object o = prop.GetValue(obj);
                    // Need to handle strings differently, or they don't match.
                    if (o is string str)
                    {
                        return str == (string)value;
                    }
                    return o == value;
                }
            }
            return false;
        }

        /// <summary>
        /// Verifies if the specified property exists in the object with the specified boolean value.
        /// </summary>
        public static bool VerifyObject(this object obj, string name, bool value)
        {
            var properties = obj.GetType().GetProperties();
            foreach (var prop in properties)
            {
                if (prop.Name == name && prop.GetValue(obj) is bool eh)
                {
                    return eh == value;
                }
            }
            return false;
        }

        /// <summary>
        /// Verifies if the specified property on an object contains a GUID.
        /// </summary>
        public static bool VerifyIsGuid(this object obj, string name)
        {
            var properties = obj.GetType().GetProperties();
            foreach (var prop in properties)
            {
                if (prop.Name == name && prop.GetValue(obj) is string value)
                {
                    return Guid.TryParse(value, out Guid result);
                }
            }
            return false;
        }

        /// <summary>
        /// Verifies if the specified NameValueCollection contains the specified name and value pair.
        /// </summary>
        public static bool VerifyContains(this NameValueCollection c, string name, string value)
        {
            return c.Get(name) == value;
        }

        /// <summary>
        /// Verifies if the name contains the same JSON as the value.
        /// </summary>
        public static bool VerifyJson(this string s, string name, string value)
        {
            JObject o = JObject.Parse(s);
            return o.GetValue(name).ToString() == value;
        }

        /// <summary>
        /// Gets a completed async task with the specified result.
        /// </summary>
        public static Task<T> GetCompletedTask<T>(T result)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetResult(result);
            return tcs.Task;
        }

        /// <summary>
        /// Gets an async task with the specified exception.
        /// </summary>
        public static Task<TResult> GetExceptionTask<TResult>(WebException exception)
        {
            var tcs = new TaskCompletionSource<TResult>();
            tcs.SetException(exception);
            return tcs.Task;
        }
    }
}
