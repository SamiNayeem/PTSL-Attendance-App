using System;
using System.ComponentModel.DataAnnotations;

namespace YourNamespace.Models
{
    public class Otp
    {
        [Key]
        public long Id { get; set; }

        public required string PhoneNumber { get; set; }

        public required string OTP { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}