using System;
using System.ComponentModel.DataAnnotations;

namespace DotsApi.Models
{
    public class AddNoticeDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime TimeCompleted { get; set; }
    }
}
