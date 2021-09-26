using System;
using System.ComponentModel.DataAnnotations;

namespace TestingThings.Configuration
{
    public class SomeConfig
    {
        public static readonly string SectionName = "SomeConfig";

        [Required]
        public Uri SomeUri { get; set; }
    }
}
