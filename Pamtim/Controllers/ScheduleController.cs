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
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Pamtim.Models;

namespace Pamtim.Controllers
{
    public class ScheduleController : Controller
    {



        [Authorize]
        public ActionResult Pregled()
        {

            var User_ID = Request.Cookies["User_ID"].Values;



            DataTable dt3 = new DataTable();
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {

                    var upit = "select Semester.ID_Semester from Semester INNER JOIN [User] ON Semester.User_ID = [User].ID_User " +
                               "where [User].ID_User = " + User_ID;
                    using (var cmd = new SqlCommand(upit, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        var sqlda = new SqlDataAdapter(cmd);
                        sqlda.Fill(dt3);
                    }
                }
            }
            ViewBag.podaci = dt3;

            int SemesterID = 0;
            foreach (DataRow r in dt3.Rows)
            {
                SemesterID = (int)r["ID_Semester"];
            }


            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {

                    var upit = "select Event.*, Activity.Name as Activity, Course.Name AS Course, [User].ID_User, Activity.AllowedAbsence from " +
                               "Event INNER JOIN Activity ON Event.Activity_ID = Activity.ID_Activity INNER JOIN " +
                               "Course ON Activity.Course_ID = Course.ID_Course INNER JOIN " +
                               "Semester ON Course.Semester_ID = Semester.ID_Semester INNER JOIN " +
                               "[User] ON Semester.User_ID = [User].ID_User " +
                               "where [User].ID_User = " + User_ID + "AND Semester.ID_Semester = " + SemesterID + "order by Event.Start";
                    using (var cmd = new SqlCommand(upit, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        var sqlda = new SqlDataAdapter(cmd);
                        sqlda.Fill(dt);
                    }
                }
            }


            ViewBag.podaci = dt;

            var Schedules = new ListofScheduleViewModel();

            Schedules.Items = new List<ScheduleViewModel>();

            foreach (DataRow r in dt.Rows)
            {


                DataTable dt2 = new DataTable();
                using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {

                        var upit = "select COUNT(Activity_ID) as broj from Absence where Activity_ID = '" + r["Activity_ID"] + "'";
                        using (var cmd = new SqlCommand(upit, conn))
                        {
                            cmd.CommandType = CommandType.Text;
                            var sqlda = new SqlDataAdapter(cmd);
                            sqlda.Fill(dt2);
                        }
                    }
                }


                var Schedule = new ScheduleViewModel();
                Schedule.ID_Event = r["ID_Event"].ToString();
                Schedule.Activity_ID = r["Activity_ID"].ToString();
                Schedule.Day = r["Day"].ToString();
                Schedule.Start = (TimeSpan)r["Start"];
                Schedule.Finish = (TimeSpan)r["Finish"];
                Schedule.CourseName = r["Course"].ToString();
                Schedule.ActivityName = r["Activity"].ToString();
                Schedule.AllowedAbsence = (int)r["AllowedAbsence"];
                Schedule.CountAbsence = dt2.Rows[0].Field<int>("broj");
                Schedule.Location = r["Location"].ToString();

                Schedules.Items.Add(Schedule);
            }

            

            return View(Schedules);
        }


        

    }
}
