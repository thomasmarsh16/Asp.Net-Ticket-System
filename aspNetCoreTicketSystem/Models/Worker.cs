using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace aspNetCoreTicketSystem.Models
{
    public class Worker
    {
        [JsonProperty(PropertyName = "workerID")]
        public int WorkerId { get; set; }

        [JsonProperty(PropertyName = "workerName")]
        public string WorkerName { get; set; }

        [JsonProperty(PropertyName = "workerPosition")]
        public string WorkerPosition { get; set; }

        [JsonProperty(PropertyName = "workerPhNum")]
        public string WorkerPhoneNum { get; set; }

        [JsonProperty(PropertyName = "workerEmail")]
        public string WorkerEmail { get; set; }
    }
}
