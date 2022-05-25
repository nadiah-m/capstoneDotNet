using System;
using capstoneDotNet.Models;

namespace capstoneDotNet.Interfaces
{
	public interface ITokenService
	{
		string CreateToken(UserDetails user);
	}
}

