using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SWE1_PPB.Test
{
    [TestFixture]
    public class BattleTests
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
        public async Task TestStandardBattle()
        {
            User user = new User();
            DBHandler dBHandler = new DBHandler();

            string username1 = "testuser1";
            string password1 = "testpw1";

            string username2 = "testuser2";
            string password2 = "testpw2";

            // register users for battle
            await user.Register(username1, password1);
            await user.Register(username2, password2);

            // login users for battle
            string token1 = await user.Login(username1, password1);
            string token2 = await user.Login(username2, password2);

            // manually add values to database
            string insertAction1 = $"INSERT INTO actions VALUES ('{username1}', 'SSSSS')";
            string insertAction2 = $"INSERT INTO actions VALUES ('{username2}', 'RRRRR')";
            await dBHandler.ExecuteInsertOrDeleteSQL(insertAction1);
            await dBHandler.ExecuteInsertOrDeleteSQL(insertAction2);

            BattleHandler battleHandler;
            Thread battleThread = new Thread(() => battleHandler = new BattleHandler());
            battleThread.Start();

            user.setOnline(token1);
            user.setOnline(token2);

            Thread.Sleep(15000);

            string selectGamesPlayed1 = $"SELECT games_played FROM stats WHERE username = '{username1}'";
            string selectGamesPlayed2 = $"SELECT games_played FROM stats WHERE username = '{username2}'";
            int gamesPlayed1 = Int32.Parse(await dBHandler.ExecuteSingleSelectSQL(selectGamesPlayed1));
            int gamesPlayed2 = Int32.Parse(await dBHandler.ExecuteSingleSelectSQL(selectGamesPlayed2));

            Assert.That(gamesPlayed1 == 1);
            Assert.That(gamesPlayed2 == 1);

            string selectScore1 = $"SELECT score FROM stats WHERE username = '{username1}'";
            string selectScore2 = $"SELECT score FROM stats WHERE username = '{username2}'";
            int score1 = Int32.Parse(await dBHandler.ExecuteSingleSelectSQL(selectScore1));
            int score2 = Int32.Parse(await dBHandler.ExecuteSingleSelectSQL(selectScore2));

            Assert.That(score1 == 99);
            Assert.That(score2 == 101);

            battleThread.Join(3000);
        }

        [Test]
        public async Task TestStandardBattleDraw()
        {
            User user = new User();
            DBHandler dBHandler = new DBHandler();

            string username1 = "testuser1";
            string password1 = "testpw1";

            string username2 = "testuser2";
            string password2 = "testpw2";

            // register users for battle
            await user.Register(username1, password1);
            await user.Register(username2, password2);

            // login users for battle
            string token1 = await user.Login(username1, password1);
            string token2 = await user.Login(username2, password2);

            // manually add values to database
            string insertAction1 = $"INSERT INTO actions VALUES ('{username1}', 'SSSSS')";
            string insertAction2 = $"INSERT INTO actions VALUES ('{username2}', 'SSSSS')";
            await dBHandler.ExecuteInsertOrDeleteSQL(insertAction1);
            await dBHandler.ExecuteInsertOrDeleteSQL(insertAction2);

            BattleHandler battleHandler;
            Thread battleThread = new Thread(() => battleHandler = new BattleHandler());
            battleThread.Start();

            user.setOnline(token1);
            user.setOnline(token2);

            Thread.Sleep(15000);

            string selectGamesPlayed1 = $"SELECT games_played FROM stats WHERE username = '{username1}'";
            string selectGamesPlayed2 = $"SELECT games_played FROM stats WHERE username = '{username2}'";
            int gamesPlayed1 = Int32.Parse(await dBHandler.ExecuteSingleSelectSQL(selectGamesPlayed1));
            int gamesPlayed2 = Int32.Parse(await dBHandler.ExecuteSingleSelectSQL(selectGamesPlayed2));

            Assert.That(gamesPlayed1 == 1);
            Assert.That(gamesPlayed2 == 1);

            string selectScore1 = $"SELECT score FROM stats WHERE username = '{username1}'";
            string selectScore2 = $"SELECT score FROM stats WHERE username = '{username2}'";
            int score1 = Int32.Parse(await dBHandler.ExecuteSingleSelectSQL(selectScore1));
            int score2 = Int32.Parse(await dBHandler.ExecuteSingleSelectSQL(selectScore2));

            Assert.That(score1 == 100);
            Assert.That(score2 == 100);

            battleThread.Join(3000);
        }

        [Test]
        public async Task TestSpecialBattleDraw()
        {
            User user = new User();
            DBHandler dBHandler = new DBHandler();

            string username1 = "testuser1";
            string password1 = "testpw1";

            string username2 = "testuser2";
            string password2 = "testpw2";

            // register users for battle
            await user.Register(username1, password1);
            await user.Register(username2, password2);

            // login users for battle
            string token1 = await user.Login(username1, password1);
            string token2 = await user.Login(username2, password2);

            // manually add values to database
            string insertAction1 = $"INSERT INTO actions VALUES ('{username1}', 'RSVLP')";
            string insertAction2 = $"INSERT INTO actions VALUES ('{username2}', 'RSVLP')";
            await dBHandler.ExecuteInsertOrDeleteSQL(insertAction1);
            await dBHandler.ExecuteInsertOrDeleteSQL(insertAction2);

            BattleHandler battleHandler;
            Thread battleThread = new Thread(() => battleHandler = new BattleHandler());
            battleThread.Start();

            user.setOnline(token1);
            user.setOnline(token2);

            Thread.Sleep(15000);

            string selectGamesPlayed1 = $"SELECT games_played FROM stats WHERE username = '{username1}'";
            string selectGamesPlayed2 = $"SELECT games_played FROM stats WHERE username = '{username2}'";
            int gamesPlayed1 = Int32.Parse(await dBHandler.ExecuteSingleSelectSQL(selectGamesPlayed1));
            int gamesPlayed2 = Int32.Parse(await dBHandler.ExecuteSingleSelectSQL(selectGamesPlayed2));

            Assert.That(gamesPlayed1 == 1);
            Assert.That(gamesPlayed2 == 1);

            string selectScore1 = $"SELECT score FROM stats WHERE username = '{username1}'";
            string selectScore2 = $"SELECT score FROM stats WHERE username = '{username2}'";
            int score1 = Int32.Parse(await dBHandler.ExecuteSingleSelectSQL(selectScore1));
            int score2 = Int32.Parse(await dBHandler.ExecuteSingleSelectSQL(selectScore2));

            Assert.That(score1 == 100);
            Assert.That(score2 == 100);

            battleThread.Join(3000);
        }

        [Test]
        public async Task TestSpecialBattle()
        {
            User user = new User();
            DBHandler dBHandler = new DBHandler();

            string username1 = "testuser1";
            string password1 = "testpw1";

            string username2 = "testuser2";
            string password2 = "testpw2";

            // register users for battle
            await user.Register(username1, password1);
            await user.Register(username2, password2);

            // login users for battle
            string token1 = await user.Login(username1, password1);
            string token2 = await user.Login(username2, password2);

            // manually add values to database
            string insertAction1 = $"INSERT INTO actions VALUES ('{username1}', 'RSVLP')";
            string insertAction2 = $"INSERT INTO actions VALUES ('{username2}', 'RRRRR')";
            await dBHandler.ExecuteInsertOrDeleteSQL(insertAction1);
            await dBHandler.ExecuteInsertOrDeleteSQL(insertAction2);

            BattleHandler battleHandler;
            Thread battleThread = new Thread(() => battleHandler = new BattleHandler());
            battleThread.Start();

            user.setOnline(token1);
            user.setOnline(token2);

            Thread.Sleep(15000);

            string selectGamesPlayed1 = $"SELECT games_played FROM stats WHERE username = '{username1}'";
            string selectGamesPlayed2 = $"SELECT games_played FROM stats WHERE username = '{username2}'";
            int gamesPlayed1 = Int32.Parse(await dBHandler.ExecuteSingleSelectSQL(selectGamesPlayed1));
            int gamesPlayed2 = Int32.Parse(await dBHandler.ExecuteSingleSelectSQL(selectGamesPlayed2));

            Assert.That(gamesPlayed1 == 1);
            Assert.That(gamesPlayed2 == 1);

            string selectScore1 = $"SELECT score FROM stats WHERE username = '{username1}'";
            string selectScore2 = $"SELECT score FROM stats WHERE username = '{username2}'";
            int score1 = Int32.Parse(await dBHandler.ExecuteSingleSelectSQL(selectScore1));
            int score2 = Int32.Parse(await dBHandler.ExecuteSingleSelectSQL(selectScore2));

            Assert.That(score1 == 101);
            Assert.That(score2 == 99);

            battleThread.Join(3000);
        }

    }
}