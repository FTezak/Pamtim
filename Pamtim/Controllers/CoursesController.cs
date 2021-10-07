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
    public class CoursesController : Controller
    {

        [Authorize]
        public ActionResult Pregled(int Semester_ID)
        {

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {

                    var upit = "select * from Course where Semester_ID = '" + Semester_ID + "'";
                    using (var cmd = new SqlCommand(upit, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        var sqlda = new SqlDataAdapter(cmd);
                        sqlda.Fill(dt);
                    }
                }
            }

            DataTable dt2 = new DataTable();
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
                        sqlda.Fill(dt2);
                    }
                }
            }



            ViewBag.SemesterName = dt2.Rows[0].Field<string>("Name");

            ViewBag.podaci = dt;
            ViewBag.Semester_ID = Semester_ID;
             

            var courses = new ListofCourseViewModel();

            courses.Items = new List<CourseViewModel>();

            foreach (DataRow r in dt.Rows)
            {
                var course = new CourseViewModel();
                course.ID_Course = r["ID_Course"].ToString();
                course.Name = r["Name"].ToString();
                course.Semester_ID = r["Semester_ID"].ToString();

                courses.Items.Add(course);
            }
            return View(courses);
        }



        [Authorize]
        public ActionResult Dodaj(int Semester_ID)
        {
            var semester = new EditCourseViewModel();
            ViewBag.Semester_ID = Semester_ID;
            return View(semester);
            

        }

        [Authorize]
        [HttpPost]
        public ActionResult Zapis(EditCourseViewModel podaci, int Semester_ID)
        {


            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    var upit = "insert into Course (Semester_ID, Name) values ( " +
                               "'" + Semester_ID + "'," +
                               "'" + podaci.Name + "')";
                    using (var cmd = new SqlCommand(upit, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return RedirectToAction("Pregled", "Courses", new { Semester_ID = Semester_ID });
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
                    var upit = "select * from Course where ID_Course=" + id;
                    using (var cmd = new SqlCommand(upit, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        var sqlda = new SqlDataAdapter(cmd);
                        sqlda.Fill(dt);
                    }
                }
            }

            ViewBag.podaci = dt;

            var semester = new EditCourseViewModel();

            foreach (DataRow r in dt.Rows)
            {

                semester.Semester_ID = r["Semester_ID"].ToString();
                semester.Name = r["Name"].ToString();
                semester.ID_Course = id;
            }

            ViewBag.Semester_ID = semester.Semester_ID;
            ViewBag.ID_Course = semester.ID_Course;

            return View(semester);
        }



        [Authorize]
        [HttpPost]
        public ActionResult Update(EditCourseViewModel podaci, int ID_Course, int Semester_ID)
        {
            
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    var upit = "update Course set Name=" +
                                "'" + podaci.Name + "'" + "where ID_Course=" + "'" + podaci.ID_Course + "'";
                    using (var cmd = new SqlCommand(upit, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return RedirectToAction("Pregled", "Courses", new { Semester_ID = Semester_ID });
        }


        [Authorize]
        public ActionResult Delete(int ID_Course, int Semester_ID)
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    var upit = "delete from Course where ID_Course=" + ID_Course;

                    using (var cmd = new SqlCommand(upit, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return RedirectToAction("Pregled", "Courses", new { Semester_ID = Semester_ID });
        }

    }
}
