using Microsoft.Phone.Controls;
using System;
using System.Windows.Navigation;
using TinderApp.Lib;
using TinderApp.Library.ViewModels;
using TinderApp.Library.Linkedin;

namespace TinderApp
{
    public partial class ProfileInfoPage : PhoneApplicationPage
    {
        private static LinkedinUser _data;

        private JobInfoViewModel _vm;

        public ProfileInfoPage()
        {
            InitializeComponent();
        }

        public static LinkedinUser Data 
        { 
            get {
                return ProfileInfoPage._data;
            }
            set { ProfileInfoPage._data = value; } 
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.LayoutUpdated += ProfileInfoPage_LayoutUpdated;

            base.OnNavigatedTo(e);
        }

        private async void ProfileInfoPage_LayoutUpdated(object sender, EventArgs e)
        {
            //this.LayoutUpdated -= ProfileInfoPage_LayoutUpdated;

            //string id = NavigationContext.QueryString["id"].Replace("-", "");
            //if (id == "data" && _data != null)
            //    _vm = new JobInfoViewModel(Data);
            //else
            //    _vm = await JobInfoViewModel.LoadUserInfo(id);
            //DataContext = _vm;
        }
    }
}