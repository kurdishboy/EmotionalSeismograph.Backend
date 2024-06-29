using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace Core.Services
{
    public class CreateResponse
    {
        public string result { get; set; }
        public int status { get; set; }
        public List<string> errors { get; set; }

        public CreateResponse(string result, int status, List<string> errors)
        {
            this.result = result;
            this.status = status;
            this.errors = errors;
        }
        public static string getResult(string result, int status, List<string>? errors)
        {
            string resultStr = "";
            if (result != null && result != "")
            {
                resultStr = result;

            }
            else
            {
                resultStr = "null";
            }
            return @"{ " +
                "\"result\": " + resultStr + ", " +
                "\"status\":" + status + "," +
                "\"errors\": [" + errors + "]" +
                " }";
        }
        public static string getScalarResult(string result, int status, List<string>? errors)
        {
            if (errors != null)
            {
                return @"{ " +
               "\"result\": \"" + result + "\", " +
               "\"status\":" + status + "," +
               "\"errors\": [" + string.Join(",", errors) + "]" +
               " }";
            }
            else
            {
                return @"{ " +
                    "\"result\": \"" + "null" + "\", " +
                    "\"status\":" + status +
                    " }";
            }
        }
    }
    public class CreateResponse<T>
    {
        public T result { get; set; }
        public int status { get; set; }
        public List<string> errors { get; set; }
        public CreateResponse(T result, int status, List<string> errors)
        {
            this.result = result;
            this.status = status;
            this.errors = errors;
        }
    }
}
