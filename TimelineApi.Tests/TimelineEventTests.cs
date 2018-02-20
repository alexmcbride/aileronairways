using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace Echelon.TimelineApi.Tests
{
    [TestClass]
    public class TimelineEventTests
    {
        [TestMethod]
        public async Task TestEventCreate()
        {
            string json = "{\"Id\":\"ID1\",\"Title\":\"Test Title\",\"Description\":\"Test Description\",\"EventDateTime\":\"636546626588300000\", \"Location\":\"-1.1234,1.1234\",\"TenantId\" : \"123\",\"IsDeleted\":\"true\"}";

            var mock = new Mock<ITimelineService>();
            mock.Setup(m => m.PutJsonAsync(It.IsAny<string>(), It.IsAny<object>())).Returns(TestUtils.GetCompletedTask(json));

            var dateTime = new DateTime(636546626588300000);
            var result = await TimelineEvent.CreateAsync(mock.Object, "Test Title", "Test Description", dateTime, "-1.1234,1.1234");

            mock.Verify(m => m.PutJsonAsync("TimelineEvent/Create", It.Is<object>(o => o.VerifyIsGuid("TimelineEventId") &&
                o.VerifyObject("Title", "Test Title") &&
                o.VerifyObject("Description", "Test Description") &&
                o.VerifyObject("EventDateTime", "636546626588300000") &&
                o.VerifyObject("Location", "-1.1234,1.1234"))));
            Assert.AreEqual(result.Id, "ID1");
            Assert.AreEqual(result.TenantId, "123");
            Assert.AreEqual(result.Title, "Test Title");
            Assert.AreEqual(result.Description, "Test Description");
            Assert.AreEqual(result.EventDateTime, dateTime);
            Assert.AreEqual(result.Location, "-1.1234,1.1234");
        }

        [TestMethod]
        public async Task TestEventEditTitle()
        {
            var mock = new Mock<ITimelineService>();

            await new TimelineEvent
            {
                Id = "ID1",
                Title = "Edited Title"
            }.EditTitleAsync(mock.Object);

            mock.Verify(m => m.PutJsonAsync("TimelineEvent/EditTitle", It.Is<object>(o => o.VerifyObject("TimelineEventId", "ID1") &&
                o.VerifyObject("Title", "Edited Title"))));
        }

        [TestMethod]
        public async Task TestEventEditDescription()
        {
            var mock = new Mock<ITimelineService>();

            await new TimelineEvent
            {
                Id = "ID1",
                Description = "Edited description"
            }.EditDescriptionAsync(mock.Object);

            mock.Verify(m => m.PutJsonAsync("TimelineEvent/EditDescription", It.Is<object>(o => o.VerifyObject("TimelineEventId", "ID1") &&
                o.VerifyObject("Description", "Edited description"))));
        }

        [TestMethod]
        public async Task TestEventEditLocation()
        {
            var mock = new Mock<ITimelineService>();

            await new TimelineEvent
            {
                Id = "ID1",
                Location = "-1.1234,2.1234"
            }.EditLocationAsync(mock.Object);

            mock.Verify(m => m.PutJsonAsync("TimelineEvent/EditLocation", It.Is<object>(o => o.VerifyObject("TimelineEventId", "ID1") &&
                o.VerifyObject("Location", "-1.1234,2.1234"))));
        }

        [TestMethod]
        public async Task TestEventEditEventDateTime()
        {
            var now = DateTime.Now;
            var mock = new Mock<ITimelineService>();

            await new TimelineEvent
            {
                Id = "ID1",
                EventDateTime = now
            }.EditEventDateTimeAsync(mock.Object);

            mock.Verify(m => m.PutJsonAsync("TimelineEvent/EditEventDateTime", It.Is<object>(o => o.VerifyObject("TimelineEventId", "ID1") &&
                o.VerifyObject("EventDateTime", now.Ticks.ToString()))));
        }

        [TestMethod]
        public async Task TestEventDelete()
        {
            var mock = new Mock<ITimelineService>();

            await new TimelineEvent
            {
                Id = "ID1"
            }.DeleteAsync(mock.Object);

            mock.Verify(m => m.PutJsonAsync("TimelineEvent/Delete", It.Is<object>(o => o.VerifyObject("TimelineEventId", "ID1"))));
        }

        [TestMethod]
        public async Task TestEventGetTimelineEvent()
        {
            var dt = DateTime.Now;
            string json = "{\"Id\":\"ID1\",\"Title\":\"Test Title\",\"Description\":\"Test Description\",\"EventDateTime\":\"" + dt.Ticks + "\", \"Location\":\"-1.1234,1.1234\",\"TenantId\" : \"123\",\"IsDeleted\":\"true\"}";

            var mock = new Mock<ITimelineService>();
            mock.Setup(m => m.GetJsonAsync(It.IsAny<string>(), It.IsAny<NameValueCollection>())).Returns(TestUtils.GetCompletedTask<string>(json));

            TimelineEvent evt = await TimelineEvent.GetTimelineEventAsync(mock.Object, "ID1");

            mock.Verify(m => m.GetJsonAsync("TimelineEvent/GetTimelineEvent", It.Is<NameValueCollection>(c => c.VerifyContains("TimelineEventId", "ID1"))));
            Assert.AreEqual(evt.Id, "ID1");
            Assert.AreEqual(evt.Title, "Test Title");
            Assert.AreEqual(evt.Description, "Test Description");
            Assert.AreEqual(evt.EventDateTime, dt);
            Assert.AreEqual(evt.Location, "-1.1234,1.1234");
            Assert.IsTrue(evt.IsDeleted);
        }

        [TestMethod]
        public async Task TestEventLinkTimelineEvents()
        {
            var mock = new Mock<ITimelineService>();
            TimelineEvent a = new TimelineEvent();
            a.Id = "ID1";
            TimelineEvent b = new TimelineEvent();
            b.Id = "ID2";

            await a.LinkTimelineEventsAsync(mock.Object, b);

            mock.Verify(m => m.PutJsonAsync("TimelineEvent/LinkEvents",
                It.Is<object>(o => o.VerifyObject("TimelineEventId", "ID1") && o.VerifyObject("LinkedToTimelineEventId", "ID2"))));
        }

        [TestMethod]
        public async Task TestEventUnlinkTimelineEvents()
        {
            var mock = new Mock<ITimelineService>();
            TimelineEvent a = new TimelineEvent();
            a.Id = "ID1";
            TimelineEvent b = new TimelineEvent();
            b.Id = "ID2";

            await a.UnlinkTimelineEventsAsync(mock.Object, b);

            mock.Verify(m => m.PutJsonAsync("TimelineEvent/UnlinkEvents",
                It.Is<object>(o => o.VerifyObject("TimelineEventId", "ID1") && o.VerifyObject("UnlinkedFromTimelineEventId", "ID2"))));
        }

        [TestMethod]
        public async Task TestEventGetLinkedTimelineEvents()
        {
            string json = "[{\"TimelineEventId\":\"ID1\",\"LinkedToTimelineEventId\":\"ID2\",\"Id\":\"ID3\",\"TenantId\":\"Team3\"}," +
                "{\"TimelineEventId\":\"ID4\",\"LinkedToTimelineEventId\":\"ID5\",\"Id\":\"ID6\",\"TenantId\":\"Team3\"}]";

            var mock = new Mock<ITimelineService>();
            mock.Setup(m => m.GetJsonAsync(It.IsAny<string>(), It.IsAny<NameValueCollection>())).Returns(TestUtils.GetCompletedTask<string>(json));

            TimelineEvent timelinkEvent = new TimelineEvent();
            timelinkEvent.Id = "ID1";

            IList<TimelineEventLink> links = await timelinkEvent.GetLinkedTimelineEventsAsync(mock.Object);
            Assert.AreEqual(2, links.Count);
            mock.Verify(m => m.GetJsonAsync("TimelineEvent/GetLinkedTimelineEvents", It.Is<NameValueCollection>(c => c.VerifyContains("TimelineEventId", "ID1"))));
            Assert.AreEqual(links[0].TimelineEventId, "ID1");
            Assert.AreEqual(links[0].LinkedToTimelineEventId, "ID2");
            Assert.AreEqual(links[0].Id, "ID3");
            Assert.AreEqual(links[0].TenantId, "Team3");
            Assert.AreEqual(links[1].TimelineEventId, "ID4");
            Assert.AreEqual(links[1].LinkedToTimelineEventId, "ID5");
            Assert.AreEqual(links[1].Id, "ID6");
            Assert.AreEqual(links[1].TenantId, "Team3");
        }
    }
}