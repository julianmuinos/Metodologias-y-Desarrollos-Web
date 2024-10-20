using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.DAL;

namespace MyApp.Services
{
    public static class Logger
    {
        static ArrayList _al = new ArrayList();
        public static DataTable GetLogs()
        {
            try { return DAO.Leer("sp_Log_List"); }
            catch (Exception ex) { throw ex; }
        }
        public static void AddLog(string logMessage, int logCriticity, string userEmail)
        {
            try
            {
                _al.Add(new SqlParameter("@message", SqlDbType.NVarChar) { Value = logMessage });
                _al.Add(new SqlParameter("@criticity", SqlDbType.Int) { Value = logCriticity });
                _al.Add(new SqlParameter("@time", SqlDbType.Date) { Value = DateTime.Now });
                _al.Add(new SqlParameter("@EmailUser", SqlDbType.NVarChar) { Value = userEmail });
                DAO.Escribir("sp_Log_Add", _al);
            }
            catch (Exception ex) { throw ex; }
            finally { _al.Clear(); }
        }
    }
}
