using System;
using System.Windows.Input;
using System.Windows.Media;
using TinderApp.Lib;
using TinderApp.Lib.API;
using TinderApp.Library;
using TinderApp.Library.MVVM;
using TinderApp.Library.Linkedin;

namespace TinderApp.Views
{
    public class JobReccommendationsViewModel : ObservableObject
    {
        private static SolidColorBrush GRAY_BRUSH = new SolidColorBrush(Colors.LightGray);
        private static SolidColorBrush BLACK_BRUSH = new SolidColorBrush(Colors.Black);

        private readonly ICommand _likeUserCommand;

        private readonly ICommand _rejectUserCommand;

        private UserResult _currentRec;

        private LinkedinJob _currentJob;

        public JobReccommendationsViewModel()
        {
            _likeUserCommand = new RelayCommand(LikeUser);
            _rejectUserCommand = new RelayCommand(RejectUser);

            //&& TinderSession.CurrentSession.IsAuthenticated 

            if (TinderSession.CurrentSession != null 
                && TinderSession.CurrentSession.JobRecommendations.Count > 0)
            {
                _currentJob = TinderSession.CurrentSession.JobRecommendations.Pop();

                RaisePropertyChanged("Id");
                RaisePropertyChanged("Name");
                RaisePropertyChanged("DescriptionSnippet");
                RaisePropertyChanged("LocationDescription");
                //RaisePropertyChanged("LikeCount");
                //RaisePropertyChanged("LikesBrush");
                //RaisePropertyChanged("FriendsBrush");
                //RaisePropertyChanged("PhotosBrush");
                //RaisePropertyChanged("ProfilePhoto");
                RaisePropertyChanged("CurrentJobReccomendation");
                RaisePropertyChanged("JobCount");

                  //[JsonProperty("company")]
                    //[JsonProperty("descriptionSnippet")]
                    //[JsonProperty("id")]
                    //[JsonProperty("jobPoster")]
                    //[JsonProperty("locationDescription")]
        
            }
        }

        public event EventHandler<AnimationEventArgs> OnAnimation;

        public event EventHandler OnMatch;

        public String DescriptionSnippet
        {
            get
            {
                if (_currentJob == null)
                    return "No description found";
                return _currentJob.DescriptionSnippet;
            }
        }

        public String LocationDescription
        {
            get
            {
                if (_currentJob == null)
                    return "No location found";
                return _currentJob.LocationDescription;
            }
        }

        public String Age
        {
            get
            {
                if (_currentRec == null)
                    return "Try Later";
                return String.Format("{0:N0}", Math.Floor(DateTime.UtcNow.Subtract(DateTime.Parse(_currentRec.BirthDate)).TotalDays / 365));
            }
        }

        public LinkedinJob CurrentJobReccomendation
        {
            get { return _currentJob; }
        }

        public Int32 JobCount
        {
            get
            {
                if (TinderSession.CurrentSession.JobRecommendations.Count == null)
                    return 0;
                return TinderSession.CurrentSession.JobRecommendations.Count;
            }
        }

        public Int32 LikeCount
        {
            get
            {
                if (_currentRec == null)
                    return 0;
                return _currentRec.CommonLikeCount;
            }
        }

        public SolidColorBrush FriendsBrush
        {
            get
            {
                return JobCount > 0 ? BLACK_BRUSH : GRAY_BRUSH;
            }
        }
        public SolidColorBrush LikesBrush
        {
            get
            {
                return JobCount > 0 ? BLACK_BRUSH : GRAY_BRUSH;
            }
        }
        public SolidColorBrush PhotosBrush
        {
            get
            {
                return PhotoCount > 0 ? BLACK_BRUSH : GRAY_BRUSH;
            }
        }

        public ICommand LikeUserCommand
        {
            get { return _likeUserCommand; }
        }

        public String Name
        {
            get
            {
                if (_currentJob == null)
                    return "No job found";
                return _currentJob.Company.Name;
            }
        }

        public String Id
        {
            get
            {
                if (_currentJob == null)
                    return "No job found";
                return _currentJob.Id;
            }
        }

        public Int32 PhotoCount
        {
            get
            {
                if (_currentRec == null)
                    return 0;
                return _currentRec.Photos.Length;
            }
        }

        public Uri ProfilePhoto
        {
            get
            {
                if (_currentRec == null)
                    return null;
                return Utils.GetMainPhoto(_currentRec.Photos);
            }
        }

        public ICommand RejectUserCommand
        {
            get { return _rejectUserCommand; }
        }

        public async void LikeUser()
        {
            RaiseAnimation("Like");

            //TODO - DESABILITEI
            //LikeResponse response = await Client.Get<LikeResponse>("like/" + _currentRec.Id);
            //if (response.Match)
            //{
            //    RaiseOnMatch();
            //}
            //else
            //{
                NextJobSuggestion();
            //}
        }

        //public async void NextRecommendation()
        //{
        //    if (TinderSession.CurrentSession.Recommendations.Count > 0)
        //        _currentRec = TinderSession.CurrentSession.Recommendations.Pop();
        //    else
        //        _currentRec = null;
        //    RaisePropertyChanged("PhotoCount");
        //    RaisePropertyChanged("Name");
        //    RaisePropertyChanged("Age");
        //    RaisePropertyChanged("FriendCount");
        //    RaisePropertyChanged("LikeCount");
        //    RaisePropertyChanged("ProfilePhoto");
        //    RaisePropertyChanged("CurrentReccomendation");
        //    RaisePropertyChanged("LikesBrush");
        //    RaisePropertyChanged("FriendsBrush");
        //    RaisePropertyChanged("PhotosBrush");

        //    if (TinderSession.CurrentSession.Recommendations.Count == 0)
        //        await TinderSession.CurrentSession.GetRecommendations();
        //}

        public async void NextJobSuggestion()
        {
            if (TinderSession.CurrentSession.JobRecommendations.Count > 0)
                _currentJob = TinderSession.CurrentSession.JobRecommendations.Pop();
            else
                _currentJob = null;

            RaisePropertyChanged("Id");
            RaisePropertyChanged("Name");
            RaisePropertyChanged("DescriptionSnippet");
            RaisePropertyChanged("LocationDescription");
            //RaisePropertyChanged("LikeCount");
            //RaisePropertyChanged("ProfilePhoto");
            RaisePropertyChanged("CurrentJobReccomendation");
            //RaisePropertyChanged("LikesBrush");
            //RaisePropertyChanged("FriendsBrush");
            //RaisePropertyChanged("PhotosBrush");

            //if (TinderSession.CurrentSession.Recommendations.Count == 0)
            //    await TinderSession.CurrentSession.GetRecommendations();
        }

        public async void RejectUser()
        {
            RaiseAnimation("Pass");

            //TODO - DESABILITEI
            //await Client.Get("pass/" + _currentRec.Id);
            NextJobSuggestion();
        }

        private void RaiseAnimation(string animation)
        {
            if (OnAnimation != null)
                OnAnimation(this, new AnimationEventArgs(animation));
        }

        private void RaiseOnMatch()
        {
            if (OnMatch != null)
                OnMatch(this, new EventArgs());
        }

        public class AnimationEventArgs : EventArgs
        {
            public AnimationEventArgs(string animation)
            {
                AnimationName = animation;
            }

            public String AnimationName { get; set; }
        }
    }
}