using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace WebJobs
{
    public class Functions
    {
        [NoAutomaticTrigger]
        public static async Task ProcessContinuouslyMethod(TextWriter log)
        {
            log.WriteLine(DateTime.UtcNow.ToShortTimeString() + ": Started");
            while (true)
            {
                Task.Run(() => RunAllAsync(log));
                //It will run daily
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
            log.WriteLine(DateTime.UtcNow.ToShortTimeString() + "Shutting down..");
        }
        public static void RunAllAsync(TextWriter log)
        {
            log.WriteLine("Write code for task which you want to process daily");
            using (SqlConnection con = new SqlConnection(AppConfig.MyDbConnection))
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM [Todoes] WHERE DueDate<GETDATE() AND Status='Active'", con))
                    {
                        cmd.CommandType = CommandType.Text;
                        con.Open();
                        SqlDataReader sqlDataReader = cmd.ExecuteReader();
                        while (sqlDataReader.Read())
                        {
                            var Id = Convert.ToInt32(sqlDataReader["ID"]);
                            UpdateStatus(Id);
                        }
                        if (sqlDataReader != null)
                        {
                            sqlDataReader.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                }
            }
        }
        public static bool UpdateStatus(int Id)
        {
            var result = false;
            using (SqlConnection con = new SqlConnection(AppConfig.MyDbConnection))
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE [Todoes] SET Status='Pending' WHERE ID=@Id", con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@Id", Id);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                }
            }
            return result;
        }
    }
}
