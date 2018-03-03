using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace Echelon.TimelineApi.Tests
{
    [TestClass]
    public class AttachmentTests
    {
        const string AttachmentJson = "{\"Title\": \"Test Title\",\"TimelineEventId\": \"ID2\",\"IsDeleted\": true,\"Id\": \"ID1\",\"TenantId\": \"Team3\"}";
        const string AttachmentJsonList = "[{\"Title\": \"Test Title\",\"TimelineEventId\": \"ID2\",\"IsDeleted\": true,\"Id\": \"ID1\",\"TenantId\": \"Team3\"}," +
            "{\"Title\": \"Test Title 2\",\"TimelineEventId\": \"ID4\",\"IsDeleted\": true,\"Id\": \"ID3\",\"TenantId\": \"Team3\"}]";

        [TestMethod]
        public async Task AttachmentCreate()
        {
            var mock = new Mock<ITimelineService>();
            mock.Setup(m => m.PutJsonAsync(It.IsAny<string>(), It.IsAny<object>())).Returns(TestUtils.GetCompletedTask(AttachmentJson));

            Attachment attachment = await Attachment.CreateAsync(mock.Object, "ID1", "Test Title");

            mock.Verify(m => m.PutJsonAsync("TimelineEventAttachment/Create", It.Is<object>(o => o.VerifyIsGuid("AttachmentId") && o.VerifyObject("TimelineEventId", "ID1") && o.VerifyObject("Title", "Test Title"))));
            Assert.AreEqual(attachment.Id, "ID1");
            Assert.AreEqual(attachment.TimelineEventId, "ID2");
            Assert.AreEqual(attachment.Title, "Test Title");
            Assert.AreEqual(attachment.TenantId, "Team3");
            Assert.IsTrue(attachment.IsDeleted);
        }

        [TestMethod]
        public async Task AttachmentEditTitle()
        {
            var mock = new Mock<ITimelineService>();

            Attachment attachment = new Attachment();
            attachment.Id = "ID1";
            attachment.Title = "Edited Title";

            await attachment.EditTitleAsync(mock.Object);

            mock.Verify(m => m.PutJsonAsync("TimelineEventAttachment/EditTitle", It.Is<object>(o => o.VerifyObject("AttachmentId", "ID1") && o.VerifyObject("Title", "Edited Title"))));
        }

        [TestMethod]
        public async Task AttachmentDelete()
        {
            var mock = new Mock<ITimelineService>();
            mock.Setup(m => m.FileExists(It.IsAny<string>())).Returns(true);

            Attachment attachment = new Attachment();
            attachment.Id = "ID1";

            await attachment.DeleteAsync(mock.Object, "cache");

            mock.Verify(m => m.PutJsonAsync("TimelineEventAttachment/Delete", It.Is<object>(o => o.VerifyObject("AttachmentId", "ID1"))));
            mock.Verify(m => m.FileDelete($@"cache\{attachment.Name}"));
        }

        [TestMethod]
        public async Task GenerateUploadPresignedUrl()
        {
            string presignedUrl = "http://www.test.com/presignedurl";

            var mock = new Mock<ITimelineService>();
            mock.Setup(m => m.GetJsonAsync(It.IsAny<string>(), It.IsAny<NameValueCollection>())).Returns(TestUtils.GetCompletedTask(presignedUrl));

            Attachment attachment = new Attachment();

            attachment.Id = "ID1";

            string result = await attachment.GenerateUploadPresignedUrlAsync(mock.Object);

            Assert.AreEqual(presignedUrl, result);
            mock.Verify(m => m.GetJsonAsync("TimelineEventAttachment/GenerateUploadPresignedUrl", It.Is<NameValueCollection>(c => c.VerifyContains("AttachmentId", "ID1"))));
        }

        [TestMethod]
        public async Task GenerateGetPresignedUrl()
        {
            string presignedUrl = "http://www.test.com/presignedurl";

            var mock = new Mock<ITimelineService>();
            mock.Setup(m => m.GetJsonAsync(It.IsAny<string>(), It.IsAny<NameValueCollection>())).Returns(TestUtils.GetCompletedTask(presignedUrl));

            Attachment attachment = new Attachment();
            attachment.Id = "ID1";

            string result = await attachment.GenerateGetPresignedUrlAsync(mock.Object);

            Assert.AreEqual(presignedUrl, result);
            mock.Verify(m => m.GetJsonAsync("TimelineEventAttachment/GenerateGetPresignedUrl", It.Is<NameValueCollection>(c => c.VerifyContains("AttachmentId", "ID1"))));
        }

        [TestMethod]
        public async Task GetTimelineEventAttachment()
        {
            var mock = new Mock<ITimelineService>();
            mock.Setup(m => m.GetJsonAsync(It.IsAny<string>(), It.IsAny<NameValueCollection>())).Returns(TestUtils.GetCompletedTask(AttachmentJson));

            Attachment attachment = await Attachment.GetAttachmentAsync(mock.Object, "ID1");

            mock.Verify(m => m.GetJsonAsync("TimelineEventAttachment/GetAttachment", It.Is<NameValueCollection>(c => c.VerifyContains("AttachmentId", "ID1"))));
            Assert.AreEqual(attachment.Id, "ID1");
            Assert.AreEqual(attachment.Title, "Test Title");
            Assert.AreEqual(attachment.TimelineEventId, "ID2");
            Assert.AreEqual(attachment.TenantId, "Team3");
        }

        [TestMethod]
        public async Task GetTimelineEventAttachments()
        {
            var mock = new Mock<ITimelineService>();
            mock.Setup(m => m.GetJsonAsync(It.IsAny<string>(), It.IsAny<NameValueCollection>())).Returns(TestUtils.GetCompletedTask(AttachmentJsonList));

            var attachments = await Attachment.GetAttachmentsAsync(mock.Object, "ID1");

            Assert.AreEqual(attachments.Count, 2);
            Assert.AreEqual(attachments[0].Id, "ID1");
            Assert.AreEqual(attachments[0].Title, "Test Title");
            Assert.IsTrue(attachments[0].IsDeleted);
            Assert.AreEqual(attachments[0].TimelineEventId, "ID2");
            Assert.AreEqual(attachments[0].TenantId, "Team3");
            Assert.AreEqual(attachments[1].Id, "ID3");
            Assert.AreEqual(attachments[1].Title, "Test Title 2");
            Assert.IsTrue(attachments[1].IsDeleted);
            Assert.AreEqual(attachments[1].TimelineEventId, "ID4");
            Assert.AreEqual(attachments[1].TenantId, "Team3");
        }

        [TestMethod]
        public async Task UploadAttachment()
        {
            string presignedUrl = "http://www.test.com/presignedurl";
            string filename = "filename.docx";

            var attachment = new Attachment();
            attachment.Id = "ID1";
            var mock = new Mock<ITimelineService>();
            mock.Setup(m => m.GetJsonAsync(It.IsAny<string>(), It.IsAny<NameValueCollection>())).Returns(TestUtils.GetCompletedTask(presignedUrl));

            await attachment.UploadAsync(mock.Object, filename);

            mock.Verify(m => m.UploadFileAsync(presignedUrl, filename));
        }

        [TestMethod]
        public async Task DownloadAttachment()
        {
            string presignedUrl = "http://www.test.com/presignedurl";
            var mock = new Mock<ITimelineService>();
            mock.Setup(m => m.GetJsonAsync(It.IsAny<string>(), It.IsAny<NameValueCollection>())).Returns(TestUtils.GetCompletedTask(presignedUrl));
            mock.Setup(m => m.FileExists(It.IsAny<string>())).Returns(false);

            Attachment attachment = new Attachment();
            attachment.Id = "ID1";
            attachment.Title = "filename.docx";

            await attachment.DownloadAsync(mock.Object, "cache");

            mock.Verify(m => m.DownloadFileAsync(presignedUrl, @"cache\ID1.docx"));
        }

        [TestMethod]
        public async Task DownloadAttachmentAlreadyInCache()
        {
            string presignedUrl = "http://www.test.com/presignedurl";
            var mock = new Mock<ITimelineService>();
            mock.Setup(m => m.GetJsonAsync(It.IsAny<string>(), It.IsAny<NameValueCollection>())).Returns(TestUtils.GetCompletedTask(presignedUrl));
            mock.Setup(m => m.FileExists(It.IsAny<string>())).Returns(true);

            Attachment attachment = new Attachment();
            attachment.Id = "ID1";
            attachment.Title = "filename.docx";

            await attachment.DownloadAsync(mock.Object, "cache");

            mock.Verify(m => m.DownloadFileAsync(presignedUrl, @"cache\ID1.docx"), Times.Never());
        }
    }
}
