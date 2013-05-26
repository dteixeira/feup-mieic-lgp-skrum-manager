﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PopupFormControlLib
{
	/// <summary>
	/// Interaction logic for StoryPriorityPage.xaml
	/// </summary>
	public partial class StoryPriorityPage : UserControl, IFormPage
	{
		public StoryPriorityPage()
		{
			this.InitializeComponent();
            this.MustButton.Selected = true;
            this.PageValue = ServiceLib.DataService.StoryPriority.Must;
		}

        public string PageName { get; set; }

        public string PageTitle { get; set; }

        public object PageValue { get; set; }

        private void MustButton_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.MustButton.Selected = true;
            this.ShouldButton.Selected = false;
            this.CouldButton.Selected = false;
            this.WouldButton.Selected = false;
            this.PageValue = ServiceLib.DataService.StoryPriority.Must;
        }

        private void ShouldButton_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.MustButton.Selected = false;
            this.ShouldButton.Selected = true;
            this.CouldButton.Selected = false;
            this.WouldButton.Selected = false;
            this.PageValue = ServiceLib.DataService.StoryPriority.Should;
        }

        private void CouldButton_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.MustButton.Selected = false;
            this.ShouldButton.Selected = false;
            this.CouldButton.Selected = true;
            this.WouldButton.Selected = false;
            this.PageValue = ServiceLib.DataService.StoryPriority.Could;
        }

        private void WouldButton_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.MustButton.Selected = false;
            this.ShouldButton.Selected = false;
            this.CouldButton.Selected = false;
            this.WouldButton.Selected = true;
            this.PageValue = ServiceLib.DataService.StoryPriority.Wont;
        }

    }
}