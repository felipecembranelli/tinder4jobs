using Microsoft.Phone.Controls;
using System;
using System.Device.Location;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using TinderApp.Library;
using TinderApp.Library.Facebook;
using TinderApp.Library.MVVM;
using Windows.Devices.Geolocation;
using OAuthLinkedIn;
using System.Text;
using Windows.Web.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace TinderApp
{
    public partial class InitialPage : PhoneApplicationPage
    {
        string _consumerKey = "772jmojzy2vnra";
        string _consumerSecretKey = "hQu5DFEqP5JQyPGr";
        string _linkedInRequestTokenUrl = "https://api.linkedin.com/uas/oauth/requestToken";
        string _linkedInAccessTokenUrl = "https://api.linkedin.com/uas/oauth/accessToken";

        string _requestPeopleUrl = "http://api.linkedin.com/v1/people/~";
        string _requestConnectionsUrl = "http://api.linkedin.com/v1/people/~/connections";
        string _requestPositionsUrl = "http://api.linkedin.com/v1/people/~:(positions)";

        string _requestJobsUrl = "http://api.linkedin.com/v1/people/~/suggestions/job-suggestions";
        string _requestJobsByKeyWordsUrl = "https://api.linkedin.com/v1/job-search?keywords=quality";

        string _oAuthAuthorizeLink = "";
        string _requestToken = "";
        string _oAuthVerifier = "";
        string _requestTokenSecretKey = "";
        string _accessToken = "";
        string _accessTokenSecretKey = "";

        string _linkedInProfile = "";

        string callback = "https://www.linkedin.com/sucess.htm";

        OAuthUtil oAuthUtil = new OAuthUtil();

        public InitialPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            webBrowser.Navigating += webBrowser_Navigating;
            this.LayoutUpdated += InitialPage_LayoutUpdated;

            Open.Completed += Open_Completed;

            base.OnNavigatedTo(e);
        }

        void Open_Completed(object sender, EventArgs e)
        {
            if (ConsentManager.HasConsented)
                LoginButtonBorder.Visibility = Visibility.Visible;
            else
                TermsBorder.Visibility = System.Windows.Visibility.Visible;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            webBrowser.Navigating -= webBrowser_Navigating;
            Open.Completed -= Open_Completed;
            base.OnNavigatingFrom(e);
        }

        private void agreeButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            TermsBorder.Visibility = Visibility.Collapsed;
            ConsentManager.HasConsented = true;
            LoginButtonBorder.Visibility = Visibility.Visible;
        }

        //private async System.Threading.Tasks.Task Authenticate(string accessToken, string fbid)
        //{
        //    ProfilePhoto.Background = new ImageBrush() { ImageSource = new BitmapImage(new Uri(String.Format("https://graph.facebook.com/me/picture?access_token={0}&height=100&width=100", accessToken))) };

        //    FacebookSessionInfo sessionInfo = new FacebookSessionInfo();
        //    sessionInfo.FacebookToken = accessToken;
        //    sessionInfo.FacebookID = fbid;

        //    Geolocator location = new Geolocator();
        //    location.DesiredAccuracy = PositionAccuracy.Default;
        //    var usrLocation = await location.GetGeopositionAsync();

        //    TinderSession activeSession = TinderSession.CreateNewSession(sessionInfo, new GeographicalCordinates() { Latitude = usrLocation.Coordinate.Latitude, Longitude = usrLocation.Coordinate.Longitude });
        //    if (await activeSession.Authenticate())
        //    {
        //        (App.Current as App).RightSideBar.DataContext = activeSession.Matches;

        //        TopBarViewModel.ShowTopButtons = System.Windows.Visibility.Visible;

        //        NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));

        //        App.RootFrame.RemoveBackEntry();
        //    }
        //}

        private async System.Threading.Tasks.Task Authenticate(string accessToken, string lkId)
        {

            // INICIO
            string _requestPeopleUrl = "http://api.linkedin.com/v1/people/~";
            //string _requestPeopleUrl = "https://api.linkedin.com/v1/people/~:(id,first-name,last-name,headline)";
            
            string nonce = oAuthUtil.GetNonce();
            string timeStamp = oAuthUtil.GetTimeStamp();

            try
            {

                System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
                httpClient.MaxResponseContentBufferSize = int.MaxValue;
                httpClient.DefaultRequestHeaders.ExpectContinue = false;
                System.Net.Http.HttpRequestMessage requestMsg = new System.Net.Http.HttpRequestMessage();
                requestMsg.Method = new System.Net.Http.HttpMethod("GET");
                requestMsg.RequestUri = new Uri(_requestPeopleUrl, UriKind.Absolute);
                requestMsg.Headers.Add("x-li-format", "json");


                string sigBaseStringParams = "oauth_consumer_key=" + _consumerKey;
                sigBaseStringParams += "&" + "oauth_nonce=" + nonce;
                sigBaseStringParams += "&" + "oauth_signature_method=" + "HMAC-SHA1";
                sigBaseStringParams += "&" + "oauth_timestamp=" + timeStamp;
                sigBaseStringParams += "&" + "oauth_token=" + _accessToken;
                sigBaseStringParams += "&" + "oauth_verifier=" + _oAuthVerifier;
                sigBaseStringParams += "&" + "oauth_version=1.0";
                string sigBaseString = "GET&";
                sigBaseString += Uri.EscapeDataString(_requestPeopleUrl) + "&" + Uri.EscapeDataString(sigBaseStringParams);

                // LinkedIn requires both consumer secret and request token secret
                string signature = oAuthUtil.GetSignature(sigBaseString, _consumerSecretKey, _accessTokenSecretKey);

                string data = "realm=\"http://api.linkedin.com/\", oauth_consumer_key=\"" + _consumerKey
                              +
                              "\", oauth_token=\"" + _accessToken +
                              "\", oauth_verifier=\"" + _oAuthVerifier +
                              "\", oauth_nonce=\"" + nonce +
                              "\", oauth_signature_method=\"HMAC-SHA1\", oauth_timestamp=\"" + timeStamp +
                              "\", oauth_version=\"1.0\", oauth_signature=\"" + Uri.EscapeDataString(signature) + "\"";

                requestMsg.Headers.Authorization = new AuthenticationHeaderValue("OAuth", data);
                var response = await httpClient.SendAsync(requestMsg);

                var text = response.Content.ReadAsStringAsync();

                _linkedInProfile = await text;

            }
            catch (Exception Err)
            {
                throw;
            }

            //<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
            //<person>
            //  <first-name>Felipe</first-name>
            //  <last-name>Cembranelli</last-name>
            //  <headline>Manager at CI&amp;T</headline>
            //  <site-standard-profile-request>
            //    <url>https://www.linkedin.com/profile/view?id=3770090&amp;authType=name&amp;authToken=moVF&amp;trk=api*a3576543*s3647743*</url>
            //  </site-standard-profile-request>
            //</person>


            var linkedinUser = JsonConvert.DeserializeObject<LinkedinUser>(_linkedInProfile);


            // FIM

            //ProfilePhoto.Background = new ImageBrush() { ImageSource = new BitmapImage(new Uri(String.Format("https://graph.facebook.com/me/picture?access_token={0}&height=100&width=100", accessToken))) };

            LinkedInSessionInfo sessionInfo = new LinkedInSessionInfo();
            sessionInfo.AcessToken = accessToken;
            sessionInfo.LinkedInID = linkedinUser.FirstName;

            //Geolocator location = new Geolocator();
            //location.DesiredAccuracy = PositionAccuracy.Default;
            //var usrLocation = await location.GetGeopositionAsync();

            //TinderSession activeSession = TinderSession.CreateNewSession(sessionInfo, new GeographicalCordinates() { Latitude = usrLocation.Coordinate.Latitude, Longitude = usrLocation.Coordinate.Longitude });

            TinderSession activeSession = TinderSession.CreateNewSession(sessionInfo);


            if (await activeSession.Authenticate(_consumerKey,_accessToken,_oAuthVerifier, _consumerSecretKey, _accessTokenSecretKey))
            {
                (App.Current as App).RightSideBar.DataContext = activeSession.Matches;

                TopBarViewModel.ShowTopButtons = System.Windows.Visibility.Visible;

                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));

                App.RootFrame.RemoveBackEntry();
            }
        }

        private void FacebookLoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginButtonBorder.Visibility = System.Windows.Visibility.Collapsed;
            WebViewBorder.Visibility = System.Windows.Visibility.Visible;

            //webBrowser.Navigate(new Uri("https://www.facebook.com/dialog/oauth?client_id=464891386855067&redirect_uri=https://www.facebook.com/connect/login_success.html&scope=basic_info,email,public_profile,user_about_me,user_activities,user_birthday,user_education_history,user_friends,user_interests,user_likes,user_location,user_photos,user_relationship_details&response_type=token", UriKind.Absolute));

            this.GetRequestToken();
            
        }

        private void InitialPage_LayoutUpdated(object sender, EventArgs e)
        {
            this.LayoutUpdated -= InitialPage_LayoutUpdated;

            if (ConsentManager.HasConsented)
            {
                if (!(App.Current as App).JustLoggedOut)
                {
                    TombstoneData data = TombstoneManager.Load();
                    if (data != null)
                    {
                        TinderSession activeSession = TinderSession.FromTombstoneData(data);
                        (App.Current as App).RightSideBar.DataContext = activeSession.Matches;

                        TopBarViewModel.ShowTopButtons = System.Windows.Visibility.Visible;

                        NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));

                        App.RootFrame.RemoveBackEntry();

                        return;
                    }
                }
                else
                {
                    LoginButtonBorder.Visibility = System.Windows.Visibility.Visible;
                }
            }

            if (!(App.Current as App).JustLoggedOut)
                Open.Begin();
        }

        private async void webBrowser_Navigating(object sender, NavigatingEventArgs e)
        {
            if (e.Uri.ToString().StartsWith("https://www.linkedin.com/sucess.htm"))
            {
                e.Cancel = true;

                WebViewBorder.Visibility = System.Windows.Visibility.Collapsed;
                LoginButtonBorder.Visibility = System.Windows.Visibility.Collapsed;
                LoggedInPanel.Visibility = System.Windows.Visibility.Visible;

                if (Pulsate.GetCurrentState() != ClockState.Active)
                {
                    Pulsate.RepeatBehavior = RepeatBehavior.Forever;
                    Pulsate.Begin();
                }

                //FacebookUser user = null;
                //string accessToken = "";

                try
                {

                    //accessToken = e.Uri.ToString().Substring(e.Uri.ToString().IndexOf("access_token=") + "access_token=".Length);
                    //accessToken = _accessToken;

                      string queryParams = e.Uri.Query;

                      if (queryParams.Length > 0)
                      {
                          QueryString qs = new QueryString(queryParams);

                          if (qs["oauth_verifier"] != null)
                          {
                              this._oAuthVerifier = qs["oauth_verifier"];
                          }
                      }


                    //if (accessToken.IndexOf("&") > 0)
                    //    accessToken = accessToken.Substring(0, accessToken.IndexOf("&"));

                    //user = await FacebookUserResponse.GetFacebookUser(accessToken);
                    await this.GetAccessToken();

                }
                catch { }


                if (_oAuthVerifier == "")
                {
                    if (Pulsate.GetCurrentState() == ClockState.Active)
                    {
                        Pulsate.Stop();
                    }
                    await webBrowser.ClearCookiesAsync();
                    WebViewBorder.Visibility = System.Windows.Visibility.Collapsed;
                    LoginButtonBorder.Visibility = System.Windows.Visibility.Visible;
                    LoggedInPanel.Visibility = System.Windows.Visibility.Collapsed;
                    MessageBox.Show("Unable to login using Linkedin.  Please try again.");
                }

                //if (user == null)
                //{
                //    if (Pulsate.GetCurrentState() == ClockState.Active)
                //    {
                //        Pulsate.Stop();
                //    }
                //    await webBrowser.ClearCookiesAsync();
                //    WebViewBorder.Visibility = System.Windows.Visibility.Collapsed;
                //    LoginButtonBorder.Visibility = System.Windows.Visibility.Visible;
                //    LoggedInPanel.Visibility = System.Windows.Visibility.Collapsed;
                //    MessageBox.Show("Unable to login using Facebook.  Please try again.");
                //}
                else 
                { 
                  

                    await Authenticate(_accessToken, _requestToken);

                }
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (webBrowser.Visibility == System.Windows.Visibility.Visible)
            {
                LoginButtonBorder.Visibility = System.Windows.Visibility.Visible;
                webBrowser.Visibility = System.Windows.Visibility.Collapsed;
                e.Cancel = true;
            }

            base.OnBackKeyPress(e);
        }

        #region LinkedIn auth

        private async void RequestLinkedInApi(string url)
        {
            string nonce = oAuthUtil.GetNonce();
            string timeStamp = oAuthUtil.GetTimeStamp();

            try
            {
                System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
                httpClient.MaxResponseContentBufferSize = int.MaxValue;
                httpClient.DefaultRequestHeaders.ExpectContinue = false;
                System.Net.Http.HttpRequestMessage requestMsg = new System.Net.Http.HttpRequestMessage();
                requestMsg.Method = new System.Net.Http.HttpMethod("GET");
                requestMsg.RequestUri = new Uri(url, UriKind.Absolute);

                string sigBaseStringParams = "oauth_consumer_key=" + _consumerKey;
                sigBaseStringParams += "&" + "oauth_nonce=" + nonce;
                sigBaseStringParams += "&" + "oauth_signature_method=" + "HMAC-SHA1";
                sigBaseStringParams += "&" + "oauth_timestamp=" + timeStamp;
                sigBaseStringParams += "&" + "oauth_token=" + _accessToken;
                sigBaseStringParams += "&" + "oauth_verifier=" + _oAuthVerifier;
                sigBaseStringParams += "&" + "oauth_version=1.0";
                string sigBaseString = "GET&";
                sigBaseString += Uri.EscapeDataString(url) + "&" + Uri.EscapeDataString(sigBaseStringParams);

                // LinkedIn requires both consumer secret and request token secret
                string signature = oAuthUtil.GetSignature(sigBaseString, _consumerSecretKey, _accessTokenSecretKey);

                string data = "realm=\"http://api.linkedin.com/\", oauth_consumer_key=\"" + _consumerKey
                              +
                              "\", oauth_token=\"" + _accessToken +
                              "\", oauth_verifier=\"" + _oAuthVerifier +
                              "\", oauth_nonce=\"" + nonce +
                              "\", oauth_signature_method=\"HMAC-SHA1\", oauth_timestamp=\"" + timeStamp +
                              "\", oauth_version=\"1.0\", oauth_signature=\"" + Uri.EscapeDataString(signature) + "\"";

                requestMsg.Headers.Authorization = new AuthenticationHeaderValue("OAuth", data);
                var response = await httpClient.SendAsync(requestMsg);

                var text = response.Content.ReadAsStringAsync();

                _linkedInProfile = await text;

            }
            catch (Exception Err)
            {
                throw;
            }
        }
        
        public string UrlEncode(string value)
        {
            StringBuilder result = new StringBuilder();
            string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

            foreach (char symbol in value)
            {
                if (unreservedChars.IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else
                {
                    result.Append('%' + String.Format("{0:X2}", (int)symbol));
                }
            }

            return result.ToString();
        }

        private async System.Threading.Tasks.Task GetRequestToken()
        {
            string nonce = oAuthUtil.GetNonce();
            string timeStamp = oAuthUtil.GetTimeStamp();

            string sigBaseStringParams = "oauth_callback=" + this.UrlEncode(callback);
            sigBaseStringParams += "&" + "oauth_consumer_key=" + _consumerKey;
            sigBaseStringParams += "&" + "oauth_nonce=" + nonce;
            sigBaseStringParams += "&" + "oauth_signature_method=" + "HMAC-SHA1";
            sigBaseStringParams += "&" + "oauth_timestamp=" + timeStamp;

            sigBaseStringParams += "&" + "oauth_version=1.0";

            string sigBaseString = "POST&";
            sigBaseString += Uri.EscapeDataString(_linkedInRequestTokenUrl) + "&" + Uri.EscapeDataString(sigBaseStringParams);

            string signature = oAuthUtil.GetSignature(sigBaseString, _consumerSecretKey);

            var responseText = await oAuthUtil.PostData(_linkedInRequestTokenUrl, sigBaseStringParams + "&oauth_signature=" + Uri.EscapeDataString(signature));

            if (!string.IsNullOrEmpty(responseText))
            {
                string oauth_token = null;
                string oauth_token_secret = null;
                string oauth_authorize_url = null;
                string[] keyValPairs = responseText.Split('&');

                for (int i = 0; i < keyValPairs.Length; i++)
                {
                    String[] splits = keyValPairs[i].Split('=');
                    switch (splits[0])
                    {
                        case "oauth_token":
                            oauth_token = splits[1];
                            break;
                        case "oauth_token_secret":
                            oauth_token_secret = splits[1];
                            break;
                        case "xoauth_request_auth_url":
                            oauth_authorize_url = splits[1];
                            break;
                    }
                }

                _requestToken = oauth_token;
                _requestTokenSecretKey = oauth_token_secret;
                _oAuthAuthorizeLink = Uri.UnescapeDataString(oauth_authorize_url + "?oauth_token=" + oauth_token);

                //// Step 2 : Call linkedin web page for authentication
                webBrowser.Navigate(new Uri(_oAuthAuthorizeLink));

            }
        }

        private async System.Threading.Tasks.Task GetAccessToken()
        {
            string nonce = oAuthUtil.GetNonce();
            string timeStamp = oAuthUtil.GetTimeStamp();

            string sigBaseStringParams = "oauth_consumer_key=" + _consumerKey;
            sigBaseStringParams += "&" + "oauth_nonce=" + nonce;
            sigBaseStringParams += "&" + "oauth_signature_method=" + "HMAC-SHA1";
            sigBaseStringParams += "&" + "oauth_timestamp=" + timeStamp;
            sigBaseStringParams += "&" + "oauth_token=" + _requestToken;
            sigBaseStringParams += "&" + "oauth_verifier=" + _oAuthVerifier;
            sigBaseStringParams += "&" + "oauth_version=1.0";

            string sigBaseString = "POST&";
            sigBaseString += Uri.EscapeDataString(_linkedInAccessTokenUrl) + "&" + Uri.EscapeDataString(sigBaseStringParams);

            // LinkedIn requires both consumer secret and request token secret
            string signature = oAuthUtil.GetSignature(sigBaseString, _consumerSecretKey, _requestTokenSecretKey);

            var responseText = await oAuthUtil.PostData(_linkedInAccessTokenUrl, sigBaseStringParams + "&oauth_signature=" + Uri.EscapeDataString(signature));

            if (!string.IsNullOrEmpty(responseText))
            {
                string oauth_token = null;
                string oauth_token_secret = null;
                string[] keyValPairs = responseText.Split('&');

                for (int i = 0; i < keyValPairs.Length; i++)
                {
                    String[] splits = keyValPairs[i].Split('=');
                    switch (splits[0])
                    {
                        case "oauth_token":
                            oauth_token = splits[1];
                            break;
                        case "oauth_token_secret":
                            oauth_token_secret = splits[1];
                            break;
                    }
                }

                _accessToken = oauth_token;
                _accessTokenSecretKey = oauth_token_secret;

                //if (oauth_token == null)
                //    rootPage.NotifyUser("Error getting accessToken", NotifyType.ErrorMessage);
                //else
                //    rootPage.NotifyUser("accessToken:" + oauth_token, NotifyType.StatusMessage);
            }
        }
        #endregion
    }
}