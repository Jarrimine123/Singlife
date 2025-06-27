using System;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;

class Program
{
    static void Main()
    {
        string connStr = "Singlife";
        DateTime reminderDate = DateTime.Now.Date.AddDays(7); // remind 7 days ahead

        using (var conn = new SqlConnection(connStr))
        {
            conn.Open();
            string sql = @"
                SELECT Email, FullName, PlanName, NextBillingDate
                FROM Purchases
                WHERE PaymentFrequency = 'Monthly'
                  AND CAST(NextBillingDate AS DATE) = @ReminderDate";
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@ReminderDate", reminderDate);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SendReminderEmail(
                            reader.GetString(0), // Email
                            reader.GetString(1), // FullName
                            reader.GetString(2), // PlanName
                            reader.GetDateTime(3));
                    }
                }
            }
        }
    }

    static void SendReminderEmail(string email, string name, string planName, DateTime nextDate)
    {
        var msg = new MailMessage();
        msg.To.Add(email);
        msg.From = new MailAddress("no-reply@singlife.com", "Singlife");
        msg.Subject = "Upcoming Insurance Payment Reminder";
        msg.IsBodyHtml = true;
        msg.Body = $@"
            <p>Hi {name},</p>
            <p>This is a friendly reminder that your insurance plan <b>{planName}</b> is due for renewal on <b>{nextDate:dd MMM yyyy}</b>.</p>
            <p>Please log in to make your payment.</p>
            <p>Thank you,<br/>Singlife Team</p>";

        using (var client = new SmtpClient("smtp.yourhost.com", 587))
        {
            client.Credentials = new NetworkCredential("smtpuser", "smtppass");
            client.EnableSsl = true;
            client.Send(msg);
        }
        Console.WriteLine($"Reminder sent to {email} for plan {planName}");
    }
}
