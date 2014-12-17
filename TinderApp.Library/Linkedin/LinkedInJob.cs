using Newtonsoft.Json;

namespace TinderApp.Library.Linkedin
{
    public class LinkedinJob
    {
        [JsonProperty("company")]
        public LinkedinCompany Company { get; set; }

        [JsonProperty("descriptionSnippet")]
        public string DescriptionSnippet { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("jobPoster")]
        public LinkedinJobPoster JobPoster { get; set; }

        [JsonProperty("locationDescription")]
        public string LocationDescription { get; set; }
    }

    public class LinkedinCompany
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

    }

    public class LinkedinJobPoster
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("headline")]
        public string Headline { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }
    }

    public class JobList
    {
        [JsonProperty("_total")]
        public string Total { get; set; }

        [JsonProperty("values")]
        public LinkedinJob[] Values { get; set; }

    }

    public class Facets
    {
        [JsonProperty("_total")]
        public string Total { get; set; }
    }

    public class LinkedinJobList
    {
        [JsonProperty("facets")]
        public Facets Facets { get; set; }

        [JsonProperty("jobs")]
        public JobList Jobs { get; set; }

        [JsonProperty("numResults")]
        public string NumResults { get; set; }



    }
}