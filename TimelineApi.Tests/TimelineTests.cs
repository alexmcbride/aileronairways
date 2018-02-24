﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace Echelon.TimelineApi.Tests
{
    [TestClass]
    public class TimelineTests
    {
        // Fake timeline JSON.
        private const string TimelineJson = "{\"Id\": \"ID1\", \"Title\": \"Test Title\", \"CreationTimeStamp\": \"636544632390000000\", \"IsDeleted\": true, \"TenantId\": \"123\"}";
        private const string TimelinesJson = "[{\"Id\": \"ID1\", \"Title\": \"Test Title\", \"CreationTimeStamp\": \"636544632390000000\", \"IsDeleted\": true, \"TenantId\": \"123\"}," +
            "{\"Id\": \"ID2\", \"Title\": \"Test Title 2\", \"CreationTimeStamp\": \"636544632350000000\", \"IsDeleted\": true, \"TenantId\": \"123\"}]";

        [TestMethod]
        public async Task TimelineCreate()
        {
            var mock = new Mock<ITimelineService>();
            mock.Setup(m => m.PutJsonAsync(It.IsAny<string>(), It.IsAny<object>())).Returns(TestUtils.GetCompletedTask(TimelineJson));

            var timeline = await Timeline.CreateAsync(mock.Object, "Test Title");

            mock.Verify(m => m.PutJsonAsync("Timeline/Create", It.Is<object>(o => o.VerifyObject("Title", "Test Title"))));
            mock.Verify(m => m.PutJsonAsync("Timeline/Create", It.Is<object>(o => o.VerifyIsGuid("TimelineId"))));
            Assert.AreEqual(timeline.Id, "ID1");
            Assert.AreEqual(timeline.Title, "Test Title");
            Assert.AreEqual(timeline.CreationTimeStamp.Ticks.ToString(), "636544632390000000");
            Assert.IsTrue(timeline.IsDeleted);
            Assert.AreEqual(timeline.TenantId, "123");
        }

        [TestMethod]
        public async Task GetTimeline()
        {
            var mock = new Mock<ITimelineService>();
            mock.Setup(m => m.GetJsonAsync(It.IsAny<string>(), It.IsAny<NameValueCollection>())).Returns(TestUtils.GetCompletedTask(TimelineJson));

            var timeline = await Timeline.GetTimelineAsync(mock.Object, "ID1");

            mock.Verify(m => m.GetJsonAsync("Timeline/GetTimeline", It.Is<NameValueCollection>(c => c.VerifyContains("TimelineId", "ID1"))));
            Assert.AreEqual(timeline.Id, "ID1");
            Assert.AreEqual(timeline.Title, "Test Title");
            Assert.AreEqual(timeline.CreationTimeStamp.Ticks.ToString(), "636544632390000000");
            Assert.IsTrue(timeline.IsDeleted);
            Assert.AreEqual(timeline.TenantId, "123");
        }

        [TestMethod]
        public async Task GetTimelines()
        {
            var mock = new Mock<ITimelineService>();
            mock.Setup(m => m.GetJsonAsync("Timeline/GetTimelines")).Returns(TestUtils.GetCompletedTask(TimelinesJson));

            var timelines = await Timeline.GetTimelinesAsync(mock.Object);

            Assert.AreEqual(timelines.Count, 2);
            Assert.AreEqual(timelines[0].Id, "ID1");
            Assert.AreEqual(timelines[1].Id, "ID2");
        }

        [TestMethod]
        public async Task TimelineEditTitle()
        {
            var mock = new Mock<ITimelineService>();

            Timeline timeline = new Timeline
            {
                Id = "ID1",
                Title = "Test Title 2"
            };
            await timeline.EditTitleAsync(mock.Object);

            mock.Verify(m => m.PutJsonAsync("Timeline/EditTitle", It.Is<object>(t => t.VerifyObject("TimelineId", "ID1"))));
            mock.Verify(m => m.PutJsonAsync("Timeline/EditTitle", It.Is<object>(t => t.VerifyObject("Title", "Test Title 2"))));
        }

        [TestMethod]
        public async Task TimelineDelete()
        {
            var mock = new Mock<ITimelineService>();

            Timeline timeline = new Timeline
            {
                Id = "ID1",
            };
            await timeline.DeleteAsync(mock.Object);

            mock.Verify(m => m.PutJsonAsync("Timeline/Delete", It.Is<object>(t => t.VerifyObject("TimelineId", "ID1"))));
        }
    }
}
