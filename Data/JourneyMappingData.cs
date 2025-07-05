using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Singlife.Model;

namespace Singlife.Data
{
    public class JourneyMappingData
    {
        private readonly string _connectionString;

        public JourneyMappingData()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;
        }

        public bool AddJourneyMapping(int accountID, int year, string title, string description)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO JourneyMapping (AccountID, Year, Title, Description, CreatedDate, IsDeleted) 
                                 VALUES (@AccountID, @Year, @Title, @Description, @CreatedDate, 0)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountID);
                    cmd.Parameters.AddWithValue("@Year", year);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public bool EditJourneyMapping(int milestoneID, int year, string title, string description)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE JourneyMapping 
                                 SET Year = @Year, Title = @Title, Description = @Description, ModifiedDate = @ModifiedDate 
                                 WHERE MilestoneID = @MilestoneID AND IsDeleted = 0";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Year", year);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.Parameters.AddWithValue("@ModifiedDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@MilestoneID", milestoneID);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public bool DeleteJourneyMapping(int milestoneID)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE JourneyMapping 
                                 SET IsDeleted = 1, DeleteDate = @DeleteDate 
                                 WHERE MilestoneID = @MilestoneID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DeleteDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@MilestoneID", milestoneID);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public List<JourneyMapping> ReadJourneyMapping(int accountID)
        {
            List<JourneyMapping> journeyList = new List<JourneyMapping>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT * FROM JourneyMapping 
                                 WHERE AccountID = @AccountID AND IsDeleted = 0 
                                 ORDER BY Year";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountID);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            JourneyMapping jm = new JourneyMapping
                            {
                                MilestoneID = Convert.ToInt32(reader["MilestoneID"]),
                                AccountID = Convert.ToInt32(reader["AccountID"]),
                                Year = Convert.ToInt32(reader["Year"]),
                                Title = reader["Title"].ToString(),
                                Description = reader["Description"].ToString(),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : (DateTime?)reader["ModifiedDate"],
                            };
                            journeyList.Add(jm);
                        }
                    }
                }
            }

            return journeyList;
        }
    }
}
