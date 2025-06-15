using LibraryOnlineRentalSystem.Domain.Book;
using LibraryOnlineRentalSystem.Domain.Common;
using NUnit.Framework;
using System;

namespace LibraryOnlineRentalSystem.Tests.Domain.Books
{
    [TestFixture]
    [TestOf(typeof(Description))]
    public class DescriptionTest
    {
        [Test]
        public void Constructor_WithValidDescription_SetsValue()
        {
            var description = new Description("A valid description with 5+ chars.");
            Assert.That(description.BookDescription, Is.EqualTo("A valid description with 5+ chars."));
        }

        [Test]
        public void Constructor_TrimsWhitespace()
        {
            var description = new Description("   Trimmed description.   ");
            Assert.That(description.BookDescription, Is.EqualTo("Trimmed description."));
        }

        [Test]
        public void Constructor_Null_ThrowsBusinessRulesException()
        {
            Assert.Throws<BusinessRulesException>(() => new Description(null));
        }

        [Test]
        public void Constructor_Empty_ThrowsBusinessRulesException()
        {
            Assert.Throws<BusinessRulesException>(() => new Description(""));
        }

        [Test]
        public void Constructor_TooLong_ThrowsBusinessRulesException()
        {
            var longText = new string('a', 2001); // Exceeds 2000 character limit
            Assert.Throws<BusinessRulesException>(() => new Description(longText));
        }

        [Test]
        public void GetBookDescription_ReturnsCorrectValue()
        {
            var description = new Description("Details about the book.");
            Assert.That(description.GetBookDescription(), Is.EqualTo("Details about the book."));
        }

        [Test]
        public void ValueOf_ReturnsNewInstanceWithGivenDescription()
        {
            var description = new Description("First description");
            var newDescription = description.ValueOf("Second valid description");
            Assert.That(newDescription.BookDescription, Is.EqualTo("Second valid description"));
        }

        [Test]
        public void Equals_SameValues_CaseInsensitive_ReturnsTrue()
        {
            var a = new Description("Test description");
            var b = new Description("test description");
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void Equals_DifferentValues_ReturnsFalse()
        {
            var a = new Description("First");
            var b = new Description("Second");
            Assert.That(a.Equals(b), Is.False);
        }

        [Test]
        public void Equals_Null_ReturnsFalse()
        {
            var a = new Description("Value");
            Assert.That(a.Equals(null), Is.False);
        }

        [Test]
        public void Equals_DifferentType_ReturnsFalse()
        {
            var a = new Description("Value");
            Assert.That(a.Equals("Value"), Is.False);
        }

        [Test]
        public void ToString_ReturnsBookDescription()
        {
            var description = new Description("Sample description");
            Assert.That(description.ToString(), Is.EqualTo("Sample description"));
        }

        [Test]
        public void GetHashCode_ReturnsSameForEqualDescriptions()
        {
            var a = new Description("Equal");
            var b = new Description("Equal");
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void GetHashCode_ReturnsDifferentForDifferentDescriptions()
        {
            var a = new Description("First description");
            var b = new Description("Second description");
            Assert.That(a.GetHashCode(), Is.Not.EqualTo(b.GetHashCode()));
        }
    }
}
