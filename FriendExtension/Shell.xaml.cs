//------------------------------------------------------------------------------
// <copyright file="Shell.xaml.cs" company="PricewaterhouseCoopers LLP">
//     Copyright (c) PricewaterhouseCoopers LLP.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace FriendExtension
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using System.Xml;

    /// <summary>
    /// Interaction logic for Shell.
    /// </summary>
    public partial class Shell : UserControl
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Shell"/> class.
        /// </summary>
        public Shell()
        {
            this.InitializeComponent();

            ReportConfigFile = new XmlDocument();


            grdReportDefinition.Visibility = Visibility.Visible;
            this.DataContext = this;
            toolDataConfig = new ConfigXmlDocument();

            toolDataConfigPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Resources\\ToolData.config");
            toolDataConfig.Load(toolDataConfigPath);


            string photoPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Resources\\Photo.jpg");
            Uri photoUri = new Uri(photoPath, UriKind.Absolute);
            imgPhoto.Source = new BitmapImage(photoUri);


            displayStyles = new ObservableCollection<string>();
            XmlNodeList DisplayStyle = toolDataConfig.GetElementsByTagName("Item");
            foreach (XmlNode item in DisplayStyle)
            {
                displayStyles.Add(item.InnerText.ToString());
            }
            criteriaObjects = new ObservableCollection<CriteriaObject>();
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Invoked '{0}'", this.ToString()),
                "RightAngle Friend");
        }

        XmlNode reportConfiguration;
        private string _text;

        public string MyText
        {
            get { return _text; }
            set { _text = value; }
        }

        ConfigXmlDocument toolDataConfig;
        string toolDataConfigPath;
        private ObservableCollection<CriteriaObject> criteriaObjects = null;
        public ObservableCollection<CriteriaObject> CriteriaObjects
        {
            get { return criteriaObjects; }
        }
        private ObservableCollection<string> displayStyles = null;
        public ObservableCollection<string> DisplayStyles { get { return displayStyles; } }


        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            CriteriaObject c = (CriteriaObject)dataGridCriteriaObjects.SelectedItem;
            CriteriaObjects.Remove(c);
        }

        private void btnCreateReportConfig_Click(object sender, RoutedEventArgs e)
        {
            int resolutionId = 0;
            foreach (var item in CriteriaObjects)
            {
                item.Id = resolutionId++;
            }
            createReportDefinitionTags();
            if (criteriaObjects.Count > 0)
            {
                if (!ValidateCriteriaControls())
                    return;
                bool preesent = false;
                XmlNode criteriaObject = null;
                foreach (XmlNode item in reportConfiguration.ChildNodes)
                {
                    if (item.Name.Equals("CriteriaObject"))
                    {
                        criteriaObject = item;
                        criteriaObject.RemoveAll();
                        preesent = true; break;
                    }
                }
                if (!preesent)
                {
                    criteriaObject = ReportConfigFile.CreateElement("CriteriaObject", reportConfiguration.NamespaceURI);
                    reportConfiguration.AppendChild(criteriaObject);
                }



                List<XmlNode> controls = new List<XmlNode>();
                foreach (XmlNode item in reportConfiguration.ChildNodes)
                {
                    if (item.Name.Equals("Control"))
                    {
                        controls.Add(item);
                    }
                }
                foreach (XmlNode item in controls)
                {
                    reportConfiguration.RemoveChild(item);
                }
                UpdateOrCreateNode(criteriaObject, "CriteriaName", txtCriteriaName.Text, reportConfiguration.NamespaceURI);


                StringBuilder criteriaUIDefinitionString = new StringBuilder();
                criteriaUIDefinitionString.Append("\n");
                bool last = true;
                foreach (var item in criteriaObjects)
                {
                    last = true;
                    criteriaUIDefinitionString.Append("\t");
                    if (item.BoldSentence && !string.IsNullOrEmpty(item.DisplayName))
                    {
                        criteriaUIDefinitionString.Append("[b] " + item.DisplayName + ":" + " [/b]");
                    }
                    else if (!string.IsNullOrEmpty(item.DisplayName))
                    {
                        criteriaUIDefinitionString.Append(item.DisplayName + ":");
                    }
                    criteriaUIDefinitionString.Append(" [" + item.Id + "]");
                    if (item.NextLine)
                    {
                        criteriaUIDefinitionString.Append(" [br]\n");
                        last = false;
                    }

                }
                if (last)
                    criteriaUIDefinitionString.Append("\n");

                UpdateOrCreateNode(criteriaObject, "CriteriaUIDefinition", criteriaUIDefinitionString.ToString(), reportConfiguration.NamespaceURI);


                foreach (var item in criteriaObjects)
                {
                    XmlNode control = ReportConfigFile.CreateElement("Control", reportConfiguration.NamespaceURI);


                    XmlAttribute displayStyleAttribute = ReportConfigFile.CreateAttribute("DisplayStyle");
                    displayStyleAttribute.Value = item.SelectedDisplayStyle;
                    control.Attributes.Append(displayStyleAttribute);

                    XmlAttribute idAttribute = ReportConfigFile.CreateAttribute("ID");
                    idAttribute.Value = item.Id.ToString();
                    control.Attributes.Append(idAttribute);

                    XmlAttribute parameterNameAttribute = ReportConfigFile.CreateAttribute("ParameterName");
                    parameterNameAttribute.Value = "@" + item.ParameterName;
                    control.Attributes.Append(parameterNameAttribute);

                    if (item.Width > 0)
                    {
                        XmlAttribute widthAttribute = ReportConfigFile.CreateAttribute("Width");
                        widthAttribute.Value = item.Width.ToString();
                        control.Attributes.Append(widthAttribute);
                    }

                    if (!item.Nulllable)
                    {
                        XmlAttribute isNullableAttribute = ReportConfigFile.CreateAttribute("Nullable");
                        isNullableAttribute.Value = item.Nulllable.ToString();
                        control.Attributes.Append(isNullableAttribute);
                    }
                    reportConfiguration.AppendChild(control);
                }
                ReportConfigFile.Save(ReportConfigFilePath);
            }
        }

        private void createReportDefinitionTags()
        {
            XmlElement rootElement = ReportConfigFile.DocumentElement;
            if (rootElement == null || rootElement.Name != "ReportConfiguration")
            {
                ReportConfigFile.RemoveAll();
                reportConfiguration = ReportConfigFile.CreateElement("ReportConfiguration");
                XmlAttribute attribute = ReportConfigFile.CreateAttribute("xmlns");
                attribute.Value = "http://tempuri.org/ReportConfiguration.xsd";
                reportConfiguration.Attributes.Append(attribute);
                ReportConfigFile.AppendChild(reportConfiguration);
            }
            else
            {
                reportConfiguration = rootElement;
            }


            bool preesent = false;
            XmlNode reportDefinition = null;
            foreach (XmlNode item in reportConfiguration.ChildNodes)
            {
                if (item.Name.Equals("ReportDefinition"))
                {
                    reportDefinition = item;
                    preesent = true; break;
                }
            }
            if (!preesent)
            {
                reportDefinition = ReportConfigFile.CreateElement("ReportDefinition");
                reportConfiguration.AppendChild(reportDefinition);
            }

            UpdateOrCreateNode(reportDefinition, "ReportName", txtReportName.Text, reportConfiguration.NamespaceURI);
            UpdateOrCreateNode(reportDefinition, "Class", txtClass.Text + ", " + txtAssemblyName.Text, reportConfiguration.NamespaceURI);
            UpdateOrCreateNode(reportDefinition, "BusinessDomain", txtBusinessDomain.Text, reportConfiguration.NamespaceURI);
            UpdateOrCreateNode(reportDefinition, "ReportEntity", txtReportEntity.Text, reportConfiguration.NamespaceURI);

            if (!string.IsNullOrEmpty(txtDataBaseConnectionName.Text))
            {
                UpdateOrCreateNode(reportDefinition, "DataBaseConnectionName", txtDataBaseConnectionName.Text, reportConfiguration.NamespaceURI);
            }

            if (!string.IsNullOrEmpty(txtExternalCriteriaName.Text))
            {
                UpdateOrCreateNode(reportDefinition, "ExternalCriteriaName", txtExternalCriteriaName.Text, reportConfiguration.NamespaceURI);
            }
            if (chkAllowOneToManyNavigation.IsChecked != null)
            {
                UpdateOrCreateNode(reportDefinition, "AllowOneToManyNavigation", chkAllowOneToManyNavigation.IsChecked.ToString().ToLower(), reportConfiguration.NamespaceURI);
            }

            if (chkShowReportInNavigation.IsChecked != null)
            {
                UpdateOrCreateNode(reportDefinition, "ShowReportInNavigation", chkShowReportInNavigation.IsChecked.ToString().ToLower(), reportConfiguration.NamespaceURI);
            }

            if (chkRequiresBusinessAssociateSecurity.IsChecked != null)
            {
                UpdateOrCreateNode(reportDefinition, "RequiresBusinessAssociateSecurity", chkRequiresBusinessAssociateSecurity.IsChecked.ToString().ToLower(), reportConfiguration.NamespaceURI);
            }

            if (!string.IsNullOrEmpty(txtSortOrder.Text))
            {
                UpdateOrCreateNode(reportDefinition, "SortOrder", txtSortOrder.Text, reportConfiguration.NamespaceURI);
            }

            if (!string.IsNullOrEmpty(txtIncludeInToolsList.Text))
            {
                UpdateOrCreateNode(reportDefinition, "IncludeInToolsList", txtIncludeInToolsList.Text, reportConfiguration.NamespaceURI);
            }

            if (!string.IsNullOrEmpty(txtModuleId.Text))
            {
                UpdateOrCreateNode(reportDefinition, "ModuleId", txtModuleId.Text, reportConfiguration.NamespaceURI);
            }
        }

        private void UpdateOrCreateNode(XmlNode parent, string tagName, string innerText, string namespaceUri)
        {
            bool preesent = false;
            foreach (XmlNode item in parent.ChildNodes)
            {
                if (item.Name.Equals(tagName))
                {
                    item.InnerText = string.IsNullOrEmpty(innerText) ? string.Empty : innerText;
                    preesent = true; break;
                }
            }
            if (!preesent)
            {
                XmlNode childNode = ReportConfigFile.CreateElement(tagName, namespaceUri);
                childNode.InnerText = string.IsNullOrEmpty(innerText) ? string.Empty : innerText;
                parent.AppendChild(childNode);
            }
        }

        private bool ValidateCriteriaControls()
        {
            bool status = true;
            foreach (var item in criteriaObjects)
            {
                //Id = newId++,
                //DisplayName = String.Empty,
                //BoldSentence = false,
                //NextLine = false,
                //DispStyles = displayStyles,
                //SelectedDisplayStyle = string.Empty,
                //ParameterName = String.Empty,
                //Width = 0
                if (string.IsNullOrEmpty(txtCriteriaName.Text))
                {
                    MessageBox.Show("Please specify Criteria Name");
                    txtCriteriaName.Focus();
                    return status = false;
                }


                if (string.IsNullOrEmpty(item.SelectedDisplayStyle))
                {
                    MessageBox.Show("Please specify Display Style for control Id= ' " + item.Id + " '");
                    return status = false;
                }

                if (string.IsNullOrEmpty(item.ParameterName))
                {
                    MessageBox.Show("Please specify Parameter Name for control Id= ' " + item.Id + " '");
                    return status = false;
                }

                if (item.ParameterName.Contains("@"))
                {
                    MessageBox.Show("Please remove '@' character from Parameter Name of control Id= ' " + item.Id + " '");
                    return status = false;
                }
                if (item.ParameterName.Contains(" "))
                {
                    MessageBox.Show("Please remove whitespace from Parameter Name of control Id= ' " + item.Id + " '");
                    return status = false;
                }

                Regex notSpecialCharaters = new Regex("[^a-zA-Z0-9_]");
                if (notSpecialCharaters.IsMatch(item.ParameterName))
                {
                    MessageBox.Show("Please remove special characters from Parameter Name of control Id= ' " + item.Id + " '");
                    return status = false;
                }

                if (item.Width < 0)
                {
                    MessageBox.Show("Please specify width attribute as positive value for control Id= ' " + item.Id + " '");
                    return status = false;
                }
            }
            return status;
        }

        int newId = 0;
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            int maxId = 0;
            bool notPresent = false;
            if (criteriaObjects.Count == 0)
            {
                newId = 0;
            }
            if (criteriaObjects.Count > 0)
            {
                maxId = criteriaObjects.Max(x => x.Id);
            }
            for (int i = 0; i < maxId; i++)
            {
                bool c = criteriaObjects.Any(x => x.Id == i);
                if (!c)
                {
                    newId = i;
                    notPresent = true;
                    break;
                }
            }
            if (!notPresent && criteriaObjects.Count != 0)
                newId = maxId + 1;

            criteriaObjects.Add(new CriteriaObject()
            {
                Id = newId++,
                DisplayName = String.Empty,
                BoldSentence = false,
                NextLine = false,
                DispStyles = displayStyles,
                SelectedDisplayStyle = string.Empty,
                ParameterName = String.Empty,
                Width = 0,
                Nulllable = true
            });

        }


        private void btnRead_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ReportConfigFilePath = @"C:\Users\rvayalil001\Documents\Visual Studio 2015\Projects\App1.Config";
                ReportConfigFile.Load(ReportConfigFilePath);
                ParseConfigFile();
            }
            catch (XmlException ex)
            {
                //Do nothing file is not an xml file
            }
            catch (Exception ex)
            {
                MessageBox.Show("File is not in RightAngle compatible format, Parsing Error Occured");
            }
        }

        public void ParseConfigFile()
        {
            XmlElement rootElement = ReportConfigFile.DocumentElement;
            foreach (XmlNode item in rootElement.ChildNodes)
            {
                if (item.Name.Equals("ReportDefinition"))
                {
                    XmlElement criteriaObject = item as XmlElement;
                    foreach (XmlNode node in criteriaObject.ChildNodes)
                    {
                        if (node.Name.Equals("ReportName"))
                        {
                            txtReportName.Text = node.InnerText;
                        }
                        if (node.Name.Equals("Class"))
                        {
                            string[] classDetails = node.InnerText.Split(new char[] { ',' });
                            txtClass.Text = classDetails[0].Trim();
                            txtAssemblyName.Text = classDetails[1].Trim();

                        }
                        if (node.Name.Equals("BusinessDomain"))
                        {
                            txtBusinessDomain.Text = node.InnerText;
                        }
                        if (node.Name.Equals("ReportEntity"))
                        {
                            txtReportEntity.Text = node.InnerText;
                        }
                        if (node.Name.Equals("DataBaseConnectionName"))
                        {
                            txtDataBaseConnectionName.Text = node.InnerText;
                        }
                        if (node.Name.Equals("ExternalCriteriaName"))
                        {
                            txtExternalCriteriaName.Text = node.InnerText;
                        }
                        if (node.Name.Equals("AllowOneToManyNavigation"))
                        {
                            bool result = false;
                            if (bool.TryParse(node.InnerText, out result))
                            {
                                chkAllowOneToManyNavigation.IsChecked = result;
                            }
                        }
                        if (node.Name.Equals("ShowReportInNavigation"))
                        {
                            bool result = false;
                            if (bool.TryParse(node.InnerText, out result))
                            {
                                chkShowReportInNavigation.IsChecked = result;
                            }
                        }
                        if (node.Name.Equals("RequiresBusinessAssociateSecurity"))
                        {
                            bool result = false;
                            if (bool.TryParse(node.InnerText, out result))
                            {
                                chkRequiresBusinessAssociateSecurity.IsChecked = result;
                            }
                        }
                        if (node.Name.Equals("SortOrder"))
                        {
                            txtSortOrder.Text = node.InnerText;
                        }
                        if (node.Name.Equals("IncludeInToolsList"))
                        {
                            txtIncludeInToolsList.Text = node.InnerText;
                        }
                        if (node.Name.Equals("ModuleId"))
                        {
                            txtModuleId.Text = node.InnerText;
                        }
                    }
                    break;
                }
            }


            // Parsing criteria objects UI definition
            string criteriaUIDefinition = null;
            foreach (XmlNode item in rootElement.ChildNodes)
            {
                if (item.Name.Equals("CriteriaObject"))
                {
                    XmlElement criteriaObject = item as XmlElement;
                    foreach (XmlNode node in criteriaObject.ChildNodes)
                    {
                        if (node.Name.Equals("CriteriaName"))
                        {
                            txtCriteriaName.Text = node.InnerText;
                        }
                        else if (node.Name.Equals("CriteriaUIDefinition"))
                        {
                            criteriaUIDefinition = node.InnerText;

                        }
                    }
                    break;
                }
            }

            foreach (XmlNode controlNode in rootElement.GetElementsByTagName("Control"))
            {
                CriteriaObject newObject = new CriteriaObject();
                newObject.Nulllable = true;
                foreach (XmlAttribute attributes in controlNode.Attributes)
                {
                    if (attributes.Name.Equals("ID"))
                    {
                        int id = -1;
                        if (int.TryParse(attributes.Value, out id))
                        {
                            newObject.Id = id;
                        }
                        {
                            //Parsing Error
                        }
                        continue;
                    }
                    else if (attributes.Name.Equals("DisplayStyle"))
                    {
                        newObject.DispStyles = displayStyles;
                        newObject.SelectedDisplayStyle = attributes.Value;
                        continue;
                    }
                    else if (attributes.Name.Equals("ParameterName"))
                    {
                        newObject.ParameterName = attributes.Value.Substring(1);
                        continue;
                    }
                    else if (attributes.Name.Equals("Width"))
                    {
                        int width = -1;
                        if (int.TryParse(attributes.Value, out width))
                        {
                            newObject.Width = width;
                        }
                        {
                            //Parsing Error
                        }
                        continue;
                    }
                    else if (attributes.Name.Equals("Nullable"))
                    {
                        bool isNullable = true;
                        if (bool.TryParse(attributes.Value, out isNullable))
                        {
                            newObject.Nulllable = isNullable;
                        }
                        {
                            //Parsing Error
                        }
                        continue;
                    }
                }
                if (!criteriaObjects.Any(x => x.Id == newObject.Id))
                    criteriaObjects.Add(newObject);
            }
            //Parse criteria UI definition
            string temp = criteriaUIDefinition;
            int lastId = -1;
            while (temp != null && temp.Length > 0 && temp.Contains("["))
            {
                int indexofleft = temp.IndexOf("[");
                int indexofRight = temp.IndexOf("]", indexofleft);
                string bracketcontent = temp.Substring(indexofleft + 1, indexofRight - indexofleft - 1);
                string actualbracketcontent = bracketcontent;
                bracketcontent = bracketcontent.Trim();
                int id = -1;
                if (int.TryParse(bracketcontent, out id))
                {
                    if (criteriaObjects.Any(x => x.Id == id))
                    {
                        CriteriaObject c = criteriaObjects.Single(x => x.Id == id);
                        string temp1 = criteriaUIDefinition;
                        if (lastId != -1)
                        {

                            int lastIdIndex = temp1.IndexOf(lastId.ToString());
                            int currentIdIndex = temp1.IndexOf(id.ToString());
                            temp1 = temp1.Substring(lastIdIndex, currentIdIndex - lastIdIndex);
                            if (temp1.Contains("[br]") || temp1.Contains("[ br ]") || temp1.Contains("[ br]") || temp1.Contains("[br ]"))
                            {
                                CriteriaObject previousCriteria = criteriaObjects.Single(x => x.Id == lastId);
                                previousCriteria.NextLine = true;
                            }
                            if (temp1.Contains("/b"))
                            {
                                c.BoldSentence = true;
                                temp1 = temp1.Substring(0, temp1.IndexOf("/b"));
                                temp1 = temp1.Substring(temp1.LastIndexOf("]") + 1);
                                temp1 = temp1.Substring(0, temp1.IndexOf("["));

                            }

                        }
                        else
                        {
                            if (temp1.Contains("/b"))
                            {
                                temp1 = temp1.Substring(0, temp1.IndexOf(id.ToString()));
                                temp1 = temp1.Substring(0, temp1.LastIndexOf("["));
                                temp1 = temp1.Substring(temp1.IndexOf("]") + 1, temp1.LastIndexOf("[") - temp1.IndexOf("]") - 1);
                                c.BoldSentence = true;

                            }

                        }
                        if (temp1.Contains(":"))
                            c.DisplayName = temp1.Substring(0, temp1.IndexOf(":")).Trim();
                        else
                            c.DisplayName = temp1.Trim();
                        lastId = id;
                    }
                }
                temp = temp.Substring(indexofRight + 1);
            }

        }


        /////////////// old file content
        private XmlDocument _ReportConfigFile;
        private string _ReportConfigFilePath;
        public XmlDocument ReportConfigFile
        {
            get { return _ReportConfigFile; }
            private set
            {
                _ReportConfigFile = value;
                //  _ReportConfigFile.Load(_ReportConfigFilePath);

            }
        }
        public string ReportConfigFilePath
        {
            get { return _ReportConfigFilePath; }
            set
            {
                _ReportConfigFilePath = value;
            }
        }
        static Assembly MyAssemblyResolveHandler(object source, ResolveEventArgs e)
        {
            // Assembly.LoadFrom("Assembly1.dll")
            // Assembly.LoadFrom("Assembly2.dll")

            return Assembly.Load(e.Name);
        }
        private void btnReportDefinitionNext_Click(object sender, RoutedEventArgs e)
        {
            // string sourcePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            //  sourcePath = sourcePath.Remove(sourcePath.LastIndexOf("\\")) + "\\MyProject.dll";
            //    Assembly assembly = Assembly.LoadFile(sourcePath);
            //ad.AssemblyResolve += MyAssemblyResolveHandler;
            // AppDomain.CurrentDomain.Load("MyProject.dll");


            //AppDomain ad = AppDomain.CurrentDomain;
            //AssemblyName an = new AssemblyName("MyProject");
            //an.Version = new Version("1.0.0.0");
            //Assembly assembly = ad.Load(an);
            //MyProject.Class1 t = new MyProject.Class1();
            //int result = t.Add(5, 6);
            //MessageBox.Show(result.ToString());


            if (string.IsNullOrEmpty(ReportConfigFilePath))
            {
                MessageBox.Show("Please point to a config file, either existing one or newly created one");
                return;
            }
            if (!IsValid())
            {
                MessageBox.Show("Please provide mandatory details such as Report Name,Class,Report Entity and Business Domain");
                return;
            }
            grdReportCriteria.Visibility = Visibility.Visible;
            grdReportDefinition.Visibility = Visibility.Collapsed;


            Storyboard sb = this.FindResource("ReportConfigNextAnimation") as Storyboard;
            sb.Begin();
        }
        public bool IsValid()
        {
            bool status = true;
            if (string.IsNullOrEmpty(txtReportName.Text) || string.IsNullOrEmpty(txtReportEntity.Text) ||
                    string.IsNullOrEmpty(txtClass.Text) || string.IsNullOrEmpty(txtBusinessDomain.Text) || string.IsNullOrEmpty(txtAssemblyName.Text))
                status = false;
            return status;
        }


        private void btnBackReportDefinition_Click(object sender, RoutedEventArgs e)
        {
            grdReportDefinition.Visibility = Visibility.Visible;
            grdReportCriteria.Visibility = Visibility.Collapsed;
           // Storyboard sb = this.FindResource("ReportConfigBackAnimation") as Storyboard;
          //  sb.Begin(); 
        }

        private void btnAddNewDisplayStyle_Click(object sender, RoutedEventArgs e)
        {
            string newDisplayStyle = txtNewDisplayStyle.Text.Trim();
            if (string.IsNullOrEmpty(newDisplayStyle))
            {
                MessageBox.Show("Please provide a name for new Display Style");
            }
            else if (newDisplayStyle.Contains(' '))
            {
                MessageBox.Show("Display Styles shouldn't have whitespaces");
            }
            else if (!displayStyles.Contains(newDisplayStyle))
            {
                displayStyles.Add(newDisplayStyle);
                foreach (XmlNode item in toolDataConfig.GetElementsByTagName("DisplayStyle"))
                {
                    XmlElement newDisplayStyleTag = toolDataConfig.CreateElement("Item");
                    newDisplayStyleTag.InnerText = newDisplayStyle;
                    item.AppendChild(newDisplayStyleTag);
                    toolDataConfig.Save(toolDataConfigPath);
                }
            }
            else
            {
                MessageBox.Show("Already contains in the Display Styles");
            }
        }

        private void btnDeleteDisplayStyle_Click(object sender, RoutedEventArgs e)
        {
            string selectedItemsToDelete = lstDisplayStyles.SelectedItem as string;
            if (selectedItemsToDelete.Equals("TextBox") || selectedItemsToDelete.Equals("CheckBox") || selectedItemsToDelete.Equals("DateTime") || selectedItemsToDelete.Equals("ComboBox"))
            {
                MessageBox.Show("Default Display styles can't be deleted..!");
            }
            else if (selectedItemsToDelete != null)
            {
                foreach (XmlNode parent in toolDataConfig.GetElementsByTagName("DisplayStyle"))
                {
                    foreach (XmlNode child in parent.ChildNodes)
                    {
                        if (child.InnerText.Equals(selectedItemsToDelete))
                        {
                            parent.RemoveChild(child);
                        }
                    }
                }
                displayStyles.Remove(selectedItemsToDelete);
                toolDataConfig.Save(toolDataConfigPath);
            }
            else
            {
                MessageBox.Show("Please select any display style to delete.");
            }
        }
    }


    public class CriteriaObject : INotifyPropertyChanged
    {
        private int _id;
        private string _parametername;
        private string _selectedDisplayStyle;
        private ObservableCollection<string> _dispStyles;
        private string _displayName;
        private bool _boldSentence;
        private bool _nextLine;
        private int _width;
        private bool _nullable;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("Id");
            }
        }

        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                _displayName = value;
                OnPropertyChanged("DisplayName");
            }
        }


        public bool BoldSentence
        {
            get { return _boldSentence; }
            set
            {
                _boldSentence = value;
                OnPropertyChanged("BoldSentence");
            }
        }
        public bool NextLine
        {
            get { return _nextLine; }
            set
            {
                _nextLine = value;
                OnPropertyChanged("NextLine");
            }
        }
        // public List<string>  DispStyles{ get; set; }


        public ObservableCollection<string> DispStyles
        {
            get { return _dispStyles; }
            set
            {
                _dispStyles = value;
                OnPropertyChanged("DispStyles");
            }
        }

        //public string SelectedDisplayStyle { get; set; }
        public string SelectedDisplayStyle
        {
            get { return _selectedDisplayStyle; }
            set
            {
                _selectedDisplayStyle = value;
                OnPropertyChanged("SelectedDisplayStyle");
            }
        }
        //public string ParameterName { get; set; }
        public string ParameterName
        {
            get { return _parametername; }
            set
            {
                _parametername = value;
                OnPropertyChanged("ParameterName");
            }
        }
        public int Width
        {
            get { return _width; }
            set
            {
                _width = value;
                OnPropertyChanged("Width");
            }
        }

        public bool Nulllable
        {
            get { return _nullable; }
            set
            {
                _nullable = value;
                OnPropertyChanged("Nulllable");
            }
        }
    }

}