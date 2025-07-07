using Singlife.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Singlife.Data
{
    public class RewardsTypeData
    {
        private readonly string _connectionString;

        public RewardsTypeData()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;
        }

        public List<RewardsType> ReadRewardsType()
        {
            List<RewardsType> rewardsTypes = new List<RewardsType>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT VoucherTypeID, VoucherName, Value FROM RewardsType";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            RewardsType rt = new RewardsType
                            {
                                VoucherTypeID = Convert.ToInt32(reader["VoucherTypeID"]),
                                VoucherName = reader["VoucherName"].ToString(),
                                Value = Convert.ToInt32(reader["Value"])
                            };

                            rewardsTypes.Add(rt);
                        }
                    }
                }
            }

            return rewardsTypes;
        }
    }
}