using Friend.Infra;
using Microsoft.Practices.Prism.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System;
using System.Windows;

namespace DataLists.Content
{
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        private List<DataListResult> OriginalDataListSet;
        private string title;

        public string Title
        {
            get { return "Data Lists"; }
            set { title = value; }
        }

        private string searchText;

        public string SearchText
        {
            get { return searchText; }
            set
            {
                searchText = value;
                OnPropertyChanged("SearchText");
                searchCommand.RaiseCanExecuteChanged();
                if (CanExecuteSearch())
                {
                    ExecuteSearch();
                }
                else
                {
                    DataListResultSet = new ObservableCollection<DataListResult>(OriginalDataListSet);
                }
            }
        }
        private ObservableCollection<DataListResult> dataListResultSet;

        public ObservableCollection<DataListResult> DataListResultSet
        {
            get { return dataListResultSet; }
            set
            {
                dataListResultSet = value;
                OnPropertyChanged("DataListResultSet");
            }
        }


        private DelegateCommand searchCommand;

        public DelegateCommand SearchCommand
        {
            get { return searchCommand; }
            set { searchCommand = value; }
        }

        private DelegateCommand<DataListResult> copyXMLCommand;

        public DelegateCommand<DataListResult> CopyXMLCommand
        {
            get { return copyXMLCommand; }
            set { copyXMLCommand = value; }
        }

        private DelegateCommand<DataListResult> copyTextCommand;

        public DelegateCommand<DataListResult> CopyTextCommand
        {
            get { return copyTextCommand; }
            set { copyTextCommand = value; }
        }



        public MainViewModel()
        {
            SearchCommand = new DelegateCommand(ExecuteSearch, CanExecuteSearch);
            CopyXMLCommand = new DelegateCommand<DataListResult>(ExecuteCopyXML, CanExecuteCopyXML);
            CopyTextCommand = new DelegateCommand<DataListResult>(ExecuteCopyText, CanExecuteCopyText);

            LoadFromXml();
        }

        private bool CanExecuteCopyText(DataListResult dataListResult)
        {
            return dataListResult != null;
        }

        private void ExecuteCopyText(DataListResult dataListResult)
        {
            Clipboard.SetText(dataListResult.CommandText);
        }

        private bool CanExecuteCopyXML(DataListResult dataListResult)
        {
            return dataListResult != null;
        }

        private void ExecuteCopyXML(DataListResult dataListResult)
        {
            Clipboard.SetText("rashid");
        }

        private bool CanExecuteSearch()
        {
            return SearchText != null && SearchText != String.Empty;
        }

        private async void ExecuteSearch()
        {
            List<DataListResult> temp = new List<DataListResult>();
            await Task.Run(() =>
            {
                foreach (DataListResult item in OriginalDataListSet)
                {
                    if (item.DataListName.ToUpper().Contains(SearchText.ToUpper()))
                    {
                        temp.Add(item);
                    }
                }
            }
            );
            if (temp.Count > 0)
            {
                DataListResultSet.Clear();
                DataListResultSet = new ObservableCollection<DataListResult>(temp);
            }
        }

        private async void LoadFromXml()
        {
            await Task.Run(() =>
            {
                List<DataListResult> temp = null;
                //string source = @"C:\Users\rvayalil001\Documents\Rashid\P66\ConfigurationData\System\DataListConfiguration.config";
                string source = @"C:\Users\rvayalil001\Documents\Rashid\P66\ConfigurationData\System\Override\DataListConfiguration.config";
                string dataLists = File.ReadAllText(source);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(dataLists);
                if (doc.HasChildNodes)
                {
                    foreach (XmlNode item in doc.ChildNodes)
                    {
                        if (item.Name.Equals("DataListConfiguration"))
                        {
                            temp = new List<DataListResult>();
                            if (item.HasChildNodes)
                            {
                                foreach (XmlNode dataListNode in item.ChildNodes)
                                {
                                    if (dataListNode.Name.Equals("SqlDataList"))
                                    {
                                        DataListResult dataList = new DataListResult();
                                        if (dataListNode.HasChildNodes)
                                        {
                                            foreach (XmlNode child in dataListNode.ChildNodes)
                                            {
                                                if (child.InnerText.Trim() != String.Empty)
                                                {
                                                    if (child.Name.Equals("DataListName"))
                                                    {
                                                        dataList.DataListName = child.InnerText.Trim();
                                                    }
                                                    else if (child.Name.Equals("DisplayName"))
                                                    {
                                                        dataList.DisplayName = child.InnerText.Trim();
                                                    }
                                                    else if (child.Name.Equals("CommandText"))
                                                    {
                                                        dataList.CommandText = child.InnerText.Trim();
                                                    }
                                                    else if (child.Name.Equals("CacheBehavior"))
                                                    {
                                                        dataList.CacheBehavior = child.InnerText.Trim();
                                                    }
                                                    else if (child.Name.Equals("KeyColumnName"))
                                                    {
                                                        dataList.KeyColumnName = child.InnerText.Trim();
                                                    }
                                                    else if (child.Name.Equals("DefaultDisplayColumnName"))
                                                    {
                                                        dataList.DefaultDisplayColumnName = child.InnerText.Trim();
                                                    }
                                                }
                                            }
                                        }
                                        temp.Add(dataList);
                                    }
                                }
                            }
                        }
                    }
                }
                if (temp != null && temp.Count > 0)
                {
                    DataListResultSet = new ObservableCollection<DataListResult>(temp);
                    OriginalDataListSet = temp;
                }
            }
            );

        }
    }
}
