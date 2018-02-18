using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace Echelon.TimelineApi.Tests
{
    [TestClass]
    public class TimelineEventTests
    {
        [TestMethod]
        public async Task TestTimelineEventCreate()
        {
            string json = "{\"TenantId\" : \"123\"}";
            var dt = DateTime.Now;
            var mock = new Mock<ITimelineService>();
            mock.Setup(m => m.PutJsonAsync(It.IsAny<string>(), It.IsAny<object>())).Returns(TestUtils.GetCompletedTask(json));

            var timeline = new TimelineEvent()
            {
                Title = "Test Title",
                Description = "Test Description",
                EventDateTime = dt,
                IsDeleted = true,
                Location = "-1.1234,1.1234",
            };
            var result = await timeline.CreateAsync(mock.Object);

            mock.Verify(m => m.PutJsonAsync("TimelineEvent/Create", It.Is<object>(o => o.VerifyIsGuid("TimelineEventId") &&
                o.VerifyObject("Title", "Test Title") &&
                o.VerifyObject("Description", "Test Description") &&
                o.VerifyObject("EventDateTime", dt) &&
                o.VerifyObject("IsDeleted", true)&&
                o.VerifyObject("Location", "-1.1234,1.1234"))));
            Assert.AreEqual(timeline.TenantId, "123");
            Assert.AreEqual(result.TenantId, "123");
            Assert.AreEqual(result.Title, "Test Title");
            Assert.AreEqual(result.Description, "Test Description");
            Assert.AreEqual(result.EventDateTime, dt);
            Assert.IsTrue(result.IsDeleted);
            Assert.AreEqual(result.Location, "-1.1234,1.1234");
        }
    }
}
