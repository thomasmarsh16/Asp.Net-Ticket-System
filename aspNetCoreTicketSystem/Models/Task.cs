using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace aspNetCoreTicketSystem.Models
{
    public class Task
    {
        [JsonProperty(PropertyName = "taskId")]
        public int TaskId { get; set; }

        [JsonProperty(PropertyName = "taskName")]
        public string TaskName { get; set; }

        [JsonProperty(PropertyName = "taskDescription")]
        public string TaskDescription { get; set; }

        [JsonProperty(PropertyName = "projectID")]
        public int ProjectID { get; set; }

        [JsonProperty(PropertyName = "completeTask")]
        public Boolean CompletedTask { get; set; }

        [JsonProperty(PropertyName = "taskStartDate")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [JsonProperty(PropertyName = "taskCompleteDate")]
        [DataType(DataType.Date)]
        public DateTime CompletionDate { get; set; }

    }
}