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
            // Test with valid Portuguese mobile numbers (9xxxxxxxx)
            var phone1 = new PhoneNumber("912345678");
            Assert.That(phone1.Number, Is.EqualTo("912345678"));
            
            // Test with valid Portuguese landline (2xxxxxxxx)
            var phone2 = new PhoneNumber("212345678");
            Assert.That(phone2.Number, Is.EqualTo("212345678"));
            
            // Test with valid Portuguese mobile number starting with 93
            var phone3 = new PhoneNumber("931234567");
            Assert.That(phone3.Number, Is.EqualTo("931234567"));
        }

        [Test]
        public void Constructor_TrimsWhitespace()
        {
            var phone = new PhoneNumber(" 912345678 ");
            Assert.That(phone.Number, Is.EqualTo("912345678"), "Should trim whitespace");
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("     ")]
        public void Constructor_NullOrEmpty_ThrowsArgumentNullException(string value)
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new PhoneNumber(value));
            Assert.That(ex.ParamName, Is.EqualTo("number"));
        }

        [TestCase("12345678", "Too short (8 digits)")]
        [TestCase("1234567890", "Too long (10 digits)")]
        [TestCase("abcdefghj", "Contains letters")]
        [TestCase("1234 5678", "Contains space")]
        [TestCase("123-456789", "Contains dash")]
        [TestCase("+351912345678", "Contains country code")]
        [TestCase("812345678", "Invalid prefix (8)")]
        [TestCase("712345678", "Invalid prefix (7)")]
        [TestCase("123456789", "Invalid prefix (1)")]
        [TestCase("91234567a", "Contains letter")]
        [TestCase("91.234.5678", "Contains dots")]
        [TestCase("91234567", "Too short (8 digits)")]
        [TestCase("9123456789", "Too long (10 digits)")]
        [TestCase("0023512345678", "Invalid format")]
        [TestCase("00351912345678", "Invalid format")]
        public void Constructor_InvalidFormat_ThrowsBusinessRulesException(string value, string reason)
        {
            var ex = Assert.Throws<BusinessRulesException>(() => new PhoneNumber(value));
            Assert.That(ex.Message, Does.Contain("Invalid").Or.Contains("must contain exactly 9 digits"), $"Failed for {reason}");
        }

        [Test]
        public void Clone_ReturnsNewInstanceWithSameNumber()
        {
            var phone = new PhoneNumber("912345678");
            var clone = (PhoneNumber)phone.Clone();
            Assert.Multiple(() =>
            {
                Assert.That(clone, Is.Not.SameAs(phone), "Clone should be a different instance");
                Assert.That(clone.Number, Is.EqualTo("912345678"), "Clone should have same number");
            });
        }

        [Test]
        public void ToString_ReturnsNumber()
        {
            var phone = new PhoneNumber("912345678");
            Assert.That(phone.ToString(), Is.EqualTo("912345678"), "ToString should return the phone number");
        }

        [Test]
        public void Equals_SameNumber_ReturnsTrue()
        {
            var a = new PhoneNumber("912345678");
            var b = new PhoneNumber("912345678");
            Assert.That(a.Equals(b), Is.True, "Same number should be equal");
        }

        [Test]
        public void Equals_DifferentNumber_ReturnsFalse()
        {
            var a = new PhoneNumber("912345678");
            var b = new PhoneNumber("962345678"); // Different prefix
            var c = new PhoneNumber("212345678"); // Landline vs mobile
            
            Assert.Multiple(() =>
            {
                Assert.That(a.Equals(b), Is.False, "Different mobile numbers should not be equal");
                Assert.That(a.Equals(c), Is.False, "Mobile and landline should not be equal");
            });
        }

        [Test]
        public void Equals_Null_ReturnsFalse()
        {
            var a = new PhoneNumber("912345678");
            Assert.That(a.Equals(null), Is.False, "Should return false when comparing with null");
        }

        [Test]
        public void Equals_DifferentType_ReturnsFalse()
        {
            var a = new PhoneNumber("912345678");
            Assert.Multiple(() =>
            {
                Assert.That(a.Equals("912345678"), Is.False, "Should return false when comparing with string");
                Assert.That(a.Equals(912345678), Is.False, "Should return false when comparing with number");
                Assert.That(a.Equals(new object()), Is.False, "Should return false when comparing with different type");
            });
        }

        [Test]
        public void GetHashCode_SameNumber_ReturnsSameHash()
        {
            var a = new PhoneNumber("912345678");
            var b = new PhoneNumber("912345678");
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()), 
                "Same numbers should have same hash code");
                
            // Test with different numbers
            var c = new PhoneNumber("962345678");
            Assert.That(a.GetHashCode(), Is.Not.EqualTo(c.GetHashCode()),
                "Different numbers should have different hash codes");
        }

        [Test]
        public void GetHashCode_DifferentNumbers_ReturnDifferentHash()
        {
            var a = new PhoneNumber("912345678");
            var b = new PhoneNumber("932165478");
            Assert.That(a.GetHashCode(), Is.Not.EqualTo(b.GetHashCode()));
        }
    }
}
