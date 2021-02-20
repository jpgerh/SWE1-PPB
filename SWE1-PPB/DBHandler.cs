using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SWE1_PPB
{
    class DBHandler
    {
        string connString = "Host=localhost;Username=postgres;Password=postgres";

        public async Task<string> ExecuteSingleSelectSQL(string sql)
        {
            try
            {
                await using var conn = new NpgsqlConnection(connString);
                await conn.OpenAsync();

                using var cmd = new NpgsqlCommand(sql, conn);
                var result = cmd.ExecuteScalar().ToString();

                return result;

            }
            catch (NullReferenceException nre)
            {
                return nre.Message;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + ": Please check statement.\n");

                return "DB_ERROR";
            }

        }

        public async Task<bool> ExecuteInsertSQL(string sql)
        {
            try
            {
                await using var conn = new NpgsqlConnection(connString);
                await conn.OpenAsync();

                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + ": Please check statement.\n");
                return false;
            }
            return true;

        }

    }
}
