using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InstructorInformationSystem.Models;

public class Instructor
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Required(ErrorMessage = "Instructor ID is required.")]
    [RegularExpression(@"^(22|23|24)-\d{2,4}$", ErrorMessage = "Use the format 24-01, 24-003, or 24-0001 (prefix 22, 23, or 24, dash, then 2 to 4 digits).")]
    [StringLength(7)]
    [Display(Name = "Instructor ID")]
    public string InstructorId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(150)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Age is required.")]
    [Range(1, 120, ErrorMessage = "Age must be between 1 and 120.")]
    public int? Age { get; set; }

    [Required(ErrorMessage = "Address is required.")]
    [StringLength(250)]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "Designation is required.")]
    [StringLength(100)]
    public string Designation { get; set; } = string.Empty;

    [Required(ErrorMessage = "Gender is required.")]
    [StringLength(30)]
    public string Gender { get; set; } = string.Empty;

    [Required(ErrorMessage = "Contact number is required.")]
    [Phone(ErrorMessage = "Enter a valid contact number.")]
    [StringLength(20)]
    [Display(Name = "Contact Number")]
    public string ContactNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Majorship is required.")]
    [StringLength(100)]
    public string Majorship { get; set; } = string.Empty;

    [Required(ErrorMessage = "Bachelor degree is required.")]
    [StringLength(150)]
    [Display(Name = "Bachelor Degree")]
    public string BachelorDegree { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-mail address is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid e-mail address.")]
    [StringLength(254)]
    [Display(Name = "E-mail Address")]
    public string EmailAddress { get; set; } = string.Empty;
}
