using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Pamtim.Models;

namespace Pamtim.Controllers
{
    public class AbsenceController : Controller
    {

        [Authorize]
        public ActionResult Pregled(int Activity_ID)
        {
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {

                    var upit = "select * from Absence where Activity_ID = '" + Activity_ID + "'";
                    using (var cmd = new SqlCommand(upit, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        var sqlda = new SqlDataAdapter(cmd);
                        sqlda.Fill(dt);
                    }
                }
            }


            ViewBag.podaci = dt;
            ViewBag.Activity_ID = Activity_ID;

            var absences = new ListofAbsenceViewModel();

            absences.Items = new List<AbsenceViewModel>();

            foreach (DataRow r in dt.Rows)
            {


                DataTable dt2 = new DataTable();
                using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {

                        var upit = "select Course.Name as course, Activity.Name as activity from Course inner join " +
                            "Activity ON Course.ID_Course = Activity.Course_ID INNER JOIN Absence ON Activity.ID_Activity = Absence.Activity_ID WHERE " +
                            "Absence.ID_Absence = " + r["ID_Absence"];
                        using (var cmd = new SqlCommand(upit, conn))
                        {
                            cmd.CommandType = CommandType.Text;
                            var sqlda = new SqlDataAdapter(cmd);
                            sqlda.Fill(dt2);
                        }
                    }
                }


                var absence = new AbsenceViewModel();
                absence.ID_Absence = r["ID_Absence"].ToString();
                absence.Description = r["Description"].ToString();
                absence.Activity_ID = r["Activity_ID"].ToString();
                absence.CreationDate = (DateTime)r["CreationDate"];
                absence.AbsenceDate = (DateTime)r["AbsenceDate"];
                absence.CourseName = dt2.Rows[0].Field<string>("course");
                absence.ActivityName = dt2.Rows[0].Field<string>("activity");

                absences.Items.Add(absence);
            }



            DataTable dt3 = new DataTable();
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {

                    var upit = "SELECT Course.ID_Course FROM Activity INNER JOIN " +
                        "Course ON Activity.Course_ID = Course.ID_Course " +
                        "WHERE(Activity.ID_Activity =" + Activity_ID + ")";
                    using (var cmd = new SqlCommand(upit, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        var sqlda = new SqlDataAdapter(cmd);
                        sqlda.Fill(dt3);
                    }
                }
            }

            ViewBag.Course_ID = dt3.Rows[0].Field<int>("ID_Course");


            return View(absences);
        }



        [Authorize]
        public ActionResult Dodaj(int Activity_ID)
        {
            var semester = new EditAbsenceViewModel();
            ViewBag.Activity_ID = Activity_ID;
            semester.AbsenceDate = DateTime.Now;
            return View(semester);
        }


        [Authorize]
        [HttpPost]
        public ActionResult Zapis(EditAbsenceViewModel podaci, int Activity_ID)
        {
            DateTime datum = podaci.AbsenceDate;
            string datum2 = datum.ToString("yyyy-MM-dd");

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    var upit = "insert into Absence (Activity_ID, AbsenceDate, Description) values ( " +
                               "'" + Activity_ID + "'," +

                               "'" + datum2 + "'," +
                               "'" + podaci.Description + "')";



                    using (var cmd = new SqlCommand(upit, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return RedirectToAction("Pregled", "Absence", new { Activity_ID = Activity_ID });
        }


        [Authorize]
        public ActionResult Edit(int id)
        {
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    var upit = "select * from Absence where ID_Absence=" + id;
                    using (var cmd = new SqlCommand(upit, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        var sqlda = new SqlDataAdapter(cmd);
                        sqlda.Fill(dt);
                    }
                }
            }

            ViewBag.podaci = dt;

            var activity = new EditAbsenceViewModel();

            foreach (DataRow r in dt.Rows)
            {

                activity.Activity_ID = r["Activity_ID"].ToString();
                activity.Description = r["Description"].ToString();
                activity.CreationDate = (DateTime) r["CreationDate"];
                activity.ID_Absence = id.ToString();
                activity.AbsenceDate = (DateTime)r["AbsenceDate"];
                
            }

            ViewBag.Activity_ID = activity.Activity_ID;
            ViewBag.ID_Absence = activity.ID_Absence;
            


            return View(activity);
        }



        [Authorize]
        [HttpPost]
        public ActionResult Update(EditAbsenceViewModel podaci, int ID_Absence, int Activity_ID)
        {

            DateTime datum = podaci.AbsenceDate;
            string datum2 = datum.ToString("yyyy-MM-dd");

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    var upit = "update Absence set Description=" +
                                "'" + podaci.Description  + "', AbsenceDate=" + "'" + datum2 +
                                "' where ID_Absence=" + 
                                "'" + ID_Absence + "'";
                    using (var cmd = new SqlCommand(upit, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return RedirectToAction("Pregled", "Absence", new { Activity_ID = Activity_ID });
        }


        [Authorize]
        public ActionResult Delete(int ID_Absence, int Activity_ID)
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    var upit = "delete from Absence where ID_Absence=" + ID_Absence;

                    using (var cmd = new SqlCommand(upit, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return RedirectToAction("Pregled", "Absence", new { Activity_ID = Activity_ID });
        }

    }

    
}
