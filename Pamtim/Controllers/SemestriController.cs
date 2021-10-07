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

    public class SemestriController : Controller
    {


        [Authorize]
        public ActionResult Pregled()
        {
            var User_ID = Request.Cookies["User_ID"].Values;

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    
                    var upit = "select * from Semester where User_ID = '" + User_ID + "'";
                    using (var cmd = new SqlCommand(upit, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        var sqlda = new SqlDataAdapter(cmd);
                        sqlda.Fill(dt);
                    }
                }
            }
            ViewBag.podaci = dt;

            var semesters = new ListofSemesterViewModel();

            semesters.Items = new List<SemesterViewModel>();

            foreach (DataRow r in dt.Rows)
            {
                var semester = new SemesterViewModel();
                semester.ID_Semester = r["ID_Semester"].ToString();
                semester.Name = r["Name"].ToString();

                semesters.Items.Add(semester);
            }
            return View(semesters);
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
                    var upit = "select * from Semester where ID_Semester=" + id;
                    using (var cmd = new SqlCommand(upit, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        var sqlda = new SqlDataAdapter(cmd);
                        sqlda.Fill(dt);
                    }
                }
            }

            ViewBag.podaci = dt;

            var semester = new EditSemesterViewModel();

            foreach (DataRow r in dt.Rows)
            {
                
                semester.User_ID = r["User_ID"].ToString();
                semester.Name = r["Name"].ToString();
                semester.ID_Semester = id;
            }
            return View(semester);
        }


        [Authorize]
        public ActionResult Dodaj()
        {
            var semester = new EditSemesterViewModel();

            return View(semester);

        }

        [Authorize]
        [HttpPost]
        public ActionResult Zapis(EditSemesterViewModel podaci)
        {

            var User_ID = Request.Cookies["User_ID"].Values;
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    var upit = "insert into Semester (User_ID, Name) values ( " +
                               "'" + User_ID + "'," +
                               "'" + podaci.Name + "')";
                    using (var cmd = new SqlCommand(upit, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return RedirectToAction("Pregled", "Semestri");
        }

        

        [Authorize]
        [HttpPost]
        public ActionResult Update(EditSemesterViewModel podaci)
        {
            var User_ID = Request.Cookies["User_ID"].Values;
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    var upit = "update Semester set User_ID=" +
                               "'" + User_ID + "', Name=" +
                               "'" + podaci.Name + "'" + "where ID_Semester=" + "'" + podaci.ID_Semester + "'";
                    using (var cmd = new SqlCommand(upit, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return RedirectToAction("Pregled", "Semestri");
        }

        [Authorize]
        public ActionResult Delete(int id)
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    var upit = "delete from Semester where ID_Semester=" + id;

                    using (var cmd = new SqlCommand(upit, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return RedirectToAction("Pregled", "Semestri");
        }

     
    }


}