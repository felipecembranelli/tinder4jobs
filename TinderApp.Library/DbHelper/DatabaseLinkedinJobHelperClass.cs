using SQLite;
using TinderApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinderApp.Library.Linkedin;

namespace TinderApp.DbHelper
{
    //This class for perform all database CRUID operations
    public class DatabaseLinkedinJobHelperClass
    {
        public static string DB_PATH = Path.Combine(Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, DB_FILE_NAME));//DataBase Name
        private const string DB_FILE_NAME = "Tinder4Jobs.sqlite";
        
        SQLiteConnection dbConn;
       
        //Create Tabble
        public async Task<bool> onCreate(string DB_PATH)
        {
            try
            {

                if (!CheckFileExists(DB_PATH).Result)
                {
                    using (dbConn = new SQLiteConnection(DB_PATH))
                    {
                        dbConn.CreateTable<LinkedinJob>();
                    }
                } 
                return true;
            }
            catch
            {
                return false;
            }
        }
        private async Task<bool> CheckFileExists(string fileName)
        {
            try
            {
                var store = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Retrieve the specific Job from the database.
        public Tb_LinkedinJob ReadJob(string jobId)
        {
            using (var dbConn = new SQLiteConnection(DB_PATH))
            {
                var existingJob = dbConn.Query<Tb_LinkedinJob>("select * from Tb_LinkedinJob where jobId ='" + jobId + "'").FirstOrDefault();
                return existingJob;
            }
        }
        // Retrieve the all Job list from the database.
        public ObservableCollection<LinkedinJob> ReadLinkedinJob()
        {
            using (var dbConn = new SQLiteConnection(DB_PATH))
            {
                List<LinkedinJob> myCollection  = dbConn.Table<LinkedinJob>().ToList<LinkedinJob>();
                ObservableCollection<LinkedinJob> LinkedinJobList = new ObservableCollection<LinkedinJob>(myCollection);
                return LinkedinJobList;
            }
        }
        
        //Update existing conatct
        public void UpdateJob(LinkedinJob job)
        {
            using (var dbConn = new SQLiteConnection(DB_PATH))
            {
                var existingJob = dbConn.Query<Tb_LinkedinJob>("select * from Tb_LinkedinJob where jobId =" + job.Id).FirstOrDefault();
                if (existingJob != null)
                {
                    existingJob.CompanyId = job.Company.Id;
                    existingJob.CompanyName = job.Company.Name;
                    existingJob.DescriptionSnippet = job.DescriptionSnippet;
                    existingJob.JobId = job.Id;
                    existingJob.JobPosterFirstName= job.JobPoster.FirstName;
                    existingJob.LocationDescription = job.LocationDescription;

                    dbConn.RunInTransaction(() =>
                    {
                        dbConn.Update(existingJob);
                    });
                }
            }
        }
        // Insert the new Job in the LinkedinJob table.
        public void Insert(LinkedinJob newJob)
        {
            Tb_LinkedinJob j = new Tb_LinkedinJob();

            j.CompanyId = newJob.Company.Id;
            j.CompanyName = newJob.Company.Name;
            j.DescriptionSnippet = newJob.DescriptionSnippet;
            j.JobId = newJob.Id;
            j.JobPosterFirstName = newJob.JobPoster.FirstName;
            j.LocationDescription = newJob.LocationDescription;

            using (var dbConn = new SQLiteConnection(DB_PATH))
            {
                dbConn.RunInTransaction(() =>
                    {
                        dbConn.Insert(j);
                    });
            }
        }
       
        //Delete specific Job
        public void DeleteJob(int Id)
        {
            using (var dbConn = new SQLiteConnection(DB_PATH))
            {
                var existingJob = dbConn.Query<LinkedinJob>("select * from Tb_LinkedinJob where Id =" + Id).FirstOrDefault();
                if (existingJob != null)
                {
                    dbConn.RunInTransaction(() =>
                    {
                        dbConn.Delete(existingJob);
                    });
                }
            }
        }
        //Delete all Joblist or delete LinkedinJob table
        public void DeleteAllJob()
        {
            using (var dbConn = new SQLiteConnection(DB_PATH))
            {
                //dbConn.RunInTransaction(() =>
                //   {
                       dbConn.DropTable<LinkedinJob>();
                       dbConn.CreateTable<LinkedinJob>();
                       dbConn.Dispose();
                       dbConn.Close();
                   //}); 
            }
        }
    }
}
