using LibraryOnlineRentalSystem.Domain.Book;
using LibraryOnlineRentalSystem.Domain.Common;
using NUnit.Framework;
using System;

namespace LibraryOnlineRentalSystem.Tests.Domain.Books
{
    [TestFixture]
    [TestOf(typeof(AmountOfCopies))]
    public class AmountOfCopiesTest
    {
        [Test]
        public void Constructor_WithValidNumber_SetsValue()
        {
            var amount = new AmountOfCopies(5);
            Assert.That(amount.BookAmountOfCopies, Is.EqualTo(5));
        }

        [Test]
        public void Constructor_WithNegativeNumber_ThrowsBusinessRulesException()
        {
            Assert.Throws<BusinessRulesException>(() => new AmountOfCopies(-1));
        }

        [Test]
        public void GetBookAmountOfCopies_ReturnsCorrectValue()
        {
            var amount = new AmountOfCopies(7);
            Assert.That(amount.GetBookAmountOfCopies(), Is.EqualTo(7));
        }

        [Test]
        public void ValueOf_ReturnsNewInstanceWithGivenValue()
        {
            var amount = new AmountOfCopies(3);
            var newAmount = amount.ValueOf(10);
            Assert.That(newAmount.BookAmountOfCopies, Is.EqualTo(10));
        }

        [Test]
        public void Equals_SameValues_ReturnsTrue()
        {
            var a = new AmountOfCopies(4);
            var b = new AmountOfCopies(4);
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void Equals_DifferentValues_ReturnsFalse()
        {
            var a = new AmountOfCopies(4);
            var b = new AmountOfCopies(5);
            Assert.That(a.Equals(b), Is.False);
        }

        [Test]
        public void Equals_Null_ReturnsFalse()
        {
            var a = new AmountOfCopies(2);
            Assert.That(a.Equals(null), Is.False);
        }

        [Test]
        public void Equals_DifferentType_ReturnsFalse()
        {
            var a = new AmountOfCopies(2);
            Assert.That(a.Equals("string"), Is.False);
        }

        [Test]
        public void ToString_ReturnsCorrectString()
        {
            var a = new AmountOfCopies(6);
            Assert.That(a.ToString(), Is.EqualTo("6"));
        }

        [Test]
        public void GetHashCode_ReturnsSameForEqualValues()
        {
            var a = new AmountOfCopies(8);
            var b = new AmountOfCopies(8);
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void GetHashCode_ReturnsDifferentForDifferentValues()
        {
            var a = new AmountOfCopies(8);
            var b = new AmountOfCopies(9);
            Assert.That(a.GetHashCode(), Is.Not.EqualTo(b.GetHashCode()));
        }
    }
}
