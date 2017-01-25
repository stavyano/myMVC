using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        enum Place { גליל, גולן, חיפה, אזור_השרון, ירשולים, שפלה, תל_אביב, גוש_דן, ערבה, באר_שבע, דרום, אילת }
        enum Gender { זכר, נקבה }

        public Register()
        {
            InitializeComponent();
            gender.ItemsSource = Enum.GetValues(typeof(Gender));

        }

        private void notAvailable(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This functionality is not yet availble.", "Informatiom", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (email.Text == "" || password.Text == "" || FirstName.Text == "" || LastName.Text == "" || date.Text == "" || gender.SelectedIndex == -1 || phone.Text == "")
            {
                MessageBox.Show("Please enter value in all attributes", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (password.Text.Length < 6)
            {
                MessageBox.Show("Password must contains at least 6 chars", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            else if (phone.Text.Length != 10 || phone.Text[0] != '0')
            {
                MessageBox.Show("Please be sure you enter valid cell phone", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                ((MainWindow)Application.Current.MainWindow).TryRegister(email.Text, password.Text, repassword.Text, FirstName.Text, LastName.Text, date.Text, gender.Text, phone.Text);

            }

        }

    }
}
