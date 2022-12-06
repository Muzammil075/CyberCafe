using System;
using System.Collections.Generic;
using System.Data;
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
            string Role = "";
            DataSet ds = new DataSet("Detail");
            Models.ServiceDetail ServiceDetail = new Models.ServiceDetail();
            try
            {
                using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(myConnectionString))
                {
                    MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("select Role from tblcustomers o where o.Username=@userName and Password=@password", conn);
                    cmd.Parameters.AddWithValue("@userName", obj.Username);
                    cmd.Parameters.AddWithValue("@password", obj.password);
                    try
                    {
                        conn.Open();
                        Role = Convert.ToString(cmd.ExecuteScalar());
                        if (Role == "" || Role == null)
                        {
                            return View("Login");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    //--------------
                    MySql.Data.MySqlClient.MySqlDataAdapter da = new MySql.Data.MySqlClient.MySqlDataAdapter("SELECT `ID`,`SERVICENAME`,`DESCRIPTION`,`RATE`,`IMAGE` FROM `TBLSERVICES` WHERE 1", conn);
                    da.Fill(ds, "Detail");
                    ServiceDetail.dt = ds.Tables[0];
                    Session["Role"] = Role;
                    //--------------
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Role = "";
            }
            Session["uid"] = obj.Username;
            Session["pwd"] = obj.password;
            return View("Index", ServiceDetail);
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
                    MySql.Data.MySqlClient.MySqlCommand cmd1 = new MySql.Data.MySqlClient.MySqlCommand("select Count(*) from tblcustomers o where o.Username=@username and Password=@password", conn);
                    cmd1.Parameters.AddWithValue("@username", obj.Username);
                    cmd1.Parameters.AddWithValue("@password", obj.Password);
                    try
                    {
                        conn.Open();
                        count = Convert.ToInt32(cmd1.ExecuteScalar());
                        if (count > 0)
                        {
                            //ViewBag["MSG"]= "User Allready Exist.";
                            ViewBag.Message = "User Already Exist.";
                            return View("UserRegistration");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("insert into tblcustomers(FullName,Username,UserEmail,Password,RegDate,Role) values(@FullName,@Username,@UserEmail,@Password,@RegDate,@Role)", conn);
                    cmd.Parameters.AddWithValue("@FullName", obj.name);
                    cmd.Parameters.AddWithValue("@Username", obj.Username);
                    cmd.Parameters.AddWithValue("@UserEmail", obj.email);
                    cmd.Parameters.AddWithValue("@Password", obj.Password);
                    cmd.Parameters.AddWithValue("@RegDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Role", obj.Role);
                    try
                    {
                        //conn.Open();
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


        public ActionResult ServiceDet()
        {       
            DataSet ds = new DataSet("Detail");
            Models.ServiceDetail ServiceDetail = new Models.ServiceDetail();
            using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(myConnectionString))
            {
                //--------------
                MySql.Data.MySqlClient.MySqlDataAdapter da = new MySql.Data.MySqlClient.MySqlDataAdapter("SELECT `ID`,`SERVICENAME`,`DESCRIPTION`,`RATE`,`IMAGE` FROM `TBLSERVICES` WHERE 1", conn);
                da.Fill(ds, "Detail");
                ServiceDetail.dt = ds.Tables[0];
                //--------------
            }
            return View("Index", ServiceDetail);
        }

        public ActionResult SelectPC(string Amt, string Task)
        {
            if ((Session["Role"]).ToString() == "Owner")
            {
                return RedirectToAction("ServiceDet");
            }

            DataSet ds = new DataSet("Detail");
            Models.ServiceDetail ServiceDetail = new Models.ServiceDetail();
            try
            {
                using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(myConnectionString))
                {
                    MySql.Data.MySqlClient.MySqlDataAdapter da = new MySql.Data.MySqlClient.MySqlDataAdapter("Select B.BookingId,B.Service,B.PCNo,(B.TotHours * B.Amt) as TotAmt from tblcustomers C, tblbooking B Where C.Username='" + (Session["uid"]).ToString() + "' and C.Password='" + (Session["pwd"]).ToString() + "' AND C.id=B.CustId And billstatus = 'NO PAID' AND B.Service='" + Task + "'", conn);
                    da.Fill(ds, "Detail");
                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        TempData["Amt"] = Amt;
                        TempData["Service"] = Task;
                        return View();
                    }
                    ServiceDetail.dt = ds.Tables[0];
                    Session["id"] = ds.Tables[0].Rows[0]["BookingId"];
                    //--------------
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
            }
            return View("PaymentDetail", ServiceDetail);          
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
                    string ServiceDate = (obj.ServiceDate).ToString("yyyy-MM-dd");
                    MySql.Data.MySqlClient.MySqlCommand cmd1 = new MySql.Data.MySqlClient.MySqlCommand("insert into tblBooking(CustId,PCNo,Service,TotHours,TotAmt,billstatus,BookingDate,Amt) values(@CustId,@PCNo,@Service,@TotHours,@TotAmt,@billstatus,@ServiceDate,@Amt); SELECT LAST_INSERT_ID();", conn);
                    cmd1.Parameters.AddWithValue("@CustId", id);
                    cmd1.Parameters.AddWithValue("@ServiceDate", ServiceDate);
                    cmd1.Parameters.AddWithValue("@PCNo", obj.PCNo);
                    cmd1.Parameters.AddWithValue("@Service", obj.Service);
                    cmd1.Parameters.AddWithValue("@TotHours", obj.TotalHour);
                    cmd1.Parameters.AddWithValue("@TotAmt", "0");
                    cmd1.Parameters.AddWithValue("@billstatus", "NO PAID");
                    cmd1.Parameters.AddWithValue("@Amt", obj.Amt);
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
                    MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("update tblBooking set TotAmt=@TotAmt,billstatus='PAID' where BookingId = @BookingId", conn);
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

        public ActionResult ServiceData()
        { 
            DataSet ds = new DataSet("Detail");
            Models.ServiceDetail ServiceDetail = new Models.ServiceDetail();
            try
            {
                using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(myConnectionString))
                {                     
                    //--------------
                    MySql.Data.MySqlClient.MySqlDataAdapter da = new MySql.Data.MySqlClient.MySqlDataAdapter("SELECT `ID`,`SERVICENAME`,`DESCRIPTION`,`RATE`,`IMAGE` FROM `TBLSERVICES` WHERE 1", conn);
                    da.Fill(ds, "Detail");
                    ServiceDetail.dt = ds.Tables[0]; 
                    //--------------
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            { 
            } 
            return View("Index", ServiceDetail);
        }

        public JsonResult AvailablePC(Models.ServiceDetail obj) 
        {
            DataSet ds = new DataSet("Detail");
            string date1 = (obj.ServiceDate).ToString("yyyy-MM-dd"); 
            //Models.SystemDetail SystemDetails = new Models.SystemDetail();
            List<Models.SystemDetail> SystemDetails = new List<Models.SystemDetail>();
            try
            {
                using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(myConnectionString))
                {
                    //--------------
                    //MySql.Data.MySqlClient.MySqlDataAdapter da = new MySql.Data.MySqlClient.MySqlDataAdapter("SELECT `id`,`SystemNo` FROM `tblsystem` WHERE id not in (Select PCNo from tblbooking Where BookingDate='"+ date1 + "' )", conn);
                    //da.Fill(ds, "Detail");
                    //ServiceDetail.dt = ds.Tables[0];
                    //--------------
                    using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT `id`,`SystemNo` FROM `tblsystem` WHERE id not in (Select PCNo from tblbooking Where BookingDate='" + date1 + "' )", conn))
                    {
                        conn.Open();
                        MySql.Data.MySqlClient.MySqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            SystemDetails.Add(
                                new Models.SystemDetail
                                {
                                    id = reader.GetValue(0).ToString(),
                                    SystemNo = reader.GetString(1).ToString()
                                }
                            );
                        }
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
            } 
            return Json(SystemDetails, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BookDetails()
        {
            DataSet ds = new DataSet("Detail");
            Models.PCBookDetail PCBookDetail = new Models.PCBookDetail();
            try
            {
                string Query = "";
                if ((Session["Role"]).ToString() == "Owner")
                {
                    Query = "Select C.FullName, C.UserEmail, B.Service, B.TotHours, B.Amt,B.TotAmt, B.PCNo, B.BookingDate, B.billStatus from tblcustomers C, tblbooking B Where C.id = B.CustId Order By B.BookingDate";
                }
                else
                {
                    Query = "Select C.FullName, C.UserEmail, B.Service, B.TotHours, B.Amt, B.TotAmt, B.PCNo, B.BookingDate, B.billStatus from tblcustomers C, tblbooking B Where C.id = B.CustId AND  Username='"+ (Session["uid"]).ToString() + "' and Password='" + (Session["pwd"]).ToString() + "' Order By B.BookingDate";

                    
                }
                using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(myConnectionString))
                {
                    MySql.Data.MySqlClient.MySqlDataAdapter da = new MySql.Data.MySqlClient.MySqlDataAdapter(Query, conn);
                    da.Fill(ds, "Detail");
                    PCBookDetail.dt = ds.Tables[0];
                    if (ds.Tables[0].Rows.Count==0)
                    {
                        return View("NoDataFound");
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
            }
            return View(PCBookDetail);
        }

        public ActionResult NoDataFound()
        {
            return View();
        }
        //public ActionResult PaymentDetail()
        //{ 
        //    DataSet ds = new DataSet("Detail");
        //    Models.ServiceDetail ServiceDetail = new Models.ServiceDetail();
        //    try
        //    {
        //        using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(myConnectionString))
        //        {
        //            MySql.Data.MySqlClient.MySqlDataAdapter da = new MySql.Data.MySqlClient.MySqlDataAdapter("Select B.Service,B.PCNo,(B.TotHours * B.Amt) as TotAmt from tblcustomers C, tblbooking B Where C.Username='"+ (Session["uid"]).ToString() + "' and C.Password='"+ (Session["pwd"]).ToString() + "' AND C.id=B.CustId And billstatus = 'NO PAID'", conn);
        //            da.Fill(ds, "Detail");
        //            ServiceDetail.dt = ds.Tables[0];
        //            //--------------
        //        }
        //    }
        //    catch (MySql.Data.MySqlClient.MySqlException ex)
        //    {
        //    }
        //    return View("Index", ServiceDetail);
        //}
    }
}