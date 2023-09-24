using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public class Student
    {
        public int ID { get; set; }
        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[A-Z]+[a-zA-Z]*$")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[A-Z]+[a-zA-Z]*$")]
        [Column("FirstName")]
        [Display(Name = "First Name")]
        public string FirstMidName { get; set; }

        // The DataType attribute is used to specify a data type that's more specific than the database intrinsic type.
        // In this case we only want to keep track of the date, not the date and time.
        [DataType(DataType.Date)]
        // The DisplayFormat attribute is used to explicitly specify the date format.
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Enrollment Date")]
        public DateTime EnrollmentDate { get; set; }

        [Display(Name = "Full Name")]
        // FullName is a calculated property that returns a value that's created by concatenating two other properties.
        // Therefore it has only a get accessor, and no FullName column will be generated in the database.
        public string FullName
        {
            get
            {
                return LastName + ", " + FirstMidName;
            }
        }
        // The Display attribute specifies that the caption for the text boxes should be
        // "First Name", "Last Name", "Full Name", and "Enrollment Date" instead of the
        // property name in each instance (which has no space dividing the words).

        public ICollection<Enrollment> Enrollments { get; set; }
    }
}