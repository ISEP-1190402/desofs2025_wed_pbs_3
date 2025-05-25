using LibraryOnlineRentalSystem.Domain.User;
using LibraryOnlineRentalSystem.Domain.Common;
using NUnit.Framework;
using System;

namespace LibraryOnlineRentalSystem.Tests.Domain.User
{
    [TestFixture]
    [TestOf(typeof(UserName))]
    public class UserNameTest
    {
        [TestCase("Alice123")]
        [TestCase("john_doe")]
        [TestCase("UserName1")]
        [TestCase("Name_With_Underscore")]
        public void Constructor_ValidUserName_SetsTag(string value)
        {
            var userName = new UserName(value);
            Assert.That(userName.Tag, Is.EqualTo(value));
        }

        [Test]
        public void Constructor_TrimsWhitespace()
        {
            var userName = new UserName("   alice   ");
            Assert.That(userName.Tag, Is.EqualTo("alice"));
        }

        [TestCase("")]
        [TestCase(null)]
        public void Constructor_NullOrEmpty_ThrowsArgumentNullException(string value)
        {
            Assert.Throws<ArgumentNullException>(() => new UserName(value));
        }

        [TestCase("123456")]              // Only numbers
        [TestCase("______")]              // Only underscores
        [TestCase("__user__")]            // Starts and ends with _
        [TestCase("_user")]               // Starts with _
        [TestCase("user_")]               // Ends with _
        [TestCase("user.name")]           // Special character .
        [TestCase("user-name")]           // Special character -
        [TestCase("user!")]               // Special character !
        [TestCase("!user")]               // Special character !
        [TestCase("user name")]           // Space not allowed
        [TestCase("name@user")]           // @ not allowed
        [TestCase("user$")]               // $ not allowed
        public void Constructor_InvalidUserName_ThrowsBusinessRulesException(string value)
        {
            Assert.Throws<BusinessRulesException>(() => new UserName(value));
        }

        [Test]
        public void Constructor_TooLong_ThrowsBusinessRulesException()
        {
            var longName = new string('a', 31);
            Assert.Throws<BusinessRulesException>(() => new UserName(longName));
        }

        [Test]
        public void Clone_ReturnsNewInstanceWithSameTag()
        {
            var userName = new UserName("bob_2024");
            var clone = (UserName)userName.Clone();
            Assert.That(clone, Is.Not.SameAs(userName));
            Assert.That(clone.Tag, Is.EqualTo("bob_2024"));
        }

        [Test]
        public void ToString_ReturnsTag()
        {
            var userName = new UserName("TestUser");
            Assert.That(userName.ToString(), Is.EqualTo("TestUser"));
        }

        [Test]
        public void Equals_SameUserName_CaseInsensitive_ReturnsTrue()
        {
            var a = new UserName("Alice123");
            var b = new UserName("alice123");
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void Equals_DifferentUserNames_ReturnsFalse()
        {
            var a = new UserName("Alice123");
            var b = new UserName("Bob123");
            Assert.That(a.Equals(b), Is.False);
        }

        [Test]
        public void Equals_Null_ReturnsFalse()
        {
            var a = new UserName("TestUser");
            Assert.That(a.Equals(null), Is.False);
        }

        [Test]
        public void Equals_DifferentType_ReturnsFalse()
        {
            var a = new UserName("User123");
            Assert.That(a.Equals("User123"), Is.False);
        }

        [Test]
        public void GetHashCode_SameUserName_ReturnsSameHash()
        {
            var a = new UserName("alice");
            var b = new UserName("ALICE");
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void GetHashCode_DifferentUserNames_ReturnDifferentHash()
        {
            var a = new UserName("alice");
            var b = new UserName("bob");
            Assert.That(a.GetHashCode(), Is.Not.EqualTo(b.GetHashCode()));
        }
    }
}
