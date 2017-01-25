using MVCPartnersMatcher.Model;
using MVCPartnersMatcher.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace MVCPartnersMatcher.Conroller
{
    class MyController
    {
        private MyModel m_model;
        private MainWindow m_view;

        public MyController()
        {

        }

        public void SetModel(MyModel model)
        {
            m_model = model;
        }

        public void SetView(MainWindow view)
        {
            m_view = view;
        }

        internal void TryLogin(string mail, string password)
        {
            m_model.TryLogin(mail, password);
        }

        internal void DisplayError(string error)
        {
            m_view.DisplayError(error);
        }

        internal void SetCurrenUser(string mail, string gender, string age)
        {
            m_view.SetCurrentUser(mail, gender, age);
        }

        internal void TryRegister(string mail, string password, string repassword, string first, string last, string date, string gender, string phone)
        {
            m_model.TryRegister(mail, password, repassword, first, last, date, gender, phone);
        }

        internal void DisplayInformation(string text, string header)
        {
            m_view.DisplayInformation(text, header);
        }

        internal List<string> GetTypes()
        {
            return m_model.GetTypes();
        }

        internal void SearchSport(string sport, string level, string numOfPartners, string loc, string currentUser)
        {
            m_model.SearchSport(sport, level, numOfPartners, loc, currentUser);
        }

        internal void SetDataForView(ArrayList searchResults)
        {

            m_view.Data = searchResults;
        }

        internal void SearchDate(string m_gender, string m_numOfPartners1, string m_startAge, string m_endAge, string date, string loc, bool m_longdistance, string currentUser, string currentUserAge, string currentUserGender)
        {
            m_model.SearchDate(m_gender, m_numOfPartners1, m_startAge, m_endAge, date, loc, m_longdistance, currentUser, currentUserAge, currentUserGender);
        }

        internal void SearchHouse(string loc, string price, string roomates, bool? smoke, bool? pet, bool? elevator, bool? clean, bool? furniture, bool? kosher, bool? veg, string date, string currentUser)
        {
            m_model.SearchHouse(loc, price, roomates, smoke, pet, elevator, clean, furniture, kosher, veg, date, currentUser);
        }

        internal void SearchTravel(string days, string date, string location, string type, string numberOfTravelers, bool? rlgn, string startAge, string endAge, string currentUser, string age)
        {
            m_model.SearchTravel(days, date, location, type, numberOfTravelers, rlgn, startAge, endAge, currentUser, age);
        }

        internal void SearchOther(string type, string loc, string currentUser)
        {
            m_model.SearchOther(type, loc, currentUser);
        }

        internal void AddSport(string groupName, string m_sport, string m_level, string m_numOfPartners, string loc, string currentUser, string date, string hour)
        {
            m_model.AddSport(groupName, m_sport, m_level, m_numOfPartners, loc, currentUser, date, hour);
        }

        internal void AddDate(string groupName, string loc, string m_numOfPartners1, string m_startAge, string m_endAge, string myHobbies, string m_gender, string m_smoking, bool m_longdistance, string m_religous, string dateDate, string hourdate, string currentUser)
        {
            m_model.AddDate(groupName, loc, m_numOfPartners1, m_startAge, m_endAge, myHobbies, m_gender, m_smoking, m_longdistance, m_religous, dateDate, hourdate, currentUser);
        }

        internal void AddTravel(string numOfDays, string date, string loc, string typeTravel, string numOfTravelers, bool? isCheckedrel, string startAge, string endAge, string groupName, string currentUser)
        {
            m_model.AddTravel(numOfDays, date, loc, typeTravel, numOfTravelers, isCheckedrel, startAge, endAge, groupName, currentUser);
        }

        internal void AddHouse(string loc, string price, string roomates, string rooms, bool? smoke, bool? pet, bool? elevator, bool? clean, bool? furniture, bool? kosher, bool? veg, string date, string address, string size, string groupName, string currentUser)
        {
            m_model.AddHouse(loc, price, roomates, rooms, smoke, pet, elevator, clean, furniture, kosher, veg, date, address, size, groupName, currentUser);
        }

        internal void AddOther(string m_date1, string m_numofpartners, string m_type, string m_desc, string loc, string groupName, string currentUser)
        {
            m_model.AddOther(m_date1, m_numofpartners, m_type, m_desc, loc, groupName, currentUser);
        }

        internal void JoinRequest(string chooseResult, string currentUser)
        {
            m_model.JoinRequest(chooseResult, currentUser);
        }
        internal void setMessagesToShow(List<string> messageToShow)
        {
            m_view.SetMessagesToShow(messageToShow);
        }

        internal void showPartnershipsToSendMessages(string currentUser)
        {
            m_model.showPartnershipsToSendMessages(currentUser);
        }

        internal void setUserToSendThemMessages(Dictionary<string, List<string>> usersInPartnership)
        {
            m_view.setUserToSendThemMessages(usersInPartnership);
        }
        internal void sendMessage(IList selectedItems, string content, string nameOfPartnerShip, string currentUser)
        {
            m_model.sendMessage(selectedItems, content, nameOfPartnerShip, currentUser);
        }
        internal void showMessages(string mail)
        {
            m_model.showMessages(mail);
        }
        internal void DisplayRequests(List<ArrayList> requests)
        {
            m_view.DisplayRequests(requests);
        }
        internal void AddToGroup(string userEmail, string userGroupName)
        {
            m_model.AddToGroup(userEmail, userGroupName);
        }
        internal void DeleteFromAdvertise(string groupName)
        {
            m_model.DeleteFromAdvertise(groupName);
        }
    }
}
