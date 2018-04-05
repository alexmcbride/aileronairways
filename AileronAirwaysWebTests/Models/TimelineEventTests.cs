using AileronAirwaysWeb.Models;
using AileronAirwaysWeb.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace AileronAirwaysWebTests.Models
{
    [TestClass]
    public class TimelineEventTests
    {
        const string TimelineEventJson = "{\"Id\":\"ID1\",\"Title\":\"Test Title\",\"Description\":\"Test Description\",\"EventDateTime\":\"636546626588300000\", \"Location\":\"-1.1234,1.1234\",\"TenantId\" : \"123\",\"IsDeleted\":\"true\"}";
        const string TimelineEventsJson = "[{\"Id\":\"ID1\",\"Title\":\"Test Title 1\",\"Description\":\"Test Description 1\",\"EventDateTime\":\"636546626588300000\", \"Location\":\"-1.1234,1.1234\",\"TenantId\" : \"123\",\"IsDeleted\":\"true\"}," +
            "{\"Id\":\"ID2\",\"Title\":\"Test Title 2\",\"Description\":\"Test Description 2\",\"EventDateTime\":\"636546626588300000\", \"Location\":\"-1.1234,1.1234\",\"TenantId\" : \"123\",\"IsDeleted\":\"true\"}]";


        [TestMethod]
        public async Task EventCreate()
        {

            var mock = new Mock<ITimelineService>();
            mock.Setup(m => m.PutJsonAsync(It.IsAny<string>(), It.IsAny<object>())).Returns(TestUtils.GetCompletedTask(TimelineEventJson));

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
        public async Task EventCreateAndLink()
        {
            var mock = new Mock<ITimelineService>();
            mock.Setup(m => m.PutJsonAsync(It.IsAny<string>(), It.IsAny<object>())).Returns(TestUtils.GetCompletedTask(TimelineEventJson));

            var timelineEvent = await TimelineEvent.CreateAndLinkAsync(mock.Object, "Test Title", "Test Description", DateTime.Now, "-1.1234,1.1234", "ID1");

            mock.Verify(m => m.PutJsonAsync("TimelineEvent/Create", It.IsAny<object>()));
            mock.Verify(m => m.PutJsonAsync("Timeline/LinkEvent", It.IsAny<object>()));
        }

        [TestMethod]
        public async Task UnlinkAndDeleteAsync()
        {
            var mock = new Mock<ITimelineService>();

            await TimelineEvent.UnlinkAndDeleteAsync(mock.Object, "ID1", "ID2");

            mock.Verify(m => m.PutJsonAsync("TimelineEvent/Delete", It.IsAny<object>()));
            mock.Verify(m => m.PutJsonAsync("Timeline/UnlinkEvent", It.IsAny<object>()));
        }

        [TestMethod]
        public async Task EventEditTitle()
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
        public async Task EventEditDescription()
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
        public async Task EventEditLocation()
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
        public async Task EventEditEventDateTime()
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
        public async Task EventDelete()
        {
            var mock = new Mock<ITimelineService>();

            await new TimelineEvent
            {
                Id = "ID1"
            }.DeleteAsync(mock.Object);

            mock.Verify(m => m.PutJsonAsync("TimelineEvent/Delete", It.Is<object>(o => o.VerifyObject("TimelineEventId", "ID1"))));
        }

        [TestMethod]
        public async Task EventGetTimelineEvent()
        {
            var dt = DateTime.Now;
            string json = "{\"Id\":\"ID1\",\"Title\":\"Test Title\",\"Description\":\"Test Description\",\"EventDateTime\":\"" + dt.Ticks + "\", \"Location\":\"-1.1234,1.1234\",\"TenantId\" : \"123\",\"IsDeleted\":\"true\"}";

            var mock = new Mock<ITimelineService>();
            mock.Setup(m => m.GetJsonAsync(It.IsAny<string>(), It.IsAny<NameValueCollection>())).Returns(TestUtils.GetCompletedTask<string>(json));

            TimelineEvent evt = await TimelineEvent.GetEventAsync(mock.Object, "ID1");

            mock.Verify(m => m.GetJsonAsync("TimelineEvent/GetTimelineEvent", It.Is<NameValueCollection>(c => c.VerifyContains("TimelineEventId", "ID1"))));
            Assert.AreEqual(evt.Id, "ID1");
            Assert.AreEqual(evt.Title, "Test Title");
            Assert.AreEqual(evt.Description, "Test Description");
            Assert.AreEqual(evt.EventDateTime, dt);
            Assert.AreEqual(evt.Location, "-1.1234,1.1234");
            Assert.IsTrue(evt.IsDeleted);
        }

        [TestMethod]
        public async Task EditEvent()
        {
            var now = DateTime.Now;

            var mock = new Mock<ITimelineService>();
            mock.Setup(m => m.PutJsonAsync(It.IsAny<string>(), It.IsAny<object>())).Returns(TestUtils.GetCompletedTask(TimelineEventJson));

            TimelineEvent timelineEvent = new TimelineEvent();
            timelineEvent.Id = "ID1";
            timelineEvent.Title = "Edited Title";
            timelineEvent.Description = "Edited description";
            timelineEvent.EventDateTime = now;
            timelineEvent.Location = "-1.1234,1.1234";
            await timelineEvent.EditAsync(mock.Object);

            // We reuse Event/Create to do a full on edit of all attributes at once.
            mock.Verify(m => m.PutJsonAsync("TimelineEvent/Create", It.Is<object>(o => o.VerifyObject("TimelineEventId", "ID1"))));
            mock.Verify(m => m.PutJsonAsync("TimelineEvent/Create", It.Is<object>(o => o.VerifyObject("Title", "Edited Title"))));
            mock.Verify(m => m.PutJsonAsync("TimelineEvent/Create", It.Is<object>(o => o.VerifyObject("Description", "Edited description"))));
            mock.Verify(m => m.PutJsonAsync("TimelineEvent/Create", It.Is<object>(o => o.VerifyObject("EventDateTime", now.Ticks.ToString()))));
            mock.Verify(m => m.PutJsonAsync("TimelineEvent/Create", It.Is<object>(o => o.VerifyObject("Location", "-1.1234,1.1234"))));
        }

        [TestMethod]
        public async Task TimelineLinkEvent()
        {
            var mock = new Mock<ITimelineService>();

            TimelineEvent timelineEvent = new TimelineEvent();
            timelineEvent.Id = "IDE1";

            await timelineEvent.LinkEventAsync(mock.Object, "ID1");

            mock.Verify(m => m.PutJsonAsync("Timeline/LinkEvent", It.Is<object>(o => o.VerifyObject("TimelineId", "ID1") && o.VerifyObject("EventId", "IDE1"))));
        }

        [TestMethod]
        public async Task TimelineUnlinkEvent()
        {
            var mock = new Mock<ITimelineService>();

            TimelineEvent timelineEvent = new TimelineEvent();
            timelineEvent.Id = "IDE1";

            await timelineEvent.UnlinkEventAsync(mock.Object, "ID1");

            mock.Verify(m => m.PutJsonAsync("Timeline/UnlinkEvent", It.Is<object>(o => o.VerifyObject("TimelineId", "ID1") && o.VerifyObject("EventId", "IDE1"))));
        }

        [TestMethod]
        public async Task TimelineGetLinkedEvents()
        {
            string json = "[{\"TimelineEventId\":\"ID1\",\"TimelineId\":\"ID2\",\"IsDeleted\":true,\"Id\":\"ID3\",\"TenantId\":\"123\"}," +
                "{\"TimelineEventId\":\"ID4\",\"TimelineId\":\"ID5\",\"IsDeleted\":true,\"Id\":\"ID6\",\"TenantId\":\"123\"}]";
            var mock = new Mock<ITimelineService>();
            mock.Setup(m => m.GetJsonAsync(It.IsAny<string>(), It.IsAny<NameValueCollection>())).Returns(TestUtils.GetCompletedTask(json));

            var linkedEvents = await TimelineEvent.GetEventsAsync(mock.Object, "ID1");

            mock.Verify(m => m.GetJsonAsync("Timeline/GetEvents", It.Is<NameValueCollection>(c => c.VerifyContains("TimelineId", "ID1"))));
            Assert.AreEqual(2, linkedEvents.Count);
            Assert.AreEqual(linkedEvents[0].TimelineEventId, "ID1");
            Assert.AreEqual(linkedEvents[0].TimelineId, "ID2");
            Assert.IsTrue(linkedEvents[0].IsDeleted);
            Assert.AreEqual(linkedEvents[0].Id, "ID3");
            Assert.AreEqual(linkedEvents[0].TenantId, "123");
            Assert.AreEqual(linkedEvents[1].TimelineEventId, "ID4");
        }


        [TestMethod]
        public async Task TimelineEventGetAllEvents()
        {
            var mock = new Mock<ITimelineService>();
            mock.Setup(m => m.GetJsonAsync(It.IsAny<string>())).Returns(TestUtils.GetCompletedTask(TimelineEventsJson));

            var events = await TimelineEvent.GetAllEventsAsync(mock.Object);

            Assert.AreEqual(2, events.Count);
            mock.Verify(m => m.GetJsonAsync("TimelineEvent/GetAllEvents"));
            Assert.AreEqual(events[0].Id, "ID1");
            Assert.AreEqual(events[0].Title, "Test Title 1");
            Assert.AreEqual(events[0].Description, "Test Description 1");
            Assert.AreEqual(events[0].EventDateTime.Ticks.ToString(), "636546626588300000");
            Assert.AreEqual(events[0].Location, "-1.1234,1.1234");
            Assert.AreEqual(events[0].TenantId, "123");
            Assert.IsTrue(events[0].IsDeleted);
            Assert.AreEqual(events[1].Id, "ID2");
            Assert.AreEqual(events[1].Title, "Test Title 2");
            Assert.AreEqual(events[1].Description, "Test Description 2");
            Assert.AreEqual(events[1].EventDateTime.Ticks.ToString(), "636546626588300000");
            Assert.AreEqual(events[1].Location, "-1.1234,1.1234");
            Assert.AreEqual(events[1].TenantId, "123");
            Assert.IsTrue(events[1].IsDeleted);
        }

        //[TestMethod]
        //public void TestLocationJson()
        //{
        //    var e = new TimelineEvent();
        //    e.Longitude = 1.123;
        //    e.Latitude = 1.123;

        //    var json = e.Location;
        //    e.Location = json;

        //    Assert.AreEqual(e.Longitude, 1.123);
        //    Assert.AreEqual(e.Latitude, 1.123);
        //}
    }
}