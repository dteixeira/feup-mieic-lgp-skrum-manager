﻿using System.Windows.Controls;

namespace WPFApplication
{
    /// <summary>
    /// Interaction logic for UsersPage.xaml
    /// </summary>
    public partial class UsersPage : UserControl
    {
        private ApplicationController backdata;

        public UsersPage()
        {
            InitializeComponent();
            this.backdata = ApplicationController.Instance;
            this.backdata.CurrentPage = ApplicationPages.UsersPage;
        }
    }
}