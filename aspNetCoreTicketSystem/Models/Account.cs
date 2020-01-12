using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace aspNetCoreTicketSystem.Models
{
    public class Account
    {
        [JsonProperty(PropertyName = "id")]
        public int accountId { get; set; }

        [JsonProperty(PropertyName = "workerName")]
        public string accountName { get; set; }

        [JsonProperty(PropertyName = "workerPosition")]
        public string accountPosition { get; set; }

        [JsonProperty(PropertyName = "workerPhoneNum")]
        public string accountPhoneNum { get; set; }

        [JsonProperty(PropertyName = "workerEmail")]
        public string accountEmail { get; set; }
    }
}
