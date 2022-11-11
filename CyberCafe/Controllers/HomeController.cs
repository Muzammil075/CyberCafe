using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CyberCafe.Controllers
{
    public class HomeController : Controller
    {

        string myConnectionString = "server=localhost;uid=root;" + "database=cybercafe";
        [HttpGet]
        public ActionResult Login()
        {
            
            
            ViewBag.Message = "Login.";
            return View();
        }
        [HttpGet]
        public ActionResult Logout()
        {
            Session["uid"] = null;
            Session["pwd"] = null;
            return RedirectToAction("Login");
        }

        [HttpPost]
        public ActionResult Login(Models.loginDetail obj)
        {
            string newProdID = "";
            try
            {
                using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(myConnectionString))
                {
                    MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("select Count(*) from tblcustomers o where o.Username=@userName and Password=@password", conn);
                    cmd.Parameters.AddWithValue("@userName", obj.Username);
                    cmd.Parameters.AddWithValue("@password", obj.password);
                    try
                    {
                        conn.Open();
                        newProdID = Convert.ToString(cmd.ExecuteScalar());
                        if (Convert.ToInt32(newProdID) == 0)
                        {
                            return View("Login");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                newProdID = "";
            }
            Session["uid"] = obj.Username;
            Session["pwd"] = obj.password;
            return View("Index");
        }

        [HttpGet]
        public ActionResult UserRegistration()
        {
            ViewBag.Message = "User Registration.";
            return View();
        }

        [HttpPost]
        public ActionResult UserRegistration(Models.RegistrationDetails obj)
        {
            int count = 0;
            try
            {
                using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(myConnectionString))
                {
                    MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("insert into tblcustomers(FullName,Username,UserEmail,Password,RegDate) values(@FullName,@Username,@UserEmail,@Password,@RegDate)", conn);
                    cmd.Parameters.AddWithValue("@FullName", obj.name);
                    cmd.Parameters.AddWithValue("@Username", obj.Username);
                    cmd.Parameters.AddWithValue("@UserEmail", obj.email);
                    cmd.Parameters.AddWithValue("@Password", obj.Password);
                    cmd.Parameters.AddWithValue("@RegDate", DateTime.Now);
                    try
                    {
                        conn.Open();
                        count = cmd.ExecuteNonQuery();
                        if (count == 0)
                        {
                            return View("UserRegistration");
                        }
                    }
                    catch (Exception ex)
                    {
                        return View("UserRegistration");
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                count = 0;
            }
            return View("Login");
        }

        public ActionResult SelectPC(string Amt, string Task)
        {
            TempData["Amt"] = Amt;
            TempData["Service"] = Task;
            return View();
        }

        [HttpPost]
        public JsonResult Proceed(Models.PaymentDetail obj)
        {
            string id = "";
            int Count = 0;
            try
            {
                using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(myConnectionString))
                {
                    MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("select id from tblcustomers o where o.Username=@userName and Password=@password", conn);
                    cmd.Parameters.AddWithValue("@userName", (Session["uid"]).ToString());
                    cmd.Parameters.AddWithValue("@password", (Session["pwd"]).ToString());
                    try
                    {
                        conn.Open();
                        id = Convert.ToString(cmd.ExecuteScalar());
                    }
                    catch (Exception ex)
                    {
                        id = "0";
                    }
                    MySql.Data.MySqlClient.MySqlCommand cmd1 = new MySql.Data.MySqlClient.MySqlCommand("insert into tblBooking(CustId,PCNo,Service,TotHours,TotAmt,billstatus) values(@CustId,@PCNo,@Service,@TotHours,@TotAmt,@billstatus); SELECT LAST_INSERT_ID();", conn);
                    cmd1.Parameters.AddWithValue("@CustId", id);
                    cmd1.Parameters.AddWithValue("@PCNo", obj.PCNo);
                    cmd1.Parameters.AddWithValue("@Service", obj.Service);
                    cmd1.Parameters.AddWithValue("@TotHours", obj.TotalHour);
                    cmd1.Parameters.AddWithValue("@TotAmt", "0");
                    cmd1.Parameters.AddWithValue("@billstatus", "NO PAID");
                    try
                    {
                        Count = Convert.ToInt32(cmd1.ExecuteScalar());
                        Session["id"] = Count;
                        if (Count <= 0)
                        {
                            obj.status = Convert.ToString("Failed");
                            return Json(obj, JsonRequestBehavior.AllowGet);
                        }
                    }
                    catch (Exception ex)
                    {
                        obj.status = Convert.ToString("Failed");
                        return Json(obj, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                obj.status = Convert.ToString("Failed");
                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            Models.PaymentDetail obj1 = new Models.PaymentDetail();
            obj.Amt = Convert.ToString(Convert.ToInt32(obj.TotalHour)* Convert.ToDouble(obj.Amt));
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ProceedPay(Models.PaymentDetail obj)
        {
            int Count = 0;
            try
            {
                using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(myConnectionString))
                {
                    MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("update tblBooking set TotAmt=@TotAmt where BookingId = @BookingId", conn);
                    cmd.Parameters.AddWithValue("@TotAmt", obj.Amt);
                    cmd.Parameters.AddWithValue("@BookingId", (Session["id"]).ToString());
                    try
                    {
                        conn.Open();
                        Count = Convert.ToInt32(cmd.ExecuteNonQuery());
                        if (Count <= 0)
                        {
                            obj.status = Convert.ToString("Failed");
                            return Json(obj, JsonRequestBehavior.AllowGet);
                        }
                    }
                    catch (Exception ex)
                    {
                        obj.status = Convert.ToString("Failed");
                        return Json(obj, JsonRequestBehavior.AllowGet);
                    }          
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                obj.status = Convert.ToString("Failed");
                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            obj.status = Convert.ToString("Successful...");
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

    }
}