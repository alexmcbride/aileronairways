using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
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

        [TestMethod]
        public void TestEditTitle()
        {
            var mock = new Mock<ITimelineService>();

            Timeline timeline = new Timeline
            {
                Id = "ID1",
                Title = "Test Title 2"
            };
            timeline.EditTitle(mock.Object);

            mock.Verify(m => m.PutJson("Timeline/EditTitle", It.Is<object>(t => VerifyObject(t, "TimelineId", "ID1") && VerifyObject(t, "Title", "Test Title 2"))));
        }

        [TestMethod]
        public void TestDelete()
        {
            var mock = new Mock<ITimelineService>();

            Timeline timeline = new Timeline
            {
                Id = "ID1",
            };
            timeline.Delete(mock.Object);

            mock.Verify(m => m.PutJson("Timeline/Delete", It.Is<object>(t => VerifyObject(t, "TimelineId", "ID1"))));
        }

        // Verifies that a particular property is present in an object.
        private static bool VerifyObject(object obj, string key, object value)
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
    }
}
