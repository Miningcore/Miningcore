using System.Data;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Miningcore.Extensions;
using Miningcore.Persistence.Model;
using Miningcore.Persistence.Repositories;
using NLog;

namespace Miningcore.Persistence.Postgres.Repositories
{
    public class MinerRepository : IMinerRepository
    {
        public MinerRepository(IMapper mapper)
        {
            this.mapper = mapper;
        }

        private readonly IMapper mapper;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public async Task<MinerSettings> GetSettings(IDbConnection con, IDbTransaction tx, string poolId, string address)
        {
            logger.LogInvoke();

            const string query = "SELECT * FROM miner_settings WHERE poolid = @poolId AND address = @address";

            var entity = await con.QuerySingleOrDefaultAsync<Entities.MinerSettings>(query, new {poolId, address});

            return mapper.Map<MinerSettings>(entity);
        }

        public async Task<MinerSettings> UpdateSettings(IDbConnection con, IDbTransaction tx, MinerSettings settings)
        {
            const string query = "INSERT INTO miner_settings(poolid, address, paymentthreshold, created, updated) " +
                                 "VALUES(@poolid, @address, @paymentthreshold, now(), now()) " +
                                 "ON CONFLICT ON CONSTRAINT miner_settings_pkey DO UPDATE " +
                                 "SET paymentthreshold = @paymentthreshold, updated = now() " +
                                 "WHERE miner_settings.poolid = @poolid AND miner_settings.address = @address";

            await con.ExecuteAsync(query, settings, tx);

            return await GetSettings(con, tx, settings.PoolId, settings.Address);
        }
    }
}
