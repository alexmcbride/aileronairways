using System;
using System.Net;
using System.Threading.Tasks;

namespace Echelon.TimelineApi.Tests
{
    public static class TestUtils
    {
        // Verifies that a particular property is present in an object.
        public static bool VerifyObject(object obj, string key, object value)
        {
            var properties = obj.GetType().GetProperties();
            foreach (var prop in properties)
            {
                if (prop.Name == key)
                {
                    return prop.GetValue(obj) == value;
                }
            }
            return false;
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
