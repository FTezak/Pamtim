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
    public class EventController : Controller
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

                    var upit = "select * from Event where Activity_ID = '" + Activity_ID + "'";
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

            var Events = new ListofEventViewModel();

            Events.Items = new List<EventViewModel>();

            foreach (DataRow r in dt.Rows)
            {


                DataTable dt2 = new DataTable();
                using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {

                        var upit = "select Course.Name as course, Activity.Name as activity from Activity inner join " +
                                   "Course ON Activity.Course_ID = Course.ID_Course INNER JOIN Event ON Activity.ID_Activity = Event.Activity_ID WHERE " +
                                   "Event.ID_Event = " + r["ID_Event"];
                        using (var cmd = new SqlCommand(upit, conn))
                        {
                            cmd.CommandType = CommandType.Text;
                            var sqlda = new SqlDataAdapter(cmd);
                            sqlda.Fill(dt2);
                        }
                    }
                }


                var Event = new EventViewModel();
                Event.ID_Event = r["ID_Event"].ToString();
                Event.Activity_ID = r["Activity_ID"].ToString();
                Event.Day = r["Day"].ToString();
                Event.Start = (TimeSpan) r["Start"];
                Event.Finish = (TimeSpan)r["Finish"];
                Event.CourseName = dt2.Rows[0].Field<string>("course");
                Event.ActivityName = dt2.Rows[0].Field<string>("activity");
                Event.Location = r["Location"].ToString();

                Events.Items.Add(Event);
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


            return View(Events);
        }



        [Authorize]
        public ActionResult Dodaj(int Activity_ID)
        {
            var semester = new EditEventViewModel();

            ViewBag.Activity_ID = Activity_ID;
            return View(semester);
        }


        [Authorize]
        [HttpPost]
        public ActionResult Zapis(EditEventViewModel podaci, int Activity_ID)
        {
            string message = "";
            if (TimeSpan.Compare(podaci.Start, podaci.Finish) == 1)
            {
                return RedirectToAction("Dodaj", new {Activity_ID = Activity_ID});
            }

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    var upit = "insert into Event (Activity_ID, Location, Start, Finish, Day) values ( " +
                               "'" + Activity_ID + "'," +
                               "'" + podaci.Location + "'," +
                               "'" + podaci.Start + "'," +
                               "'" + podaci.Finish + "'," +
                               "'" + podaci.Day + "')";
                    using (var cmd = new SqlCommand(upit, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return RedirectToAction("Pregled", "Event", new { Activity_ID = Activity_ID });
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
                    var upit = "select * from Event where ID_Event=" + id;
                    using (var cmd = new SqlCommand(upit, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        var sqlda = new SqlDataAdapter(cmd);
                        sqlda.Fill(dt);
                    }
                }
            }

            ViewBag.podaci = dt;

            var activity = new EditEventViewModel();

            foreach (DataRow r in dt.Rows)
            {

                activity.Activity_ID = r["Activity_ID"].ToString();
                activity.Day = r["Day"].ToString();
                activity.Location = r["Location"].ToString();

                string temp = r["Start"].ToString();

                activity.Start = (TimeSpan)r["Start"];
                activity.Finish = (TimeSpan)r["Finish"];
                activity.ID_Event = id.ToString();
            }

            ViewBag.Activity_ID = activity.Activity_ID;
            ViewBag.ID_Event = activity.ID_Event;

            bool T = true;
            bool F = false;

            ViewBag.dan = "pon";
            

            if (activity.Day == "Ponedjeljak") ViewBag.dan = "pon";
            if (activity.Day == "Utorak") ViewBag.dan = "uto";
            if (activity.Day == "Srijeda") ViewBag.dan = "sre";
            if (activity.Day == "Četvrtak") ViewBag.dan = "cet";
            if (activity.Day == "Petak") ViewBag.dan = "pet";
            if (activity.Day == "Subota") ViewBag.dan = "sub";
            if (activity.Day == "Nedjelja") ViewBag.dan = "ned";

            return View(activity);
        }



        [Authorize]
        [HttpPost]
        public ActionResult Update(EditEventViewModel podaci, int ID_Event, int Activity_ID)
        {

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    var upit = "update Event set Start=" +
                               "'" + podaci.Start + "', Finish=" +
                               "'" + podaci.Finish + "', Location=" +
                               "'" + podaci.Location + "', Day=" +
                               "'" + podaci.Day + "' where ID_Event=" + "'" + podaci.ID_Event + "'";
                    using (var cmd = new SqlCommand(upit, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return RedirectToAction("Pregled", "Event", new { Activity_ID = Activity_ID });
        }


        [Authorize]
        public ActionResult Delete(int ID_Event, int Activity_ID)
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    var upit = "delete from Event where ID_Event=" + ID_Event;

                    using (var cmd = new SqlCommand(upit, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return RedirectToAction("Pregled", "Event", new { Activity_ID = Activity_ID });
        }
        
    }
}
