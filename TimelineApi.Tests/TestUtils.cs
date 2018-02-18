using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;

namespace Echelon.TimelineApi.Tests
{
    public static class TestUtils
    {
        /// <summary>
        /// Uses reflection to verify that a particular property appears in an object.
        /// </summary>
        /// <param name="obj">The object to test.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <returns>True if it exists.</returns>
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

        /// <summary>
        /// Checks if the name/value pair are present in the collection.
        /// </summary>
        /// <param name="c">The collection to check.</param>
        /// <param name="name">The name to check for.</param>
        /// <param name="value">The value to check for.</param>
        /// <returns>True if it exists.</returns>
        public static bool Contains(this NameValueCollection c, string name, string value)
        {
            return c.Get(name) == value;
        }

        public static bool VerifyJson(this string s, string name, string value)
        {
            JObject o = JObject.Parse(s);
            return o.GetValue(name).ToString() == value;
        }

        /// <summary>
        /// Gets a completed task with the specified result.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="result">The result.</param>
        /// <returns>A completed task.</returns>
        public static Task<T> GetCompletedTask<T>(T result)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetResult(result);
            return tcs.Task;
        }

        /// <summary>
        /// Gets a task that throws a particular exception.
        /// </summary>
        /// <typeparam name="TResult">The result the task expects.</typeparam>
        /// <param name="exception">The exception object to throw.</param>
        /// <returns>The dodgy task that causes an error.</returns>
        public static Task<TResult> GetExceptionTask<TResult>(WebException exception)
        {
            var tcs = new TaskCompletionSource<TResult>();
            tcs.SetException(exception);
            return tcs.Task;
        }
    }
}
