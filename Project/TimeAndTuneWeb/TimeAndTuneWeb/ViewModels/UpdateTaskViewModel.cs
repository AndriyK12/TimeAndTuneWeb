﻿using System.ComponentModel.DataAnnotations;

namespace TimeAndTuneWeb.ViewModels
{
    public class UpdateTaskViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateOnly Date { get; set; }

        [Required]
        public int Priority { get; set; }

        public string Description { get; set; }
    }
}
