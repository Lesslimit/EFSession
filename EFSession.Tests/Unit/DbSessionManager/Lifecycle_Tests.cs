using EFSession.Schema.Resolvers.Contracts;
using EFSession.Session;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace EFSession.Tests.Unit.DbSessionManager
{
    public class Lifecycle_Tests
    {
        [TestCase("dbo")]
        public void Should_HasAliveSessions_After_Start(string schema)
        {
            #region Mocks Setup

            var dbContextMock = new Mock<IDbContext>();
            var dbContextProviderMock = new Mock<IDbContextProvider<IDbContext>>();
            var seedSessionProviderMock = new Mock<ISeedSessionProvider>();
            var schemaResolverMock = new Mock<ISchemaResolver>();
            var seedSessionMock = new Mock<IDbSeedSession<IDbSession>>();
            var dbSessionMock = new Mock<IDbSession>();

            SetupMocks(schema, dbContextProviderMock, dbContextMock, seedSessionProviderMock, seedSessionMock, dbSessionMock);

            #endregion


            var dbSessionManager = new Session.DbSessionManager(dbContextProviderMock.Object, seedSessionProviderMock.Object, new[] { schemaResolverMock.Object });
            dbSessionManager.Start(schema);


            dbSessionManager.HasAliveSessions.Should().BeTrue();
        }


        [TestCase("dbo")]
        public void Should_Remove_Alive_Sessions_On_Dispose(string schema)
        {
            #region Mocks Setup

            var dbContextMock = new Mock<IDbContext>();
            var dbContextProviderMock = new Mock<IDbContextProvider<IDbContext>>();
            var seedSessionProviderMock = new Mock<ISeedSessionProvider>();
            var schemaResolverMock = new Mock<ISchemaResolver>();
            var seedSessionMock = new Mock<IDbSeedSession<IDbSession>>();
            var dbSessionMock = new Mock<IDbSession>();

            SetupMocks(schema, dbContextProviderMock, dbContextMock, seedSessionProviderMock, seedSessionMock, dbSessionMock);

            #endregion


            var dbSessionManager = new Session.DbSessionManager(dbContextProviderMock.Object, seedSessionProviderMock.Object, new[] { schemaResolverMock.Object });
            dbSessionManager.Start(schema);
            dbSessionManager.Dispose();

            dbSessionManager.HasAliveSessions.Should().BeFalse();
        }

        private static void SetupMocks(string schema,
            Mock<IDbContextProvider<IDbContext>> dbContextProviderMock,
            IMock<IDbContext> dbContextMock,
            Mock<ISeedSessionProvider> seedSessionProviderMock,
            Mock<IDbSeedSession<IDbSession>> seedSessionMock,
            Mock<IDbSession> dbSessionMock)
        {
            dbContextProviderMock.Setup(cp => cp.ForSchema(It.Is<string>(v => v == schema)))
                                 .Returns(dbContextMock.Object);

            seedSessionProviderMock.Setup(cp => cp.ForSchema(It.Is<string>(v => v == schema)))
                                   .Returns(seedSessionProviderMock.Object);
            seedSessionProviderMock.Setup(cp => cp.Resolve<IDbSeedSession<IDbSession>>(It.Is<IDbContext>(v => v == dbContextMock.Object)))
                                   .Returns(seedSessionMock.Object);

            seedSessionMock.SetupGet(ds => ds.InnerSession).Returns(dbSessionMock.Object);

            dbSessionMock.SetupGet(db => db.IsActive).Returns(true);
        }
    }
}