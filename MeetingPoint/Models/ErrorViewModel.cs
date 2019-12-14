using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeetingPoint.Models
{
    [NotMapped]
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}