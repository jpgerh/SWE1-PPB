using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace SWE1_PPB.Test
{

    [TestFixture]
    public class UserTests
    {
        [SetUp]
        public void init()
        {
            SetupAsync().Wait();
        }

        public async Task SetupAsync()
        {
            DBHandler dBHandler = new DBHandler();

            string deleteActions = "DELETE FROM actions";
            string deleteLibrary = "DELETE FROM library";
            string deletePlaylist = "DELETE FROM playlist";
            string deleteStats = "DELETE FROM stats";
            string deleteTournamentscore = "DELETE FROM tournamentscore";
            string deleteUsers = "DELETE FROM users";

            await dBHandler.ExecuteInsertOrDeleteSQL(deleteActions);
            await dBHandler.ExecuteInsertOrDeleteSQL(deleteLibrary);
            await dBHandler.ExecuteInsertOrDeleteSQL(deletePlaylist);
            await dBHandler.ExecuteInsertOrDeleteSQL(deleteStats);
            await dBHandler.ExecuteInsertOrDeleteSQL(deleteTournamentscore);
            await dBHandler.ExecuteInsertOrDeleteSQL(deleteUsers);
        }

        [Test]
        public async Task TestRegisterNewUserAndLogin()
        {
            User user = new User();
            string username = "testuser";
            string password = "testpw";

            await user.Register(username, password);
            string token = await user.Login(username, password);            
            Assert.That(token, Is.EqualTo($"Basic {username}-ppbToken"));
        }

        [Test]
        public async Task TestVerifyToken()
        {
            User user = new User();
            string username = "testuser";
            string password = "testpw";

            await user.Register(username, password);
            string token = await user.Login(username, password);

            Assert.That(await user.verifyToken(token));
        }

        [Test]
        public async Task TestVerifyIsNotAdmin()
        {
            User user = new User();
            string username = "testuser";
            string password = "testpw";

            await user.Register(username, password);
            string token = await user.Login(username, password);

            Assert.IsFalse(await user.verifyAdmin(token));
        }

        [Test]
        public async Task TestEditUser()
        {
            User user = new User();
            string username = "testuser";
            string password = "testpw";

            await user.Register(username, password);
            string token = await user.Login(username, password);

            Assert.That(await user.editUser(token, username, "Testname", "Testbio", "Testimage"), Is.EqualTo("User edited."));
        }

        [Test]
        public async Task TestGetUserStat()
        {
            User user = new User();
            string username = "testuser";
            string password = "testpw";

            await user.Register(username, password);
            string token = await user.Login(username, password);

            Assert.That(await user.getUserStats(token), Is.EqualTo($"{token}: Authorization successful.\nCurrent stat for user '{username}': 100"));
        }

        [Test]
        public async Task TestAddActions()
        {
            User user = new User();
            string username = "testuser";
            string password = "testpw";

            await user.Register(username, password);
            string token = await user.Login(username, password);

            Assert.That(await user.addAction(token, "RRRRR"), Is.EqualTo("Added actions successfully.\n"));
        }

        [Test]
        public async Task TestGetActions()
        {
            User user = new User();
            string username = "testuser";
            string password = "testpw";

            await user.Register(username, password);
            string token = await user.Login(username, password);

            await TestAddActions();

            Assert.That(await user.getAction(token), Is.EqualTo($"{token}: Authorization successful.\nCurrent action for user '{username}': RRRRR\n"));
        }
    }
}