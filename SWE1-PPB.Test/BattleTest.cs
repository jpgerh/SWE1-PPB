using NUnit.Framework;
using System.Threading.Tasks;

namespace SWE1_PPB.Test
{
    public class BattleTests
    {

        [Test]
        public async Task TestStandardBattle()
        {
            User user = new User();
            string username1 = "testuser1";
            string password1 = "testpw2";

            string username2 = "testuser1";
            string password2 = "testpw2";

            bool success = await user.Register(username1, password1);
            Assert.That(success);
            string token = await user.Login(username1, password1);
            Assert.That(token, Is.EqualTo($"Basic {username}-ppbToken"));
        }

    }
}