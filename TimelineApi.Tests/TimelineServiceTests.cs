using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System.Collections.Specialized;

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
    }
}
