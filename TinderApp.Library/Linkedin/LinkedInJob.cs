using Newtonsoft.Json;

namespace TinderApp.Library.Facebook
{
    // RETURNED JSON

    // "company": {
    //  "id": 1288,
    //  "name": "Yahoo!"
    //},
    //"descriptionSnippet": "Senior Specialist, Display Supply Ops &amp; ManagementThe Supply/Partner Ops team is directly responsible for general acquisition and operation of 3P Supply. The team is responsible for identifying and sourcing the inventory that helps fuel the Yahoo Audience proposition in Brazil. The team is also responsible to bring and manage the large majority of our search supply and to bring video supply to",
    //"id": 19148244,
    //"jobPoster": {
    //  "firstName": "Emily",
    //  "headline": "Talent Aquisition Program Specialist at Yahoo",
    //  "id": "fv3jf4cuhB",
    //  "lastName": "C."
    //},
    //"locationDescription": "Brazil-Sao Paulo-Sao Paulo"

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
        public System.Collections.Generic.List<LinkedinJob> Values { get; set; }

    }

    public class LinkedinJobList
    {
        [JsonProperty("facets")]
        public string Facets { get; set; }

        [JsonProperty("jobs")]
        public System.Collections.Generic.List<JobList> Jobs { get; set; }

        [JsonProperty("numResults")]
        public string NumResults { get; set; }


        
    }
}