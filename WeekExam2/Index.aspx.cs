using ProjectImmediateReply.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WeekExam2
{
    public partial class Index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BuildDataTableCommit();
            }

        }

        public void BuildDataTableCommit()
        {
            string page = Request.QueryString["Page"];
            int pIndex;
            if (string.IsNullOrEmpty(page))
                pIndex = 1;
            else
            {
                int.TryParse(page, out pIndex);

                if (pIndex <= 0)
                    pIndex = 1;
            }

            int totalSize;
            int _pageSize = Convert.ToInt32(DropDownList1.SelectedValue);

            DataTable Comdata = readTableForPage(out totalSize, pIndex, _pageSize);

            if (Comdata.Rows.Count>0)
            {
                Nothingdiv.Visible = false;
            }
            else
            {
                Nothingdiv.Visible = true;
            }

            int pages = CalculatePages(totalSize, _pageSize);
            List<PagingLink> pagingList = new List<PagingLink>();
            HyperLink1.NavigateUrl = $"Index.aspx?Page={pages}";
            for (var i = 1; i <= pages; i++)
            {
                pagingList.Add(new PagingLink()
                {
                    Link = $"Index.aspx?Page={i}",
                    Name = $"{i}",
                    Title = $"前往第 {i} 頁"
                });
            }

            this.repPaging.DataSource = pagingList;
            this.repPaging.DataBind();




            Repeater1.DataSource = Comdata;
            Repeater1.DataBind();
        }

        protected void InsertTable(int id)
        {
            DBTool dBTool = new DBTool();
            if (id == 0)
            {
                string[] colname = { "UserName", "Title", "UserCommit", "CommitTime" };
                string[] colnamep = { "@UserName", "@Title", "@UserCommit", "@CommitTime" };
                List<string> p = new List<string>();
                p.Add(TextBox1.Text);
                p.Add(TextBox2.Text);
                p.Add(TextBox3.Text);
                p.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                dBTool.InsertTable("Exam2_1", colname, colnamep, p);

                BuildDataTableCommit();
            }
            else
            {
                string[] colname = { "CommitID", "UserName", "Title", "UserCommit", "CommitTime" };
                string[] colnamep = { "@CommitID", "@UserName", "@Title", "@UserCommit", "@CommitTime" };
                List<string> p = new List<string>();
                p.Add(id.ToString());
                p.Add(TextBox1.Text);
                p.Add(TextBox2.Text);
                p.Add(TextBox3.Text);
                p.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                dBTool.InsertTable("Exam2_2", colname, colnamep, p);

                BuildDataTableCommit();
            }
        }

        private bool Checkerror()
        {
            bool check = true;
            if (string.IsNullOrWhiteSpace(TextBox1.Text))
            {
                ltlNickName.Text = "必須輸入";
                ltlNickName.Visible = true;
                check = false;
            }
            else
            {
                ltlNickName.Visible = false;
            }
            if (string.IsNullOrWhiteSpace(TextBox2.Text))
            {
                ltlTitle.Text = "必須輸入";
                ltlTitle.Visible = true;
                check = false;
            }
            else
            {
                ltlTitle.Visible = false;
            }
            if (string.IsNullOrWhiteSpace(TextBox3.Text))
            {
                ltlCom.Text = "必須輸入";
                ltlCom.Visible = true;
                check = false;
            }
            else
            {
                ltlCom.Visible = false;
            }
            return check;
        }


        protected void Submit_Click(object sender, EventArgs e)
        {

            if (TextBox1.Text.Length > 30 || TextBox2.Text.Length > 100 || TextBox3.Text.Length > 250)
            {
                Response.Redirect("/Index.aspx?Page=1");
                return;
            }
            if (!Checkerror())
            {
                return;
            }


            if (Label1.Text == "0" || Label1.Text == "")
            {
                InsertTable(0);
            }
            else
            {
                InsertTable(Convert.ToInt32(Label1.Text));
            }
            Label1.Text = "0";
            TextBox1.Text = "";
            TextBox2.Text = "";
            TextBox3.Text = "";
            Response.Redirect("/Index.aspx?Page=1");
        }

        protected void Cancel_Click1(object sender, EventArgs e)
        {
            TextBox1.Text = "";
            TextBox2.Text = "";
            TextBox3.Text = "";
            Label1.Text = "0";
        }

        protected void Repeater1_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string cmdName = e.CommandName;
            string cmdArgu = e.CommandArgument.ToString();
            if (cmdName == "CommitItem")
            {
                Label1.Text = cmdArgu;

            }
        }

        protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Repeater rel = e.Item.FindControl("Repeater2") as Repeater;
                DBTool dBTool = new DBTool();
                Button btn = e.Item.FindControl("ButtonReCommit") as Button;
                Literal ltl = e.Item.FindControl("Literal2") as Literal;
                //Literal lt2 = e.Item.FindControl("Literal3") as Literal;

                string cmdArgu = btn.CommandArgument.ToString();
                if (!string.IsNullOrWhiteSpace(cmdArgu))
                {
                    string[] Comcolname = { "ReCommitID", "CommitID", "UserName", "CommitTime", "Title", "UserCommit" };
                    string[] Comcolnamep = { "@CommitID" };
                    string[] Comp = { cmdArgu };
                    string Comlogic = @"
                                    WHERE CommitID=@CommitID
                                    ORDER BY ReCommitID DESC
                                    ";
                    DataTable Comdata = dBTool.readTable("Exam2_2", Comcolname, Comlogic, Comcolnamep, Comp);

                    List<ReCommitData> list = new List<ReCommitData>();
                    for (int i = 0; i < Comdata.Rows.Count; i++)
                    {
                        ReCommitData RCD = new ReCommitData()
                        {
                            ReCommitID = $"{Comdata.Rows[i]["CommitID"]}-{Comdata.Rows.Count - i}",
                            CommitID = Comdata.Rows[i]["CommitID"].ToString(),
                            UserName = Comdata.Rows[i]["UserName"].ToString(),
                            CommitTime = Convert.ToDateTime(Comdata.Rows[i]["CommitTime"]),
                            Title = Comdata.Rows[i]["Title"].ToString(),
                            UserCommit = Comdata.Rows[i]["UserCommit"].ToString(),
                        };
                        list.Add(RCD);

                    };



                    rel.DataSource = list;
                    rel.DataBind();
                    ltl.Text = Comdata.Rows.Count.ToString();



                }

            }
        }














        private const string connectionString =
              "Data Source=localhost\\SQLEXPRESS;Initial Catalog=Exam; Integrated Security=true";
        public DataTable readTableForPage(out int totalSize, int currentPage = 1, int pageSize = 10)
        {
            string queryString =
                $@" 
                    SELECT TOP {pageSize} * FROM
                    (
                        SELECT 
                            ROW_NUMBER() OVER(ORDER BY CommitID DESC) AS RowNumber,
                             CommitID,
                             UserName, 
                             CommitTime,
                             Title,
                             UserCommit 
                        FROM Exam2_1
                    ) AS TempT
                    WHERE RowNumber > {pageSize * (currentPage - 1)}
                    ORDER BY CommitID DESC
                ";

            //資料庫開啟並執行SQL
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader(); //執行指令串
                    DataTable dt = new DataTable();
                    dt.Load(reader); // 將reader放入dt表
                    reader.Close();
                    connection.Close();
                    DataTable totalSize1 = readTablePageNum();
                    int? totalSize2 = totalSize1.Rows[0]["COUNT"] as int?;
                    totalSize = (totalSize2.HasValue) ? totalSize2.Value : 0;
                    return dt;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }


        public DataTable readTableNum(string id)
        {
            string countQuery =
                $@" SELECT 
                        COUNT(ReCommitID) AS COUNT
                    FROM Exam2_2 
                    WHERE CommitID=@CommitID
                ";
            //資料庫開啟並執行SQL
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(countQuery, connection);
                command.Parameters.AddWithValue("@CommitID", id);
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader(); //執行指令串
                    DataTable dt = new DataTable();
                    dt.Load(reader); // 將reader放入dt表
                    reader.Close();
                    connection.Close();
                    return dt;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public DataTable readTablePageNum()
        {
            string countQuery =
                $@" SELECT 
                        COUNT(CommitID) AS COUNT
                    FROM Exam2_1
                ";
            //資料庫開啟並執行SQL
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(countQuery, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader(); //執行指令串
                    DataTable dt = new DataTable();
                    dt.Load(reader); // 將reader放入dt表
                    reader.Close();
                    connection.Close();
                    return dt;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public int CalculatePages(int totalSize, int pageSize)
        {
            int pages = totalSize / pageSize;

            if (totalSize % pageSize != 0)
                pages += 1;

            return pages;
        }


        internal class PagingLink
        {
            public string Name { get; set; }
            public string Link { get; set; }
            public string Title { get; set; }
        }

        internal class ReCommitData
        {
            public string ReCommitID { get; set; }
            public string CommitID { get; set; }
            public string UserName { get; set; }
            public DateTime CommitTime { get; set; }
            public string Title { get; set; }
            public string UserCommit { get; set; }

        }

        protected void repPaging_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            BuildDataTableCommit();
        }
    }
}