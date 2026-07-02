using System.ComponentModel.DataAnnotations;

namespace WorkoutTrackerApi.Application;

public class RegisterUserDto {
    [Required]
    [StringLength(20, MinimumLength = 2)]
    public string Name {get; set;}

    [Required]
    [EmailAddress]
    public string Email {get; set;}

    [Required]
    [MinLength(6)]
    public string Password {get; set;}
    
    [Required]
    [Compare("Password", ErrorMessage = "Password does not match.")]
    public string ConfirmPassword {get; set;}
}