using PillDispencer.Pages;
using System;
using System.Collections.Generic;
using System.Windows;


namespace PillDispencer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Machines child;
        HMI machine;

        public MainWindow()
        {
            InitializeComponent();
            //child = new Machines();
            //child.ParentWindow = this;
            //this.frame.Navigate(child);


            machine = new HMI();
            machine.ParentWindow = this;
            this.frame.Navigate(machine);
        }

        #region custom window functions

        private void OnMinimizeButtonClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void OnMaximizeRestoreButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "you want to exit", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                this.Close();
            }

        }

        private void RefreshMaximizeRestoreButton()
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.maximizeButton.Visibility = Visibility.Collapsed;
                this.restoreButton.Visibility = Visibility.Visible;
            }
            else
            {
                this.maximizeButton.Visibility = Visibility.Visible;
                this.restoreButton.Visibility = Visibility.Collapsed;
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            this.RefreshMaximizeRestoreButton();
        }



        #endregion
    }
}
