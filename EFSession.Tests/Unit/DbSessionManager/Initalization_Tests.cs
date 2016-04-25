using EFSession.Schema.Resolvers.Contracts;
using EFSession.Session;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace EFSession.Tests.Unit.DbSessionManager
{
    [TestFixture]
    public class Initalization_Tests
    {
        [Test]
        public void Should_Have_False_HasAliveSessions_Property()
        {
            var dbContextProviderMock = new Mock<IDbContextProvider<IDbContext>>();
            var seedSessionProviderMock = new Mock<ISeedSessionProvider>();
            var schemaResolverMock = new Mock<ISchemaResolver>();


            var dbSessionManager = new Session.DbSessionManager(dbContextProviderMock.Object, seedSessionProviderMock.Object, new[] { schemaResolverMock.Object });


            dbSessionManager.HasAliveSessions.Should().BeFalse("DbSessionManager was just created, and no session were activated");
        }
    }
}
