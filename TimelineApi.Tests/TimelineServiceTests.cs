using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
            string json = "{\"Test\": \"Result\"}";
            var mock = new Mock<IWebClientHelper>();
            mock.Setup(m => m.DownloadStringAsync(It.IsAny<string>(), It.IsAny<NameValueCollection>())).Returns(TestUtils.GetCompletedTask(json));

            ITimelineService api = new TimelineService(BaseUrl, "ABC", "123", mock.Object);
            string result = await api.GetJsonAsync("Test/Get");

            mock.Verify(m => m.DownloadStringAsync($"{BaseUrl}Test/Get", It.Is<NameValueCollection>(c => c.VerifyContains("AuthToken", "ABC"))));
            mock.Verify(m => m.DownloadStringAsync($"{BaseUrl}Test/Get", It.Is<NameValueCollection>(c => c.VerifyContains("TenantId", "123"))));
            Assert.AreEqual(json, result);
        }

        [TestMethod]
        public async Task TestPutJson()
        {
            string json = "{\"Test\": \"Result\"}";
            var mock = new Mock<IWebClientHelper>();
            mock.Setup(m => m.UploadStringAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(TestUtils.GetCompletedTask(json));

            ITimelineService api = new TimelineService(BaseUrl, "ABC", "123", mock.Object);
            string result = await api.PutJsonAsync("Test/Put", new
            {
                Test = "Result"
            });

            mock.Verify(m => m.UploadStringAsync($"{BaseUrl}Test/Put", It.Is<string>(s => s.VerifyJson("AuthToken", "ABC"))));
            mock.Verify(m => m.UploadStringAsync($"{BaseUrl}Test/Put", It.Is<string>(s => s.VerifyJson("TenantId", "123"))));
            Assert.AreEqual(json, result);
        }

        [TestMethod, ExpectedException(typeof(TimelineException))]
        public async Task HandlesGetJson400Error()
        {
            var mock = new Mock<IWebClientHelper>();
            mock.Setup(m => m.GetStatusCode(It.IsAny<WebResponse>())).Returns(HttpStatusCode.BadRequest);
            mock.Setup(m => m.GetResponseMessage(It.IsAny<WebResponse>())).Returns("Bad request error");
            mock.Setup(m => m.DownloadStringAsync(It.IsAny<string>(), It.IsAny<NameValueCollection>())).Returns(TestUtils.GetExceptionTask<string>(new WebException("Hello")));

            ITimelineService api = new TimelineService(BaseUrl, "ABC", "123", mock.Object);
            await api.GetJsonAsync("Test/Get");
        }

        [TestMethod, ExpectedException(typeof(TimelineException))]
        public async Task HandlesGetJson500Error()
        {
            var mock = new Mock<IWebClientHelper>();
            mock.Setup(m => m.GetStatusCode(It.IsAny<WebResponse>())).Returns(HttpStatusCode.InternalServerError);
            mock.Setup(m => m.GetResponseMessage(It.IsAny<WebResponse>())).Returns("Internal server error");
            mock.Setup(m => m.DownloadStringAsync(It.IsAny<string>(), It.IsAny<NameValueCollection>())).Returns(TestUtils.GetExceptionTask<string>(new WebException("Hello")));

            ITimelineService api = new TimelineService(BaseUrl, "ABC", "123", mock.Object);
            await api.GetJsonAsync("Test/Get");
        }


        [TestMethod, ExpectedException(typeof(TimelineException))]
        public async Task HandlesPutJson400Error()
        {
            var mock = new Mock<IWebClientHelper>();
            mock.Setup(m => m.UploadStringAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(TestUtils.GetExceptionTask<string>(new WebException()));
            mock.Setup(m => m.GetStatusCode(It.IsAny<WebResponse>())).Returns(HttpStatusCode.BadRequest);
            mock.Setup(m => m.GetResponseMessage(It.IsAny<WebResponse>())).Returns("Bad request error");

            ITimelineService api = new TimelineService(BaseUrl, "ABC", "123", mock.Object);
            string result = await api.PutJsonAsync("Test/Put", new
            {
                Test = "Result"
            });
        }

        [TestMethod, ExpectedException(typeof(TimelineException))]
        public async Task HandlesPutJson500Error()
        {
            var mock = new Mock<IWebClientHelper>();
            mock.Setup(m => m.UploadStringAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(TestUtils.GetExceptionTask<string>(new WebException()));
            mock.Setup(m => m.GetStatusCode(It.IsAny<WebResponse>())).Returns(HttpStatusCode.InternalServerError);
            mock.Setup(m => m.GetResponseMessage(It.IsAny<WebResponse>())).Returns("Internal server error");

            ITimelineService api = new TimelineService(BaseUrl, "ABC", "123", mock.Object);
            string result = await api.PutJsonAsync("Test/Put", new
            {
                Test = "Result"
            });
        }
    }
}
