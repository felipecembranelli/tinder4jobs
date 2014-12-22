using System;

namespace TinderApp.Library
{
    public class LinkedInSessionInfo
    {
        public String LinkedInID { get; set; }

        public String AcessToken { get; set; }

        public Linkedin.LinkedinUser LinkedinUser { get; set; }
    }
}