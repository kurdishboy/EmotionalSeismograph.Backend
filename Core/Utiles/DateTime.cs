using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Core.Utiles
{
    public class DT
    {
        public static DateTime Shamsi_To_Miladi(string target_str)
        {
            PersianCalendar pc = new PersianCalendar();
            DateTime dt = new DateTime(int.Parse(target_str.Split('/')[0]), int.Parse(target_str.Split('/')[1]), int.Parse(target_str.Split('/')[2]), pc);
            return dt;
        }

        public static string Miladi_To_Shamsi(string target_str)
        {
            DateTime d = DateTime.Parse(target_str);
            PersianCalendar pc = new PersianCalendar();
            return string.Format("{0}/{1}/{2}", pc.GetYear(d), pc.GetMonth(d), pc.GetDayOfMonth(d));
        }
    }
}
