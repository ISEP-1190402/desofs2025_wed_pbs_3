using LibraryOnlineRentalSystem.Domain.User;
using LibraryOnlineRentalSystem.Domain.Common;
using NUnit.Framework;
using System;

namespace LibraryOnlineRentalSystem.Tests.Domain.User
{
    [TestFixture]
    [TestOf(typeof(PhoneNumber))]
    public class PhoneNumberTest
    {
        [Test]
        public void Constructor_ValidPhoneNumber_SetsNumber()
        {
            var phone = new PhoneNumber("912345678");
            Assert.That(phone.Number, Is.EqualTo("912345678"));
        }

        [Test]
        public void Constructor_TrimsWhitespace()
        {
            var phone = new PhoneNumber(" 912345678 ");
            Assert.That(phone.Number, Is.EqualTo("912345678"));
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("     ")]
        public void Constructor_NullOrEmpty_ThrowsArgumentNullException(string value)
        {
            Assert.Throws<ArgumentNullException>(() => new PhoneNumber(value));
        }

        [TestCase("12345678")]      // Too short
        [TestCase("1234567890")]    // Too long
        [TestCase("abcdefghj")]     // Letters
        [TestCase("1234 5678")]     // Space
        [TestCase("123-456789")]    // Dash
        [TestCase("+351912345678")] // With country code
        public void Constructor_InvalidFormat_ThrowsBusinessRulesException(string value)
        {
            Assert.Throws<BusinessRulesException>(() => new PhoneNumber(value));
        }

        [Test]
        public void Clone_ReturnsNewInstanceWithSameNumber()
        {
            var phone = new PhoneNumber("987654321");
            var clone = (PhoneNumber)phone.Clone();
            Assert.That(clone, Is.Not.SameAs(phone));
            Assert.That(clone.Number, Is.EqualTo("987654321"));
        }

        [Test]
        public void ToString_ReturnsNumber()
        {
            var phone = new PhoneNumber("912345678");
            Assert.That(phone.ToString(), Is.EqualTo("912345678"));
        }

        [Test]
        public void Equals_SameNumber_ReturnsTrue()
        {
            var a = new PhoneNumber("912345678");
            var b = new PhoneNumber("912345678");
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void Equals_DifferentNumber_ReturnsFalse()
        {
            var a = new PhoneNumber("912345678");
            var b = new PhoneNumber("987654321");
            Assert.That(a.Equals(b), Is.False);
        }

        [Test]
        public void Equals_Null_ReturnsFalse()
        {
            var a = new PhoneNumber("912345678");
            Assert.That(a.Equals(null), Is.False);
        }

        [Test]
        public void Equals_DifferentType_ReturnsFalse()
        {
            var a = new PhoneNumber("912345678");
            Assert.That(a.Equals("912345678"), Is.False);
        }

        [Test]
        public void GetHashCode_SameNumber_ReturnsSameHash()
        {
            var a = new PhoneNumber("123456789");
            var b = new PhoneNumber("123456789");
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void GetHashCode_DifferentNumbers_ReturnDifferentHash()
        {
            var a = new PhoneNumber("123456789");
            var b = new PhoneNumber("987654321");
            Assert.That(a.GetHashCode(), Is.Not.EqualTo(b.GetHashCode()));
        }
    }
}
