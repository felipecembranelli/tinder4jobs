﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;
using System.Windows.Threading;
using TinderApp.Lib;
using TinderApp.Library.ViewModels;
using TinderApp.Lib.API;
using System.Net.Http;
using TinderApp.Library.Controls;

namespace TinderApp.Library
{
    [DataContract]
    public class TinderSession
    {
        public TinderSession()
        {
        }

        private static TinderSession _currentSession = null;

        private readonly FacebookSessionInfo _fbSessionInfo;

        private readonly GeographicalCordinates _location;

        private Profile _currentProfile;

        private User _currentUser = null;

        private Globals _globalInfo;

        private volatile bool _isUpdating = false;

        private DateTime? _lastActivity = new DateTime?();

        private MatchesViewModel _matches = new MatchesViewModel();

        private Stack<UserResult> _recommendations = new Stack<UserResult>();

        private DispatcherTimer _updateTimer;

        private TinderSession(FacebookSessionInfo fbSession, GeographicalCordinates location)
        {
            _fbSessionInfo = fbSession;
            _location = location;
        }

        public static TinderSession CurrentSession
        {
            get
            {
                if (_currentSession == null)
                    return null;
                return _currentSession;
            }
        }

        public Profile CurrentProfile
        {
            get { return _currentProfile; }
        }

        public User CurrentUser
        {
            get { return _currentUser; }
            set
            {
                _currentUser = value;
            }
        }

        public FacebookSessionInfo FbSessionInfo
        {
            get { return _fbSessionInfo; }
        }

        public Globals GlobalInfo
        {
            get { return _globalInfo; }
        }

        public Boolean IsAuthenticated
        {
            get
            {
                return _currentUser != null && !String.IsNullOrEmpty(Client.AuthToken);
            }
        }

        public DateTime? LastActivity
        {
            get { return _lastActivity; }
            set { _lastActivity = value; }
        }

        public MatchesViewModel Matches
        {
            get { return _matches; }
            set { _matches = value; }
        }

        public Stack<UserResult> Recommendations
        {
            get { return _recommendations; }
        }

        public static TinderSession CreateNewSession(FacebookSessionInfo fbSession, GeographicalCordinates location)
        {
            _currentSession = new TinderSession(fbSession, location);

            return _currentSession;
        }

        public async Task<Boolean> Authenticate()
        {
            //AuthRequest request = new AuthRequest();
            //request.facebook_token = FbSessionInfo.FacebookToken;
            //request.facebook_id = FbSessionInfo.FacebookID;
            //AuthResponse response = await request.Send();
            //if (response.token.Length > 0)
            //{
            //    _currentUser = response.user;
            //    _globalInfo = response.globals;
            //    _currentProfile = await Profile.GetProfile();

            //    await PingWithLocation();
            //    await GetUpdate();
            //    await GetRecommendations();

            //    StartUpdatesTimer();

            //    (Application.Current as TinderApp.Library.Controls.IApp).RootFrameInstance.LoggedIn();

            //    return true;
            //}

            //return false;


            //////////////////////////////////////////////
            // MOCKUP RETURN
            //////////////////////////////////////////////


            // profile

            Photo photo = new Photo();
            photo.Id = "1";
            photo.Url = "https://fbcdn-profile-a.akamaihd.net/hprofile-ak-xaf1/v/t1.0-1/c25.0.150.150/1964880_10202962758367430_1563262680_n.jpg?oh=90bc225f00ebf8e02476d16c5a3dac63&oe=54F9869C&__gda__=1425964246_a53df85d8f79bef180101254cabeef57";
            
            _currentUser = new User() { FullName = "Felipe Cembranelli", Id = "1", Gender = 1 };
            //_globalInfo = response.globals;
            _currentProfile = new Profile() { name = "Felipe Cembranelli", ID = "1", interested_in = new List<int>() { { 0 }, { 1 } } };
            Photo photo2 = new Photo();
            photo2.Url = "https://fbcdn-profile-a.akamaihd.net/hprofile-ak-xaf1/v/t1.0-1/c25.0.150.150/1964880_10202962758367430_1563262680_n.jpg?oh=90bc225f00ebf8e02476d16c5a3dac63&oe=54F9869C&__gda__=1425964246_a53df85d8f79bef180101254cabeef57";
            List<Photo> photoList = new List<Photo>();
            photoList.Add(photo2);

            _currentProfile.photos = photoList;



            // matches

            Match[] matches = new Match[1];

            matches[0] = new Match();
            matches[0].Id = "1";
            matches[0] = new Match();
            Person p1 = new Person() { Id = "100", Name = "Fulano" };
            p1.Photos = new Photo[1] {photo};
            matches[0].Person = p1;
            Msg msg = new Msg() { Id = "1", Message = "teste" };
            matches[0].Messages = new Msg[1] {msg};


            _matches.Update(matches);

            // recomendations

            UserResult userResult1 = new UserResult()
            {
                Id = "200",
                Name = "rec1",
                BirthDate = "10/01/1973",
                PingTime = "10/01/1973",
                Bio = "JSAFKADSJFKLDASJFLKDSJAFKSJDFKJDSAFDSKLFJDSKLA"
            };
            userResult1.Photos = new Photo[1] { photo };

            UserResult userResult2 = new UserResult()
            {
                Id = "300",
                Name = "rec2",
                BirthDate = "10/01/1973",
                PingTime = "10/01/1973",
                Bio = "JSAFKADSJFKLDASJFLKDSJAFKSJDFKJDSAFDSKLFJDSKLA"
            };
            userResult2.Photos = new Photo[1] { photo };

            UserResult userResult3 = new UserResult()
            {
                Id = "400",
                Name = "rec3",
                BirthDate = "10/01/1973",
                PingTime = "10/01/1973",
                Bio = "JSAFKADSJFKLDASJFLKDSJAFKSJDFKJDSAFDSKLFJDSKLA"
            };
            userResult3.Photos = new Photo[1] { photo };

            Recommendations.Push(userResult1);
            Recommendations.Push(userResult2);
            Recommendations.Push(userResult3);

            //await GetRecommendations();

            //StartUpdatesTimer();

            (Application.Current as TinderApp.Library.Controls.IApp).RootFrameInstance.LoggedIn();

            // forcei
            Client.AuthToken = FbSessionInfo.FacebookToken;

            return true;
        }

        private void StartUpdatesTimer()
        {
            //todo - desabilitei
            //Deployment.Current.Dispatcher.BeginInvoke(() =>
            //{
            //    _updateTimer = new DispatcherTimer();
            //    _updateTimer.Interval = TimeSpan.FromMilliseconds(_globalInfo.UpdatesInterval);
            //    _updateTimer.Tick += _updateTimer_Tick;
            //    _updateTimer.Start();
            //});
        }

        public async Task GetRecommendations()
        {
            //TODO - MOCKADO

            Photo photo = new Photo();
            photo.Id = "1";
            photo.Url = "https://fbcdn-profile-a.akamaihd.net/hprofile-ak-xaf1/v/t1.0-1/c25.0.150.150/1964880_10202962758367430_1563262680_n.jpg?oh=90bc225f00ebf8e02476d16c5a3dac63&oe=54F9869C&__gda__=1425964246_a53df85d8f79bef180101254cabeef57";


            UserResult userResult1 = new UserResult() { Id = "200", 
                                                        Name = "rec1", 
                                                        BirthDate = "10/01/1973", 
                                                        PingTime = "10/01/1973",
                                                        Bio= "JSAFKADSJFKLDASJFLKDSJAFKSJDFKJDSAFDSKLFJDSKLA"};
            userResult1.Photos = new Photo[1] { photo };

            UserResult userResult2 = new UserResult()
            {
                Id = "300",
                Name = "rec2",
                BirthDate = "10/01/1973",
                PingTime = "10/01/1973",
                Bio = "JSAFKADSJFKLDASJFLKDSJAFKSJDFKJDSAFDSKLFJDSKLA"
            };
            userResult2.Photos = new Photo[1] { photo };

            UserResult userResult3 = new UserResult()
            {
                Id = "400",
                Name = "rec3",
                BirthDate = "10/01/1973",
                PingTime = "10/01/1973",
                Bio = "JSAFKADSJFKLDASJFLKDSJAFKSJDFKJDSAFDSKLFJDSKLA"
            };
            userResult3.Photos = new Photo[1] { photo };

            Recommendations.Push(userResult1);
            Recommendations.Push(userResult2);
            Recommendations.Push(userResult3);

            //ReccommendationsRequest response = await ReccommendationsRequest.GetRecommendations();
            //if (response.Status == 200)
            //{
            //    if (Recommendations.Count == 0)
            //        foreach (var rec in response.Results)
            //            Recommendations.Push(rec);
            //}
        }

        public async Task GetUpdate()
        {
            if (_isUpdating)
                return;

            try
            {
                _isUpdating = true;

                UpdatesRequest request = new UpdatesRequest();
                request.last_activity_date = LastActivity;
                UpdatesResponse response = await request.GetUpdate();

                if (response.Matches != null && response.Matches.Length > 0)
                {
                    _matches.Update(response.Matches);
                }

                LastActivity = DateTime.Parse(response.LastActivityDate);
            }
            catch (HttpRequestException e)
            {
                if (e.Message.Contains("Unauthorized"))
                {
                    (Application.Current as IApp).Logout();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception during update: " + ex.Message);
            }
            finally
            {
                _isUpdating = false;
            }
        }

        public void Logout()
        {
            _updateTimer.Stop();
            _currentSession = null;
        }

        private async void _updateTimer_Tick(object sender, EventArgs e)
        {
            await GetUpdate();
        }

        private async Task PingWithLocation()
        {
            PingRequest ping = new PingRequest();
            ping.lat = _location.Latitude;
            ping.lon = _location.Longitude;
            await ping.Ping();
        }

        public TombstoneData ToTombstoneData()
        {
            return new TombstoneData()
            {
                 AuthToken = Client.AuthToken,
                 CurrentGlobals = _globalInfo,
                 CurrentProfile = _currentProfile,
                 CurrentUser = _currentUser,
                 FBSession = _fbSessionInfo,
                 LastActivity = _lastActivity,
                 Location = _location,
                 Matches = new List<Match>(Matches.Matches.Select(a=>a.Data)),
                 Recommendations = _recommendations.ToList()
            };
        }

        public static TinderSession FromTombstoneData(TombstoneData data)
        {
            Client.AuthToken = data.AuthToken;
            _currentSession = new TinderSession(data.FBSession, data.Location);
            _currentSession._currentProfile = data.CurrentProfile;
            _currentSession._currentUser = data.CurrentUser;
            _currentSession._globalInfo = data.CurrentGlobals;
            _currentSession._lastActivity = data.LastActivity;
            _currentSession._matches = new MatchesViewModel(data.Matches);
            _currentSession._recommendations = new Stack<UserResult>(data.Recommendations);
            _currentSession.StartUpdatesTimer();

            (Application.Current as TinderApp.Library.Controls.IApp).RootFrameInstance.LoggedIn();

            return _currentSession;
        }
    }
}