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
            set { query = value;
                OnPropertyChanged("Query");
            }
        }

        private DataView result;

        public DataView Result
        {
            get { return result; }
            set { result = value;
                OnPropertyChanged("Result");
            }
        }


        public string ConnectionDetails
        {
            get { return "Connected server is \""+Server+"\" and Database is \""+Database+"\""; }
            
        }


    }
}
