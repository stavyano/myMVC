using System;
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
using System.Windows.Shapes;

namespace MVCPartnersMatcher.View
{
    /// <summary>
    /// Interaction logic for SendMessagesWin.xaml
    /// </summary>
    public partial class SendMessagesWin : Window
    {
        private Dictionary<string, List<string>> userPartnerships;
        private string partnershipName = "";

        public Dictionary<string, List<string>> UserPartnerships
        {
            get
            {
                return userPartnerships;
            }

            set
            {
                userPartnerships = value;
                ListPartnerShipToChoose.ItemsSource = UserPartnerships.Keys;

            }
        }

        public SendMessagesWin()
        {
            InitializeComponent();

        }

        public SendMessagesWin(Dictionary<string, List<string>> userPartnerships)
        {
            this.userPartnerships = userPartnerships;
        }

        private void ListPartnerShipToChoose_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UsersInGroup.SelectedIndex = -1;
            string selectedItem = (string)ListPartnerShipToChoose.SelectedItem;
            send.Visibility = Visibility.Visible;
            UserChoosing.Visibility = Visibility.Visible;
            ContentOfMessages.Visibility = Visibility.Visible;
            content.Visibility = Visibility.Visible;
            UsersInGroup.Visibility = Visibility.Visible;
            UsersInGroup.ItemsSource = userPartnerships[selectedItem];
            partnershipName = selectedItem;
        }

        private void send_Click(object sender, RoutedEventArgs e)
        {
            if (UsersInGroup.SelectedItems.Count == 0)
            {
                ((MainWindow)Application.Current.MainWindow).DisplayError("please choose user before you send the message");
            }
            else
            {
                if (content.Text == "")
                {
                    ((MainWindow)Application.Current.MainWindow).DisplayError("you cant sent empty message");
                }
                else
                {
                    ((MainWindow)Application.Current.MainWindow).sendThemMessages(UsersInGroup.SelectedItems, content.Text, partnershipName);
                    this.Close();
                }
            }

        }
    }
}
