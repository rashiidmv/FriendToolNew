using Friend.Infra;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryWindow
{
    public class QueryResult : ViewModelBase
    {
        private bool isSQLAuthenticated;

        public bool IsSQLAuthenticated
        {
            get { return isSQLAuthenticated; }
            set
            {
                isSQLAuthenticated = value;
                OnPropertyChanged("IsSQLAuthenticated");

            }
        }

        private string password;

        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                OnPropertyChanged("Password");
            }
        }

        private string username;

        public string Username
        {
            get { return username; }
            set
            {
                username = value;
                OnPropertyChanged("Username");
            }
        }



        private string server;

        public string Server
        {
            get { return server; }
            set { server = value; }
        }

        private string database;

        public string Database
        {
            get { return database; }
            set { database = value; }
        }

        private string query;

        public string Query
        {
            get { return query; }
            set
            {
                query = value;
                OnPropertyChanged("Query");
            }
        }

        private DataView result;

        public DataView Result
        {
            get { return result; }
            set
            {
                result = value;
                OnPropertyChanged("Result");
            }
        }


        public string ConnectionDetails
        {
            get
            {
                if (IsSQLAuthenticated)
                {
                    return "Connected server is \"" + Server + "\" and Database is \"" + Database + "\" with SQL Authentication";

                }
                else
                {
                    return "Connected server is \"" + Server + "\" and Database is \"" + Database + "\"";
                }
            }

        }


    }
}
