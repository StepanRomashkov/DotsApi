﻿using System.ComponentModel.DataAnnotations;

namespace DotsApi.Models
{
    public class AuthenticateDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}