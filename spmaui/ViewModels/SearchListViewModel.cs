
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using spmaui.Models;
using spmaui.Services;
using System.Windows.Input;

namespace spmaui.ViewModels
{
    public class SearchListViewModel : INotifyPropertyChanged
    {
        List<ContactsModel> _searchResult;
        public List<ContactsModel> SearchResults
        {
            get
            {
                return _searchResult;
            }
            set
            {
                _searchResult = value;
                OnPropertyChanged();
            }
        }

        private readonly Connections _conSvc = new Connections();

        public ICommand PerformSearch => new Command<string>((string query) =>
        {
            SearchResults =  GetSearchResults(query);
        });

        public SearchListViewModel()
        {

        }

        List<ContactsModel> GetSearchResults(string query)
        {
            string jwtToken = Preferences.Get("AccessToken", "").ToString();
            string memberID = "0";
            if (Preferences.Get("UserID", "").ToString() != null)
                memberID = Preferences.Get("UserID", "").ToString();

            var result = (List<ContactsModel>) _conSvc.GetSearchResult(memberID, jwtToken, query);
            if (result != null)
            {
                int i = 0;
                foreach (var r in result)
                {
                    string img = App.AppSettings.AppMemberImagesURL + "default.png";
                    if (r.picturePath != null || r.picturePath != "")
                    {
                        img = App.AppSettings.AppMemberImagesURL  + r.picturePath;
                    }
                    result[i].picturePath = img;

                    if (r.titleDesc == null || r.titleDesc == "")
                    {
                        result[i].titleDesc = "Unknown Title";
                    }
                    i++;
                }
            }
            return result;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}

