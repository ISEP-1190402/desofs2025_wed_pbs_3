using LibraryOnlineRentalSystem.Domain.User;
using LibraryOnlineRentalSystem.Domain.Common;
using NUnit.Framework;
using System;

namespace LibraryOnlineRentalSystem.Tests.Domain.User
{
    [TestFixture]
    [TestOf(typeof(UserID))]
    public class UserIDTest
    {
        [Test]
        public void Constructor_GuidValue_SetsValue()
        {
            var guid = Guid.NewGuid();
            var userId = new UserID(guid);
            Assert.That(userId.AsGuid(), Is.EqualTo(guid));
            Assert.That(userId.AsString(), Is.EqualTo(guid.ToString()));
        }

        [Test]
        public void Constructor_GuidEmpty_ThrowsBusinessRulesException()
        {
            Assert.Throws<BusinessRulesException>(() => new UserID(Guid.Empty));
        }

        [Test]
        public void Constructor_ValidStringGuid_SetsValue()
        {
            var guid = Guid.NewGuid();
            var userId = new UserID(guid.ToString());
            Assert.That(userId.AsGuid(), Is.EqualTo(guid));
            Assert.That(userId.AsString(), Is.EqualTo(guid.ToString()));
        }

        /*[Test]
        public void Constructor_EmptyString_ThrowsBusinessRulesException()
        {
            var ex = Assert.Throws<FormatException>(() => new UserId(""));
            Assert.That(ex.Message, Is.EqualTo("ID cannot be null or empty."));
        }

        [Test]
        public void Constructor_NullString_ThrowsBusinessRulesException()
        {
            var ex = Assert.Throws<NullReferenceException>(() => new UserId(null));
            Assert.That(ex.Message, Is.EqualTo("ID cannot be null or empty."));
        }*/
        
        [Test]
        public void createFromString_ReturnsGuid()
        {
            var guid = Guid.NewGuid();
            var userId = new UserID(guid);
            var result = userId.GetType().GetMethod("createFromString", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(userId, new object[] { guid.ToString() });
            Assert.That(result, Is.TypeOf<Guid>());
            Assert.That((Guid)result, Is.EqualTo(guid));
        }

        [Test]
        public void AsString_ReturnsGuidAsString()
        {
            var guid = Guid.NewGuid();
            var userId = new UserID(guid);
            Assert.That(userId.AsString(), Is.EqualTo(guid.ToString()));
        }

        [Test]
        public void AsGuid_ReturnsGuid()
        {
            var guid = Guid.NewGuid();
            var userId = new UserID(guid);
            Assert.That(userId.AsGuid(), Is.EqualTo(guid));
        }
    }
}
