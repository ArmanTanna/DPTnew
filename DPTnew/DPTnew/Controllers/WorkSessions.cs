using DPTnew.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using System.Web.Http;

namespace DptLicensingServer.Controllers
{
    public class WorkSessionsController : ApiController
    {
        [HttpPost]   
        [ActionName("Service")]
        public void Service([FromUri] string id, JObject traceback)
        {
            if (!Authenticate() || traceback == null)
            {
                LogHelper.WriteLog("WorkSessionsController (Service): traceback null");
                return;
            }
            var machineid1 = "";
            var machineid2 = "";
            var xmu = "";
            try
            {
                foreach (var pair in traceback)
                {
                    switch (pair.Key)
                    {
                        case "machineid1": machineid1 = pair.Value.ToString();
                            break;
                        case "machineid2": machineid2 = pair.Value.ToString();
                            break;
                        case "xmu": xmu = pair.Value.ToString();
                            break;
                        default:
                            break;
                    }
                }
                using (SqlConnection dbconn = new SqlConnection(ConfigurationManager.AppSettings["DbConn"]))
                {
                    dbconn.Open();
                    string sql = "insert into [dbo].[WorkSessions](machineid1,machineid2,sessionlog,creationdate)values('"
                        + machineid1.Replace("'", "''") + "','" + machineid2.Replace("'", "''") + "','"
                        + xmu.Replace("'", "''") + "','" + DateTime.Now + "')";
                    using (SqlCommand dbcomm = new SqlCommand(sql, dbconn))
                    {
                        dbcomm.CommandTimeout = 600;
                        dbcomm.ExecuteNonQuery();
                        dbcomm.Dispose();
                    }
                    dbconn.Dispose();
                    dbconn.Close();
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteLog("WorkSessionsController (Service1): " + e.Message);
            }
        }

        private bool Authenticate()
        {
            return true;
            //IEnumerable<string> headers = new List<string>();
            //if (!this.Request.Headers.TryGetValues("Token", out headers) || !headers.FirstOrDefault<string>().Equals(token))
            //  return false;
            //return true;
        }

        [HttpPost]
        [ActionName("Service")]
        public void Service([FromUri]string machineid1, [FromUri]string machineid2, [FromUri]string xmu)
        {
            if (!Authenticate())
                return;

            try
            {
                using (SqlConnection dbconn = new SqlConnection(ConfigurationManager.AppSettings["DbConn"]))
                {
                    dbconn.Open();
                    string sql = "insert into [dbo].[WorkSessions](machineid1,machineid2,sessionlog,creationdate)values('"
                        + machineid1.Replace("'", "''") + "','" + machineid2.Replace("'", "''") + "','"
                        + xmu.Replace("'", "''") + "','" + DateTime.Now + "')";
                    using (SqlCommand dbcomm = new SqlCommand(sql, dbconn))
                    {
                        dbcomm.CommandTimeout = 600;
                        dbcomm.ExecuteNonQuery();
                        dbcomm.Dispose();
                    }
                    dbconn.Dispose();
                    dbconn.Close();
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteLog("WorkSessionsController (Service): " + e.Message);
            }
        }
    }

}
