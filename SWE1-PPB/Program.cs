using Npgsql;
using System;
using System.Net;
using System.Threading.Tasks;

namespace SWE1_PPB
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 10001;
            string ipAddress = "127.0.0.1";
            DBHandler dBHandler = new DBHandler();
            User user = new User();
            BattleHandler battleHandler = new BattleHandler();

            //string version = "SELECT version()";
            //Console.WriteLine($"Using PostgreSQL version: {await dBHandler.ExecuteSQL(version)}\n");

            //string sql = "INSERT INTO users (username, password) values ('testuser', 'daniel')";

            //string sql = "select password from users";
            //Console.WriteLine($"Using PostgreSQL version: {await dBHandler.ExecuteSQL(sql)}\n");

            //await dBHandler.ExecuteInsertSQL(sql);

            if (args.Length != 0)
            {
                if (args[0] != null)
                    port = Int32.Parse(args[0]);
            }

            Server server = new Server(IPAddress.Parse(ipAddress), port);

        }
    }
}
