﻿using System;
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

namespace PillDispencer.Pages
{
    /// <summary>
    /// Interaction logic for HMICard.xaml
    /// </summary>
    public partial class HMICard : Page
    {
        public HMICard()
        {
            InitializeComponent();
            var MyListItems = GetMyListItems();
            if (MyListItems.Count > 0)
            {
                ListViewItems.ItemsSource = MyListItems;
            }
        }
        private List<MyListItems> GetMyListItems()
        {
            return new List<MyListItems>()
            {
                new MyListItems("/Icons/pill.png",10)
            };

        }
    }
}
