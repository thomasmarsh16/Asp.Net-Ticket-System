﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace aspNetCoreTicketSystem.Models
{
    
    public class Project
    {
        [JsonProperty(PropertyName = "projectId")]
        public int ProjectId { get; set; }

        [JsonProperty(PropertyName = "projectName")]
        public string ProjectName { get; set; }

        [JsonProperty(PropertyName = "projectDescription")]
        public string ProjectDescription { get; set; }

        [JsonProperty(PropertyName = "completedProj")]
        public Boolean CompletedProj { get; set; }

        [JsonProperty(PropertyName = "projectStartDate")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [JsonProperty(PropertyName = "projectCompleteDate")]
        [DataType(DataType.Date)]
        public DateTime CompletionDate { get; set; }
    }
}