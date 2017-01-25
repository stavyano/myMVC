using MVCPartnersMatcher.Conroller;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MVCPartnersMatcher.Model
{
    class MyModel
    {
        private MyController m_c;
        private List<string> m_groups;
        public MyModel(MyController c)
        {
            m_c = c;
        }

        internal void TryLogin(string mail, string password)
        {
            DirectoryInfo di1 = Directory.GetParent(Directory.GetCurrentDirectory());
            DirectoryInfo di2 = Directory.GetParent(di1.FullName);
            string pathOfUsers = di2.FullName + "\\Model\\Database\\Users.txt";
            using (StreamReader sr = new StreamReader(pathOfUsers))
            {
                string line = sr.ReadLine();
                bool found = false;
                bool pass = false;
                while (!found && (line != null))
                {
                    string lineEmail = line.Split('|')[0];
                    string linePassword = line.Split('|')[1];
                    if (lineEmail.Equals(mail))
                    {
                        if (linePassword.Equals(password))
                        {
                            found = true;
                            break;
                        }
                        m_c.DisplayError("Your password is incorrect.");
                        pass = true;
                        break;
                    }
                    line = sr.ReadLine();
                }
                if (found)
                {
                    m_c.SetCurrenUser(mail, line.Split('|')[5], (2017 - (Convert.ToInt32(((line.Split('|')[4]).Split('/')[2])))).ToString()); //mail,gender,age  
                    m_groups = new List<string>();
                    GetGroups(mail);
                    List<ArrayList> requests = GetUsersRequests();
                    m_c.DisplayRequests(requests);
                }
                else if (!pass)
                {
                    m_c.DisplayError("This email is not registered!");
                }
            }
        }

        internal void SearchDate(string m_gender, string m_numOfPartners1, string m_startAge, string m_endAge, string date, string loc, bool m_longdistance, string currentUser, string currentUserAge, string currentUserGender)
        {
            int n1, n2;
            if (!Int32.TryParse(m_startAge, out n1) || !Int32.TryParse(m_startAge, out n2))
            {
                m_c.DisplayError("גיל חייב להיות מספר");
            }
            else if (n1 > n2)
            {
                m_c.DisplayError("גיל התחלה חייב להיות גדול מגיל סיום");
            }
            else
            {
                ArrayList searchResults = new ArrayList();
                string[] dates = new string[8] { "MM/dd/yyyy", "MM/d/yyyy", "M/dd/yyyy", "M/d/yyyy", "dd/MM/yyyy", "d/MM/yyyy", "dd/M/yyyy", "d/MM/yyyy" };
                DateTime d = DateTime.ParseExact(date, dates, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
                var todaysDate = DateTime.Today;
                if (d < todaysDate)
                {
                    m_c.DisplayError("Please pick a future date");
                }
                else
                {
                    string mes_startAge;
                    string mes_endAge;
                    string mes_intrestedInGender;
                    string mes_smoking = "";
                    string mes_longDistance;
                    string mes_religous = "";
                    string mes_place;
                    string[] mes;
                    DirectoryInfo di1 = Directory.GetParent(Directory.GetCurrentDirectory());
                    DirectoryInfo di2 = Directory.GetParent(di1.FullName);
                    string pathOfUsers = di2.FullName + "\\Model\\Database\\dates.txt";
                    using (StreamReader sr = new StreamReader(pathOfUsers))
                    {
                        string line = sr.ReadLine();
                        while ((line != null))
                        {
                            mes = line.Split('|');
                            string mail = mes[16];
                            if (mail != currentUser)
                            {
                                mes_place = mes[3];
                                mes_intrestedInGender = mes[10];
                                mes_startAge = mes[6];
                                mes_endAge = mes[7];
                                mes_longDistance = mes[12];
                                if (mes_place == loc && ((mes_longDistance == "t" && m_longdistance == true) || (mes_longDistance == "f" && m_longdistance == false)))
                                {

                                    string matchGender = getUserGender(mail);
                                    string matchAge = getUserAge(mail).ToString();
                                    if ((Convert.ToInt32(currentUserAge) <= Convert.ToInt32(mes_endAge) && Convert.ToInt32(currentUserAge) >= Convert.ToInt32(mes_startAge)) && (Convert.ToInt32(m_startAge) <= Convert.ToInt32(matchAge) && Convert.ToInt32(m_endAge) >= Convert.ToInt32(matchAge)))
                                    {
                                        if ((currentUserGender == mes_intrestedInGender) && m_gender == matchGender)
                                        {
                                            if (mes[11] == "t")
                                            {
                                                mes_smoking = "כן";

                                            }
                                            else
                                                mes_smoking = "לא";
                                            if (mes[12] == "t")
                                            {
                                                mes_longDistance = "כן";
                                            }
                                            else mes_longDistance = "לא";
                                            if (mes[13] == "t")
                                            {
                                                mes_religous = "כן";
                                            }
                                            else
                                                mes_religous = "לא";
                                            string s = (("שם קבוצה: " + mes[17] + " תאריך: " + mes[14] + " שעה: " + mes[15] + " מיקום: " + mes[3] + " מספר פרטנרים: " + mes[5] + " גיל התחלתי: " + mes_startAge + " גיל סופי: " + mes_endAge + " תחביבים: " + mes[8] + " מעוניין ב" + mes_intrestedInGender + " מעשן: " + mes_smoking + " קשר רציני: " + mes_longDistance + " דתי: " + mes_religous));
                                            searchResults.Add(s);
                                        }

                                    }



                                }
                            }

                            line = sr.ReadLine();
                        }
                    }
                    m_c.SetDataForView(searchResults);
                }
            }
        }

        private void sendRequestEmail(string mail)
        {
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("partnersMatcherApplication@gmail.com", "0544204320");

            MailMessage mm = new MailMessage("partnersMatcherApplication@gmail.com", mail, "New Request For Your Advertise", "Please confirm or reject the request.");
            mm.BodyEncoding = UTF8Encoding.UTF8;
            mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            client.Send(mm);
        }

        internal void JoinRequest(string partnershipName, string userMail)
        {
            //int index = partnershipName.IndexOf(":");
            // partnershipName = partnershipName.Substring(index + 1);
            DirectoryInfo di11 = Directory.GetParent(Directory.GetCurrentDirectory());
            DirectoryInfo di22 = Directory.GetParent(di11.FullName);
            string pathOfPartnerships = di22.FullName + "\\Model\\Database\\Partnerships.txt";

            DirectoryInfo di1 = Directory.GetParent(Directory.GetCurrentDirectory());
            DirectoryInfo di2 = Directory.GetParent(di1.FullName);
            string pathOfRequests = di2.FullName + "\\Model\\Database\\Requests.txt";

            StreamReader sr = new StreamReader(pathOfPartnerships);
            StreamReader sr1 = new StreamReader(pathOfRequests);
            bool found = false;
            string lineInRequests = sr1.ReadLine();

            while (lineInRequests != null)
            {
                string[] checkInRequests = lineInRequests.Split('|');
                if (checkInRequests[0].Equals(partnershipName) && checkInRequests[1].Equals(userMail))
                {
                    found = true;
                    break;
                }
                lineInRequests = sr1.ReadLine();
            }
            sr1.Close();
            string line = sr.ReadLine();
            if (!found)
            {
                while (line != null)
                {
                    string[] partnership = line.Split('|');
                    if (partnership[2].Equals(partnershipName))
                    {
                        using (StreamWriter sw = new StreamWriter(pathOfRequests, true))
                        {
                            sw.WriteLine(partnership[2] + "|" + userMail);
                            sendRequestEmail(partnership[4]);
                            string[] mailsOfPartners = partnership[5].Split('=');
                            foreach (string mail in mailsOfPartners)
                            {
                                if (!mail.Equals(partnership[4]))
                                    sendRequestEmail(mail);
                            }
                        }
                        break;
                    }
                    line = sr.ReadLine();
                }
            }
            sr.Close();
            if (found)
                m_c.DisplayInformation("You have already sent a request to this partnership", "");
            else
                m_c.DisplayInformation("The request sent to the relevant users", "");
        }

        internal void AddOther(string m_date1, string m_numofpartners, string m_type, string m_desc, string loc, string groupName, string currentUser)
        {
            int n3;
            if (!Int32.TryParse(m_numofpartners, out n3))
            {
                m_c.DisplayError("אנא הכנס מספר לשותפים");
            }
            else if (n3 <= 1)
            {
                m_c.DisplayError("אנא הכנס מספר חיובי גדול מאחד לשותפים");
            }
            else
            {
                string[] dates = new string[8] { "MM/dd/yyyy", "MM/d/yyyy", "M/dd/yyyy", "M/d/yyyy", "dd/MM/yyyy", "d/MM/yyyy", "dd/M/yyyy", "d/MM/yyyy" };
                DateTime d = DateTime.ParseExact(m_date1, dates, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
                var todaysDate = DateTime.Today;
                if (d < todaysDate)
                {
                    m_c.DisplayError("Please pick a future date");
                }
                else
                {
                    DirectoryInfo di1 = Directory.GetParent(Directory.GetCurrentDirectory());
                    DirectoryInfo di2 = Directory.GetParent(di1.FullName);
                    string pathOfUsers = di2.FullName + "\\Model\\Database\\other.txt";
                    string path = di2.FullName + "\\Model\\Database\\Partnerships.txt";
                    string types = di2.FullName + "\\Model\\Database\\types.txt";
                    using (StreamWriter sw = new StreamWriter(pathOfUsers, true))
                    {
                        using (StreamWriter sw2 = new StreamWriter(path, true))
                        {
                            string lineToAdvert = "0|" + DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + "|" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + "|" + loc + "|" + m_type + "|a|" + m_numofpartners + "|" + m_desc + "|" + m_date1 + "|" + currentUser + "|" + groupName;
                            string lineToPartner = DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + "|" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + "|" + groupName + "|" + m_numofpartners + "|" + currentUser + "|" + currentUser;
                            sw.WriteLine(lineToAdvert);
                            sw2.WriteLine(lineToPartner);
                        }
                    }
                    using (StreamWriter sw = new StreamWriter(types, true))
                    {
                        sw.WriteLine(m_type);
                    }
                    m_c.DisplayInformation("Your new advertise was successfuly added", "");
                }
            }
        }

        internal void AddHouse(string loc, string price, string roomates, string rooms, bool? smoke, bool? pet, bool? elevator, bool? clean, bool? furniture, bool? kosher, bool? veg, string date, string address, string size, string groupName, string currentUser)
        {
            if (CheckGroupNameExists(groupName))
            {
                m_c.DisplayError("Group name already exists in the system. Please choose another name.");
            }
            else
            {
                int n1, n2;
                if (!Int32.TryParse(price, out n1))
                {
                    m_c.DisplayError("price must be a number");
                }
                else if (!Int32.TryParse(size, out n2))
                {
                    m_c.DisplayError("size must be a number");
                }
                else
                {
                    string[] dates = new string[8] { "MM/dd/yyyy", "MM/d/yyyy", "M/dd/yyyy", "M/d/yyyy", "dd/MM/yyyy", "d/MM/yyyy", "dd/M/yyyy", "d/MM/yyyy" };
                    DateTime d = DateTime.ParseExact(date, dates, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
                    var todaysDate = DateTime.Today;
                    if (d < todaysDate)
                    {
                        m_c.DisplayError("Please pick a future date");
                    }
                    else
                    {
                        DirectoryInfo di1 = Directory.GetParent(Directory.GetCurrentDirectory());
                        DirectoryInfo di2 = Directory.GetParent(di1.FullName);
                        string pathOfUsers = di2.FullName + "\\Model\\Database\\housing.txt";
                        string path = di2.FullName + "\\Model\\Database\\Partnerships.txt";
                        string fur = (furniture == true) ? "t" : "f";
                        string smo = (smoke == true) ? "t" : "f";
                        string pets = (pet == true) ? "t" : "f";
                        string elev = (elevator == true) ? "t" : "f";
                        string kosh = (kosher == true) ? "t" : "f";
                        string clea = (clean == true) ? "t" : "f";
                        string veggie = (veg == true) ? "t" : "f";
                        using (StreamWriter sw = new StreamWriter(pathOfUsers, true))
                        {
                            using (StreamWriter sw2 = new StreamWriter(path, true))
                            {
                                string lineToAdvert = "0|" + DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + "|" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + "|" + loc + "|a|" + roomates + "|" + rooms + "|" + address + "|a|" + size + "|" + price + "|" + fur + "|" + pets + "|" + smo + "|" + veggie + "|" + kosh + "|" + clea + "|" + elev + "|" + date + "|" + currentUser + "|" + groupName;
                                string lineToPartner = DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + "|" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + "|" + groupName + "|" + roomates + "|" + currentUser + "|";
                                sw.WriteLine(lineToAdvert);
                                sw2.WriteLine(lineToPartner);
                            }
                        }
                        m_c.DisplayInformation("Your house advertise was successfuly added", "");
                    }
                }
            }

        }

        internal void AddTravel(string numOfDays, string date, string loc, string typeTravel, string numOfTravelers, bool? isCheckedrel, string startAge, string endAge, string groupName, string currentUser)
        {
            if (CheckGroupNameExists(groupName))
            {
                m_c.DisplayError("Group name already exists in the system. Please choose another name.");
            }
            else
            {
                int n1, n2, days;
                if (!Int32.TryParse(numOfDays, out days))
                {
                    m_c.DisplayError("number of days must be a number");
                }
                else if (!Int32.TryParse(startAge, out n1))
                {
                    m_c.DisplayError("start age must be a number");
                }
                else if (!Int32.TryParse(endAge, out n2))
                {
                    m_c.DisplayError("end age must be a number");
                }
                else if (n1 > n2)
                {
                    m_c.DisplayError("end age must be a greater or equals to start age");
                }
                else
                {
                    string[] dates = new string[8] { "MM/dd/yyyy", "MM/d/yyyy", "M/dd/yyyy", "M/d/yyyy", "dd/MM/yyyy", "d/MM/yyyy", "dd/M/yyyy", "d/M/yyyy" };
                    DateTime d = DateTime.ParseExact(date, dates, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
                    var todaysDate = DateTime.Today;
                    if (d < todaysDate)
                    {
                        m_c.DisplayError("Please pick a future date");
                    }
                    DirectoryInfo di1 = Directory.GetParent(Directory.GetCurrentDirectory());
                    DirectoryInfo di2 = Directory.GetParent(di1.FullName);
                    string pathOfUsers = di2.FullName + "\\Model\\Database\\travel.txt";
                    string path = di2.FullName + "\\Model\\Database\\Partnerships.txt";
                    string rel = (isCheckedrel == true) ? "t" : "f";
                    using (StreamWriter sw = new StreamWriter(pathOfUsers, true))
                    {
                        using (StreamWriter sw2 = new StreamWriter(path, true))
                        {
                            string lineToAdvert = "0|" + DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + "|" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + "|" + loc + "|a|" + numOfTravelers + "|" + numOfDays + "|" + loc + "|" + rel + "|" + startAge + "|" + endAge + "|" + typeTravel + "|a|" + date + "|" + currentUser + "|" + groupName;
                            string lineToPartner = DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + "|" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + "|" + groupName + "|" + numOfTravelers + "|" + currentUser + "|" + currentUser;
                            sw.WriteLine(lineToAdvert);
                            sw2.WriteLine(lineToPartner);
                        }
                    }
                    m_c.DisplayInformation("Your travel advertise was successfuly added", "");

                }
            }
        }

        internal void AddDate(string groupName, string loc, string m_numOfPartners1, string m_startAge, string m_endAge, string myHobbies, string m_gender, string m_smoking, bool m_longdistance, string m_religous, string dateDate, string hourdate, string currentUser)
        {
            if (CheckGroupNameExists(groupName))
            {
                m_c.DisplayError("Group name already exists in the system. Please choose another name.");
            }
            else
            {
                string[] dates = new string[8] { "MM/dd/yyyy", "MM/d/yyyy", "M/dd/yyyy", "M/d/yyyy", "dd/MM/yyyy", "d/M/yyyy", "dd/M/yyyy", "d/MM/yyyy" };
                DateTime d = DateTime.ParseExact(dateDate, dates, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
                var todaysDate = DateTime.Today;
                if (d < todaysDate)
                {
                    m_c.DisplayError("Please pick a future date");
                }
                else
                {
                    DirectoryInfo di1 = Directory.GetParent(Directory.GetCurrentDirectory());
                    DirectoryInfo di2 = Directory.GetParent(di1.FullName);
                    string pathOfUsers = di2.FullName + "\\Model\\Database\\dates.txt";
                    string path = di2.FullName + "\\Model\\Database\\Partnerships.txt";
                    string smoke = (m_smoking == "True") ? "t" : "f";
                    string longd = (m_longdistance == true) ? "t" : "f";
                    string rel = (m_religous == "True") ? "t" : "f";
                    using (StreamWriter sw = new StreamWriter(pathOfUsers, true))
                    {
                        using (StreamWriter sw2 = new StreamWriter(path, true))
                        {
                            string lineToAdvert = "0|" + DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + "|" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + "|" + loc + "|a|" + m_numOfPartners1 + "|" + m_startAge + "|" + m_endAge + "|" + myHobbies + "|a|" + m_gender + "|" + smoke + "|" + longd + "|" + rel + "|" + dateDate + "|" + hourdate + "|" + currentUser + "|" + groupName;
                            string lineToPartner = DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + "|" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + "|" + groupName + "|" + m_numOfPartners1 + "|" + currentUser + "|" + currentUser;
                            sw.WriteLine(lineToAdvert);
                            sw2.WriteLine(lineToPartner);
                        }
                    }
                    m_c.DisplayInformation("Your date advertise was successfuly added", "");
                }
            }
        }

        internal void AddSport(string groupName, string m_sport, string m_level, string m_numOfPartners, string loc, string currentUser, string date, string hour)
        {
            if (CheckGroupNameExists(groupName))
            {
                m_c.DisplayError("Group name already exists in the system. Please choose another name.");
            }

            else
            {
                string[] dates = new string[8] { "MM/dd/yyyy", "MM/d/yyyy", "M/dd/yyyy", "M/d/yyyy", "dd/MM/yyyy", "d/M/yyyy", "dd/M/yyyy", "d/MM/yyyy" };
                DateTime d = DateTime.ParseExact(date, dates, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
                var todaysDate = DateTime.Today;
                if (d < todaysDate)
                {
                    m_c.DisplayError("Please pick a future date");
                }
                else
                {
                    DirectoryInfo di1 = Directory.GetParent(Directory.GetCurrentDirectory());
                    DirectoryInfo di2 = Directory.GetParent(di1.FullName);
                    string pathOfUsers = di2.FullName + "\\Model\\Database\\sport.txt";
                    string path = di2.FullName + "\\Model\\Database\\Partnerships.txt";
                    using (StreamWriter sw = new StreamWriter(pathOfUsers, true))
                    {
                        using (StreamWriter sw2 = new StreamWriter(path, true))
                        {
                            string lineToAdvert = "0|" + DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + "|" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + "|" + loc + "|a|" + m_numOfPartners + "|" + m_sport + "|a|" + m_level + "|a|" + hour + "|" + date + "|" + currentUser + "|" + groupName;
                            string lineToPartner = DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year + "|" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + "|" + groupName + "|" + m_numOfPartners + "|" + currentUser + "|" + currentUser;
                            sw.WriteLine(lineToAdvert);
                            sw2.WriteLine(lineToPartner);
                        }
                    }
                    m_c.DisplayInformation("Your sport advertise was successfuly added", "");
                }
            }
        }

        private bool CheckGroupNameExists(string groupName)
        {
            DirectoryInfo di1 = Directory.GetParent(Directory.GetCurrentDirectory());
            DirectoryInfo di2 = Directory.GetParent(di1.FullName);
            bool found = false;
            string pathOfUsers = di2.FullName + "\\Model\\Database\\Partnerships.txt";
            using (StreamReader sr = new StreamReader(pathOfUsers))
            {
                string line = sr.ReadLine();
                while (!found && line != null && line != "")
                {
                    if (line.Split('|')[2] == groupName)
                    {
                        found = true;
                    }
                    line = sr.ReadLine();
                }
            }
            return found;
        }

        internal void SearchOther(string type, string loc, string currentUser)
        {

            string[] mes;
            DirectoryInfo di1 = Directory.GetParent(Directory.GetCurrentDirectory());
            DirectoryInfo di2 = Directory.GetParent(di1.FullName);
            string pathOfUsers = di2.FullName + "\\Model\\Database\\other.txt";
            ArrayList searchResults = new ArrayList();
            using (StreamReader sr = new StreamReader(pathOfUsers))
            {
                string line = sr.ReadLine();
                while ((line != null))
                {
                    mes = line.Split('|');
                    string mail = mes[9];
                    if (mail != currentUser)
                    {
                        string mes_type = mes[4];
                        if (mes_type.Equals(type) && (loc == mes[3]))
                        {
                            searchResults.Add("שם קבוצה: " + mes[10] + ", תאריך: " + mes[8] + " מיקום: " + mes[3] + " מספר פרטנרים: " + mes[6] + " תחום: " + mes_type + " תיאור: " + mes[7]);

                        }
                    }
                    line = sr.ReadLine();
                }
            }


            m_c.SetDataForView(searchResults);

        }

        internal void SearchTravel(string days, string date, string location, string type, string numberOfTravelers, bool? rlgn, string startAge, string endAge, string currentUser, string age)
        {
            int n1, n2, numOfDays;
            if (!Int32.TryParse(days, out numOfDays))
            {
                m_c.DisplayError("number of days must be a number");
            }
            if (!Int32.TryParse(startAge, out n1))
            {
                m_c.DisplayError("start age must be a number");
            }
            if (!Int32.TryParse(endAge, out n2))
            {
                m_c.DisplayError("end age must be a number");
            }
            if (n1 > n2)
            {
                m_c.DisplayError("end age must be a greater or equals to start age");
            }
            string[] dates = new string[8] { "MM/dd/yyyy", "MM/d/yyyy", "M/dd/yyyy", "M/d/yyyy", "dd/MM/yyyy", "d/MM/yyyy", "dd/M/yyyy", "d/MM/yyyy" };
            DateTime d = DateTime.ParseExact(date, dates, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            var todaysDate = DateTime.Today;
            if (d < todaysDate)
            {
                m_c.DisplayError("Please pick a future date");
            }
            ArrayList searchResults = new ArrayList();
            DirectoryInfo di1 = Directory.GetParent(Directory.GetCurrentDirectory());
            DirectoryInfo di2 = Directory.GetParent(di1.FullName);
            string pathOfUsers = di2.FullName + "\\Model\\Database\\travel.txt";
            using (StreamReader sr = new StreamReader(pathOfUsers))
            {
                string line = sr.ReadLine();
                while (line != null)
                {
                    string[] lineSplitted = line.Split('|');
                    string lineLocation = lineSplitted[7];
                    string lineNumOfTravelers = lineSplitted[5];
                    string lineNumOfDays = lineSplitted[6];
                    string lineRelig = lineSplitted[8];
                    if (lineRelig == "t")
                    {
                        lineRelig = "True";
                    }
                    else
                    {
                        lineRelig = "False";
                    }
                    string lineStartAge = lineSplitted[9];
                    string lineEndAge = lineSplitted[10];
                    string lineType = lineSplitted[11];
                    string lineDate = lineSplitted[13];
                    string lineMail = lineSplitted[14];
                    string lineGroup = lineSplitted[15];
                    if (!(lineMail == currentUser))
                    {
                        if (lineLocation.Equals(location) && lineNumOfTravelers.Equals(numberOfTravelers) && lineType.Equals(type))
                        {
                            if (Convert.ToInt32(age) >= Convert.ToInt32(lineStartAge) && Convert.ToInt32(age) <= Convert.ToInt32(lineEndAge))
                            {
                                if (getUserAge(lineMail) >= Convert.ToInt32(startAge) && getUserAge(lineMail) <= Convert.ToInt32(endAge))
                                {
                                    searchResults.Add("שם קבוצה: " + lineGroup + ", תאריך טיול: " + lineDate + ", מיקום: " + lineLocation + ", מספר ימים: " + lineNumOfDays + ", מספר מטיילים: " + lineNumOfTravelers + ", דתי: " + lineRelig + ", גיל התחלתי: " + lineStartAge + ", גיל סיום: " + lineEndAge + ", סוג טיול: " + lineType);
                                }
                            }
                        }
                    }
                    line = sr.ReadLine();
                }
            }
            m_c.SetDataForView(searchResults);
        }

        internal void SearchHouse(string loc, string price, string roomates, bool? smoke, bool? pet, bool? elevator, bool? clean, bool? furniture, bool? kosher, bool? veg, string date, string currentUser)
        {
            string[] dates = new string[8] { "MM/dd/yyyy", "MM/d/yyyy", "M/dd/yyyy", "M/d/yyyy", "dd/MM/yyyy", "d/MM/yyyy", "dd/M/yyyy", "d/MM/yyyy" };
            DateTime d = DateTime.ParseExact(date, dates, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            var todaysDate = DateTime.Today;
            if (d < todaysDate)
            {
                m_c.DisplayError("Please pick a future date");
            }
            ArrayList searchResults = new ArrayList();
            DirectoryInfo di1 = Directory.GetParent(Directory.GetCurrentDirectory());
            DirectoryInfo di2 = Directory.GetParent(di1.FullName);
            string pathOfUsers = di2.FullName + "\\Model\\Database\\housing.txt";
            using (StreamReader sr = new StreamReader(pathOfUsers))
            {
                string line = sr.ReadLine();
                while (line != null)
                {
                    string[] lineSplitted = line.Split('|');
                    string lineLocation = lineSplitted[3];
                    string lineNumOfRoomates = lineSplitted[5];
                    string lineNumOfRooms = lineSplitted[6];
                    string lineAddress = lineSplitted[7];
                    string lineSize = lineSplitted[9] + "מטר רבוע";
                    string linePrice = lineSplitted[10];
                    string lineFur = lineSplitted[11];
                    string linePets = lineSplitted[12];
                    string lineSmoke = lineSplitted[13];
                    string lineVeg = lineSplitted[14];
                    string lineKosher = lineSplitted[15];
                    string lineClean = lineSplitted[16];
                    string lineElevator = lineSplitted[17];
                    string lineDate = lineSplitted[18];
                    string lineMail = lineSplitted[19];
                    string lineGroup = lineSplitted[20];
                    if (lineFur == "t")
                    {
                        lineFur = "True";
                    }
                    else
                    {
                        lineFur = "False";
                    }
                    if (linePets == "t")
                    {
                        linePets = "True";
                    }
                    else
                    {
                        linePets = "False";
                    }
                    if (lineSmoke == "t")
                    {
                        lineSmoke = "True";
                    }
                    else
                    {
                        lineSmoke = "False";
                    }
                    if (lineVeg == "t")
                    {
                        lineVeg = "True";
                    }
                    else
                    {
                        lineVeg = "False";
                    }
                    if (lineKosher == "t")
                    {
                        lineKosher = "True";
                    }
                    else
                    {
                        lineKosher = "False";
                    }
                    if (lineClean == "t")
                    {
                        lineClean = "True";
                    }
                    else
                    {
                        lineClean = "False";
                    }
                    if (lineElevator == "t")
                    {
                        lineElevator = "True";
                    }
                    else
                    {
                        lineElevator = "False";
                    }
                    int startPrice, endPrice;
                    if (price == "0-1000")
                    {
                        startPrice = 0; endPrice = 1000;
                    }
                    else if (price == "1000-2000")
                    {
                        startPrice = 1000; endPrice = 2000;
                    }
                    else if (price == "2000-3000")
                    {
                        startPrice = 2000; endPrice = 3000;
                    }
                    else
                    {
                        startPrice = 3000; endPrice = Int32.MaxValue;
                    }
                    if (!(lineMail == currentUser))
                    {
                        if (lineLocation.Equals(loc) && lineNumOfRoomates.Equals(roomates))
                        {
                            if ((linePets == "True" && pet == true) || (linePets == "False" && pet == false))
                            {
                                if (Convert.ToInt32(linePrice) >= startPrice && Convert.ToInt32(linePrice) <= endPrice)
                                {
                                    searchResults.Add("שם קבוצה: " + lineGroup + ", תאריך כניסה: " + lineDate + ", כתובת: " + lineAddress + ", מספר שותפים: " + lineNumOfRoomates + ", מספר חדרים: " + lineNumOfRooms + ", גודל: " + lineSize + ", מחיר מבוקש: " + linePrice + ", בעלי חיים: " + linePets + ", עישון: " + lineSmoke + ", צמחונות: " + lineVeg + ", כשרות: " + lineKosher + ", נקיון: " + lineClean + ", מעלית: " + lineElevator);
                                }
                            }
                        }
                    }
                    line = sr.ReadLine();
                }
            }
            m_c.SetDataForView(searchResults);
        }

        private int getUserAge(string email)
        {
            DirectoryInfo di1 = Directory.GetParent(Directory.GetCurrentDirectory());
            DirectoryInfo di2 = Directory.GetParent(di1.FullName);
            string pathOfUsers = di2.FullName + "\\Model\\Database\\Users.txt";
            using (StreamReader sr = new StreamReader(pathOfUsers))
            {
                string line = sr.ReadLine();
                while (line != null)
                {
                    string lineEmail = line.Split('|')[0];
                    if (lineEmail.Equals(email))
                    {
                        return (2017 - (Convert.ToInt32(((line.Split('|')[4]).Split('/')[2]))));
                    }
                    line = sr.ReadLine();
                }
            }
            return 0;

        }
        private string getUserGender(string email)
        {
            DirectoryInfo di1 = Directory.GetParent(Directory.GetCurrentDirectory());
            DirectoryInfo di2 = Directory.GetParent(di1.FullName);
            string pathOfUsers = di2.FullName + "\\Model\\Database\\Users.txt";
            string ans = "";
            using (StreamReader sr = new StreamReader(pathOfUsers))
            {
                string line = sr.ReadLine();
                while (line != null)
                {
                    string lineEmail = line.Split('|')[0];
                    if (lineEmail.Equals(email))
                    {
                        return line.Split('|')[5];
                    }
                    line = sr.ReadLine();
                }
            }
            return ans;

        }

        internal void SearchSport(string sport, string level, string numOfPartners, string loc, string currentUser)
        {
            string mes_numOfPartners;
            string mes_sport;
            string mes_level;
            string mes_place;
            string[] mes;

            DirectoryInfo di1 = Directory.GetParent(Directory.GetCurrentDirectory());
            DirectoryInfo di2 = Directory.GetParent(di1.FullName);
            string pathOfUsers = di2.FullName + "\\Model\\Database\\sport.txt";
            ArrayList searchResults = new ArrayList();
            using (StreamReader sr = new StreamReader(pathOfUsers))
            {
                string line = sr.ReadLine();
                while ((line != null))
                {
                    mes = line.Split('|');
                    if (!(mes[12] == currentUser))
                    {

                        mes_sport = mes[6];
                        mes_level = mes[8];
                        mes_numOfPartners = mes[5];
                        mes_place = mes[3];
                        if (mes_place == loc && sport == mes_sport && mes_level == level)
                        {
                            searchResults.Add(("שם קבוצה: " + mes[13] + ", תאריך: " + mes[11] + " שעה: " + mes[10] + " מיקום: " + mes_place + " מספר שחקנים: " + mes_numOfPartners + " סוג ספורט: " + mes_sport + " רמת קושי: " + mes_level));
                        }
                    }
                    line = sr.ReadLine();
                }
            }
            m_c.SetDataForView(searchResults);

        }

        internal List<string> GetTypes()
        {
            List<string> listOfTypes = new List<string>();
            DirectoryInfo di1 = Directory.GetParent(Directory.GetCurrentDirectory());
            DirectoryInfo di2 = Directory.GetParent(di1.FullName);
            string pathOfUsers = di2.FullName + "\\Model\\Database\\types.txt";
            using (StreamReader sr = new StreamReader(pathOfUsers))
            {
                string line = sr.ReadLine();
                while ((line != null))
                {
                    listOfTypes.Add(line);
                    line = sr.ReadLine();
                }
            }
            return listOfTypes;
        }

        internal void TryRegister(string mail, string password, string repassword, string first, string last, string date, string gender, string phone)
        {
            if (!Regex.IsMatch(first, @"^[a-zA-Z|]+$") || !Regex.IsMatch(last, @"^[a-zA-Z|]+$"))
            {
                m_c.DisplayError("First And Last name must contains english letters");
            }
            else if (mail.Split('@').Length != 2 || !mail.Split('@')[1].Contains("."))
            {
                m_c.DisplayError("Please enter valid Email");
            }
            string[] dates = new string[8] { "MM/dd/yyyy", "MM/d/yyyy", "M/dd/yyyy", "M/d/yyyy", "dd/MM/yyyy", "d/MM/yyyy", "dd/M/yyyy", "d/MM/yyyy" };
            DateTime d = DateTime.ParseExact(date, dates, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            var todaysDate = DateTime.Today;

            if (d > todaysDate)
            {
                m_c.DisplayError("Please be sure you didnt born tomorrow");
            }
            else if (repassword != password)
            {
                m_c.DisplayError("Your new passwords entries did not match");
            }
            else
            {
                DirectoryInfo di1 = Directory.GetParent(Directory.GetCurrentDirectory());
                DirectoryInfo di2 = Directory.GetParent(di1.FullName);
                bool found = false;
                string pathOfUsers = di2.FullName + "\\Model\\Database\\Users.txt";
                using (StreamReader sr = new StreamReader(pathOfUsers))
                {
                    string line = sr.ReadLine();
                    found = false;
                    while (!found && (line != null))
                    {
                        string lineEmail = line.Split('|')[0];
                        if (lineEmail.Equals(mail))
                        {
                            found = true;
                            m_c.DisplayError("Email is already being used by another user");
                            break;
                        }
                        line = sr.ReadLine();
                    }
                }
                if (!found)
                {
                    try
                    {
                        sendRegEmail(mail);
                        DirectoryInfo di3 = Directory.GetParent(Directory.GetCurrentDirectory());
                        DirectoryInfo di4 = Directory.GetParent(di3.FullName);
                        string pathOfMessages = di4.FullName + "\\Model\\Database\\Messages.txt";
                        using (StreamWriter sw = new StreamWriter(pathOfMessages, true))
                        {
                            sw.WriteLine(mail + ":");

                        }
                        using (StreamWriter sw = new StreamWriter(pathOfUsers, true))
                        {

                            sw.WriteLine(mail + "|" + password + "|" + first + "|" + last + "|" + date + "|" + gender + "|" + phone);
                        }
                        m_c.SetCurrenUser(mail, gender, (2017 - (Convert.ToInt32(((date).Split('/')[2])))).ToString());
                        m_c.DisplayInformation("Hello " + last + " " + first + "\nEnjoy our services", "Welcome to PartnersMatch App");
                    }
                    catch
                    {
                        m_c.DisplayError("Please enter valid Email");
                    }
                }
            }
        }
        private void sendRegEmail(string mail)
        {
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("partnersMatcherApplication@gmail.com", "0544204320");

            MailMessage mm = new MailMessage("partnersMatcherApplication@gmail.com", mail, "Welcome to PartnersMatcher Application!", "You are now officially a member of our community!");
            mm.BodyEncoding = UTF8Encoding.UTF8;
            mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            client.Send(mm);
        }
        internal void sendMessage(IList selectedItems, string content, string nameOfPartnersip, string currentUser)
        {
            DirectoryInfo di1 = Directory.GetParent(Directory.GetCurrentDirectory());
            DirectoryInfo di2 = Directory.GetParent(di1.FullName);
            string pathOfMessages = di2.FullName + "\\Model\\Database\\Messages.txt";
            string pathOfMessagesNew = di2.FullName + "\\Model\\Database\\Messages2.txt";
            HashSet<string> users = new HashSet<string>();
            foreach (string item in selectedItems)
            {
                users.Add(item);
            }
            using (File.Create(pathOfMessagesNew)) { }
            using (StreamReader sr = new StreamReader(pathOfMessages))
            {
                using (StreamWriter sw = new StreamWriter(pathOfMessagesNew, true))
                {
                    string line1 = sr.ReadLine();
                    while (line1 != null)
                    {
                        if (users.Contains(line1.Split(':')[0]))
                        {
                            sw.WriteLine(line1 + currentUser + "|" + nameOfPartnersip + "|" + content + ",");
                        }
                        else
                            sw.WriteLine(line1);
                        line1 = sr.ReadLine();
                    }
                }
            }
            File.Delete(pathOfMessages);
            File.Move(pathOfMessagesNew, pathOfMessages);
            m_c.DisplayInformation("Messages sent", "Messages sent");
        }
        internal void showMessages(string mail)
        {

            DirectoryInfo di1 = Directory.GetParent(Directory.GetCurrentDirectory());
            DirectoryInfo di2 = Directory.GetParent(di1.FullName);
            string pathOfMessages = di2.FullName + "\\Model\\Database\\Messages.txt";
            string[] messages;
            List<string> MessageToShow = new List<string>();
            using (StreamReader sr = new StreamReader(pathOfMessages))
            {
                string line = sr.ReadLine();
                while (line != null)
                {
                    string[] content = line.Split(':');
                    string userGetMessages = content[0];
                    if (userGetMessages == mail)
                    {
                        messages = content[1].Split(',');
                        for (int i = 0; i < messages.Length - 1; i++)
                        {
                            string[] OneMessage = messages[i].Split('|');
                            string sender = OneMessage[0];
                            string partnership = OneMessage[1];
                            string text = OneMessage[2];
                            MessageToShow.Add("Message from: " + sender + " Group: " + partnership + "\n" + "Content: " + text);
                        }
                        break;
                    }
                    line = sr.ReadLine();
                }
            }
            m_c.setMessagesToShow(MessageToShow);

        }
        internal void showPartnershipsToSendMessages(string currentUser)
        {
            DirectoryInfo di1 = Directory.GetParent(Directory.GetCurrentDirectory());
            DirectoryInfo di2 = Directory.GetParent(di1.FullName);
            string pathOfPartnerships = di2.FullName + "\\Model\\Database\\Partnerships.txt";
            Dictionary<string, List<string>> UsersInPartnership = new Dictionary<string, List<string>>();
            using (StreamReader sr = new StreamReader(pathOfPartnerships))
            {
                string line = sr.ReadLine();
                while (line != null)
                {
                    if (line.Contains(currentUser))
                    {
                        string[] content = line.Split('|');
                        UsersInPartnership.Add(content[2], new List<string>());
                        string[] users = content[5].Split('=');
                        foreach (string user in users)
                        {
                            if (user != currentUser)
                            {
                                UsersInPartnership[content[2]].Add(user);
                            }
                        }
                    }
                    line = sr.ReadLine();
                }

            }
            m_c.setUserToSendThemMessages(UsersInPartnership);
        }
        private void GetGroups(string mail)
        {
            DirectoryInfo di1 = Directory.GetParent(Directory.GetCurrentDirectory());
            DirectoryInfo di2 = Directory.GetParent(di1.FullName);
            string pathOfPartnersShips = di2.FullName + "\\Model\\Database\\Partnerships.txt";

            using (StreamReader sr = new StreamReader(pathOfPartnersShips))
            {
                string line = sr.ReadLine();
                while (line != null && line != "")
                {
                    string lineEmail = line.Split('|')[4];
                    if (lineEmail.Equals(mail))
                    {
                        string lineGroupName = line.Split('|')[2];
                        m_groups.Add(lineGroupName);
                    }
                    line = sr.ReadLine();
                }

            }

        }

        private List<ArrayList> GetUsersRequests()
        {
            DirectoryInfo di1 = Directory.GetParent(Directory.GetCurrentDirectory());
            DirectoryInfo di2 = Directory.GetParent(di1.FullName);
            string pathOfRequests = di2.FullName + "\\Model\\Database\\Requests.txt";
            List<ArrayList> usersData = new List<ArrayList>();
            using (StreamReader sr = new StreamReader(pathOfRequests))
            {

                string line = sr.ReadLine();
                while (line != null && line != "")
                {
                    string lineGroupName = line.Split('|')[0];
                    if (m_groups.Contains(lineGroupName))
                    {
                        string lineEmail = line.Split('|')[1];
                        ArrayList dataUser = GetDataUser(lineEmail);
                        dataUser.Add(lineEmail);
                        dataUser.Add(lineGroupName);

                        usersData.Add(dataUser);
                    }
                    line = sr.ReadLine();
                }

            }
            return usersData;
        }

        internal void AddToGroup(string userEmail, string userGroupName)
        {
            DirectoryInfo di1 = Directory.GetParent(Directory.GetCurrentDirectory());
            DirectoryInfo di2 = Directory.GetParent(di1.FullName);
            string pathOfPartnersShips = di2.FullName + "\\Model\\Database\\Partnerships.txt";
            string newPathOfPartnersShips = di2.FullName + "\\Model\\Database\\Partnerships1.txt";
            using (File.Create(newPathOfPartnersShips)) { }

            using (StreamReader sr = new StreamReader(pathOfPartnersShips))
            {
                using (StreamWriter sw = new StreamWriter(newPathOfPartnersShips))
                {
                    bool found = false;
                    string line = sr.ReadLine();
                    while (!found && line != null && line != "")
                    {
                        string lineGroupName = line.Split('|')[2];
                        if (lineGroupName.Equals(userGroupName))
                        {
                            found = true;
                            DeleteRequest(lineGroupName, userEmail);
                            int lineNumPartners = Int32.Parse(line.Split('|')[3]);
                            string[] partners = line.Split('|')[5].Split('=');
                            int numOfPartnersInGroup = partners.Length;
                            if (numOfPartnersInGroup < lineNumPartners)
                            {
                                string newLine = line + "=" + userEmail;
                                sw.WriteLine(newLine);
                                sendConfirmEmail(userEmail, lineGroupName);
                                foreach (string partnerEmail in partners)
                                {
                                    sendConfirmEmailToPartner(partnerEmail, lineGroupName);
                                }
                            }
                            if (numOfPartnersInGroup + 1 == lineNumPartners)
                            {
                                m_c.SetDataForView(new ArrayList { "#", lineGroupName });
                            }
                            break;
                        }
                        sw.WriteLine(line);
                        line = sr.ReadLine();
                    }
                    line = sr.ReadLine();
                    while (line != null && line != "")
                    {
                        sw.WriteLine(line);
                        line = sr.ReadLine();
                    }
                }

            }
            File.Delete(pathOfPartnersShips);
            File.Move(newPathOfPartnersShips, pathOfPartnersShips);
        }


        private void DeleteRequest(string lineGroupName, string userEmail)
        {

            DirectoryInfo di1 = Directory.GetParent(Directory.GetCurrentDirectory());
            DirectoryInfo di2 = Directory.GetParent(di1.FullName);
            string pathOfRequests = di2.FullName + "\\Model\\Database\\Requests.txt";
            string newPathOfRequests = di2.FullName + "\\Model\\Database\\Requests1.txt";
            using (File.Create(newPathOfRequests)) { }
            ArrayList dataUser = new ArrayList();
            using (StreamReader sr = new StreamReader(pathOfRequests))
            {
                bool found = false;
                string line = sr.ReadLine();
                using (StreamWriter sw = new StreamWriter(newPathOfRequests))
                {
                    while (!found && line != null && line != "")
                    {
                        string lineGroup = line.Split('|')[0];
                        string lineEmail = line.Split('|')[1];
                        if (lineEmail.Equals(userEmail) && lineGroup.Equals(lineGroupName))
                        {
                            break;
                        }
                        sw.WriteLine(line);
                        line = sr.ReadLine();
                    }
                    line = sr.ReadLine();
                    while (line != null && line != "")
                    {
                        sw.WriteLine(line);
                        line = sr.ReadLine();
                    }

                }

            }
            File.Delete(pathOfRequests);
            File.Move(newPathOfRequests, pathOfRequests);
        }

        private ArrayList GetDataUser(string email)
        {
            DirectoryInfo di1 = Directory.GetParent(Directory.GetCurrentDirectory());
            DirectoryInfo di2 = Directory.GetParent(di1.FullName);
            string pathOfUsers = di2.FullName + "\\Model\\Database\\Users.txt";
            ArrayList dataUser = new ArrayList();
            using (StreamReader sr = new StreamReader(pathOfUsers))
            {
                bool found = false;
                string line = sr.ReadLine();
                while (!found && line != null && line != "")
                {
                    string lineEmail = line.Split('|')[0];
                    if (lineEmail.Equals(email))
                    {
                        found = true;
                        string userFirstName = line.Split('|')[2];
                        string userLastName = line.Split('|')[3];
                        dataUser.Add(userFirstName);
                        dataUser.Add(userLastName);
                        break;
                    }
                    line = sr.ReadLine();
                }

            }
            return dataUser;
        }
        private void sendConfirmEmail(string mail, string groupName)
        {
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("partnersMatcherApplication@gmail.com", "0544204320");

            MailMessage mm = new MailMessage("partnersMatcherApplication@gmail.com", mail, "This is a approval message!", "You are now officially a member of the group " + groupName);
            mm.BodyEncoding = UTF8Encoding.UTF8;
            mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
            client.Send(mm);
        }

        private void sendConfirmEmailToPartner(string mail, string groupName)
        {
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("partnersMatcherApplication@gmail.com", "0544204320");

            MailMessage mm = new MailMessage("partnersMatcherApplication@gmail.com", mail, "This is a approval message!", "a new member join to the group: " + groupName);
            mm.BodyEncoding = UTF8Encoding.UTF8;
            mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
            client.Send(mm);
        }

        internal void DeleteFromAdvertise(string groupName)
        {
            bool found = false;
            found = CheckIfInDates(groupName);
            if (!found)
                found = CheckIfInHousing(groupName);
            if (!found)
                found = CheckIfInSport(groupName);
            if (!found)
                found = CheckIfInTravel(groupName);
            if (!found)
                found = CheckIfInOther(groupName);

        }

        private bool CheckIfInOther(string groupName)
        {
            DirectoryInfo di1 = Directory.GetParent(Directory.GetCurrentDirectory());
            DirectoryInfo di2 = Directory.GetParent(di1.FullName);
            string pathOfother = di2.FullName + "\\Model\\Database\\other.txt";
            string newPathOfother = di2.FullName + "\\Model\\Database\\other1.txt";
            using (File.Create(newPathOfother)) { }

            bool found = false;
            using (StreamReader sr = new StreamReader(pathOfother))
            {
                string line = sr.ReadLine();
                using (StreamWriter sw = new StreamWriter(newPathOfother))
                {
                    while (!found && line != null && line != "")
                    {
                        string lineGroup = line.Split('|')[10];

                        if (groupName.Equals(lineGroup))
                        {
                            found = true;
                            break;
                        }
                        sw.WriteLine(line);
                        line = sr.ReadLine();
                    }
                    line = sr.ReadLine();
                    while (line != null && line != "")
                    {
                        sw.WriteLine(line);
                        line = sr.ReadLine();
                    }

                }

            }
            File.Delete(pathOfother);
            File.Move(newPathOfother, pathOfother);
            return found;
        }

        private bool CheckIfInTravel(string groupName)
        {
            DirectoryInfo di1 = Directory.GetParent(Directory.GetCurrentDirectory());
            DirectoryInfo di2 = Directory.GetParent(di1.FullName);
            string pathOftravel = di2.FullName + "\\Model\\Database\\travel.txt";
            string newPathOftravel = di2.FullName + "\\Model\\Database\\travel1.txt";
            using (File.Create(newPathOftravel)) { }

            bool found = false;
            using (StreamReader sr = new StreamReader(pathOftravel))
            {
                string line = sr.ReadLine();
                using (StreamWriter sw = new StreamWriter(newPathOftravel))
                {
                    while (!found && line != null && line != "")
                    {
                        string lineGroup = line.Split('|')[15];

                        if (groupName.Equals(lineGroup))
                        {
                            found = true;
                            break;
                        }
                        sw.WriteLine(line);
                        line = sr.ReadLine();
                    }
                    line = sr.ReadLine();
                    while (line != null && line != "")
                    {
                        sw.WriteLine(line);
                        line = sr.ReadLine();
                    }

                }

            }
            File.Delete(pathOftravel);
            File.Move(newPathOftravel, pathOftravel);
            return found;
        }

        private bool CheckIfInSport(string groupName)
        {
            DirectoryInfo di1 = Directory.GetParent(Directory.GetCurrentDirectory());
            DirectoryInfo di2 = Directory.GetParent(di1.FullName);
            string pathOfsport = di2.FullName + "\\Model\\Database\\sport.txt";
            string newPathOfsport = di2.FullName + "\\Model\\Database\\sport1.txt";
            using (File.Create(newPathOfsport)) { }

            bool found = false;
            using (StreamReader sr = new StreamReader(pathOfsport))
            {
                string line = sr.ReadLine();
                using (StreamWriter sw = new StreamWriter(newPathOfsport))
                {
                    while (!found && line != null && line != "")
                    {
                        string lineGroup = line.Split('|')[13];

                        if (groupName.Equals(lineGroup))
                        {
                            found = true;
                            break;
                        }
                        sw.WriteLine(line);
                        line = sr.ReadLine();
                    }
                    line = sr.ReadLine();
                    while (line != null && line != "")
                    {
                        sw.WriteLine(line);
                        line = sr.ReadLine();
                    }

                }

            }
            File.Delete(pathOfsport);
            File.Move(newPathOfsport, pathOfsport);
            return found;
        }

        private bool CheckIfInHousing(string groupName)
        {
            DirectoryInfo di1 = Directory.GetParent(Directory.GetCurrentDirectory());
            DirectoryInfo di2 = Directory.GetParent(di1.FullName);
            string pathOfhousing = di2.FullName + "\\Model\\Database\\housing.txt";
            string newPathOfhousing = di2.FullName + "\\Model\\Database\\housing1.txt";
            using (File.Create(newPathOfhousing)) { }

            bool found = false;
            using (StreamReader sr = new StreamReader(pathOfhousing))
            {
                string line = sr.ReadLine();
                using (StreamWriter sw = new StreamWriter(newPathOfhousing))
                {
                    while (!found && line != null && line != "")
                    {
                        string lineGroup = line.Split('|')[20];

                        if (groupName.Equals(lineGroup))
                        {
                            found = true;
                            break;
                        }
                        sw.WriteLine(line);
                        line = sr.ReadLine();
                    }
                    line = sr.ReadLine();
                    while (line != null && line != "")
                    {
                        sw.WriteLine(line);
                        line = sr.ReadLine();
                    }

                }

            }
            File.Delete(pathOfhousing);
            File.Move(newPathOfhousing, pathOfhousing);
            return found;
        }

        private bool CheckIfInDates(string groupName)
        {
            DirectoryInfo di1 = Directory.GetParent(Directory.GetCurrentDirectory());
            DirectoryInfo di2 = Directory.GetParent(di1.FullName);
            string pathOfdates = di2.FullName + "\\Model\\Database\\dates.txt";
            string newPathOfdates = di2.FullName + "\\Model\\Database\\dates1.txt";
            using (File.Create(newPathOfdates)) { }

            bool found = false;
            using (StreamReader sr = new StreamReader(pathOfdates))
            {
                string line = sr.ReadLine();
                using (StreamWriter sw = new StreamWriter(newPathOfdates))
                {
                    while (!found && line != null && line != "")
                    {
                        string lineGroup = line.Split('|')[17];

                        if (groupName.Equals(lineGroup))
                        {
                            found = true;
                            break;
                        }
                        sw.WriteLine(line);
                        line = sr.ReadLine();
                    }
                    line = sr.ReadLine();
                    while (line != null && line != "")
                    {
                        sw.WriteLine(line);
                        line = sr.ReadLine();
                    }

                }

            }
            File.Delete(pathOfdates);
            File.Move(newPathOfdates, pathOfdates);
            return found;
        }
    }
}
