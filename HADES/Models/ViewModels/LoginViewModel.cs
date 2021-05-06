using System.ComponentModel.DataAnnotations;

namespace HADES.Models
{
	public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

    }
}
