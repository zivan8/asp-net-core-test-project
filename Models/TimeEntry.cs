using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test.Models
{
    public class TimeEntry
    {
        [JsonProperty(PropertyName = "Id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "EmployeeName")]
        public string EmployeeName { get; set; }

        [JsonProperty(PropertyName = "StarTimeUtc")]
        public DateTime StartTime { get; set; }

        [JsonProperty(PropertyName = "EndTimeUtc")]
        public DateTime EndTime { get; set; }

        [JsonProperty(PropertyName = "EntryNotes")]
        public string EntryNotes { get; set; }

        [JsonProperty(PropertyName = "DeletedOn")]
        public DateTime? DeleteTime { get; set; }

        public TimeSpan TotalTime
        {
            get
            {
                return EndTime.Subtract(StartTime); 
            }
        }
    }
}
