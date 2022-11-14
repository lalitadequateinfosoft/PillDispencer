using System.Collections.Generic;
using System.Windows;


namespace PillDispencer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var MyListItems = GetMyListItems();
            if(MyListItems.Count>0)
            {
                ListViewItems.ItemSource = MyListItems;
            }
        }

        private List<MyListItems> GetMyListItems()
        {
            return new List<MyListItems>()
            {
                new MyListItems("/Icons/pill.png",10),
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
