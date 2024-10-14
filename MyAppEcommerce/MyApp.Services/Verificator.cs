using MyApp.DAL;
using MyApp.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Services
{
    public static class Verificator
    {
        static string CreateVerificator(IEntity entity)
        {
            Type t = entity.GetType();
            string vd = string.Empty;
            var props = t.GetProperties();
            foreach (var attr in props)
            {
                if (attr.GetValue(entity) != null)
                {
                    if (attr.PropertyType.FullName.Equals(typeof(DateTime).FullName))
                    {
                        DateTime dt = (DateTime)attr.GetValue(entity);
                        vd += dt.ToString("yyyymmddhhmmss");
                    }
                    else
                    {
                        vd += attr.GetValue(entity).ToString();
                    }
                }
            }
            return Crypto.Parse(vd);
        }
        private static string CreateTotalVerificator(List<IEntity> pList)
        {
            string vdt = string.Empty;
            foreach (IEntity e in pList)
            {
                vdt += CreateVerificator(e);
            }
            return Crypto.Parse(vdt);
        }
        public static bool CompareVerificator(List<IEntity> pList, string tableName)
        {
            if (pList.Count == 0) return false;
            string dvt = CreateTotalVerificator(pList);
            string storedProcedure = $"sp_{tableName}Vd_GetVerificator";
            string stored_dvt = DAO.Leer(storedProcedure).Rows[0].ItemArray[1].ToString();
            return dvt == stored_dvt ? true : false;
        }
        public static void SetVerificator(List<IEntity> pList, string tableName)
        {
            ArrayList al = new ArrayList();
            DAO.Escribir($"sp_{tableName}Vd_DeleteVerificator");
            string storedProcedure = $"sp_{tableName}Vd_AddVerificator";
            al.Add(new SqlParameter("@id", SqlDbType.Int) { Value = 0 });
            al.Add(new SqlParameter("@vd", SqlDbType.NVarChar) { Value = pList.Count > 0 ? CreateTotalVerificator(pList) : "0" });
            DAO.Escribir(storedProcedure, al);
            al.Clear();
            foreach (IEntity e in pList)
            {
                al.Add(new SqlParameter("@id", SqlDbType.Int) { Value = e.Id });
                al.Add(new SqlParameter("@vd", SqlDbType.NVarChar) { Value = CreateVerificator(e) });
                DAO.Escribir(storedProcedure, al);
                al.Clear();
            }
        }

        public static List<int> CompareEachRowVerificator(List<IEntity> pList, string tableName)
        {
            List<int> rowsWithError = new List<int>();
            if (pList.Count == 0) return rowsWithError;
            Dictionary<int, string> Keys = new Dictionary<int, string>();
            string storedProcedure = $"sp_{tableName}Vd_GetVerificator";
            foreach (DataRow dr in DAO.Leer(storedProcedure).Rows)
            {
                if (dr.ItemArray[0].ToString() != "0")
                {
                    object[] aux = dr.ItemArray;
                    Keys.Add(int.Parse(aux[0].ToString()), aux[1].ToString());
                }
            }
            foreach (IEntity e in pList)
            {
                Keys.TryGetValue(e.Id, out string verificatorInDB);
                string verificator = CreateVerificator(e);
                if (verificatorInDB != verificator) rowsWithError.Add(e.Id);
            }
            return rowsWithError;
        }

        public static List<int> CompareIfDeleteRowVerificator(List<IEntity> pList, string tableName)
        {
            List<int> rowsDeleted = new List<int>();
            if(pList.Count == 0) return rowsDeleted;
            Dictionary<int, string> Keys = new Dictionary<int, string>();
            string storedProcedure = $"sp_{tableName}Vd_GetVerificator";
            foreach(DataRow dr in DAO.Leer(storedProcedure).Rows)
            {
                if (dr.ItemArray[0].ToString() != "0")
                {
                    object[] aux = dr.ItemArray;
                    Keys.Add(int.Parse(aux[0].ToString()), aux[1].ToString());
                }
            }
            foreach(KeyValuePair<int, string> kv in Keys)
            {
                bool exists = pList.Exists(x => x.Id == kv.Key);
                if(!exists) rowsDeleted.Add(kv.Key);
            }
            return rowsDeleted;
        }

        public static bool HasError(List<IEntity> pList, string tableName)
        {
            if (CompareEachRowVerificator(pList, tableName).Count > 0) return true;
            if (CompareIfDeleteRowVerificator(pList, tableName).Count > 0) return true;
            else { return false; }
        }
    }
}
