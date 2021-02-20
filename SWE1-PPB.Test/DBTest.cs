using NUnit.Framework;
using System.Threading.Tasks;

namespace SWE1_PPB.Test
{
    public class Tests
    {

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task RegisterNewUserAndLogin()
        {
            User user = new User();
            string username = "testuser";
            string password = "testpw";

            bool success = await user.Register(username, password);
            Assert.That(success);
            string token = await user.Login(username, password);
            Assert.That(token, Is.EqualTo($"Basic {username}-ppbToken"));
        }
    }
}