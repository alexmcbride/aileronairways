using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;

namespace Echelon.TimelineApi.Tests
{
    [TestClass]
    public class TimelineServiceTests
    {
        private const string BaseUrl = "http://baseurl.com/";

        [TestMethod]
        public async Task TestGetJson()
        {
            // We need it in the same deserialised state as the returned json object, otherwise it all goes wrong.
            string json = JsonConvert.DeserializeObject("{\"Test\": \"Result\"}").ToString();

            var mock = new Mock<IWebClientHelper>();
            mock.Setup(m => m.DownloadStringAsync(It.IsAny<string>(), It.IsAny<NameValueCollection>())).Returns(TestUtils.GetCompletedTask(json));

            ITimelineService api = new TimelineService(mock.Object, BaseUrl, "ABC", "123");

            string result = await api.GetJsonAsync("Test/Get");

            Assert.AreEqual(json, result);
        }

        [TestMethod]
        public async Task TestPutJson()
        {
            // We need it in the same deserialised state as the returned json object, otherwise it all goes wrong.
            string json = JsonConvert.DeserializeObject("{\"Test\": \"Result\"}").ToString();

            var mock = new Mock<IWebClientHelper>();
            mock.Setup(m => m.UploadStringAsync($"{BaseUrl}Test/Put", It.IsAny<string>())).Returns(TestUtils.GetCompletedTask(json));

            ITimelineService api = new TimelineService(mock.Object, BaseUrl, "ABC", "123");

            string result = await api.PutJsonAsync("Test/Put", new
            {
                Test = "Result"
            });

            Assert.AreEqual(json, result);
        }

        [TestMethod]
        [ExpectedException(typeof(TimelineException))]
        public async Task TestGet400Error()
        {
            var mock = new Mock<IWebClientHelper>();
            mock.Setup(m => m.GetStatusCode(It.IsAny<WebResponse>())).Returns(HttpStatusCode.BadRequest);
            mock.Setup(m => m.GetResponseMessage(It.IsAny<WebResponse>())).Returns("Bad request error");
            mock.Setup(m => m.DownloadStringAsync(It.IsAny<string>(), It.IsAny<NameValueCollection>())).Returns(TestUtils.GetExceptionTask<string>(new WebException("Hello")));

            ITimelineService api = new TimelineService(mock.Object, BaseUrl, "ABC", "123");

            await api.GetJsonAsync("Test/Get");
        }

        [TestMethod]
        [ExpectedException(typeof(TimelineException))]
        public async Task TestGet500Error()
        {
            var mock = new Mock<IWebClientHelper>();
            mock.Setup(m => m.GetStatusCode(It.IsAny<WebResponse>())).Returns(HttpStatusCode.InternalServerError);
            mock.Setup(m => m.GetResponseMessage(It.IsAny<WebResponse>())).Returns("Internal server error");
            mock.Setup(m => m.DownloadStringAsync(It.IsAny<string>(), It.IsAny<NameValueCollection>())).Returns(TestUtils.GetExceptionTask<string>(new WebException("Hello")));

            ITimelineService api = new TimelineService(mock.Object, BaseUrl, "ABC", "123");

            await api.GetJsonAsync("Test/Get");
        }
    }
}
