using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SeminarApp.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Course title is required!")]
        [StringLength(200,
            ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        [Display(Name = "Course Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required!")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Course start date is required!")]
        [DataType(DataType.Date)]
        [Display(Name = "Course start date")]
        public DateTime StartDate { get; set; }

        public bool IsFull { get; set; }


        public virtual ICollection<Enrollment> Enrollments { get; set; }

    }
}