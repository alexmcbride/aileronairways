using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Net;

namespace Echelon.TimelineApi.Tests
{
    [TestClass]
    public class TimelineServiceTests
    {
        private const string BaseUrl = "http://baseurl.com/";

        [TestMethod]
        public void TestGetJson()
        {
            // We need it in the same deserialised state as the returned json object, otherwise it all goes wrong.
            string json = JsonConvert.DeserializeObject("{\"Test\": \"Result\"}").ToString();

            var mock = new Mock<IWebClientHelper>();
            mock.Setup(m => m.DownloadString(It.IsAny<string>(), It.IsAny<NameValueCollection>())).Returns(json);

            ITimelineService api = new TimelineService(mock.Object, BaseUrl, "ABC", "123");

            string result = api.GetJson("Test/Get");

            Assert.AreEqual(json, result);
        }

        [TestMethod]
        public void TestPutJson()
        {
            // We need it in the same deserialised state as the returned json object, otherwise it all goes wrong.
            string json = JsonConvert.DeserializeObject("{\"Test\": \"Result\"}").ToString();

            var mock = new Mock<IWebClientHelper>();
            mock.Setup(m => m.UploadString($"{BaseUrl}Test/Put", It.IsAny<string>())).Returns(json);

            ITimelineService api = new TimelineService(mock.Object, BaseUrl, "ABC", "123");

            string result = api.PutJson("Test/Put", new
            {
                Test = "Result"
            });

            Assert.AreEqual(json, result);
        }

        [TestMethod]
        [ExpectedException(typeof(TimelineException))]
        public void TestGet400Error()
        {
            var mock = new Mock<IWebClientHelper>();
            mock.Setup(m => m.GetStatusCode(It.IsAny<WebResponse>())).Returns(HttpStatusCode.BadRequest);
            mock.Setup(m => m.GetResponseMessage(It.IsAny<WebResponse>())).Returns("Bad request error");
            mock.Setup(m => m.DownloadString(It.IsAny<string>(), It.IsAny<NameValueCollection>())).Throws(new WebException("Hello"));

            ITimelineService api = new TimelineService(mock.Object, BaseUrl, "ABC", "123");

            api.GetJson("Test/Get");
        }

        [TestMethod]
        [ExpectedException(typeof(TimelineException))]
        public void TestGet500Error()
        {
            var mock = new Mock<IWebClientHelper>();
            mock.Setup(m => m.GetStatusCode(It.IsAny<WebResponse>())).Returns(HttpStatusCode.InternalServerError);
            mock.Setup(m => m.GetResponseMessage(It.IsAny<WebResponse>())).Returns("Internal server error");
            mock.Setup(m => m.DownloadString(It.IsAny<string>(), It.IsAny<NameValueCollection>())).Throws(new WebException("Hello"));

            ITimelineService api = new TimelineService(mock.Object, BaseUrl, "ABC", "123");

            api.GetJson("Test/Get");
        }
    }
}
