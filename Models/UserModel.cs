using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Honeys_Kitchen_backend.Models;

public class User
{
    [JsonIgnore]
    public int UserId {get; set;}
    [Required]
    public string? Password {get; set;}
    [Required]
    public string? FirstName {get; set;}
    [Required]
    public string? LastName {get; set;}
    [Required]
    [EmailAddress]
    public string? Email {get; set;} //email address, phone #, and address will be unavailable for right now
    public string? PhoneNumber {get; set;}
    public string? Address {get; set;}

    public string? ProfilePicture{get; set;}
}