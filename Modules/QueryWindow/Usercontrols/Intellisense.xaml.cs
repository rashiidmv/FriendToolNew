using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QueryWindow.Usercontrols
{
    /// <summary>
    /// Interaction logic for Intellisense.xaml
    /// </summary>
    public partial class Intellisense : UserControl
    {
        public Intellisense()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty MyItemsSourceProperty;
        static Intellisense()
        {
            MyItemsSourceProperty = DependencyProperty.Register("MyItemsSource", typeof(IEnumerable), typeof(Intellisense));

        }

        public IEnumerable MyItemsSource
        {
            get
            {
                return (IEnumerable)GetValue(MyItemsSourceProperty);
            }
            set { SetValue(MyItemsSourceProperty, value); }
        }

        private void OnMethodsSelectionKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case System.Windows.Input.Key.Enter:
                    // Hide the Popup
                    PopupIntellisense.IsOpen = false;

                    ListBox lb = sender as ListBox;
                    if (lb == null)
                        return;

                    // Get the selected item value
                    string methodName = lb.SelectedItem.ToString();

                    // Save the Caret position
                    int i = txtQueryString.CaretIndex;
                    int spaceIndex = 0;
                    string temp = txtQueryString.Text;
                    int previousSpaceIndex = 0;
                    while (temp.Contains(" "))
                    {
                        spaceIndex = spaceIndex + temp.IndexOf(" ");
                        spaceIndex++;
                        if (spaceIndex >= i)
                        {
                            break;
                        }
                        previousSpaceIndex = spaceIndex;
                        temp = temp.Substring(temp.IndexOf(" ") + 1);
                    }


                    if (i - spaceIndex < 0)
                        spaceIndex = previousSpaceIndex;



                      string oldString = txtQueryString.Text.Substring(spaceIndex, i - spaceIndex);
                    // Add text to the text
                    //if (spaceIndex > 0)
                    //{
                    //    methodName = " " + methodName;
                    //}

                    if(oldString.Trim() !=String.Empty)
                        txtQueryString.Text = txtQueryString.Text.Replace(oldString, methodName);
                    else
                        txtQueryString.Text = txtQueryString.Text.Insert(spaceIndex+1,methodName);

                    // Move the caret to the end of the added text
                    txtQueryString.CaretIndex = spaceIndex + methodName.Length;

                    // Move focus back to the text box.
                    // This will auto-hide the PopUp due to StaysOpen="false"
                    txtQueryString.Focus();
                  //  PopupIntellisense.IsOpen = false;
                    break;

                case System.Windows.Input.Key.Escape:
                    // Hide the Popup
                    PopupIntellisense.IsOpen = false;
                    txtQueryString.Focus();
                    break;
            }
        }

        private void OnFilterTextKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            TextBox txtBox = sender as TextBox;
            if ((txtBox == null) || (txtBox.CaretIndex == 0) || txtBox.Text==String.Empty)
                return;

            // Check for a predefined hot-key
            //if (e.Key != System.Windows.Input.Key.OemPeriod)
            //    return;

            // Get the last word in the text (preceding the ".")
            string txt = txtBox.Text;
            int wordStart = txt.LastIndexOf(' ', txtBox.CaretIndex - 1);
            if (wordStart == -1)
                wordStart = 0;

            string lastWord = txt.Substring(wordStart, txtBox.CaretIndex - wordStart);

            //Check if the last word equal to the one we're waiting
            //if (lastWord.Trim().ToLower() != "item.")
            //    return;

            if (e.Key == Key.Escape)
            {
                PopupIntellisense.IsOpen = false;
                return;
            }
            if (lstIntellisense.Items.Count > 0)
                ShowMethodsPopup(txtBox.GetRectFromCharacterIndex(txtBox.CaretIndex, true), e.Key);
        }

        private void ShowMethodsPopup(Rect placementRect, Key key)
        {
            PopupIntellisense.PlacementTarget = txtQueryString;
            PopupIntellisense.PlacementRectangle = placementRect;
            PopupIntellisense.IsOpen = true;
            lstIntellisense.SelectedIndex = 0;
            if (key == Key.Down)
                lstIntellisense.Focus();
        }
    }
}
