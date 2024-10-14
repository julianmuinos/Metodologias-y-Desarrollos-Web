using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Services
{
    public class Session
    {
        private static Session session;
        public static object _lock = new Object();
        public int id; //Id del usuario conectado
        public DateTime Date { get; set; }
        private Session() { }
        public static Session GetInstance
        {
            get
            {
                if (session == null)
                {
                    return null;
                }
                return session;
            }
        }

        public static void Login(int pId)
        {
            lock (_lock)
            {
                if (session == null)
                {
                    session = new Session();
                    session.id = pId;
                    session.Date = DateTime.Now;
                }
                else throw new Exception("Session already started.");
            }
        }

        public static void Logout()
        {
            lock (_lock)
            {
                if (session != null)
                {
                    session = null;
                }
                else throw new Exception("Session not started.");
            }
        }
    }
}
