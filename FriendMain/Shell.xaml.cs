using Friend.Infra;
using System.Windows;

namespace FriendMain
{
    public partial class Shell : Window,IView
    {
        public Shell(IShellViewModel vm)
        {
            InitializeComponent();
            ViewModel = vm;
        }

        public IViewModel ViewModel
        {
            get
            {
                return (IViewModel)DataContext;
            }

            set
            {
                DataContext = value;
            }
        }
    }
}
