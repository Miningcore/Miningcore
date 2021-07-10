using System;
using System.Data;
using System.Threading.Tasks;
using Npgsql;

namespace Miningcore.Persistence.Postgres
{
    public class PgConnectionFactory : IConnectionFactory
    {
        public PgConnectionFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }

        private readonly string connectionString;

        /// <summary>
        /// Testing Postgress Database Connection
        /// </summary>
        /// <returns></returns>
        public async Task<bool> TestConnectionAsync()
        {
            await using(NpgsqlConnection con = new(connectionString))
            {
                await con.OpenAsync();
                if(con.State == System.Data.ConnectionState.Open)
                {
                    //Console.WriteLine("Success open postgreSQL connection.");
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// This implementation ensures that Glimpse.ADO is able to collect data
        /// </summary>
        /// <returns></returns>
        public async Task<IDbConnection> OpenConnectionAsync()
        {
            if(await TestConnectionAsync()) {
                NpgsqlConnection con = new(connectionString);
                await con.OpenAsync();
                return con;
            } 

            return null;
        }
    }
}
