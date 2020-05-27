using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SeminarApp.Models
{

    public class Enrollment
    {
        [Key]
        public int EnrollmentId { get; set; }

        [DataType(DataType.Date,
            ErrorMessage = "Date of Enrollment is required!")]
        [Display(Name = "Date of Enrollment")]
        public DateTime EnrollmentDate { get; set; }

        [Required(ErrorMessage = "Name is required! Please enter your Name.")]
        [StringLength(100,
            ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required! Please enter your Last name.")]
        [StringLength(100,
            ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Address is required! Please enter your Address.")]
        [StringLength(200,
            ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]        
        public string Address { get; set; }

        [Required(ErrorMessage = "Email is required! Please enter your Email address.")]
        [StringLength(100,
            ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        //[EmailAddress(ErrorMessage = "Email address is not in the right format(name@mail.com)")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required! Please enter your phone number.")]
        [StringLength(50,
            ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        public EnrollmentStatus? Status { get; set; }

        [NotMapped] //za enum radio button - treba doraditi
        public IEnumerable<EnrollmentStatus> AllStatus { set; get; }

        public int CourseId { get; set; }

        public virtual Course Course { get; set; }
      

        public Enrollment()
        {
            EnrollmentDate = DateTime.Now;
        }

    }
}
