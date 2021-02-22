using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.Linq;

namespace SWE1_PPB
{
    public class BattleHandler
    {
        List<string> usernames = new List<string>();
        DBHandler dBHandler = new DBHandler();
        int interval = 1000;
        List<KeyValuePair<char, char>> effectiveness = new List<KeyValuePair<char, char>>()
        {
            new KeyValuePair<char, char>('R', 'S'),
            new KeyValuePair<char, char>('R', 'L'),
            new KeyValuePair<char, char>('S', 'P'),
            new KeyValuePair<char, char>('S', 'L'),
            new KeyValuePair<char, char>('V', 'S'),
            new KeyValuePair<char, char>('V', 'R'),
            new KeyValuePair<char, char>('L', 'S'),
            new KeyValuePair<char, char>('L', 'P'),
            new KeyValuePair<char, char>('P', 'R'),
            new KeyValuePair<char, char>('P', 'V')
        };

        public BattleHandler()
        {
            while(true)
            {
                CheckOnlineUsers();
                System.Threading.Thread.Sleep(interval);
            }
        }

        private async void CheckOnlineUsers()
        {
            string sql = "SELECT username FROM users WHERE online = true";
            usernames = await dBHandler.ExecuteMultiRowSingleSelectSQL(sql);

            string sql2 = "SELECT Count(*) FROM users JOIN actions ON users.username = actions.username WHERE online = true";
            int count = Int32.Parse(await dBHandler.ExecuteSingleSelectSQL(sql2));

            if (usernames.Count == count)
            {
                if (count >= 2) Battle();
            }
        }

        private async void Battle()
        {
            Dictionary<string, char[]> actions = new Dictionary<string, char[]>();
            Dictionary<string, int> roundscore = new Dictionary<string, int>();
            Dictionary<string, int> battlescore = new Dictionary<string, int>();

            List<string> processedUsernames = new List<string>();
            List<string> disqualifiedUsernames = new List<string>();

            // get the current max tournamentid from the database and add 1 for further inserts
            string idsql = "SELECT Count(*) FROM tournamentscores";
            int tournamentID = Int32.Parse(await dBHandler.ExecuteSingleSelectSQL(idsql)) + 1;

            Console.WriteLine(DateTime.Now);
            Console.WriteLine("####  Battle started ####" + Environment.NewLine + "Fighters:");
            foreach (string username in usernames)
            {
                Console.WriteLine(username);
                string sql = $"SELECT action FROM actions WHERE username = '{username}'";
                actions[username] = (await dBHandler.ExecuteSingleSelectSQL(sql)).ToCharArray();
            }

            // set round scores for all users to 0
            foreach (string username in usernames)
            {
                roundscore[username] = 0;
            }

            // set battle scores for all users to 0
            foreach (string username in usernames)
            {
                battlescore[username] = 0;
            }

            // lets fight
            foreach (string P1 in usernames)
            {
                foreach (string P2 in usernames)
                {
                    // P2 is already completely processed, no need to do it the other way around
                    if (!processedUsernames.Contains(P2) && !disqualifiedUsernames.Contains(P1) && !disqualifiedUsernames.Contains(P2))
                    {
                        if (!P2.Equals(P1))
                        {
                            Console.WriteLine("\n#### " + P1 + " vs " + P2 + " ####");

                            bool P1Special = false;
                            bool P2Special = false;


                            // check for RSVLP :O
                            string stringP1Special = new string(actions[P1]);
                            string stringP2Special = new string(actions[P2]);

                            if (stringP1Special.Equals("RSVLP") && stringP2Special.Equals("RSVLP"))
                            {
                                Console.WriteLine("\nBoth " + P2 + " and " + P1 + " chose the special Action. Both are disqualified from the tournament.\n");
                                Console.WriteLine("##################################");
                                disqualifiedUsernames.Add(P1);
                                disqualifiedUsernames.Add(P2);
                                break;
                            } 
                            else if (stringP1Special.Equals("RSVLP"))
                            {
                                Console.WriteLine("\n" + P1 + " beats " + P2 + " by using the special action.\n");
                                Console.WriteLine("##################################");
                                battlescore[P1] = battlescore[P1] + 2;
                                battlescore[P2] = battlescore[P1] - 2;
                                roundscore[P1] = 0;
                                roundscore[P2] = 0;
                                break;
                            }
                            else if (stringP2Special.Equals("RSVLP"))
                            {
                                Console.WriteLine("\n" + P2 + " beats " + P1 + " by using the special action.\n");
                                Console.WriteLine("##################################");
                                battlescore[P2] = battlescore[P2] + 2;
                                battlescore[P1] = battlescore[P1] - 2;
                                roundscore[P1] = 0;
                                roundscore[P2] = 0;
                                break;
                            }

                            for (int i = 0; i < 5; i++)
                            {

                                Console.WriteLine("\nRound " + (i+1) + ":");

                                char P1action = actions[P1][i];
                                char P2action = actions[P2][i];

                                bool draw = false;

                                foreach (KeyValuePair<char, char> pair in effectiveness)
                                {
                                    if (P1action.Equals(pair.Key) && P2action.Equals(pair.Value))
                                    {
                                        Console.WriteLine(P1action + " by " + P1 + " beats " + P2action + " by " + P2 + ".");
                                        roundscore[P1]++;
                                        draw = false;
                                        break;
                                    }
                                    else if (P2action.Equals(pair.Key) && P1action.Equals(pair.Value))
                                    {
                                        Console.WriteLine(P2action + " by " + P2 + " beats " + P1action + " by " + P1 + ".");
                                        roundscore[P2]++;
                                        draw = false;
                                        break;
                                    } else
                                    {
                                        draw = true;
                                    }
                                }
                                if (draw) Console.WriteLine(P1action + " by " + P1 + " and " + P2action + " by " + P2 + " are a draw.");
                            }

                            if (roundscore[P1] > roundscore[P2])
                            {
                                Console.WriteLine("\n" + P1 + " beats " + P2 + ".\n");
                                Console.WriteLine("##################################");
                                battlescore[P1]++;
                                battlescore[P2]--;
                                roundscore[P1] = 0;
                                roundscore[P2] = 0;
                            }
                            else if (roundscore[P2] > roundscore[P1])
                            {
                                Console.WriteLine("\n" + P2 + " beats " + P1 + ".\n");
                                Console.WriteLine("##################################");
                                battlescore[P2]++;
                                battlescore[P1]--;
                                roundscore[P1] = 0;
                                roundscore[P2] = 0;
                            }
                            else
                            {
                                // players are removed from the tournament
                                Console.WriteLine("\n" + P2 + " and " + P1 + " are even. Both are disqualified from the tournament.\n");
                                Console.WriteLine("##################################");
                                disqualifiedUsernames.Add(P1);
                                disqualifiedUsernames.Add(P2);
                            }
                        }
                    }
                }

                processedUsernames.Add(P1);

            }

            battlescore = battlescore.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            // check if its a battlescore draw and set admin
            if ((battlescore.Values.ElementAt(1)) < battlescore.Values.ElementAt(0))
            {

                string countsql = "SELECT COUNT(*) FROM users WHERE admin = true";
                int count = Int32.Parse(await dBHandler.ExecuteSingleSelectSQL(countsql));

                if (count > 0)
                {
                    string reset = $"UPDATE users SET admin = false WHERE admin = true";
                    await dBHandler.ExecuteInsertOrDeleteSQL(reset);
                }

                string update = $"UPDATE users SET admin = true WHERE username = '{battlescore.Keys.ElementAt(0)}'";
                await dBHandler.ExecuteInsertOrDeleteSQL(update);

                string selectScore = $"SELECT score FROM stats WHERE username = '{battlescore.Keys.ElementAt(0)}'";
                int score = Int32.Parse(await dBHandler.ExecuteSingleSelectSQL(selectScore)) + 1;

                string updateStats = $"UPDATE stats SET score = {score} WHERE username = '{battlescore.Keys.ElementAt(0)}'";
                await dBHandler.ExecuteInsertOrDeleteSQL(updateStats);

                Console.WriteLine("\n" + battlescore.Keys.ElementAt(0) + " won and is now administrator.\n");
            }

            Console.WriteLine("TournamentID: " + tournamentID);
            Console.WriteLine("Scoreboard: ");

            for (int i = 0; i < battlescore.Count; i++)
            {
                if (i > 0)
                {
                    if (battlescore.Values.ElementAt(i) < battlescore.Values.ElementAt(0))
                    {
                        string selectScore = $"SELECT score FROM stats WHERE username = '{battlescore.Keys.ElementAt(i)}'";
                        int score = Int32.Parse(await dBHandler.ExecuteSingleSelectSQL(selectScore)) - 1;

                        string updateStats = $"UPDATE stats SET score = {score} WHERE username = '{battlescore.Keys.ElementAt(i)}'";
                        await dBHandler.ExecuteInsertOrDeleteSQL(updateStats);
                    }
                    
                }

                string selectgamesPlayed = $"SELECT games_played FROM stats WHERE username = '{battlescore.Keys.ElementAt(i)}'";
                int gamesPlayed = Int32.Parse(await dBHandler.ExecuteSingleSelectSQL(selectgamesPlayed)) + 1;

                string updateGamesPlayed = $"UPDATE stats SET games_played = {gamesPlayed} WHERE username = '{battlescore.Keys.ElementAt(i)}'";
                await dBHandler.ExecuteInsertOrDeleteSQL(updateGamesPlayed);

                Console.WriteLine(battlescore.Keys.ElementAt(i) + " -> " + battlescore.Values.ElementAt(i) +  " points.");

                string insert = $"INSERT INTO tournamentscores VALUES ('{tournamentID}', '{battlescore.Keys.ElementAt(i)}', '{battlescore.Values.ElementAt(i)}')";
                await dBHandler.ExecuteInsertOrDeleteSQL(insert);
            }

            Console.WriteLine("\n\n");

            DBCleanup(usernames);

        }


        public async void DBCleanup(List<string> usernames)
        {
            foreach (string username in usernames)
            {
                // set online status to false
                string update = $"UPDATE users SET online = false WHERE username = '{username}'";
                await dBHandler.ExecuteInsertOrDeleteSQL(update);

                // remove actions
                string delete = $"DELETE FROM actions WHERE username = '{username}'";
                await dBHandler.ExecuteInsertOrDeleteSQL(delete);
            }
        }
    }
}
