using Npgsql;
using System;
using System.Net;
using System.Threading.Tasks;

namespace SWE1_PPB
{
    class Program
    {
        static async Task Main(string[] args)
        {
            int port = 10001;
            string ipAddress = "127.0.0.1";
            var connString = "Host=localhost;Username=postgres;Password=postgres";

            Console.WriteLine("Establishing database connection...");

            await using var conn = new NpgsqlConnection(connString);

            try
            {
                await conn.OpenAsync();

                Console.WriteLine("Connection established!");

                var sql = "SELECT version()";

                using var cmd = new NpgsqlCommand(sql, conn);

                var version = cmd.ExecuteScalar().ToString();
                Console.WriteLine($"PostgreSQL version: {version}\n");
            } 
            catch (Exception e)
            {
                Console.WriteLine(e.Message + ": Please check username and password.\n");
            }

            if (args.Length != 0)
            {
                if (args[0] != null)
                    port = Int32.Parse(args[0]);
            }

            Server server = new Server(IPAddress.Parse(ipAddress), port, conn);






        }
    }
}
