using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System.Collections;
using System.Globalization;

namespace aspNetCoreTicketSystem.Models
{
    public class ProjectTask
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "taskName")]
        public string taskName { get; set; }

        [JsonProperty(PropertyName = "Description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "Importance")]
        public string Importance { get; set; }

        [JsonProperty(PropertyName = "projectID")]
        public string ProjectID { get; set; }

        [JsonProperty(PropertyName = "completed")]
        public bool Completed { get; set; }

        [JsonProperty(PropertyName = "StartDate")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [JsonProperty(PropertyName = "dueDate")]
        [DataType(DataType.Date)]
        public DateTime dueDate { get; set; }

        [JsonProperty(PropertyName = "taskWorkers")]
        public List<string> taskWorkers { get; set; }

        [JsonProperty(PropertyName = "CompletionDate")]
        [DataType(DataType.Date)]
        public DateTime CompletionDate { get; set; }

        [JsonProperty(PropertyName = "checkoutName")]
        public string checkoutName { get; set; }
    }

    public class TaskMethods
    {
        public static List<ProjectTask> FilterTasksByWorkerEmail(List<ProjectTask> taskList, string email )
        {
            List<ProjectTask> filteredList = new List<ProjectTask>();

            foreach( ProjectTask task in taskList )
            {
                if ( task.taskWorkers.Contains(email))
                {
                    filteredList.Add(task);
                }
            }

            return filteredList;
        }

        public static List<int> CategorizeTasks(List<ProjectTask> taskList)
        {
            List<int> categoryNums = new List<int>() { 0, 0, 0 }; // low, medium, high categories

            foreach (ProjectTask task in taskList)
            {
                if (!task.Completed)
                {
                    switch (task.Importance)
                    {
                        case "Low":
                            categoryNums[0]++;
                            break;
                        case "Medium":
                            categoryNums[1]++;
                            break;
                        case "High":
                            categoryNums[2]++;
                            break;
                    }
                }
            }

            return categoryNums;
        }

        public static Dictionary<string, int> CountCompletionDatesByMonth(List<ProjectTask> taskList)
        {
            Dictionary<string, int> dateCounts = new Dictionary<string, int>();
            DateTime dateNow = DateTime.Today.AddMonths(1);

            foreach (var i in Enumerable.Range(0, 12)) // set range of months viewed to 12
            {
                int numSub = (12 - i) * -1;
                dateCounts.Add(dateNow.AddMonths(numSub).ToString("MMMM yyyy"), 0);
            }

            taskList.Sort((x, y) => DateTime.Compare(x.CompletionDate, y.CompletionDate));

            foreach (ProjectTask task in taskList)
            {
                string monthString = task.CompletionDate.ToString("MMMM yyyy");

                if (task.CompletionDate.ToShortDateString().Equals("1/1/0001")) // default for tasks that have not been completed
                {
                    // do not add to dictionary
                }
                else if (dateCounts.ContainsKey(monthString)) // Add only finished tasks for the last 12 months
                {
                    dateCounts[monthString]++;
                }
            }

            return dateCounts;
        }

        public static string FormatListForView(List<String> values)
        {
            string formattedString = "[\"";
            int count = 0;

            foreach (string value in values)
            {
                string temp = "Month: " + value;

                if ((count + 1) != values.Count())
                {
                    temp += "\",\"";
                }

                formattedString += temp;
                count++;
            }

            formattedString += "\"]";

            return formattedString;
        }

        public static string FormatListForView(List<int> values)
        {
            string formattedString = "[\"";
            int count = 0;

            foreach (int value in values)
            {
                string temp = value + "";

                if ((count + 1) != values.Count())
                {
                    temp += "\",\"";
                }

                formattedString += temp;
                count++;
            }

            formattedString += "\"]";

            return formattedString;
        }
    }
}