using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
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
            bool success1 = await dBHandler.ExecuteInsertOrDeleteSQL(sql);

            string insert = $"INSERT INTO stats (username) VALUES ('{username}')";
            bool success2 = await dBHandler.ExecuteInsertOrDeleteSQL(insert);

            if (success1 && success2)
            {
                return true;
            }
            return false;
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
        public async Task<string> editUser(string token, string username, string name, string bio, string image)
        {
            string countSQL = $"SELECT COUNT(*) FROM users WHERE token = '{token}' AND username = '{username}'";
            int count = Int32.Parse(await dBHandler.ExecuteSingleSelectSQL(countSQL));

            Console.WriteLine(count.ToString());

            if (count >= 1)
            {
                string update = $"UPDATE users SET name = '{name}', bio = '{bio}', image = '{image}' WHERE token = '{token}'";
                await dBHandler.ExecuteInsertOrDeleteSQL(update);
                return "User edited.";
            }

            return $"{token}: Authorization failed. Wrong token used.";

            
        }

        // starts
        public async void setOnline(string token)
        {
            string update = $"UPDATE users SET online = true WHERE token = '{token}'";
            await dBHandler.ExecuteInsertOrDeleteSQL(update);

            Thread thread = new Thread(() => setOffline(token));
            thread.Start();
        }

        public async void setOffline(string token)
        {
            Thread.Sleep(15000);
            string update = $"UPDATE users SET online = false WHERE token = '{token}'";
            await dBHandler.ExecuteInsertOrDeleteSQL(update);
        }

        public async Task<string> getUserData(string token, string username)
        {
            DataTable dataTable = new DataTable();

            string countSQL = $"SELECT COUNT(*) FROM users WHERE token = '{token}' AND username = '{username}'";
            int count = Int32.Parse(await dBHandler.ExecuteSingleSelectSQL(countSQL));

            if (count >= 1)
            {
                string sql = $"SELECT name, bio, image FROM users WHERE username = '{username}' AND token = '{token}'";
                dataTable = await dBHandler.ExecuteSQLGetDT(sql);

                DataRow dataRow = dataTable.Rows[0];

                string returnString = $"{token}: Authorization successful.\nCurrent user data:\nUsername: " + username + "\nName: " + dataRow["Name"] + "\nBio: " + dataRow["Bio"] + "\nImage: " + dataRow["Image"] + "\n";

                return returnString;
            }
            return $"{token}: Authorization failed. Wrong token used.";
        }

        public async Task<string> getUserStats(string token)
        {
            string select = $"SELECT username FROM users WHERE token = '{token}'";
            string username = await dBHandler.ExecuteSingleSelectSQL(select);

            string sql = $"SELECT score FROM stats WHERE username = '{username}'";
            int score = Int32.Parse(await dBHandler.ExecuteSingleSelectSQL(sql));

            string returnString = $"{token}: Authorization successful.\nCurrent stat for user '{username}': {score}";

            return returnString;
        }

        public async Task<string> getScoreboard(string token)
        {
            DataTable dataTable = new DataTable();

            string sql = $"SELECT * FROM stats order by score desc";
            dataTable = await dBHandler.ExecuteSQLGetDT(sql);

            string returnString = $"{token}: Authorization successful.\nScoreboard:";

            foreach (DataRow row in dataTable.Rows)
            {
                returnString = returnString + "\n-----------------\nUsername: " + row["username"] + "\nGames played: " + row["games_played"] + "\nScore: " + row["score"] + "\n-----------------\n";
            }

            return returnString;
        }

        public async Task<string> getAction(string token)
        {
            string select = $"SELECT username FROM users WHERE token = '{token}'";
            string username = await dBHandler.ExecuteSingleSelectSQL(select);

            string selectCount = $"SELECT COUNT(*) FROM actions WHERE username = '{username}'";
            int count = Int32.Parse(await dBHandler.ExecuteSingleSelectSQL(selectCount));

            if (count >= 1)
            {
                string sql = $"SELECT action FROM actions WHERE username = '{username}'";
                string action = await dBHandler.ExecuteSingleSelectSQL(sql);

                string returnString = $"{token}: Authorization successful.\nCurrent action for user '{username}': {action}\n";

                return returnString;
            } 

            return $"No actions set for user {username}.\n";
        }

        public async Task<string> addAction(string token, string actions)
        {
            string allowedLetters = "RSVLP";
            bool invalid = false;

            string select = $"SELECT username FROM users WHERE token = '{token}'";
            string username = await dBHandler.ExecuteSingleSelectSQL(select);

            if (actions.Length == 5)
            {
                foreach (char c in actions)
                {
                    if (!allowedLetters.Contains(c.ToString()))
                    {
                        invalid = true;
                        break;
                    }
                }
            } else invalid = true;
            
            if (!invalid)
            {
                string insert = $"INSERT INTO actions VALUES ('{username}', '{actions}')";
                await dBHandler.ExecuteInsertOrDeleteSQL(insert);

                return "Added actions successfully.\n";

            } else
            {
                return "Invalid action set.\n";
            }
            

            
        }

    }
}
