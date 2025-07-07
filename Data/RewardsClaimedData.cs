using Singlife.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Singlife.Data
{
    public class RewardsClaimedData
    {
        private readonly string _connectionString;

        public RewardsClaimedData()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;
        }

        public bool AddRewardsClaimed(int accountID, int voucherID, int amountClaim, DateTime dateCreated)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO RewardsClaimed
                                 (AccountID, VoucherID, AmountClaim, DateCreated)
                                 VALUES
                                 (@AccountID, @VoucherID, @AmountClaim, @DateCreated)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountID);
                    cmd.Parameters.AddWithValue("@VoucherID", voucherID);
                    cmd.Parameters.AddWithValue("@AmountClaim", amountClaim);
                    cmd.Parameters.AddWithValue("@DateCreated", dateCreated);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public RewardsClaimed ReadRewardsClaimed(int accountID, int voucherTypeID)
        {
            RewardsClaimed reward = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT TOP 1 *
                                 FROM RewardsClaimed
                                 WHERE AccountID = @AccountID AND VoucherTypeID = @VoucherTypeID
                                 ORDER BY DateCreated DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountID);
                    cmd.Parameters.AddWithValue("@VoucherTypeID", voucherTypeID);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            reward = new RewardsClaimed
                            {
                                RewardID = Convert.ToInt32(reader["RewardID"]),
                                AccountID = Convert.ToInt32(reader["AccountID"]),
                                VoucherTypeID = Convert.ToInt32(reader["VoucherTypeID"]),
                                AmountClaim = Convert.ToInt32(reader["AmountClaim"]),
                                DateCreated = Convert.ToDateTime(reader["DateCreated"])
                            };
                        }
                    }
                }
            }

            return reward;
        }
    }
}