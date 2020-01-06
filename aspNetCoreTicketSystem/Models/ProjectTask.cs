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

        [JsonProperty(PropertyName = "projectName")]
        public string ProjectName { get; set; }

        [JsonProperty(PropertyName = "completed")]
        public bool Completed { get; set; }

        [JsonProperty(PropertyName = "StartDate")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [JsonProperty(PropertyName = "CompletionDate")]
        [DataType(DataType.Date)]
        public DateTime CompletionDate { get; set; }

    }
}