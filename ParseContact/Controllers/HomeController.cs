using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Dynamic;
using System.Data.SqlClient;
using System.Configuration;

namespace ParseContact.Controllers
{
    public class HomeController : Controller
    {
        SQL sql = new SQL();
        [HttpGet]
        public ActionResult Index()
        {
            List<object[]> lstkalldata = sql.SelectAllData();
            List<object[]> lstkinds = sql.SelectAllKind();

            List<string> lstform = lstkalldata.Select(i => i[2].ToString()).Distinct().ToList();
            List<string> lstoblst = lstkalldata.Select(i => i[8].ToString()).Distinct().ToList();
            List<string> lstray = lstkalldata.Select(i => i[7].ToString()).Distinct().ToList();
            List<string> lstgorod = lstkalldata.Select(i => i[11].ToString()).Distinct().ToList();

            ViewBag.Form = lstform;
            ViewBag.Obl = lstoblst;
            ViewBag.Ray = lstray;
            ViewBag.Gorod = lstgorod;


            return View(lstkinds.OrderBy(i=> i[1]).ToList());
        }
        [HttpPost]
        public ActionResult Index(string[] NumbDet, string[] Form, string[] Obl, string[] Ray, string[] Gorod , string Table, int CoutMin=0, int CoutMax=1000000)
        {
            List<object[]> lstkalldata = sql.SelectAllData();
            List<object[]> lstkallActive = sql.SelectAllActivKind();
            List<object[]> allcont = sql.SelectAllContakt();

            List<string> idkund = lstkallActive.Where(i => NumbDet.Contains(i[1].ToString())).Select(q => q[0].ToString()).ToList();
            List<string> lstid = lstkalldata.Where(n => idkund == null ? 1 == 1 : idkund.Contains(n[0].ToString())).Where(f => Form == null ? 1 == 1 : Form.Contains(f[2].ToString())).Where(o => Obl == null ? 1 == 1 : Obl.Contains(o[8].ToString())).Where(r => Ray == null ? 1 == 1 : Ray.Contains(r[7].ToString())).Where(r => Ray == null ? 1 == 1 : Ray.Contains(r[7].ToString())).Where(g => Gorod == null ? 1 == 1 : Gorod.Contains(g[11].ToString())).Where(n => Convert.ToInt32(n[3])>= CoutMin).Where(x => Convert.ToInt32(x[3]) <= CoutMax).Select(s=>s[0].ToString()).ToList();
           // List<string> lstid = lstkalldata.Where(n => idkund == null ? 1 == 1 : idkund.Contains(n[0].ToString())).Where(f => Form == null ? 1 == 1 : Form.Contains(f[2].ToString())).Where(o => Obl == null ? 1 == 1 : Obl.Contains(o[8].ToString())).Where(r => Ray == null ? 1 == 1 : Ray.Contains(r[7].ToString())).Where(r => Ray == null ? 1 == 1 : Ray.Contains(r[7].ToString())).Where(g => Gorod == null ? 1 == 1 : Gorod.Contains(g[11].ToString())).Select(s => s[0].ToString()).ToList();

            List<object[]> numb = allcont.Where(i => lstid == null ? 1 == 1 : lstid.Contains(i[0].ToString())).ToList();
            try
            {
                sql.CreateTable(Table);
            }catch
            {
                return Content("Ошибка создание таблицы");
            }
            int count = 0;
            foreach (var item in numb)
            {
                string strtel = new string(item[2].ToString().Where(t => char.IsDigit(t)).ToArray());
                if (strtel.Length <= 9) { continue; }
               
                sql.ImportDataTel(Table,item[2].ToString(), item[0].ToString());
                count++;
            }

            return Content("Импортировано в таблицу " + count.ToString() + " номеров! <a href=\"/\">Назад</a>");
        }

        
    }

    public class SQL
    {
        public SqlConnection conn;
        public string connstr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public void ConnectSQLServer()
        {
            conn = new SqlConnection(connstr);
            conn.Open();
        }

        public List<object[]> SelectAllOrgKind()
        {
            ConnectSQLServer();
            SqlCommand cmd = new SqlCommand("SELECT * FROM [Contacts].[dbo].[OrgActivityKinds]", conn);
            SqlDataReader reader = cmd.ExecuteReader();
            List<object[]> result = new List<object[]>();
            while (reader.Read())
            {
                object[] str = new Object[reader.FieldCount];
                int fieldCount = reader.GetValues(str);
                result.Add(str);
            }
            reader.Close();
            conn.Close();
            conn.Dispose();
            return result;
        }
        public List<object[]> SelectAllKind()
        {
            ConnectSQLServer();
            SqlCommand cmd = new SqlCommand("SELECT * FROM [Contacts].[dbo].[ActivityKinds]", conn);
            SqlDataReader reader = cmd.ExecuteReader();
            List<object[]> result = new List<object[]>();
            while (reader.Read())
            {
                object[] str = new Object[reader.FieldCount];
                int fieldCount = reader.GetValues(str);
                result.Add(str);
            }
            reader.Close();
            conn.Close();
            conn.Dispose();
            return result;
        }

        public List<object[]> SelectAllData()
        {
            ConnectSQLServer();
            SqlCommand cmd = new SqlCommand("SELECT * FROM [Contacts].[dbo].[OrganizationsAllData]", conn);
            SqlDataReader reader = cmd.ExecuteReader();
            List<object[]> result = new List<object[]>();
            while (reader.Read())
            {
                object[] str = new Object[reader.FieldCount];
                int fieldCount = reader.GetValues(str);
                result.Add(str);
            }
            reader.Close();
            conn.Close();
            conn.Dispose();
            return result;
        }
        public List<object[]> SelectAllActivKind()
        {
            ConnectSQLServer();
            SqlCommand cmd = new SqlCommand("SELECT * FROM[Contacts].[dbo].[OrgActivityKinds]", conn);
            SqlDataReader reader = cmd.ExecuteReader();
            List<object[]> result = new List<object[]>();
            while (reader.Read())
            {
                object[] str = new Object[reader.FieldCount];
                int fieldCount = reader.GetValues(str);
                result.Add(str);
            }
            reader.Close();
            conn.Close();
            conn.Dispose();
            return result;
        }
        public List<object[]> SelectAllContakt()
        {
            ConnectSQLServer();
            SqlCommand cmd = new SqlCommand("SELECT * FROM [Contacts].[dbo].[ContactInfo]", conn);
            SqlDataReader reader = cmd.ExecuteReader();
            List<object[]> result = new List<object[]>();
            while (reader.Read())
            {
                object[] str = new Object[reader.FieldCount];
                int fieldCount = reader.GetValues(str);
                result.Add(str);
            }
            reader.Close();
            conn.Close();
            conn.Dispose();
            return result;
        }
        public bool CreateTable(string nametable)
        {
            ConnectSQLServer();
            SqlCommand cmd2 = new SqlCommand("CREATE TABLE [Contacts].[dbo].[" + nametable + "]([Id] [int] IDENTITY(1,1) NOT NULL,[Telef] [nvarchar](100) NULL,[Json] [nvarchar](max) NULL,[IdCompan] [nvarchar](50) NULL,[GuidTel] [nvarchar](100) NULL, CONSTRAINT [PK_" + nametable + "] PRIMARY KEY CLUSTERED ([Id] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]", conn);
            cmd2.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
            return true;
        }

         public bool ImportDataTel(string nametable, string tel, string idco)
        {
            ConnectSQLServer();
           
            string guid = Guid.NewGuid().ToString();
            SqlCommand cmd2 = new SqlCommand("INSERT INTO [Contacts].[dbo].[" + nametable + "] (Telef, Json, IdCompan, GuidTel) Values  ( '" + tel + "', '', '"+ idco + "', '" + guid + "' )", conn);
            cmd2.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
            return true;
        }


    }
}