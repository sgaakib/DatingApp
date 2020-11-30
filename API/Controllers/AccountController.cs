using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace API.Controllers
{
    public class AccountController : WebApiBasecontroller
    {
        private readonly DataContext _context;
        private readonly ITokenInterface _Itoken;
        public AccountController(DataContext context, ITokenInterface Itoken)
        {
            _Itoken = Itoken;
            _context = context;
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExist(registerDto.username)) return Unauthorized("username is taken");
            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                userName = registerDto.username.ToLower(),
                passwordhash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.password)),
                passwordsalt = hmac.Key

            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new UserDto
            {
                username = user.userName,
                token = _Itoken.CreateToken(user)
            };

        }
        // [Authorize]
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.userName == loginDto.username.ToLower());
            if (user == null) return Unauthorized("Invalid Username");
            var hmac = new HMACSHA512(user.passwordsalt);
            var computedhash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.password));
            for (int i = 1; i < computedhash.Length; i++)
            {
                if (computedhash[i] != user.passwordhash[i])
                    return Unauthorized("Invalid Password");
            }
            return new UserDto {
                username = user.userName,
                token = _Itoken.CreateToken(user)};
        }
        private Task<bool> UserExist(string username)
        {
            return _context.Users.AnyAsync(x => x.userName == username.ToLower());
        }
    }
}