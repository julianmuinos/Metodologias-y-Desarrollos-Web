using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace MyApp.Services
{
    public static class Backup
    {
        static string path = string.Empty;
        static string conexion = "Data Source=.;Initial Catalog=eCommerce;Integrated Security=True;";

        public static string CreateBackup(string pUserEmail)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conexion))
                {
                    con.Open();
                    path = @"C:\Backup\" + $"\\eCommerce={DateTime.Now.Day}-" +
                        $"{DateTime.Now.Month}-{DateTime.Now.Year}=" +
                        $"{DateTime.Now.Hour}-{DateTime.Now.Minute}-{DateTime.Now.Second}.bak";
                    SqlCommand command = new SqlCommand("backup database [eCommerce] to disk = @path", con);
                    command.Parameters.AddWithValue("path", path);
                    command.ExecuteNonQuery();
                    Logger.AddLog($"BACKUP - Backup creado ({DateTime.Now})", 3, pUserEmail);
                    return "Backup creado exitosamente";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string RestoreBackup(string pUserEmail, string pPath)
        {
            try
            {
                string filePath = pPath;
                if (filePath.Split('.')[1] != "bak") { throw new Exception("Archivo no es formato backup"); }

                using (SqlConnection con = new SqlConnection(conexion))
                {
                    con.Open();

                    SqlCommand command = new SqlCommand("ALTER DATABASE [eCommerce] SET OFFLINE WITH ROLLBACK IMMEDIATE", con);
                    command.ExecuteNonQuery();

                    command = new SqlCommand("RESTORE DATABASE [eCommerce] FROM DISK = @path WITH REPLACE", con);
                    command.Parameters.AddWithValue("path", filePath);
                    command.ExecuteNonQuery();

                    Logger.AddLog($"BACKUP - Base de datos restaurada exitosamente ({DateTime.Now})", 3, pUserEmail);
                    return "Base de datos restaurada exitosamente";
                }
            }
            catch (SqlException sqlEx)
            {
                Logger.AddLog($"BACKUP - Error SQL al intentar restaurar la base de datos: {sqlEx.Message} ({DateTime.Now})", 3, pUserEmail);
                return $"Error SQL: {sqlEx.Message}";
            }
            catch (Exception ex)
            {
                Logger.AddLog($"BACKUP - Intento fallido de restauración ({DateTime.Now}): {ex.Message}", 3, pUserEmail);
                return ex.Message;
            }
            finally
            {
                using (SqlConnection con = new SqlConnection(conexion))
                {
                    con.Open();
                    SqlCommand command = new SqlCommand("ALTER DATABASE [eCommerce] SET ONLINE", con);
                    command.ExecuteNonQuery();
                    Logger.AddLog($"BACKUP - Base de datos puesta en línea ({DateTime.Now})", 3, pUserEmail);
                }
            }
        }
    }
}
