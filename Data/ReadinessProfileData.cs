using Singlife.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Singlife.Data
{
    public class ReadinessProfileData
    {
        private readonly string _connectionString;

        public ReadinessProfileData()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;
        }

        public bool AddReadinessProfile(int accountID, int profileID, int q1Ans, int q2Ans, int q3Ans, int q4Ans, int q5Ans, int readinessLevel)
        {
            int totalScore = q1Ans + q2Ans + q3Ans + q4Ans + q5Ans;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO ReadinessProfile
                                 (ProfileID, AccountID, Q1Answer, Q2Answer, Q3Answer, Q4Answer, Q5Answer, TotalScore, ReadinessLevel, CreatedDate)
                                 VALUES
                                 (@ProfileID, @AccountID, @Q1Answer, @Q2Answer, @Q3Answer, @Q4Answer, @Q5Answer, @TotalScore, @ReadinessLevel, @CreatedDate)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProfileID", profileID);
                    cmd.Parameters.AddWithValue("@AccountID", accountID);
                    cmd.Parameters.AddWithValue("@Q1Answer", q1Ans);
                    cmd.Parameters.AddWithValue("@Q2Answer", q2Ans);
                    cmd.Parameters.AddWithValue("@Q3Answer", q3Ans);
                    cmd.Parameters.AddWithValue("@Q4Answer", q4Ans);
                    cmd.Parameters.AddWithValue("@Q5Answer", q5Ans);
                    cmd.Parameters.AddWithValue("@TotalScore", totalScore);
                    cmd.Parameters.AddWithValue("@ReadinessLevel", readinessLevel);
                    cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public ReadinessProfile ReadReadinessProfile(int accountID)
        {
            ReadinessProfile profile = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT TOP 1 *
                                 FROM ReadinessProfile
                                 WHERE AccountID = @AccountID
                                 ORDER BY CreatedDate DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountID);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            profile = new ReadinessProfile
                            {
                                ProfileID = Convert.ToInt32(reader["ProfileID"]),
                                AccountID = Convert.ToInt32(reader["AccountID"]),
                                Q1Answer = Convert.ToInt32(reader["Q1Answer"]),
                                Q2Answer = Convert.ToInt32(reader["Q2Answer"]),
                                Q3Answer = Convert.ToInt32(reader["Q3Answer"]),
                                Q4Answer = Convert.ToInt32(reader["Q4Answer"]),
                                Q5Answer = Convert.ToInt32(reader["Q5Answer"]),
                                TotalScore = Convert.ToInt32(reader["TotalScore"]),
                                ReadinessLevel = Convert.ToInt32(reader["ReadinessLevel"]),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                            };
                        }
                    }
                }
            }

            return profile;
        }
    }
}