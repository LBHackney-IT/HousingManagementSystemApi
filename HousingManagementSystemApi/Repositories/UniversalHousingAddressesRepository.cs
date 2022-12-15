namespace HousingManagementSystemApi.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using Ardalis.GuardClauses;
    using Dapper;
    using HACT.Dtos;
    using HousingManagementSystemApi.Repositories.Interfaces;
    using JetBrains.Annotations;
    using Models;

    public class UniversalHousingAddressesRepository : IAddressesRepository
    {
        private readonly Func<IDbConnection> _dbConnectionFunc;

        public UniversalHousingAddressesRepository([NotNull] Func<IDbConnection> dbConnectionFunc)
        {
            Guard.Against.Null(dbConnectionFunc, nameof(dbConnectionFunc));

            _dbConnectionFunc = dbConnectionFunc;
        }

        public async Task<IEnumerable<PropertyAddress>> GetAddressesByPostcode([NotNull] string postCode)
        {
            Guard.Against.NullOrWhiteSpace(postCode, nameof(postCode));

            var sql = @$"SELECT RTRIM(pr.prop_ref) AS PropertyReference
                ,pr.u_nlpguprn AS Uprn
                ,RTRIM(pr.post_desig) AS PostDesig
                ,RTRIM(REPLACE(pr.short_address,rtrim(pr.post_desig),'')) AS ShortAddress
                ,RTRIM(pr.post_code) as PostCode
                FROM property AS pr
                --rent details
                    LEFT OUTER JOIN rent AS r
                    ON pr.prop_ref = r.prop_ref
                --tenancy
                    LEFT OUTER JOIN tenagree AS ten
                    ON pr.prop_ref = ten.prop_ref
                --property type description
                    LEFT OUTER JOIN (SELECT [lu_ref],[lu_desc] FROM [lookup] WHERE lu_type ='GPT') AS ptype
                    ON r.prop_type = ptype.lu_ref
                WHERE level_code in ('2')
                AND eot = '1900-01-01 00:00:00'
                AND (UPPER(REPLACE(post_code, ' ','')) like UPPER(REPLACE('%{postCode}%', ' ','')))
            ";

            var result = Enumerable.Empty<PropertyAddress>();
            using (var dbConnection = _dbConnectionFunc())
            {
                var universalHousingAddresses = await dbConnection.QueryAsync<UniversalHousingAddress>(sql);
                result = universalHousingAddresses.Select(uhAddress => uhAddress.ToHactPropertyAddress());
            }

            return result;
        }
    }
}
