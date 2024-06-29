using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Core.Layers
{
    public class DAL : IDAL, IDisposable
    {
        private static string con_string;
        SqlConnection con;
        private SqlCommand _cmd;
        public SqlCommand cmd { get => _cmd; set => _cmd = value; }
        SqlDataAdapter da;
        private DataTable _datatable;
        private SqlTransaction Trans;
        private string _Err = "";
        public DataTable dt { get => _datatable; set => _datatable = value; }
        public string Err => _Err;
        public IDAL DirectInstance()
        {
            //setup
            return this;
        }

        public DAL(IConfiguration conf)
        {
            con_string = conf.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
            con = new SqlConnection();
            con.ConnectionString = con_string;
            cmd = new SqlCommand();
            cmd.Connection = con;
            da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            _datatable = new DataTable();
        }

        void connect()
        {
            if (con.State == ConnectionState.Closed)///اگر کانکشن بسته شده باشد
            {
                con.Open();
            }
            else
            {
                con.Close();
                con.Open();
            }
        }
        public void disconnect()
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }

        public async Task<object> executescaler(string sql)
        {
            cmd.CommandText = sql;
            connect();
            //cmd.Prepare();
            object result = await cmd.ExecuteScalarAsync();
            disconnect();
            return result;
        }
        public async Task<int> executenonquery(string sql)
        {
            try
            {
                cmd.CommandText = sql;
                connect();
                //Trans = con.BeginTransaction();
                //cmd.Transaction = Trans;
                //cmd.Prepare();
                int rowsaffeccted = await cmd.ExecuteNonQueryAsync();
                //cmd.Dispose();
                //Trans.Commit();
                disconnect();
                return rowsaffeccted;
            }
            catch (Exception x)
            {
                var t = x;
                return 0;
            }

        }
        public async Task<DataTable> select(string sql)
        {
            dt = new DataTable();
            connect();
            da.SelectCommand.Connection = con;
            da.SelectCommand.CommandText = sql;
            // cmd.Prepare();
            await da.SelectCommand.ExecuteReaderAsync();
            disconnect();
            da.Fill(dt);
            return dt;
        }
        public async Task<SqlDataReader> executereader(string sql)
        {
            SqlDataReader DATAREADER = null;
            _cmd.CommandText = sql;
            connect();
            // cmd.Prepare();
            DATAREADER = await cmd.ExecuteReaderAsync();
            return DATAREADER;
        }

        public async Task<string> executeReaderStream(string sql)
        {

            SqlDataReader DATAREADER = null;
            _cmd.CommandText = sql;
            connect();
            // cmd.Prepare();
            DATAREADER = await cmd.ExecuteReaderAsync();


            string target = "";
            while (DATAREADER.Read())
            {
                target += DATAREADER[0].ToString();
            }
            return target;
        }

        public void Dispose()
        {
            //clear and disconnect
        }
    }
}
