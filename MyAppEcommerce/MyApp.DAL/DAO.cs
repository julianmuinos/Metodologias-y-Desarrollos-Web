using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.DAL
{
    public static class DAO
    {
        static SqlConnection con;
        static SqlCommand com;
        static SqlTransaction tra;
        static void Initialize()
        {
            con = new SqlConnection("Data Source=.;Initial Catalog=eCommerce;Integrated Security=True;");
        }
        public static DataTable Leer(string pStoredProcedure, ArrayList pParams = null)
        {
            if (con == null) Initialize();
            DataTable dt = new DataTable();
            SqlDataAdapter da;
            com = new SqlCommand(pStoredProcedure, con);
            com.CommandType = CommandType.StoredProcedure;
            try
            {
                con.Open();
                da = new SqlDataAdapter(com);
                if (pParams != null)
                {
                    foreach (SqlParameter p in pParams)
                    {
                        com.Parameters.AddWithValue(p.ParameterName, p.Value);
                    }
                }
                da.Fill(dt);
                return dt;
            }
            catch (SqlException) { throw; }
            catch (Exception) { throw; }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                com = null;
            }
        }
        public static void Escribir(string pStoredProcedure, ArrayList pParams = null)
        {
            try
            {
                if (con == null) Initialize();
                con.Open();
                tra = con.BeginTransaction();
                com = new SqlCommand(pStoredProcedure, con, tra);
                com.CommandType = CommandType.StoredProcedure;
                if (pParams != null)
                {
                    foreach (SqlParameter p in pParams)
                    {
                        com.Parameters.AddWithValue(p.ParameterName, p.Value);
                    }
                }
                com.ExecuteNonQuery();
                tra.Commit();
            }
            catch (SqlException) { tra.Rollback(); throw; }
            catch (Exception) { tra.Rollback(); throw; }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                tra = null; com = null;
            }
        }
    }
}
