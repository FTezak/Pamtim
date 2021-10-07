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
    public class ActivityController : Controller
    {

        [Authorize]
        public ActionResult Pregled(int Course_ID)
        {

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {

                    var upit = "select * from Activity where Course_ID = '" + Course_ID + "'";
                    using (var cmd = new SqlCommand(upit, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        var sqlda = new SqlDataAdapter(cmd);
                        sqlda.Fill(dt);
                    }
                }
            }

            ViewBag.podaci = dt;
            ViewBag.Course_ID = Course_ID;

            var activities = new ListofActivityViewModel();

            activities.Items = new List<ActivityViewModel>();

            foreach (DataRow r in dt.Rows)
            {


                DataTable dt2 = new DataTable();
                using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {

                        var upit = "select COUNT(Activity_ID) as broj from Absence where Activity_ID = '" + r["ID_Activity"] + "'";
                        using (var cmd = new SqlCommand(upit, conn))
                        {
                            cmd.CommandType = CommandType.Text;
                            var sqlda = new SqlDataAdapter(cmd);
                            sqlda.Fill(dt2);
                        }
                    }
                }


                var Activity = new ActivityViewModel();
                Activity.ID_Activity = r["ID_Activity"].ToString();
                Activity.Name = r["Name"].ToString();
                Activity.Course_ID = r["Course_ID"].ToString();
                Activity.AllowedAbsence = (int) r["AllowedAbsence"];
                Activity.CountAbsence = dt2.Rows[0].Field<int>("broj");

                activities.Items.Add(Activity);
            }



            DataTable dt3 = new DataTable();
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {

                    var upit = "SELECT Semester.ID_Semester FROM Course INNER JOIN " +
                               "Semester ON Course.Semester_ID = Semester.ID_Semester " +
                               "WHERE(Course.ID_Course =" + Course_ID + ")";
                    using (var cmd = new SqlCommand(upit, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        var sqlda = new SqlDataAdapter(cmd);
                        sqlda.Fill(dt3);
                    }
                }
            }

            ViewBag.Semester_ID = dt3.Rows[0].Field<int>("ID_Semester");
            int Semester_ID = dt3.Rows[0].Field<int>("ID_Semester");

            DataTable dt4 = new DataTable();
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {

                    var upit = "select Semester.Name from Semester where Semester.ID_Semester = '" + Semester_ID + "'";
                    using (var cmd = new SqlCommand(upit, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        var sqlda = new SqlDataAdapter(cmd);
                        sqlda.Fill(dt4);
                    }
                }
            }

            ViewBag.SemesterName = dt4.Rows[0].Field<string>("Name");

            DataTable dt5 = new DataTable();
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {

                    var upit = "select Course.Name from Course where Course.ID_Course = '" + Course_ID + "'";
                    using (var cmd = new SqlCommand(upit, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        var sqlda = new SqlDataAdapter(cmd);
                        sqlda.Fill(dt5);
                    }
                }
            }

            ViewBag.CourseName = dt5.Rows[0].Field<string>("Name");




            return View(activities);
        }



        [Authorize]
        public ActionResult Dodaj(int Course_ID)
        {
            var semester = new EditActivityViewModel();
            ViewBag.Course_ID = Course_ID;
            return View(semester);
        }


        [Authorize]
        [HttpPost]
        public ActionResult Zapis(EditActivityViewModel podaci, int Course_ID)
        {


            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    var upit = "insert into Activity (Course_ID, Name, AllowedAbsence) values ( " +
                               "'" + Course_ID + "'," +
                               "'" + podaci.Name + "'," +
                               "'" + podaci.AllowedAbsence + "')";
                    using (var cmd = new SqlCommand(upit, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            return RedirectToAction("Pregled", "Activity", new { Course_ID = podaci.Course_ID });
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
                    var upit = "select * from Activity where ID_Activity=" + id;
                    using (var cmd = new SqlCommand(upit, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        var sqlda = new SqlDataAdapter(cmd);
                        sqlda.Fill(dt);
                    }
                }
            }

            ViewBag.podaci = dt;

            var activity = new EditActivityViewModel();

            foreach (DataRow r in dt.Rows)
            {

                activity.Course_ID = r["Course_ID"].ToString();
                activity.Name = r["Name"].ToString();
                activity.AllowedAbsence = (int) r["AllowedAbsence"];
                activity.ID_Activity = id;
            }

            ViewBag.Course_ID = activity.Course_ID;
            ViewBag.ID_Activity = activity.ID_Activity;

            return View(activity);
        }



        [Authorize]
        [HttpPost]
        public ActionResult Update(EditActivityViewModel podaci, int ID_Activity, int Course_ID)
        {

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    var upit = "update Activity set Name=" +
                                "'" + podaci.Name + "', AllowedAbsence=" +
                               "'" + podaci.AllowedAbsence + "' where ID_Activity=" + "'" + podaci.ID_Activity + "'";
                    using (var cmd = new SqlCommand(upit, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return RedirectToAction("Pregled", "Activity", new { Course_ID = Course_ID });
        }


        [Authorize]
        public ActionResult Delete(int ID_Activity, int Course_ID)
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    var upit = "delete from Activity where ID_Activity=" + ID_Activity;

                    using (var cmd = new SqlCommand(upit, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return RedirectToAction("Pregled", "Activity", new { Course_ID = Course_ID });
        }

    }
}
