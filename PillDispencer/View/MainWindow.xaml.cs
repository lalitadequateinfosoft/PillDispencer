using PillDispencer.Pages;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PillDispencer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static RoutedCommand MyCommand = new RoutedCommand();
        Machines machine;
        HMI hMI;

        public MainWindow()
        {
            InitializeComponent();
            //child = new Machines();
            //child.ParentWindow = this;
            //this.frame.Navigate(child);


            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Title = "Select your project";
            dialog.DefaultExt = ".json"; // Default file extension
            dialog.Filter = "PillDispencer Files (*.json)|*.json"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                hMI = new HMI();
                hMI.ParentWindow = this;
                this.frame.Navigate(hMI);
            }
            else
            {
                machine = new Machines();
                machine.ParentWindow = this;
                this.frame.Navigate(machine);
            }


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


        private void MACHINE_Click(object sender, RoutedEventArgs e)
        {
            machine = new Machines();
            machine.ParentWindow = this;
            this.frame.Navigate(machine);
        }
        private void INTERFACE_Click(object sender, RoutedEventArgs e)
        {

            var BTN = sender as Button;
            if(BTN.Tag.ToString().ToLower()=="machines")
            {
                BTN.Tag = "INTERFACE";
                BTN.Content = "GO TO INTERFACE";
                machine = new Machines();
                machine.ParentWindow = this;
                this.frame.Navigate(machine);
            }
            else
            {
                BTN.Tag = "MACHINES";
                BTN.Content = "VIEW MACHINES";
                hMI = new HMI();
                hMI.ParentWindow = this;
                this.frame.Navigate(hMI);
            }
           
        }
    }
}
