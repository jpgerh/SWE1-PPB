using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SWE1_PPB
{
    public class User
    {       
        DBHandler dBHandler = new DBHandler();

        // add entry to db, answer with success state
        public async Task<bool> Register(string username, string password)
        {
            string sql = $"INSERT INTO users (username, password, online, admin) VALUES ('{username}', '{password}', false, false)";

            bool success = await dBHandler.ExecuteInsertOrDeleteSQL(sql);

            if (!success)
            {
                return false;
            }
            return true;
        }

        // lookup values in db and verify, generate token
        // set variables in class
        public async Task<string> Login(string username, string password)
        {
            string sql = $"SELECT password FROM users WHERE username = '{username}'";

            string result = await dBHandler.ExecuteSingleSelectSQL(sql);

            if (result.Equals(password))
            {
                string token = $"Basic {username}-ppbToken";
                string insert = $"UPDATE users SET token = '{token}' WHERE username = '{username}'";

                bool success = await dBHandler.ExecuteInsertOrDeleteSQL(insert);

                if (success)
                {
                    return token;
                }

                // insert token into db
            }
            return "";
        }

        public async Task<bool> verifyToken(string token)
        {
            string sql = $"SELECT COUNT(*) FROM users WHERE token = '{token}'";
            string result = await dBHandler.ExecuteSingleSelectSQL(sql);

            int count = Int32.Parse(result);

            if (count == 1) return true;
            else return false;
        }

        public async Task<bool> verifyAdmin(string token)
        {
            string sql = $"SELECT COUNT(*) FROM users WHERE token = '{token}' AND admin = true";
            string result = await dBHandler.ExecuteSingleSelectSQL(sql);

            int count = Int32.Parse(result);

            if (count == 1) return true;
            else return false;
        }

        // edits user data and updates the db
        public void editUser(string completePayload)
        {

        }

        // starts
        public void startBattle(string token)
        {

        }

        public void manageLibrary()
        {

        }

    }
}
