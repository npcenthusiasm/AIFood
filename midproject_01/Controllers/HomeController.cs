using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using midproject_01.Models;
using System.Data.SqlClient;
using System.Text;

namespace midproject_01.Controllers
{
    public class HomeController : Controller
    {
        //CalDBEntities db = new CalDBEntities(); // 本地資料庫
       CalDBEntities1 db = new CalDBEntities1(); //Azure
        //CalDBEntities2 db = new CalDBEntities2(); //AzureCal0617 僑宏雲端
        // GET: Home
        //首頁

        DateTime dayTime = DateTime.Today;

        public ActionResult Index()
        {
            var CookiesID = Request.Cookies["MemberID"];

            if (CookiesID == null)
            {
                return Redirect("~/Home/Login");
            }



            //string cookie = HttpUtility.UrlDecode(Request.Cookies["UserName"].Value);
            //byte[] mybyte = System.Convert.FromBase64String(Request.Cookies["UserName"].Value.ToString());
            //string cookie = System.Text.Encoding.UTF8.GetString(mybyte);
            string Today = dayTime.ToString("yyyyMMdd");

            //LINQ to Entities 無法辨識方法 'System.String ToString(System.String)' 方法，而且這個方法無法轉譯成存放區運算式。
            //dayTime.ToString("yyyyMMdd")
            //必須+ .AsEnumerable()
            var query = from o in db.PersonCals
                        where o.MemberID.ToString() == CookiesID.Value.ToString()
                        where o.RecordDay == Today
                        select o;
            return View(query.ToList());
        }
        [HttpPost]
        public ActionResult Index(string date)
        {
            // 06/20/2019
            ViewBag.date = date;
            DateTime dateA = Convert.ToDateTime(date);
            var dateFormat = dateA.ToString("yyyyMMdd");
            var query = from o in db.PersonCals
                        where o.RecordDay == dateFormat
                        select o;
            
            return View(query.ToList());
        }
        //登入 在下方

        //註冊Step1
        public ActionResult Accounts()
        {
            Memberlist NewMember = new Memberlist();
            return View(NewMember);
        }

        [HttpPost]
        public ActionResult Accounts(string Email, string UserName, string Password)
        {
            if (Email == null || Email == "")
            {
                return Redirect("~/Home/Accounts");
            }


            //若模型沒有通過驗證則顯示目前的View
            if (ModelState.IsValid == false)
            {
                return View();
            }

            //Memberlist
            // 依帳號取得會員並指定給member
            var member = db.Memberlists
                .Where(m => m.Email == Email)
                .FirstOrDefault();

            //若member為null，代表註冊的帳號在資料庫中沒有重複
            if (member == null)
            {
                //將會員資料先用session存起來
                Session["NewMemberEmail"] = Email;
                Session["NewMemberUserName"] = UserName;
                Session["NewMemberPassword"] = Password;
                //導向新增身高體重的頁面
                return RedirectToAction("AccountsGoalAdd");
            }
            ViewBag.Message = "此帳號己有人使用，註冊失敗";
            return View();
        }

        //註冊Step2 新增目標
        public ActionResult AccountsGoalAdd()
        {
            Memberlist NewMember = new Memberlist();
            return View(NewMember);
        }

        [HttpPost]
        public ActionResult AccountsGoalAdd(Memberlist NewMember)
        {
            DateTime today = DateTime.Today;
            if (NewMember == null || NewMember.Age.ToString() == "" || NewMember.Weight.ToString() == "")
            {
                return Redirect("~/Home/AccountsGoalAdd");
            }
            else
            {
                db.Memberlists.Add(NewMember);
            }

            Personrecord pr = new Personrecord();
            pr.Height = NewMember.Height;
            pr.Weight = NewMember.Weight;
            pr.Measureday = dayTime.ToString("yyyyMMdd");
            db.Personrecords.Add(pr);

            //將剛剛用session存起來的會員資料一起存到資料庫
            NewMember.Email = Session["NewMemberEmail"].ToString();
            NewMember.UserName = Session["NewMemberUserName"].ToString();
            NewMember.Password = Session["NewMemberPassword"].ToString();
            NewMember.Startday = today.ToString("yyyyMMdd");

            db.SaveChanges();
            //Session["FirstUse"] = "True";
            return RedirectToAction("Index");
        }

        //每日紀錄
        public ActionResult UploadImgVer2()
        {
            var CookiesID = Request.Cookies["MemberID"];

            if (CookiesID == null)
            {
                return Redirect("~/Home/Login");
            }
            else
            {
                PersonCal p = new PersonCal();
                return View();
            }

        }

        [HttpPost]
        public ActionResult UploadImgVer2(PersonCal p)
        {
            var CookieID = Request.Cookies["MemberID"];

            //中文解碼
            byte[] mybyte = System.Convert.FromBase64String(Request.Cookies["UserName"].Value.ToString());
            string CookieName = System.Text.Encoding.UTF8.GetString(mybyte);

            DateTime today = DateTime.Today;
            p.UserName = CookieName;
            p.MemberID = Convert.ToInt32(CookieID.Value);
            p.RecordDay = today.ToString("yyyyMMdd");   //20190606
            p.SignInDay = today.ToString("yyyyMMdd");   //20190606

            db.PersonCals.Add(p);
            db.SaveChanges();
            return RedirectToAction("Index");

        }
   
        //進展
        public ActionResult progress(PersonCal p)
        {
            var CookiesID = Request.Cookies["MemberID"];

            if (CookiesID == null)
            {
                return Redirect("~/Home/Login");
            }

            var query = from o in db.Personrecords
                        where o.MemberID.ToString() == CookiesID.Value
                        select o;

            return View(query.ToList());
        }

        [HttpPost]
        public ActionResult progress(Personrecord p)
        {
            var CookieId = Request.Cookies["MemberID"];
            Personrecord x = new Personrecord();
            x.Weight = p.Weight;
            x.Height = p.Height;
            x.MemberID = Convert.ToInt32(CookieId.Value);
            x.Measureday = dayTime.ToString("yyyyMMdd");
            db.Personrecords.Add(x);
            db.SaveChanges();


            return RedirectToAction("progress");
        }
        //會員中心
        public ActionResult MemberInfo()
        {
            var CookiesID = Request.Cookies["MemberID"];

            if (CookiesID == null)
            {
                return Redirect("~/Home/Login");
            }
            var CookiesIDint = Convert.ToInt32(CookiesID.Value);
            var query = from o in db.Memberlists
                        where o.MemberID == CookiesIDint
                        select o;

            return View(query.FirstOrDefault());

        }
        [HttpPost]
        public ActionResult MemberInfo(Memberlist member)
        {
            var CookiesID = Request.Cookies["MemberID"];

            if (CookiesID == null)
            {
                return Redirect("~/Home/Login");
            }
            var CookiesIDint = Convert.ToInt32(CookiesID.Value);
            var query = from o in db.Memberlists
                        where o.MemberID == CookiesIDint
                        select o;
            Memberlist Sqlmember = query.FirstOrDefault();
            Sqlmember.Age = member.Age;
            Sqlmember.Gender = member.Gender;
            Sqlmember.Height = member.Height;
            Sqlmember.Weight = member.Weight;
            Sqlmember.Goal = member.Goal;
            Sqlmember.ActivityCoefficient = member.ActivityCoefficient;
            db.SaveChanges();
            return Redirect("~/Home/Setting");

        }
        //會員目標設定
        public ActionResult Setting()
        {
            var CookiesID = Request.Cookies["MemberID"];

            if (CookiesID == null)
            {
                return Redirect("~/Home/Login");
            }

            return View();
        }

        //帳號設定
        public ActionResult AccountsInfo()
        {
            var CookiesID = Request.Cookies["MemberID"];

            if (CookiesID == null)
            {
                return Redirect("~/Home/Login");
            }
            var CookiesIDint = Convert.ToInt32(CookiesID.Value);
            var query = from o in db.Memberlists
                        where o.MemberID == CookiesIDint
                        select o;

            return View(query.FirstOrDefault());
        }

        //登入
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string UserID, string Password)
        {
            // 依帳密取得會員並指定給member
            var member = db.Memberlists
                .Where(m => m.Email == UserID && m.Password == Password)
                .FirstOrDefault();


            //若member為null，表示會員未註冊 or 帳號密碼錯誤
            if (member != null)
            {
                //登入成功才生成cookies
                //直接用cookies存目標熱量 此行勿刪!!!
                HttpCookie CookieID = Request.Cookies["MemberID"];
                HttpCookie SuitableCaloriePerDay = Request.Cookies["SuitableCaloriePerDay"];

                ////中文加密 
                byte[] mybyte = System.Text.Encoding.UTF8.GetBytes(member.UserName.ToString());
                string CookieName = System.Convert.ToBase64String(mybyte);

                //把會員資訊存到Cookies
                Response.Cookies["MemberID"].Value = member.MemberID.ToString();
                Response.Cookies["UserName"].Value = CookieName;
                Response.Cookies["SuitableCaloriePerDay"].Value = member.SuitableCaloriePerDay.ToString();

                //
                //if (Session["FirstUse"].ToString() == "True")
                //{
                //    Session["FirstUse"] = "False";
                //    return Redirect("~/Home/Introduction");
                //}
                //
                return Redirect("~/Home/Index");

            }
            else
            {
                ViewBag.Message = "帳密錯誤，登入失敗";
                return View();
            }

        }

        //登出
        public ActionResult Logout()
        {
            if (Request.Cookies["MemberID"] != null)
            {
                var CookieID = new HttpCookie("MemberID");
                CookieID.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(CookieID);
            }
            return Redirect("~/Home/Index");

        }

        //營養資訊查詢
        public ActionResult SuggestFoodList()
        {
            var query = from o in db.Lists
                        select o;
            ViewBag.Class = "";
            ViewBag.ClassID = "";
            return View(query.ToList());

        }
        [HttpPost]
        public ActionResult SuggestFoodList(string FoodClass)
        {
            if (FoodClass == "")
            {
                var queryAll = from o in db.Lists
                            select o;
                return View(queryAll.ToList());

            }
            var query = from o in db.Lists
                        where o.ClassID == FoodClass
                        select o;
            ViewBag.Class = query.FirstOrDefault().Class;
            ViewBag.ClassID = query.FirstOrDefault().ClassID;
            return View(query.ToList());

        }



        //首頁的動畫 
        public ActionResult WaitPage()
        {

            return View();

        }

        //介紹的說明
        public ActionResult Introduction()
        {

            return View();

        }

        //刪除首頁的紀錄
        public ActionResult IndexDelete(int? id)
        {
            var query = from o in db.PersonCals
                        where o.PersonCalID == id
                        select o;
            PersonCal p = query.FirstOrDefault();
            db.PersonCals.Remove(p);
            db.SaveChanges();
            return Redirect("/Home/Index");
        }

        //刪除進度的紀錄
        public ActionResult ProgressDelete(int? id)
        {
            var query = from o in db.Personrecords
                        where o.RecordID == id
                        select o;
            Personrecord p = query.FirstOrDefault();
            db.Personrecords.Remove(p);
            db.SaveChanges();
            return Redirect("/Home/progress");
        }

        //日歷
        public ActionResult FullCalendar()
        {
            return View();
        }

        //日歷的Json資料
        public JsonResult FullCalendarData()
        {
            //要是用於創建代理（以及禁用延遲加載）禁用代理擾亂序列yiyang
            db.Configuration.ProxyCreationEnabled = false;
            var CookiesID = Request.Cookies["MemberID"];

            //if (CookiesID == null)
            //{
            //    return Redirect("~/Home/Login");
            //}
            var CookiesIDint = Convert.ToInt32(CookiesID.Value);
            var query = from o in db.PersonCals
                        where o.MemberID == CookiesIDint
                        select o;
            var personCalList = query.ToList();
            return new JsonResult { Data = personCalList,JsonRequestBehavior= JsonRequestBehavior.AllowGet };
        }
    }
}