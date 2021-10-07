using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Pamtim.Controllers
{
    public class WeightController : Controller
    {
        // GET: Weight
        
        public void Insert(string data)
        {

            
            string[] podaci = data.Split('-');

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["2FIT4YOU"].ConnectionString))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        var upit = "insert into UserMeasure (ID_User, ID_Measures, Value) " +
                                   "select Nfc.ID_User, Measures.Measures_ID, " +
                                   "'" + podaci[2] + "'" +
                                   " from Nfc, Measures where Nfc.NfcTag = " +
                                   "'" + podaci[0] + "' and Measures.MeasureType = '" + podaci[1] + "'";
                        using (var cmd = new SqlCommand(upit, conn))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            
        }
    }
}