using Friend.Infra;

namespace Help.Views
{
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        private string title;

        public string Title
        {
            get { return "Help"; }
            set { title = value; }
        }
               
        public MainViewModel()
        {
        }
    }
}
