using System;
using System.ComponentModel.DataAnnotations;

namespace DotsApi.Models
{
    public class NoticeDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime TimeCompleted { get; set; }
    }
}
