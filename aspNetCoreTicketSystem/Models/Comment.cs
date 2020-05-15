using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System.Collections;

namespace aspNetCoreTicketSystem.Models
{
    public class Comment
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "CommentString")]
        public string CommentString { get; set; }

        [JsonProperty(PropertyName = "CreatorName")]
        public string CreatorName { get; set; }

        [JsonProperty(PropertyName = "TaskID")]
        public string TaskID { get; set; }

        [JsonProperty(PropertyName = "DateTimeCreated")]
        [DataType(DataType.DateTime)]
        public DateTime DateTimeCreated { get; set; }
    }
}