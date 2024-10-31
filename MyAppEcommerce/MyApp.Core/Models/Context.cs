using MyApp.DAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using static MyApp.Core.Models.Context;
using Antlr.Runtime;

namespace MyApp.Core.Models
{
    public static class Context
    {
        static ArrayList _al = new ArrayList();
        //Clase de usuarios:: Context.Users
        public static class Users
        {
            public static List<User> ListAll()
            {
                try
                {
                    List<User> users = new List<User>();
                    DataTable dt = DAO.Leer("sp_User_ListAll");
                    //ItemArray devuelve los datos de la row en object[]
                    foreach (DataRow dr in dt.Rows) { users.Add(new User(dr.ItemArray)); }
                    return users;
                }
                catch (Exception) { throw; }
            }
            public static string Add(User pUser)
            {
                try
                {
                    //Parámetros para el stored procedure
                    _al.Add(new SqlParameter("@username", SqlDbType.NVarChar) { Value = pUser.Username });
                    _al.Add(new SqlParameter("@email", SqlDbType.NVarChar) { Value = pUser.Email });
                    _al.Add(new SqlParameter("@password", SqlDbType.NVarChar) { Value = Services.Crypto.Parse(pUser.Password) });
                    _al.Add(new SqlParameter("@role", SqlDbType.NVarChar) { Value = "User" });
                    DAO.Escribir("sp_User_Add", _al);
                    return "User added succesfully";
                }
                catch (Exception) { throw; }
                finally { _al.Clear(); }
            }
            public static string Delete(int pId)
            {
                try
                {
                    //Parámetros para el stored procedure
                    _al.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = pId });
                    DAO.Escribir("sp_User_Delete", _al);
                    return "User deleted succesfully";
                }
                catch (Exception) { throw; }
                finally { _al.Clear(); }
            }
            public static string Modify(User pUser)
            {
                try
                {
                    _al.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = pUser.Id });
                    _al.Add(new SqlParameter("@username", SqlDbType.NVarChar) { Value = pUser.Username });
                    _al.Add(new SqlParameter("@email", SqlDbType.NVarChar) { Value = pUser.Email });
                    _al.Add(new SqlParameter("@role", SqlDbType.NVarChar) { Value = pUser.Role });
                    DAO.Escribir("sp_User_Modify", _al);
                    return "User modified succesfully";
                }
                catch (Exception) { throw; }
                finally { _al.Clear(); }
            }
            public static string SubtractAttemp(User pUser)
            {
                try
                {
                    _al.Add(new SqlParameter("@id_usuario", SqlDbType.Int) { Value = pUser.Id });
                    DAO.Escribir("sp_User_Subtract_Attemp", _al);
                    return "Attemps subtracted succesfully";
                }
                catch (Exception) { throw; }
                finally { _al.Clear(); }
            }
            public static string Unblock(User pUser)
            {
                try
                {
                    _al.Add(new SqlParameter("@id_usuario", SqlDbType.Int) { Value = pUser.Id });
                    DAO.Escribir("sp_User_Unblock", _al);
                    return "User unblocked succesfully";
                }
                catch (Exception) { throw; }
                finally { _al.Clear(); }
            }
        }
        public static class Products
        {
            public static List<Product> ListAll()
            {
                try
                {
                    List<Product> _product = new List<Product>();
                    DataTable dt = DAO.Leer("sp_Product_ListAll");
                    foreach (DataRow dr in dt.Rows) { _product.Add(new Product(dr.ItemArray)); }
                    return _product;
                }
                catch (Exception) { throw; }
            }
            public static string Add(Product pProduct)
            {
                try
                {
                    _al.Add(new SqlParameter("@name", SqlDbType.NVarChar) { Value = pProduct.Name });
                    _al.Add(new SqlParameter("@description", SqlDbType.NVarChar) { Value = pProduct.Description });
                    _al.Add(new SqlParameter("@category", SqlDbType.NVarChar) { Value = pProduct.Category });
                    _al.Add(new SqlParameter("@price", SqlDbType.Decimal) { Value = pProduct.Price });
                    _al.Add(new SqlParameter("@imageurl", SqlDbType.NVarChar) { Value = pProduct.ImageURL });
                    DAO.Escribir("sp_Product_Add", _al);
                    return "Product added succesfully";
                }
                catch (Exception) { throw; }
                finally { _al.Clear(); }
            }
            public static string Modify(Product pProduct)
            {
                try
                {
                    _al.Add(new SqlParameter("@id", SqlDbType.Int) { Value = pProduct.Id });
                    _al.Add(new SqlParameter("@name", SqlDbType.NVarChar) { Value = pProduct.Name });
                    _al.Add(new SqlParameter("@description", SqlDbType.NVarChar) { Value = pProduct.Description });
                    _al.Add(new SqlParameter("@category", SqlDbType.NVarChar) { Value = pProduct.Category });
                    _al.Add(new SqlParameter("@price", SqlDbType.Decimal) { Value = pProduct.Price });
                    _al.Add(new SqlParameter("@imageurl", SqlDbType.NVarChar) { Value = pProduct.ImageURL });
                    DAO.Escribir("sp_Product_Modify", _al);
                    return "Product modified succesfully";
                }
                catch (Exception) { throw; }
                finally { _al.Clear(); }
            }
            public static string Delete(int pId)
            {
                try
                {
                    //almaceno el ID que quiero mandar a la base de datos para que despues se elimine.
                    _al.Add(new SqlParameter("@id", SqlDbType.Int) { Value = pId });
                    DAO.Escribir("sp_Product_Delete", _al);
                    return "Product deleted sucesfully";
                }
                catch (Exception) { throw; }
                finally { _al.Clear(); }
            }
        }
        public static class Carts
        {
            public static List<Cart> ListAll()
            {
                try
                {
                    List<Cart> _carts = new List<Cart>();
                    DataTable dt = DAO.Leer("sp_Cart_ListAll");
                    foreach (DataRow dr in dt.Rows)
                    {
                        Cart _cart = new Cart(int.Parse(dr["Id"].ToString()), Users.ListAll().FirstOrDefault(user => user.Id == int.Parse(dr["User_Id"].ToString())), DateTime.Parse(dr["Date"].ToString()));
                        /*foreach (Item i in Items.ListByCart(int.Parse(dr["Id"].ToString())))
                        {
                            _cart.AddItem(i.Product, i.Quantity);
                        }*/
                        _carts.Add(_cart);
                    }
                    return _carts;
                }
                catch (Exception) { throw; }
                finally { _al.Clear(); }
            }
            /*public static List<Cart> ListAllByUser()
            {
                List<Cart> _carts = new List<Cart>();
                DataTable dt = DAO.Leer("sp_Cart_ListByUser");
                foreach (DataRow dr in dt.Rows)
                {
                    Cart _cart = new Cart(int.Parse(dr["Id"].ToString()), Users.ListAll().FirstOrDefault(user => user.Id == int.Parse(dr["User_Id"].ToString())));
                    foreach (Item i in Items.ListByCart(int.Parse(dr["Id"].ToString())))
                    {
                        _cart.AddItem(i.Product, i.Quantity);
                    }
                    _carts.Add(_cart);
                }
                return _carts;
            }*/
            public static string Create(Cart pCart)
            {
                try
                {
                    _al.Add(new SqlParameter("@userId", SqlDbType.Int) { Value = Services.Session.GetInstance.id });
                    DAO.Escribir("sp_Cart_Add", _al);

                    _al.Clear();

                    foreach (Item i in pCart.GetItems())
                    {
                        _al.Add(new SqlParameter("@cartId", SqlDbType.Int) { Value = Carts.ListAll().Last().Id });
                        _al.Add(new SqlParameter("@quantity", SqlDbType.Int) { Value = (int)i.Quantity });
                        _al.Add(new SqlParameter("@productId", SqlDbType.Int) { Value = i.Product.Id });
                        DAO.Escribir("sp_Item_Add", _al);
                    }

                    return "Cart added succesfully";
                }
                catch (Exception) { throw; }
                finally { _al.Clear(); }
            }
            private static class Items
            {
                /*public static List<Item> ListByCart(int pId)
                {
                    List<Item> _items = new List<Item>();
                    DataTable dt = DAO.Leer("sp_Item_ListByCart");
                    foreach (DataRow dr in dt.Rows)
                    {
                        _items.Add(new Item(Products.ListAll().FirstOrDefault(p => p.Id.ToString() == dr["Product_Id"].ToString()), uint.Parse(dr["Quantity"].ToString())));
                    }
                    return _items;
                }*/
            }
        }
    }
}