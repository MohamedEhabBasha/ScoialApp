using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers;

public class AccountController(DataContext context, ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(ResgisterDto resgisterDto)
    {
        if (await UserExists(resgisterDto.Username)) return BadRequest("Username is already taken!!!");

        return Ok();

        //using var hmac = new HMACSHA512();

        //var user = new AppUser
        //{
        //    UserName = resgisterDto.Username.ToLower(),
        //    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(resgisterDto.Password)),
        //    PasswordSalt = hmac.Key
        //};

        //context.Users.Add(user);
        //await context.SaveChangesAsync();

        //return Ok(new UserDto
        //{
        //    Username = user.UserName,
        //    Token = tokenService.CreateToken(user)
        //});
    }
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

        if (user == null) return Unauthorized("Invalid username");

        using var hmac = new HMACSHA512 (user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
        }

        return Ok(new UserDto
        {
            Username = user.UserName,
            Token = tokenService.CreateToken(user)
        });
    }
    private async Task<bool> UserExists(string username)
    {
        return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
    }
}
