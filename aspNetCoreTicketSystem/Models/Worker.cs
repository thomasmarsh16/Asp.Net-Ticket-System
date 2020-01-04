using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace aspNetCoreTicketSystem.Models
{
    public class Worker
    {
        public int WorkerId { get; set; }
        public string WorkerName { get; set; }
        public string WorkerPosition { get; set; }
        public string WorkerPhoneNum { get; set; }
        public string WorkerEmail { get; set; }
    }
}
