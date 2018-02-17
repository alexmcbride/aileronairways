using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Specialized;

namespace Echelon.TimelineApi.Tests
{
    [TestClass]
    public class TimelineTests
    {
        private const string TimelineJson = "{\"Id\": \"ID1\", \"Title\": \"Test Title\", \"CreationTimeStamp\": \"636544632390000000\", \"IsDeleted\": true, \"TenantId\": \"123\"}";
        private const string TimelinesJson = "[{\"Id\": \"ID1\", \"Title\": \"Test Title\", \"CreationTimeStamp\": \"636544632390000000\", \"IsDeleted\": true, \"TenantId\": \"123\"}," +
            "{\"Id\": \"ID2\", \"Title\": \"Test Title 2\", \"CreationTimeStamp\": \"636544632350000000\", \"IsDeleted\": true, \"TenantId\": \"123\"}]";

        [TestMethod]
        public void TestCreate()
        {
            var mock = new Mock<ITimelineService>();
            mock.Setup(m => m.PutJson("Timeline/Create", It.IsAny<object>())).Returns(TimelineJson);
            var timeline = Timeline.Create(mock.Object, "Test Title");

            Assert.AreEqual(timeline.Id, "ID1");
            Assert.AreEqual(timeline.Title, "Test Title");
            Assert.AreEqual(timeline.CreationTimeStamp.Ticks.ToString(), "636544632390000000");
            Assert.IsTrue(timeline.IsDeleted);
            Assert.AreEqual(timeline.TenantId, "123");
        }

        [TestMethod]
        public void TestGetTimeline()
        {
            var mock = new Mock<ITimelineService>();
            mock.Setup(m => m.GetJson("Timeline/GetTimeline", It.IsAny<NameValueCollection>())).Returns(TimelineJson);

            var timeline = Timeline.GetTimeline(mock.Object, "ID1");

            Assert.AreEqual(timeline.Id, "ID1");
            Assert.AreEqual(timeline.Title, "Test Title");
            Assert.AreEqual(timeline.CreationTimeStamp.Ticks.ToString(), "636544632390000000");
            Assert.IsTrue(timeline.IsDeleted);
            Assert.AreEqual(timeline.TenantId, "123");
        }

        [TestMethod]
        public void TestGetTimelines()
        {
            var mock = new Mock<ITimelineService>();
            mock.Setup(m => m.GetJson("Timeline/GetTimelines")).Returns(TimelinesJson);

            var timelines = Timeline.GetTimelines(mock.Object);

            Assert.AreEqual(timelines.Count, 2);
            Assert.AreEqual(timelines[0].Id, "ID1");
            Assert.AreEqual(timelines[1].Id, "ID2");
        }
    }
}
