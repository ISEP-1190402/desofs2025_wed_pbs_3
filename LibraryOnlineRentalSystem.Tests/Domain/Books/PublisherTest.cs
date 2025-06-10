using LibraryOnlineRentalSystem.Domain.Book;
using LibraryOnlineRentalSystem.Domain.Common;
using NUnit.Framework;
using System;

namespace LibraryOnlineRentalSystem.Tests.Domain.Books
{
    [TestFixture]
    [TestOf(typeof(Publisher))]
    public class PublisherTest
    {
        [Test]
        public void Constructor_WithValidPublisher_SetsValue()
        {
            var publisher = new Publisher("Penguin Books");
            Assert.That(publisher.PublisherName, Is.EqualTo("Penguin Books"));
        }

        [Test]
        public void Constructor_TrimsWhitespace()
        {
            var publisher = new Publisher("  HarperCollins  ");
            Assert.That(publisher.PublisherName, Is.EqualTo("HarperCollins"));
        }

        [Test]
        public void Constructor_Null_ThrowsBusinessRulesException()
        {
            Assert.Throws<BusinessRulesException>(() => new Publisher(null));
        }

        [Test]
        public void Constructor_Empty_ThrowsBusinessRulesException()
        {
            Assert.Throws<BusinessRulesException>(() => new Publisher(""));
        }

        [Test]
        public void Constructor_TooLong_ThrowsBusinessRulesException()
        {
            var longName = new string('a', 51);
            Assert.Throws<BusinessRulesException>(() => new Publisher(longName));
        }

        [Test]
        public void GetBookPublisher_ReturnsCorrectValue()
        {
            var publisher = new Publisher("Oxford");
            Assert.That(publisher.GetBookPublisher(), Is.EqualTo("Oxford"));
        }

        [Test]
        public void ValueOf_ReturnsNewInstanceWithGivenName()
        {
            var publisher = new Publisher("Springer");
            var newPublisher = publisher.ValueOf("Routledge");
            Assert.That(newPublisher.PublisherName, Is.EqualTo("Routledge"));
        }

        [Test]
        public void Equals_SameValues_CaseInsensitive_ReturnsTrue()
        {
            var a = new Publisher("Elsevier");
            var b = new Publisher("elsevier");
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void Equals_DifferentValues_ReturnsFalse()
        {
            var a = new Publisher("Macmillan");
            var b = new Publisher("Bloomsbury");
            Assert.That(a.Equals(b), Is.False);
        }

        [Test]
        public void Equals_Null_ReturnsFalse()
        {
            var a = new Publisher("Thomson");
            Assert.That(a.Equals(null), Is.False);
        }

        [Test]
        public void Equals_DifferentType_ReturnsFalse()
        {
            var a = new Publisher("Pearson");
            Assert.That(a.Equals("Pearson"), Is.False);
        }

        [Test]
        public void ToString_ReturnsPublisherName()
        {
            var publisher = new Publisher("Faber & Faber");
            Assert.That(publisher.ToString(), Is.EqualTo("Faber & Faber"));
        }

        [Test]
        public void GetHashCode_ReturnsSameForEqualPublishers()
        {
            var a = new Publisher("Vintage");
            var b = new Publisher("Vintage");
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void GetHashCode_ReturnsDifferentForDifferentPublishers()
        {
            var a = new Publisher("Granta");
            var b = new Publisher("Puffin");
            Assert.That(a.GetHashCode(), Is.Not.EqualTo(b.GetHashCode()));
        }
    }
}
