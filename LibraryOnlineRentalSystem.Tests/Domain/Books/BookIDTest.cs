using LibraryOnlineRentalSystem.Domain.Book;
using NUnit.Framework;

namespace LibraryOnlineRentalSystem.Tests.Domain.Books
{
    [TestFixture]
    [TestOf(typeof(BookID))]
    public class RentalIdTest
    {
        [Test]
        public void Constructor_SetsBookIDValue()
        {
            var id = new BookID("abc123");
            Assert.That(id.AsString(), Is.EqualTo("abc123"));
        }

        [Test]
        public void AsString_ReturnsBookID()
        {
            var id = new BookID("xyz789");
            Assert.That(id.AsString(), Is.EqualTo("xyz789"));
        }

        [Test]
        public void ToString_ReturnsBookID()
        {
            var id = new BookID("test456");
            Assert.That(id.ToString(), Is.EqualTo("test456"));
        }

        [Test]
        public void Equals_SameBookID_ReturnsTrue()
        {
            var a = new BookID("id1");
            var b = new BookID("id1");
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void Equals_DifferentBookID_ReturnsFalse()
        {
            var a = new BookID("id1");
            var b = new BookID("id2");
            Assert.That(a.Equals(b), Is.False);
        }

        [Test]
        public void Equals_Null_ReturnsFalse()
        {
            var a = new BookID("id1");
            Assert.That(a.Equals(null), Is.False);
        }

        [Test]
        public void Equals_DifferentType_ReturnsFalse()
        {
            var a = new BookID("id1");
            Assert.That(a.Equals("id1"), Is.False);
        }
    }
}