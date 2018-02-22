using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace Echelon.TimelineApi.Tests
{
    [TestClass]
    public class AttachmentTests
    {
        [TestMethod]
        public async Task AttachmentCreate()
        {
            string json = "{\"Title\": \"Test Title\",\"TimelineEventId\": \"ID2\",\"IsDeleted\": true,\"Id\": \"ID1\",\"TenantId\": \"Team3\"}";
            var mock = new Mock<ITimelineService>();
            mock.Setup(m => m.PutJsonAsync(It.IsAny<string>(), It.IsAny<object>())).Returns(TestUtils.GetCompletedTask<string>(json));

            Attachment attachment = await Attachment.CreateAsync(mock.Object, "ID1", "Test Title");

            mock.Verify(m => m.PutJsonAsync("TimelineEventAttachment/Create", It.Is<object>(o => o.VerifyIsGuid("AttachmentId") && o.VerifyObject("TimelineEventId", "ID1") && o.VerifyObject("Title", "Test Title"))));
            Assert.AreEqual(attachment.Id, "ID1");
            Assert.AreEqual(attachment.TimelineEventId, "ID2");
            Assert.AreEqual(attachment.Title, "Test Title");
            Assert.AreEqual(attachment.TenantId, "Team3");
            Assert.IsTrue(attachment.IsDeleted);
        }
    }
}
