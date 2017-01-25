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
    /// Interaction logic for Search.xaml
    /// </summary>
    public partial class Add : Window
    {
        enum Gender { זכר, נקבה }
        enum Sport { כדורגל, כדורסל, כדורעף, טניס, פוטבול, שחייה, ריצה_בינונית, מרתון }
        enum Level { קל, בינוני, קשה }
        enum Place { גליל, גולן, חיפה, אזור_השרון, ירושלים, שפלה, תל_אביב, גוש_דן, ערבה, באר_שבע, דרום, אילת, חוץ_לארץ }
        enum hul { אירופה, דרום_אמריקה, ארצות_הברית, מרכז_אמריקה, אוסטרליה, ניו_זילנד, הודו, מזרח_רחוק, יפן, אפריקה }
        enum travelType { שופינג, בטן_גב, טרקינג }
        public Add()
        {
            InitializeComponent();
            gender.ItemsSource = Enum.GetValues(typeof(Gender));
            sport.ItemsSource = Enum.GetValues(typeof(Sport));
            level.ItemsSource = Enum.GetValues(typeof(Level));
            loc.ItemsSource = Enum.GetValues(typeof(Place));
            travel.ItemsSource = Enum.GetValues(typeof(travelType));
            List<string> listOfTypes = ((MainWindow)Application.Current.MainWindow).GetTypes();
            listOfTypes.Add("אחר");
            types.ItemsSource = listOfTypes;
        }
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (groupName.Text=="" || loc.SelectedIndex == -1 || types.SelectedIndex == -1)
            {
                MessageBox.Show("עליך/עלייך לבחור מיקום, תחום ושם קבוצה בכדי להמשיך", "!שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (loc.Text == "חוץ_לארץ" && types.Text != "טיול")
            {
                MessageBox.Show("האתר מאפשר לחפש שותפים בחוץ לארץ רק עבור טיולים", "!שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            else
            {
                types.Visibility = Visibility.Hidden;
                loc.Visibility = Visibility.Hidden;
                label1.Visibility = Visibility.Hidden;
                label.Visibility = Visibility.Hidden;
                groupName.Visibility = Visibility.Hidden;
                group.Visibility = Visibility.Hidden;
                button1.Visibility = Visibility.Hidden;
                button.Visibility = Visibility.Visible;
                switch (types.Text)
                {
                    case "ספורט":
                        canvasSport.Visibility = Visibility.Visible;
                        canvasTravel.Visibility = Visibility.Hidden;
                        canvasHousing.Visibility = Visibility.Hidden;
                        canvasDating.Visibility = Visibility.Hidden;
                        canvasOther.Visibility = Visibility.Hidden;
                        break;
                    case "דייטינג":
                        canvasSport.Visibility = Visibility.Hidden;
                        canvasTravel.Visibility = Visibility.Hidden;
                        canvasHousing.Visibility = Visibility.Hidden;
                        canvasDating.Visibility = Visibility.Visible;
                        canvasOther.Visibility = Visibility.Hidden;
                        break;
                    case "טיול":
                        canvasSport.Visibility = Visibility.Hidden;
                        canvasTravel.Visibility = Visibility.Visible;
                        canvasHousing.Visibility = Visibility.Hidden;
                        canvasDating.Visibility = Visibility.Hidden;
                        canvasOther.Visibility = Visibility.Hidden;
                        if (loc.Text == "חוץ_לארץ")
                        {
                            place.ItemsSource = Enum.GetValues(typeof(hul));
                        }
                        else
                        {
                            place.Items.Add(loc.Text);
                            place.SelectedItem = loc.Text;
                        }
                        break;
                    case "דיור":
                        canvasSport.Visibility = Visibility.Hidden;
                        canvasTravel.Visibility = Visibility.Hidden;
                        canvasHousing.Visibility = Visibility.Visible;
                        canvasDating.Visibility = Visibility.Hidden;
                        canvasOther.Visibility = Visibility.Hidden;
                        break;
                    default:
                        canvasSport.Visibility = Visibility.Hidden;
                        canvasTravel.Visibility = Visibility.Hidden;
                        canvasHousing.Visibility = Visibility.Hidden;
                        canvasDating.Visibility = Visibility.Hidden;
                        canvasOther.Visibility = Visibility.Visible;
                        if (types.Text != "אחר")
                        {
                            nameOfOther.Text = types.Text;
                            nameOfOther.IsReadOnly = true;
                        }
                        break;

                }
            }
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            switch (types.Text)
            {
                case "ספורט":
                    string m_sport = sport.Text;
                    string m_level = level.Text;
                    string m_numOfPartners = price82.Text;
                    string m_date = sportDate.Text;
                    string m_hour = sportHour.Text;
                    if (m_date=="" || m_hour=="" || m_sport == "" || m_level == "" || m_numOfPartners == "")
                    {
                        MessageBox.Show("עליך/עלייך לבחור את כל הפרמטרים כדי להמשיך", "!שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        ((MainWindow)Application.Current.MainWindow).AddSport(groupName.Text,m_sport, m_level, m_numOfPartners, loc.Text,m_date,m_hour);
                        this.Close();
                    }
                    break;
                case "דייטינג":
                    string m_gender = gender.Text;
                    string m_numOfPartners1 = price8.Text;
                    string m_smoking = check33.IsChecked.ToString();
                    bool m_longdistance = (bool)check11.IsChecked;
                    string m_religous = check22.IsChecked.ToString();
                    string m_startAge = textBox1.Text;
                    string m_endAge = textBox2.Text;
                    string dateDate = date1234.Text;
                    string hourdate = hourDate.Text;
                    string myHobbies = hobby.Text;
                    if (dateDate == "" || hourdate == ""||myHobbies==""|| m_gender == "" || m_numOfPartners1 == "" || m_startAge == "" || m_endAge == "")
                    {
                        MessageBox.Show("עליך/עלייך לבחור את כל הפרמטרים כדי להמשיך", "!שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        ((MainWindow)Application.Current.MainWindow).AddDate(groupName.Text,loc.Text,m_numOfPartners1,m_startAge,m_endAge,myHobbies,m_gender,m_smoking,m_longdistance,m_religous, dateDate,hourdate);
                        this.Close();
                    }
                    break;
                case "דיור":
                    if (price.Text == "" || date.Text == "" || loc.Text == "" || roomates.Text == "" || rooms.Text=="" || AddressHouse.Text=="" || sizeHouse.Text=="")
                    {
                        MessageBox.Show("Please enter value in all attributes", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        ((MainWindow)Application.Current.MainWindow).AddHouse(loc.Text, price.Text, roomates.Text,rooms.Text, smoke.IsChecked, pet.IsChecked, elevator.IsChecked, clean.IsChecked, furniture.IsChecked, kosher.IsChecked, veg.IsChecked, date.Text,AddressHouse.Text,sizeHouse.Text,groupName.Text);
                        this.Close();
                    }
                    break;
                case "טיול":
                    if (numOfDays.Text == "" || date1.Text == "" || place.Text == "" || travel.Text == "" || numOfTravelers.Text == "" || checkrel.IsChecked == null || textBoxstart.Text == "" || textBoxend.Text == "")
                    {
                        MessageBox.Show("Please enter value in all attributes", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        ((MainWindow)Application.Current.MainWindow).AddTravel(numOfDays.Text, date1.Text, place.Text, travel.Text, numOfTravelers.Text, checkrel.IsChecked, textBoxstart.Text, textBoxend.Text,groupName.Text);
                        this.Close();
                    }
                    break;
                default:
                    string m_type = nameOfOther.Text;
                    string m_desc = intrestedInWhat.Text;
                    string m_date1 = date123.Text;
                    string m_numofpartners = numofpartners.Text;
                    if (m_date1 == "" || m_numofpartners == "" || m_type == "" || m_desc == "")
                    {
                        MessageBox.Show("עליך/עלייך לבחור את כל הפרמטרים כדי להמשיך", "!שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        ((MainWindow)Application.Current.MainWindow).AddOther(m_date1, m_numofpartners, m_type, m_desc, loc.Text,groupName.Text);
                        this.Close();
                    }
                    break;
            }
        }
    }
}
