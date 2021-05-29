using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;

namespace ProjectImmediateReply.Utility
{
    public class DBTool
    {
        //資料庫連結字串
        private const string connectionString =
               "Data Source=localhost\\SQLEXPRESS;Initial Catalog=Exam; Integrated Security=true";


        public DataTable readTable(string readtablename, string[] readcolname,
           string Logic, string[] Pname, string[] P)
        {
            //將接過來的目標欄位名稱陣列用「,」連接成一個字串
            string readcoladd = string.Join(",", readcolname);
            //SQL語法參數化"SELECT 欄位名稱 FROM 資料表名稱 條件"
            string queryString =
                $@" SELECT {readcoladd} FROM {readtablename}
                    {Logic};";

            //資料庫開啟並執行SQL
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                //傳入參數至@目標欄位
                if (Pname != null && P != null
                    && Pname.Length != 0 && P.Length != 0)
                {
                    for (int i = 0; i < Pname.Length; i++)
                        command.Parameters.AddWithValue(Pname[i], P[i]);  //將command指令串內的@目標欄位以傳入參數取代
                }
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
                catch (Exception )
                {
                    throw;
                }
            }
        }





        /// <summary>
        /// 往資料庫中插入新資料列
        /// INSERT INTO 資料表名稱 (欄位名稱) VALUES (@欄位名稱)
        /// 傳入的@欄位和參數的順序必須相同
        /// </summary>
        /// <param name="inserttablename">目標資料表名稱</param>
        /// <param name="insertcolname">目標欄位名稱的陣列</param>
        /// <param name="insertcolname_P">目標欄位名稱帶有@的陣列</param>
        /// <param name="insert_P">需給予@欄位之參數值的集合</param>
        public void InsertTable(string inserttablename, string[] insertcolname,
            string[] insertcolname_P, List<string> insert_P)
        {
            //將接過來的目標欄位名稱及目標欄位名稱帶有@的陣列各自用「,」連接成一個字串
            string insertcolum = string.Join(",", insertcolname);
            string insertparameter = string.Join(",", insertcolname_P);
            //將user輸入的集合轉為陣列
            string[] puserinsert = insert_P.ToArray();
            //SQL語法參數化"INSERT INTO 資料表名稱 (欄位名稱) VALUES (@欄位名稱)"
            string queryString =
               $@" INSERT INTO {inserttablename}
                         ({insertcolum})
                   VALUES
                         ({insertparameter})";
            //資料庫開啟並執行SQL
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlTransaction sqlTransaction = connection.BeginTransaction();
                command.Transaction = sqlTransaction;
                try
                {
                    //利用迴圈將參數一個一個放進@欄位
                    for (int i = 0; i < insertcolname_P.Length; i++)
                    {
                        command.Parameters.AddWithValue($"{insertcolname_P[i]}", puserinsert[i]);
                    }
                    command.ExecuteNonQuery();
                    sqlTransaction.Commit();
                }
                catch (Exception )
                {
                    sqlTransaction.Rollback();
                    throw;
                }
            }
        }
    }
}