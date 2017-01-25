using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
    public partial class Search : Window
    {
        enum Gender { זכר, נקבה }
        enum Sport { כדורגל, כדורסל, כדורעף, טניס, פוטבול, שחייה, ריצה_בינונית, מרתון }
        enum Level { קל, בינוני, קשה }
        enum Place { גליל, גולן, חיפה, אזור_השרון, ירושלים, שפלה, תל_אביב, גוש_דן, ערבה, באר_שבע, דרום, אילת, חוץ_לארץ }
        enum hul { אירופה, דרום_אמריקה, ארצות_הברית, מרכז_אמריקה, אוסטרליה, ניו_זילנד, הודו, מזרח_רחוק, יפן, אפריקה }
        enum travelType { שופינג, בטן_גב, טרקינג }
        public Search()
        {
            InitializeComponent();
            gender.ItemsSource = Enum.GetValues(typeof(Gender));
            sport.ItemsSource = Enum.GetValues(typeof(Sport));
            level.ItemsSource = Enum.GetValues(typeof(Level));
            loc.ItemsSource = Enum.GetValues(typeof(Place));
            travel.ItemsSource = Enum.GetValues(typeof(travelType));
            List<string> listOfTypes = ((MainWindow)Application.Current.MainWindow).GetTypes();
            types.ItemsSource = listOfTypes;
        }
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (loc.SelectedIndex == -1 || types.SelectedIndex == -1)
            {
                MessageBox.Show("עליך/עלייך לבחור גם מיקום וגם תחום בכדי להמשיך", "!שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
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
                button1.Visibility = Visibility.Hidden;
                button.Visibility = Visibility.Visible;
                switch (types.Text)
                {
                    case "ספורט":
                        canvasSport.Visibility = Visibility.Visible;
                        canvasTravel.Visibility = Visibility.Hidden;
                        canvasHousing.Visibility = Visibility.Hidden;
                        canvasDating.Visibility = Visibility.Hidden;
                        break;
                    case "דייטינג":
                        canvasSport.Visibility = Visibility.Hidden;
                        canvasTravel.Visibility = Visibility.Hidden;
                        canvasHousing.Visibility = Visibility.Hidden;
                        canvasDating.Visibility = Visibility.Visible;
                        break;
                    case "טיול":
                        canvasSport.Visibility = Visibility.Hidden;
                        canvasTravel.Visibility = Visibility.Visible;
                        canvasHousing.Visibility = Visibility.Hidden;
                        canvasDating.Visibility = Visibility.Hidden;
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
                        break;
                    default:
                        canvasSport.Visibility = Visibility.Hidden;
                        canvasTravel.Visibility = Visibility.Hidden;
                        canvasHousing.Visibility = Visibility.Hidden;
                        canvasDating.Visibility = Visibility.Hidden;
                        ((MainWindow)Application.Current.MainWindow).SearchOther(types.Text, loc.Text);
                        this.Close();
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
                    if (m_sport == "" || m_level == "" || m_numOfPartners == "")
                    {
                        MessageBox.Show("עליך/עלייך לבחור את כל הפרמטרים כדי להמשיך", "!שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        ((MainWindow)Application.Current.MainWindow).SearchSport(m_sport, m_level, m_numOfPartners, loc.Text);
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
                    if (m_gender == "" || m_numOfPartners1 == "" || m_startAge == "" || m_endAge == "")
                    {
                        MessageBox.Show("עליך/עלייך לבחור את כל הפרמטרים כדי להמשיך", "!שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        ((MainWindow)Application.Current.MainWindow).SearchDate(m_gender, m_numOfPartners1, m_startAge, m_endAge, date1234.Text, loc.Text, m_longdistance);
                        this.Close();
                    }
                    break;
                case "דיור":
                    if (price.Text == "" || date.Text == "" || loc.Text == "" || roomates.Text == "" || smoke == null || pet == null || elevator == null || clean == null || furniture == null || kosher == null || veg == null)
                    {
                        MessageBox.Show("Please enter value in all attributes", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        ((MainWindow)Application.Current.MainWindow).SearchHouse(loc.Text, price.Text, roomates.Text, smoke.IsChecked, pet.IsChecked, elevator.IsChecked, clean.IsChecked, furniture.IsChecked, kosher.IsChecked, veg.IsChecked, date.Text);
                        this.Close();
                    }
                    break;
                case "טיול":
                    if (textBox01.Text == "" || date1.Text == "" || place.Text == "" || travel.Text == "" || price1_Copy.Text == "" || check12.IsChecked == null || textBox11.Text == "" || textBox21.Text == "")
                    {
                        MessageBox.Show("Please enter value in all attributes", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        ((MainWindow)Application.Current.MainWindow).SearchTravel(textBox01.Text, date1.Text, place.Text, travel.Text, price1_Copy.Text, check12.IsChecked, textBox11.Text, textBox21.Text);
                        this.Close();
                    }
                    break;
                default:

                    ((MainWindow)Application.Current.MainWindow).SearchOther(types.Text,loc.Text);
                    this.Close();

                    break;
            }
        }
    }
}

