using MVCPartnersMatcher.Conroller;
using MVCPartnersMatcher.View;
using System;
using System.Collections;
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

namespace MVCPartnersMatcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MyController m_c;
        string m_currentUser;
        string m_gender;
        string m_age;
        Dictionary<string, List<string>> m_userPartnerships;
        ArrayList m_data = new ArrayList();
        List<ArrayList> m_requests;
        private bool m_agreeRequest;

        internal void DisplayError(string error)
        {
            MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        internal void SetCurrentUser(string mail, string gender, string age)
        {
            CurrentUser = mail;
            Gender = gender;
            Age = age;
            m_c.showMessages(mail);
            foreach (Window item in App.Current.Windows)
            {
                if (item != this)
                    item.Close();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        internal void TryLogin(string mail, string password)
        {
            m_c.TryLogin(mail, password);
        }

        internal void DisplayInformation(string text, string header)
        {
            MessageBox.Show(text, header, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        internal List<string> GetTypes()
        {
            return m_c.GetTypes();
        }

        internal MyController C
        {
            get
            {
                return m_c;
            }

            set
            {
                m_c = value;
            }
        }

        internal void start()
        {
            this.Show();
        }

        public string CurrentUser
        {
            get
            {
                return m_currentUser;
            }

            set
            {
                m_currentUser = value;
                initiateUserWindow();
            }
        }
        public string Gender
        {
            get
            {
                return m_gender;
            }

            set
            {
                m_gender = value;
            }
        }

        internal void TryRegister(string mail, string password, string repassword, string first, string last, string date, string gender, string phone)
        {
            m_c.TryRegister(mail, password, repassword, first, last, date, gender, phone);
        }

        public string Age
        {
            get
            {
                return m_age;
            }

            set
            {
                m_age = value;
            }
        }
        public ArrayList Data
        {
            get
            {
                return m_data;
            }

            set
            {
                m_data = value;
                if (m_data.Count > 0 && (string)m_data[0] == "#")
                {
                    List<ArrayList> newReq = new List<ArrayList>();
                    for (int i = 0; i < m_requests.Count; i++)
                    {
                        ArrayList req = m_requests[i];
                        string group = (string)req[3];
                        if (group.Equals((string)m_data[1]))
                        {
                            m_c.DeleteFromAdvertise(group);
                            continue;
                        }
                        else
                        {
                            newReq.Add(req);
                        }
                    }
                    m_requests = newReq;
                }
                else
                    showSearchResult();
            }
        }

        private void showSearchResult()
        {
            searchResults.Items.Clear();
            messeagetToShow.Visibility = Visibility.Hidden;
            ListMessagesToShow.Visibility = Visibility.Hidden;
            firstName.Visibility = Visibility.Hidden;
            lastName.Visibility = Visibility.Hidden;
            email.Visibility = Visibility.Hidden;
            confirm.Visibility = Visibility.Hidden;
            nextRequest.Visibility = Visibility.Hidden;
            groupName.Visibility = Visibility.Hidden;


            if (Data.Count == 0)
            {
                MessageBox.Show("No matches found! change your prefernces and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                searchResults.Visibility = Visibility.Visible;
                foreach (var item in Data)
                {
                    searchResults.Items.Add(item);
                }
                request.Visibility = Visibility.Visible;
            }

        }
        private void initiateUserWindow()
        {
            ((MainWindow)App.Current.MainWindow).label.Content = "Hello " + ((MainWindow)App.Current.MainWindow).CurrentUser;
            ((MainWindow)App.Current.MainWindow).button.Visibility = Visibility.Hidden;
            ((MainWindow)App.Current.MainWindow).button1.Visibility = Visibility.Hidden;
            ((MainWindow)App.Current.MainWindow).button2.Visibility = Visibility.Hidden;
            ((MainWindow)App.Current.MainWindow).toolbar.Visibility = Visibility.Visible;
        }

        private void notAvailable(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This functionality is currently not available", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void SearchRoomate(object sender, RoutedEventArgs e)
        {
            searchResults.Visibility = Visibility.Hidden;
            request.Visibility = Visibility.Hidden;
            Search search = new Search();
            search.Show();
        }

        private void Login(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
        }

        private void Register(object sender, RoutedEventArgs e)
        {
            Register register = new Register();
            register.Show();
        }

        private void Search(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This option is not yet available", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        internal void SearchSport(string sport, string level, string numOfPartners, string loc)
        {
            m_c.SearchSport(sport, level, numOfPartners, loc, CurrentUser);
        }

        internal void SearchDate(string m_gender, string m_numOfPartners1, string m_startAge, string m_endAge, string date, string loc, bool m_longdistance)
        {
            m_c.SearchDate(m_gender, m_numOfPartners1, m_startAge, m_endAge, date, loc, m_longdistance, CurrentUser, Age, Gender);
        }

        internal void AddSport(string groupName, string m_sport, string m_level, string m_numOfPartners, string loc, string date, string hour)
        {
            m_c.AddSport(groupName, m_sport, m_level, m_numOfPartners, loc, CurrentUser, date, hour);
        }

        internal void SearchHouse(string loc, string price, string roomates, bool? smoke, bool? pet, bool? elevator, bool? clean, bool? furniture, bool? kosher, bool? veg, string date)
        {
            m_c.SearchHouse(loc, price, roomates, smoke, pet, elevator, clean, furniture, kosher, veg, date, CurrentUser);
        }

        internal void SearchTravel(string days, string date, string location, string type, string numberOfTravelers, bool? rlgn, string startAge, string endAge)
        {
            m_c.SearchTravel(days, date, location, type, numberOfTravelers, rlgn, startAge, endAge, CurrentUser, Age);
        }

        internal void SearchOther(string type, string loc)
        {
            m_c.SearchOther(type, loc, CurrentUser);
        }

        private void AddAdvertise(object sender, RoutedEventArgs e)
        {
            Add add = new Add();
            add.Show();
        }

        internal void AddDate(string groupName, string loc, string m_numOfPartners1, string m_startAge, string m_endAge, string myHobbies, string m_gender, string m_smoking, bool m_longdistance, string m_religous, string dateDate, string hourdate)
        {
            m_c.AddDate(groupName, loc, m_numOfPartners1, m_startAge, m_endAge, myHobbies, m_gender, m_smoking, m_longdistance, m_religous, dateDate, hourdate, CurrentUser);
        }

        internal void AddTravel(string numOfDays, string date, string loc, string typeTravel, string numOfTravelers, bool? isCheckedrel, string startAge, string endAge, string groupName)
        {
            m_c.AddTravel(numOfDays, date, loc, typeTravel, numOfTravelers, isCheckedrel, startAge, endAge, groupName, CurrentUser);
        }

        internal void AddHouse(string loc, string price, string roomates, string rooms, bool? smoke, bool? pet, bool? elevator, bool? clean, bool? furniture, bool? kosher, bool? veg, string date, string address, string size, string groupName)
        {
            m_c.AddHouse(loc, price, roomates, rooms, smoke, pet, elevator, clean, furniture, kosher, veg, date, address, size, groupName, CurrentUser);
        }

        internal void AddOther(string m_date1, string m_numofpartners, string m_type, string m_desc, string loc, string groupName)
        {
            m_c.AddOther(m_date1, m_numofpartners, m_type, m_desc, loc, groupName, CurrentUser);
        }

        private void Request(object sender, RoutedEventArgs e)
        {
            string[] chooseResult = searchResults.SelectedItem.ToString().Split(' ');
            m_c.JoinRequest(chooseResult[2].Substring(0, chooseResult[2].Length - 1), CurrentUser);
        }

        private void SendMessages(object sender, RoutedEventArgs e)
        {
            m_c.showPartnershipsToSendMessages(CurrentUser);

        }
        internal void setUserToSendThemMessages(Dictionary<string, List<string>> usersInPartnership)
        {
            UserPartnerships = usersInPartnership;
        }

        internal void SetMessagesToShow(List<string> messageToShow)
        {
            messeagetToShow.Visibility = Visibility.Visible;
            ListMessagesToShow.ItemsSource = messageToShow;
            ListMessagesToShow.Visibility = Visibility.Visible;
        }
        public Dictionary<string, List<string>> UserPartnerships
        {
            get
            {
                return m_userPartnerships;
            }

            set
            {
                m_userPartnerships = value;
                SendMessagesWin sendMessagesWin = new SendMessagesWin();
                sendMessagesWin.UserPartnerships = value;
                sendMessagesWin.Show();

            }
        }
        internal void sendThemMessages(IList selectedItems, string content, string nameOfPartnership)
        {
            m_c.sendMessage(selectedItems, content, nameOfPartnership, CurrentUser);
        }

        internal void DisplayRequests(List<ArrayList> requests)
        {
            m_requests = requests;
            firstName.Visibility = Visibility.Visible;
            lastName.Visibility = Visibility.Visible;
            email.Visibility = Visibility.Visible;
            confirm.Visibility = Visibility.Visible;
            groupName.Visibility = Visibility.Visible;
            nextRequest.Visibility = Visibility.Visible;
            getRequest();
        }
        private void nextRequest_Click(object sender, RoutedEventArgs e)
        {
            if (m_agreeRequest)
            {
                string userEmail = email.Text;
                string userGroupName = groupName.Text;
                m_c.AddToGroup(userEmail, userGroupName);
            }
            getRequest();
        }

        private void getRequest()
        {
            if (m_requests.Count > 0)
            {
                confirm.IsChecked = false;
                ArrayList userRequest = m_requests.ElementAt(0);
                string lineFirstName = (string)userRequest[0];
                string lineLastName = (string)userRequest[1];
                string lineEmail = (string)userRequest[2];
                string linegroupName = (string)userRequest[3];
                firstName.Text = lineFirstName;
                lastName.Text = lineLastName;
                groupName.Text = linegroupName;
                email.Text = lineEmail;
                m_requests.RemoveAt(0);

            }
            else
            {
                firstName.Visibility = Visibility.Hidden;
                lastName.Visibility = Visibility.Hidden;
                email.Visibility = Visibility.Hidden;
                confirm.Visibility = Visibility.Hidden;
                groupName.Visibility = Visibility.Hidden;
                nextRequest.Visibility = Visibility.Hidden;
            }

        }

        private void confirm_Checked(object sender, RoutedEventArgs e)
        {
            m_agreeRequest = true;

        }
        private void confirm_UnChecked(object sender, RoutedEventArgs e)
        {
            m_agreeRequest = false;

        }
    }
}
