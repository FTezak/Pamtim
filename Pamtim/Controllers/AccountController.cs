using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Pamtim.Models;

namespace Pamtim.Controllers
{
    /// <summary>
    /// Kontroler za komunikaciju korisnikovog inputa u web aplikaciji i servisa - informacije o korisnikovoj prijavi na sustav
    /// </summary>








    public class AccountController : Controller
    {

        public AccountController()
        {

        }
        
        [RequireHttps]
        public ActionResult Login()
        {
            UserPrincipal User = HttpContext.User as UserPrincipal;

            if (User != null)
            {
                return RedirectToAction("Pregled", "Semestri");
            }

            var model = new UserLoginViewModel();

            HttpCookie myCoockie = Request.Cookies["pamtim"];
            if (myCoockie != null)
            {
                try
                {
                    var un = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(myCoockie["UserName"]));
                    model.UserName = un;
                }
                catch
                {
                }

            }

            return View(model);
        }

        [RequireHttps]
        [HttpPost]
        public ActionResult Login(UserLoginViewModel model)
        {
            
            var CryptoPassword = Convert.ToBase64String(System.Security.Cryptography.SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(model.Password)));

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    var upit = "select * from [User] where Username = '" + model.UserName.TrimEnd() + 
                        "' and Password = '" + CryptoPassword + "'";
                    using (var cmd = new SqlCommand(upit, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        SqlDataAdapter sqlda = new SqlDataAdapter(cmd);
                        sqlda.Fill(dt);
 
                    
                    }
                }

            }

            int Active = 2;

            if(dt.Rows.Count > 0) {
            Active = dt.Rows[0].Field<int>("Active");
            }

            if (dt.Rows.Count > 0 && Active == 1)
            {
                int id = dt.Rows[0].Field<int>("ID_User");

                UserPrincipal mod = new UserPrincipal(model.UserName)
                {
                    ID_User = dt.Rows[0].Field<int>("ID_User"),
                    Email = dt.Rows[0].Field<string>("Email"),
                    UserName = dt.Rows[0].Field<string>("UserName"),
                };

                Authorize(mod, false);
                
                return RedirectToAction("Pregled", "Semestri");
               

            }
            else if(Active == 0)
            {
                ViewBag.BadUser = true;
                ModelState.AddModelError("mail", "Mail nije aktiviran!");
                return View(model);
            }
            else
            {
                ViewBag.BadUser = true;
                ModelState.AddModelError("neispravno", "Korisničko ime ili lozinka neispravni.");
                return View(model);
            }



        }




        public void Authorize(UserPrincipal model, bool rememeberMe)
        {
            var serializeModel = new UserPrincipalSerializeModel
            {
                ID_User = model.ID_User


            };

            

            var serializer = new JavaScriptSerializer();
            string userData = serializer.Serialize(serializeModel);

            var authTicket = new FormsAuthenticationTicket(
                1,
                model.UserName,
                DateTime.Now,
                DateTime.Now.AddDays(100),
                false,
                userData);

            string encTicket = FormsAuthentication.Encrypt(authTicket);
            var faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            Response.Cookies.Add(faCookie);

   


            

            HttpCookie Cookie = new HttpCookie("User_ID", model.ID_User.ToString());
            Cookie.Expires = DateTime.Now.AddDays(100);
            Response.Cookies.Add(Cookie);
            
        }





        public ActionResult Signout()
        {
            FormsAuthentication.SignOut();

            if (Request.Cookies["User_ID"] != null)
            {
                Response.Cookies["User_ID"].Expires = DateTime.Now.AddDays(-100);
            }

            return RedirectToAction("Login", "Account");
        }




        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude = "ActivationCode, Active")] UserRegistrationViewModel model)
        {
            bool Status = false;
            string message = "";

            if (ModelState.IsValid)
            {
                var IsExistEmail = IsEmailExist(model.Email);
                if (IsExistEmail)
                {
                    ModelState.AddModelError("EmailExist", "Email već postoji!");
                    return View(model);
                }

                var IsExistUserName = IsUserNameExist(model.UserName);
                if (IsExistUserName)
                {
                    ModelState.AddModelError("UserNameExist", "Username već postoji!");
                    return View(model);
                }

                model.ActivationCode = Guid.NewGuid();

                model.Password = Crypto.Hash(model.Password);
                model.ConfirmPassword = Crypto.Hash(model.ConfirmPassword);

                model.Active = 0;

                using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        var upit = "insert into [User] (Username, Email, Password, ActivationCode, Active) values ( " +
                                   "'" + model.UserName + "'," +
                                   "'" + model.Email + "'," +
                                   "'" + model.Password + "'," +
                                   "'" + model.ActivationCode + "'," +
                                   "'" + model.Active + "')";
                        using (var cmd = new SqlCommand(upit, conn))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }


                SendVerificationLink(model.Email, model.ActivationCode.ToString());
                message = "Registracija je uspješna! Aktivacijski link je poslan na vašu emai adresu: " + model.Email;
                Status = true;
            }
            else
            {
                message = "Invalid Request";
            }


            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View(model);
        }


        [NonAction]
        public bool IsUserNameExist(string UserName)
        {
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    var upit = "select * from [User] where Username = '" + UserName + "'";
                    using (var cmd = new SqlCommand(upit, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        SqlDataAdapter sqlda = new SqlDataAdapter(cmd);
                        sqlda.Fill(dt);
                    }
                }
            }

            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        [HttpGet]
        public ActionResult VerifyAccount(String ActivationCode)
        {
            bool Status = false;

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    var upit = "select * from [User] where ActivationCode = '" + new Guid(ActivationCode) + "'";
                    using (var cmd = new SqlCommand(upit, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        SqlDataAdapter sqlda = new SqlDataAdapter(cmd);
                        sqlda.Fill(dt);
                    }
                }
            }

            if (dt.Rows.Count > 0)
            {
                using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        var upit = "update [User] set Active=" + "'" + 1 + "' where ActivationCode=" + "'" + new Guid(ActivationCode) + "'";
                        using (var cmd = new SqlCommand(upit, conn))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                Status = true;
            }
            else
            {
                ViewBag.Message = "Invalid request!";
            }
            ViewBag.Status = true;
            return RedirectToAction("Login", "Account");
        }


        [NonAction]
        public bool IsEmailExist(string email)
        {
            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    var upit = "select * from [User] where Email = '" + email + "'";
                    using (var cmd = new SqlCommand(upit, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        SqlDataAdapter sqlda = new SqlDataAdapter(cmd);
                        sqlda.Fill(dt);
                    }
                }
            }

            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }




        [NonAction]
        public void SendVerificationLink(string Email, string ActivationCode)
        {
            var verifyUrl = "/Account/VerifyAccount/?ActivationCode=" + ActivationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("*********", "Pamtim");
            var toEmail = new MailAddress(Email);
            var fromEmailPassword = "*********";
            string subject = "Pamtim - Aktivacija računa";
            string body =
                "<br/><br/> Vaš Pamtim.com korisnički račun je uspješno kreiran! Molimo vas da pomoću linka ispod potvrdite ovaj Email:" +
                "<br/><br/><a href='" + link + "'>" + link + "</a>";
            var smtp = new SmtpClient
            {
                Host = "*********",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail))
            {
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;
                smtp.Send(message);
            }
            
        }

        private int GetUserID()
        {

            string UserName = User.Identity.Name;

            DataTable dt = new DataTable();
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["pamtim"].ConnectionString))
            {
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {

                    var upit = "select * from [User] where Username = '" + UserName + "'";
                    using (var cmd = new SqlCommand(upit, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        var sqlda = new SqlDataAdapter(cmd);
                        sqlda.Fill(dt);
                    }
                }

                int id = dt.Rows[0].Field<int>("ID_User");
                return id;
            }
        }
    }
}
