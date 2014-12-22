using Newtonsoft.Json;

namespace TinderApp.Library.Linkedin
{
    public class LinkedinUser
    {
        //[JsonProperty("id")]
        //public string Id { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("headline")]
        public string HeadLine { get; set; }
    }
}