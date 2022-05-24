using System;
using System.ComponentModel.DataAnnotations;

namespace capstoneDotNet
{
	public class RegisterUserDto
	{
		[Required]
		public string firstName { get; set; }
		[Required]
		public string lastName { get; set; }
		[Required]
		[EmailAddress]
		public string email { get; set; }
		[Required]
		public string password { get; set; }

	}
}

