using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;

namespace Echelon.TimelineApi.Tests
{
    public static class TestUtils
    {
        public static bool VerifyObject(this object obj, string name, object value)
        {
            var properties = obj.GetType().GetProperties();
            foreach (var prop in properties)
            {
                if (prop.Name == name)
                {
                    return prop.GetValue(obj) == value;
                }
            }
            return false;
        }

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

        public static bool VerifyObject(this object obj, string name, DateTime value)
        {
            var properties = obj.GetType().GetProperties();
            foreach (var prop in properties)
            {
                if (prop.Name == name && prop.GetValue(obj) is DateTime dt)
                {
                    return dt == value;
                }
            }
            return false;
        }

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

        public static bool VerifyContains(this NameValueCollection c, string name, string value)
        {
            return c.Get(name) == value;
        }

        public static bool VerifyJson(this string s, string name, string value)
        {
            JObject o = JObject.Parse(s);
            return o.GetValue(name).ToString() == value;
        }

        public static Task<T> GetCompletedTask<T>(T result)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetResult(result);
            return tcs.Task;
        }

        public static Task<TResult> GetExceptionTask<TResult>(WebException exception)
        {
            var tcs = new TaskCompletionSource<TResult>();
            tcs.SetException(exception);
            return tcs.Task;
        }
    }
}
