using Friend.Infra;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Input;

namespace QueryWindow.Views
{
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        private List<string> dataBaseNames;

        public List<string> DataBaseNames
        {
            get { return dataBaseNames; }
            set { dataBaseNames = value;
                OnPropertyChanged("DataBaseNames");
            }
        }


        private string serverName;

        public string ServerName
        {
            get { return serverName?.Trim(); }
            set { serverName = value;
                OnPropertyChanged("ServerName");
                GetDbNamesCommand.RaiseCanExecuteChanged();

            }
        }


        private DelegateCommand getDbNamesCommand;

        public DelegateCommand GetDbNamesCommand
        {
            get { return getDbNamesCommand; }
            set { getDbNamesCommand = value; }
        }

        public MainViewModel()
        {
            GetDataBaseNames();
            GetDbNamesCommand = new DelegateCommand(GetDataBaseNames, CanGetDataBaseNames);
        }

        private bool CanGetDataBaseNames()
        {
            return ServerName != null && ServerName!= String.Empty;
        }

        private void GetDataBaseNames()
        {
            List<string> dataBaseNames = new List<string>();
            if (ServerName != null && ServerName != String.Empty)
            {
                String connectionString = "Data Source=" + ServerName + ";Integrated Security=True";
                DataSet dbDataSet = new DataSet();
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "SELECT name FROM sys.databases WHERE name NOT IN ('master', 'tempdb', 'model', 'msdb')";
                            cmd.CommandType = CommandType.Text;
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                da.Fill(dbDataSet);
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
                foreach (DataRow item in dbDataSet.Tables[0].Rows)
                {
                    dataBaseNames.Add(item[0].ToString());
                }
                DataBaseNames = dataBaseNames;
            }
        }
    }
}
