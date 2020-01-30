using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

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
}