﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGSWeb.Models
{
    public class EnrollmentBindingModel
    {
        [Required]
        public string StudentId { get; set; }
        [Required]
        public int ClassId { get; set; }
    }
}