using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IDAL
    {
        IDAL DirectInstance();
        SqlCommand cmd { get; set; }
        DataTable dt { get; set; }
        string Err { get; }
        void disconnect();
        Task<int> executenonquery(string sql);
        Task<object> executescaler(string sql);
        Task<DataTable> select(string sql);
        Task<SqlDataReader> executereader(string sql);

        Task<string> executeReaderStream(string sql);
    }
}
