﻿namespace FriendExtension
{
    using Friend.Infra;
    using System.Windows.Controls;

    public partial class Shell : UserControl, IView
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
