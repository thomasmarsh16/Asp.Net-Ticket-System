using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System.Collections;

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

        public static List<int> categorizeTasks(List<ProjectTask> taskList)
        {
            List<int> categoryNums = new List<int>() { 0, 0, 0 }; // low, medium, high categories

            foreach (ProjectTask task in taskList)
            {
                if ( !task.Completed )
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

        public static Dictionary<string,int> countCompletionDates( List<ProjectTask> taskList )
        {
            Dictionary<string, int> dateCounts = new Dictionary<string, int>();

            taskList.Sort(delegate (ProjectTask x, ProjectTask y) 
            { 
                if( x.CompletionDate < y.CompletionDate)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            });

            foreach (ProjectTask task in taskList)
            {
                if( dateCounts.ContainsKey( task.CompletionDate.ToShortDateString() ) )
                {
                    dateCounts[task.CompletionDate.ToShortDateString()]++;
                }
                else if( task.CompletionDate.ToShortDateString().Equals("1/1/0001")) // default for tasks that have not been completed
                {
                    
                }
                else
                {
                    dateCounts.Add(task.CompletionDate.ToShortDateString(), 1);
                }
            }

            return dateCounts;
        }

        public static string formatListForView( List<String> values)
        {
            string formattedString = "[\"";
            int count = 0;

            foreach ( string value in values )
            {
                string temp = value;

                if ( (count + 1) != values.Count())
                {
                    temp += "\",\"";
                }

                formattedString += temp;
                count++;
            }

            formattedString += "\"]";

            return formattedString;
        }

        public static string formatListForView(List<int> values)
        {
            string formattedString = "[\"";
            int count = 0;

            foreach (int value in values)
            {
                string temp = value +"";

                if ( (count + 1) != values.Count() )
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