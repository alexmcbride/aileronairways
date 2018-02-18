using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
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
            var dt = DateTime.Now;
            string json = "{\"Id\":\"ID1\",\"Title\":\"Test Title\",\"Description\":\"Test Description\",\"EventDateTime\":\"" + dt.Ticks + "\", \"Location\":\"-1.1234,1.1234\",\"TenantId\" : \"123\"}";

            var mock = new Mock<ITimelineService>();
            mock.Setup(m => m.PutJsonAsync(It.IsAny<string>(), It.IsAny<object>())).Returns(TestUtils.GetCompletedTask(json));

            var evt = new TimelineEvent()
            {
                Title = "Test Title",
                Description = "Test Description",
                EventDateTime = dt,
                Location = "-1.1234,1.1234",
            };
            var result = await evt.CreateAsync(mock.Object);

            mock.Verify(m => m.PutJsonAsync("TimelineEvent/Create", It.Is<object>(o => o.VerifyIsGuid("TimelineEventId") &&
                o.VerifyObject("Title", "Test Title") &&
                o.VerifyObject("Description", "Test Description") &&
                o.VerifyObject("EventDateTime", dt) &&
                o.VerifyObject("Location", "-1.1234,1.1234"))));
            Assert.AreEqual(evt.TenantId, "123");
            Assert.AreEqual(result.Id, "ID1");
            Assert.AreEqual(result.TenantId, "123");
            Assert.AreEqual(result.Title, "Test Title");
            Assert.AreEqual(result.Description, "Test Description");
            Assert.AreEqual(result.EventDateTime, dt);
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
                o.VerifyObject("EventDateTime", now))));
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
    }
}
