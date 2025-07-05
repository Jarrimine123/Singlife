using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Singlife.Data
{
    public class VirtualTimeMachineData
    {
        private readonly string _connectionString;

        public VirtualTimeMachineData()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["Singlife"].ConnectionString;
        }

        public bool AddVirtualTimeMachine(int accountID)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO VirtualTimeMachine (AccountID, GeneratedDate)
                                 VALUES (@AccountID, @GeneratedDate)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountID);
                    cmd.Parameters.AddWithValue("@GeneratedDate", DateTime.Now);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public DateTime? Read(int accountID)
        {
            DateTime? generatedDate = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT TOP 1 GeneratedDate
                                 FROM VirtualTimeMachine
                                 WHERE AccountID = @AccountID
                                 ORDER BY GeneratedDate DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountID);

                    conn.Open();
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        generatedDate = Convert.ToDateTime(result);
                    }
                }
            }

            return generatedDate;
        }
    }
}