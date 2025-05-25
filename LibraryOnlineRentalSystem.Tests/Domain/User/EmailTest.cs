using LibraryOnlineRentalSystem.Domain.User;
using LibraryOnlineRentalSystem.Domain.Common;
using NUnit.Framework;
using System;

namespace LibraryOnlineRentalSystem.Tests.Domain.User
{
    [TestFixture]
    [TestOf(typeof(Email))]
    public class EmailTest
    {
        [TestCase("user@example.com")]
        [TestCase("John.Doe99@domain.co.uk")]
        [TestCase("user.name+tag+sorting@example.com")]
        [TestCase("USER@EXAMPLE.COM")]
        public void Constructor_ValidEmail_SetsEmailAddress(string validEmail)
        {
            var email = new Email(validEmail);
            Assert.That(email.EmailAddress, Is.EqualTo(validEmail.Trim()));
        }

        [Test]
        public void Constructor_TrimsWhitespace()
        {
            var email = new Email("  user@domain.com  ");
            Assert.That(email.EmailAddress, Is.EqualTo("user@domain.com"));
        }

        [TestCase("")]
        [TestCase("   ")]
        [TestCase("invalid-email")]
        [TestCase("plainaddress")]
        [TestCase("user@.com")]
        [TestCase("user.com")]
        [TestCase("@domain.com")]
        [TestCase("user@domain")]
        [TestCase("user@domain,com")]
        [TestCase("user@domain..com")]
        public void Constructor_InvalidEmail_ThrowsBusinessRulesException(string invalidEmail)
        {
            Assert.Throws<BusinessRulesException>(() => new Email(invalidEmail));
        }

        [Test]
        public void Clone_ReturnsNewInstanceWithSameEmail()
        {
            var email = new Email("a@b.com");
            var clone = (Email)email.Clone();
            Assert.That(clone, Is.Not.SameAs(email));
            Assert.That(clone.EmailAddress, Is.EqualTo("a@b.com"));
        }

        [Test]
        public void ToString_ReturnsEmailAddress()
        {
            var email = new Email("user@domain.com");
            Assert.That(email.ToString(), Is.EqualTo("user@domain.com"));
        }

        [Test]
        public void Equals_SameEmail_CaseInsensitive_ReturnsTrue()
        {
            var a = new Email("AbC@domain.com");
            var b = new Email("abc@DOMAIN.com");
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void Equals_DifferentEmail_ReturnsFalse()
        {
            var a = new Email("user1@domain.com");
            var b = new Email("user2@domain.com");
            Assert.That(a.Equals(b), Is.False);
        }

        [Test]
        public void Equals_Null_ReturnsFalse()
        {
            var a = new Email("user@domain.com");
            Assert.That(a.Equals(null), Is.False);
        }

        [Test]
        public void Equals_DifferentType_ReturnsFalse()
        {
            var a = new Email("user@domain.com");
            Assert.That(a.Equals("user@domain.com"), Is.False);
        }

        [Test]
        public void GetHashCode_SameEmail_ReturnsSameHash()
        {
            var a = new Email("a@b.com");
            var b = new Email("A@B.COM");
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void GetHashCode_DifferentEmails_ReturnDifferentHash()
        {
            var a = new Email("user1@domain.com");
            var b = new Email("user2@domain.com");
            Assert.That(a.GetHashCode(), Is.Not.EqualTo(b.GetHashCode()));
        }
    }
}
