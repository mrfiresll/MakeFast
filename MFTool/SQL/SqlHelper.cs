//------------------------------------------------------------------------
// 程序名称：
// 功能：封装SQL的操作
// 编制者：宋亮亮         完成日期：2010.10.20
// 版本：v1.0
// 其他：
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Specialized;

namespace MFTool
{
    /// <summary>
    /// SQL帮助类
    /// </summary>
    public class SqlHelper
    {
        public SqlHelper(string conn, bool isDBName = false)
        {
            if (isDBName)
            {
                conn = WebConfigHelper.GetConnSettingNameByDBName(conn);
                conn.CheckNotNull("无法找到数据库{0}的对应ConnectionStrings的name".ReplaceArg(conn));
            }
            System.Configuration.ConfigurationManager.ConnectionStrings[conn].CheckNotNull("无法找到name为{0}的ConnectionStrings".ReplaceArg(conn));
            m_strSqlConnect = System.Configuration.ConfigurationManager.ConnectionStrings[conn].ToString();
        }

        private string m_strSqlConnect = "";

        /// <summary>
        /// 1.0 执行查询语句，返回一个表
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="ps">参数数组</param>
        /// <returns>返回一张表</returns>
        public DataTable ExcuteTable(string sql, params SqlParameter[] ps)
        {
            SqlDataAdapter da = new SqlDataAdapter(sql, m_strSqlConnect);
            da.SelectCommand.Parameters.AddRange(ps);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        } 

        /// <summary>
        /// 2.0 执行增删改的方法
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="ps">参数数组</param>
        /// <returns>返回一条记录</returns>
        public int ExcuteNoQuery(string sql, params SqlParameter[] ps)
        {
            using (SqlConnection conn = new SqlConnection(m_strSqlConnect))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddRange(ps);
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 3.0 执行存储过程的方法
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="ps">参数数组</param>
        /// <returns></returns>
        public int ExcuteProc(string procName, params SqlParameter[] ps)
        {
            using (SqlConnection conn = new SqlConnection(m_strSqlConnect))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(procName, conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddRange(ps);
                return command.ExecuteNonQuery();
            }
        } 
        
        /// <summary>
        /// 4.0 查询结果集，返回的是首行首列
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="ps">参数数组</param>
        /// <returns></returns>
        public object ExecScalar(string sql, params SqlParameter[] ps)  //调用的时候才判断是什么类型
        {
            using (SqlConnection conn = new SqlConnection(m_strSqlConnect))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddRange(ps);
                return command.ExecuteScalar();
            }
        }
    }
}