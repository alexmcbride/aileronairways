using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Echelon.TimelineApi.Tests
{
    [TestClass]
    public class TimelineServiceTests
    {
        private const string BaseUrl = "http://baseurl.com/";

        [TestMethod]
        public async Task GetJson()
        {
            string json = "{\"Test\": \"Result\"}";
            var mock = new Mock<IWebClientHelper>();
            mock.Setup(m => m.DownloadStringAsync(It.IsAny<string>(), It.IsAny<NameValueCollection>())).Returns(TestUtils.GetCompletedTask(json));

            var api = new TimelineService(BaseUrl, "ABC", "123", mock.Object);
            string result = await api.GetJsonAsync("Test/Get");

            mock.Verify(m => m.DownloadStringAsync($"{BaseUrl}Test/Get", It.Is<NameValueCollection>(c => c.VerifyContains("AuthToken", "ABC"))));
            mock.Verify(m => m.DownloadStringAsync($"{BaseUrl}Test/Get", It.Is<NameValueCollection>(c => c.VerifyContains("TenantId", "123"))));
            Assert.AreEqual(json, result);
        }

        [TestMethod]
        public async Task PutJson()
        {
            string json = "{\"Test\": \"Result\"}";
            var mock = new Mock<IWebClientHelper>();
            mock.Setup(m => m.UploadStringAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(TestUtils.GetCompletedTask(json));

            var api = new TimelineService(BaseUrl, "ABC", "123", mock.Object);
            string result = await api.PutJsonAsync("Test/Put", new
            {
                Test = "Result"
            });

            mock.Verify(m => m.UploadStringAsync($"{BaseUrl}Test/Put", It.Is<string>(s => s.VerifyJson("AuthToken", "ABC"))));
            mock.Verify(m => m.UploadStringAsync($"{BaseUrl}Test/Put", It.Is<string>(s => s.VerifyJson("TenantId", "123"))));
            Assert.AreEqual(json, result);
        }

        [TestMethod, ExpectedException(typeof(TimelineException))]
        public async Task GetJson400Error()
        {
            var mock = new Mock<IWebClientHelper>();
            mock.Setup(m => m.GetStatusCode(It.IsAny<WebResponse>())).Returns(HttpStatusCode.BadRequest);
            mock.Setup(m => m.GetResponseMessage(It.IsAny<WebResponse>())).Returns("Bad request error");
            mock.Setup(m => m.DownloadStringAsync(It.IsAny<string>(), It.IsAny<NameValueCollection>())).Returns(TestUtils.GetExceptionTask<string>(new WebException("Hello")));

            var api = new TimelineService(BaseUrl, "ABC", "123", mock.Object);
            await api.GetJsonAsync("Test/Get");
        }

        [TestMethod, ExpectedException(typeof(TimelineException))]
        public async Task GetJson500Error()
        {
            var mock = new Mock<IWebClientHelper>();
            mock.Setup(m => m.GetStatusCode(It.IsAny<WebResponse>())).Returns(HttpStatusCode.InternalServerError);
            mock.Setup(m => m.GetResponseMessage(It.IsAny<WebResponse>())).Returns("Internal server error");
            mock.Setup(m => m.DownloadStringAsync(It.IsAny<string>(), It.IsAny<NameValueCollection>())).Returns(TestUtils.GetExceptionTask<string>(new WebException("Hello")));

            var api = new TimelineService(BaseUrl, "ABC", "123", mock.Object);
            await api.GetJsonAsync("Test/Get");
        }


        [TestMethod, ExpectedException(typeof(TimelineException))]
        public async Task PutJson400Error()
        {
            var mock = new Mock<IWebClientHelper>();
            mock.Setup(m => m.UploadStringAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(TestUtils.GetExceptionTask<string>(new WebException()));
            mock.Setup(m => m.GetStatusCode(It.IsAny<WebResponse>())).Returns(HttpStatusCode.BadRequest);
            mock.Setup(m => m.GetResponseMessage(It.IsAny<WebResponse>())).Returns("Bad request error");

            var api = new TimelineService(BaseUrl, "ABC", "123", mock.Object);
            string result = await api.PutJsonAsync("Test/Put", new
            {
                Test = "Result"
            });
        }

        [TestMethod, ExpectedException(typeof(TimelineException))]
        public async Task PutJson500Error()
        {
            var mock = new Mock<IWebClientHelper>();
            mock.Setup(m => m.UploadStringAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(TestUtils.GetExceptionTask<string>(new WebException()));
            mock.Setup(m => m.GetStatusCode(It.IsAny<WebResponse>())).Returns(HttpStatusCode.InternalServerError);
            mock.Setup(m => m.GetResponseMessage(It.IsAny<WebResponse>())).Returns("Internal server error");

            var api = new TimelineService(BaseUrl, "ABC", "123", mock.Object);
            string result = await api.PutJsonAsync("Test/Put", new
            {
                Test = "Result"
            });
        }

        [TestMethod]
        public async Task UploadFile()
        {
            var fileBuffer = TestUtils.CreateRandomByteRange(40000).ToArray();

            var requestBuffer = new byte[fileBuffer.Length];
            var fileStream = new MemoryStream(fileBuffer);
            var requestStream = new MemoryStream();
            var mock = new Mock<IWebClientHelper>();
            mock.Setup(m => m.GetRequestStreamAsync(It.IsAny<string>())).Returns(TestUtils.GetCompletedTask<Stream>(requestStream));
            var ts = new TimelineService(BaseUrl, "ABC", "123", mock.Object);

            await ts.UploadFileAsync("http://www.upload.com/url", fileStream);

            // Move stream back to start.
            requestStream.Seek(0, SeekOrigin.Begin);

            int result = requestStream.Read(requestBuffer, 0, fileBuffer.Length);
            TestUtils.AssertAreEqual(fileBuffer, requestBuffer);
            mock.Verify(m => m.DisposeRequestStream(requestStream));
        }
    }
}
