using Microsoft.AspNetCore.Identity; //UserManager presents: privides api for managing user in persistent(MS-SQL Server database) stores
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchoolApp.API.Data;
using SchoolApp.API.Data.Helper;
using SchoolApp.API.Models;
using SchoolApp.API.Models.ViewModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SchoolApp.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{

    private readonly UserManager<Applicationuser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;


    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly TokenValidationParameters _tokenValidationParameters;


    public AuthenticationController(UserManager<Applicationuser> userManager,
                                    RoleManager<IdentityRole> roleManager,
                                    AppDbContext context,
                                    IConfiguration configuration,
                                    TokenValidationParameters tokenValidationParameters)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _configuration = configuration;
        _tokenValidationParameters = tokenValidationParameters;

    }


    [HttpPost("register-user")]
    public async Task<IActionResult> Register([FromBody] RegisterVM? registerVM)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Please, provide all the required fields");
        }

        var userExists = await _userManager.FindByEmailAsync(registerVM.EmailAddress);
        if (userExists != null)
        {
            return BadRequest($"User {registerVM.EmailAddress} already exists");
        }

        Applicationuser newUser = new Applicationuser()
        {
            Firstname = registerVM.FirstName,
            Lastname = registerVM.LastName,
            Email = registerVM.EmailAddress,
            UserName = registerVM.UserName,
            SecurityStamp = Guid.NewGuid().ToString(),
        };

        var result = await _userManager.CreateAsync(newUser, registerVM.Password);



        if (result.Succeeded)
        {
            //add user role
            switch (/*registerVM.Role*/ char.ToUpper(registerVM.Role[0]) + registerVM.Role.Substring(1))
            {
                case UserRoles.Manager:
                    await _userManager.AddToRoleAsync(newUser, UserRoles.Manager);
                    break;

                case UserRoles.Student:
                    await _userManager.AddToRoleAsync(newUser, UserRoles.Student);
                    break;

                default:
                    break;
            }


            return Ok(result);
        }
        else
        {
            return BadRequest("user could not be created");
        }
    }


    [HttpPost("login-user")]
    public async Task<IActionResult> Login([FromBody] LoginVM loginVM)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Please, provide all required fields");
        }
        
        var userExists = await _userManager.FindByEmailAsync(loginVM.EmailAddress);
        if (userExists != null && await _userManager.CheckPasswordAsync(userExists, loginVM.Password))
        {
            // User logged in successfully Generate JWT Token
            var tokenValue = await GenerateJWTTokenAsync(userExists, null);

            return Ok(tokenValue);
        }
        return Unauthorized();

    }





    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenRequestVM tokenRequestVM) //To refresh token we need old expired token and generated refresh token
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Please, provide all required fields");
        }

        var result = await VerifyAndGenerateTokenAsync(tokenRequestVM);
        return Ok(result);
    }

    private async Task<dynamic> VerifyAndGenerateTokenAsync(TokenRequestVM tokenRequestVM) //old Token and Refresh Token
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();

        // Getting Refresh Token from Database table RefreshTokens jo rfresh token login k time pe generate kark save kiyatha
        var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRequestVM.RefreshToken & x.DateExpire >= DateTime.Now);

        //Jis user k liye RefreshTokens save huah tha uska UserId se Identity _userManager se data lehreh.
        var dbUser = await _userManager.FindByIdAsync(storedToken.UserId);
        //var tokenValue = await GenerateJWTTokenAsync(dbUser, null);

        try
        {
            //var tokenCheckRequest = jwtTokenHandler.ValidateToken(tokenRequestVM.RefreshToken, _tokenValidationParameters,
            //                                                      out var validatedToken);
            //return await GenerateJWTTokenAsync(dbUser, storedToken);

            var tokenValue = await GenerateJWTTokenAsync(dbUser, null);

            return Ok(tokenValue);

        }
        catch (SecurityTokenExpiredException)
        {
            if (storedToken.DateExpire >= DateTime.UtcNow)
            {
                return await GenerateJWTTokenAsync(dbUser, null);
            }
            else
            {
                return await GenerateJWTTokenAsync(dbUser, null);
            }
        }

    }

    private async Task<AuthResultVM> GenerateJWTTokenAsync(Applicationuser user, RefreshToken rToken)
    {
        var authClaims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        //Add User Role Claims
        var userRoles = await _userManager.GetRolesAsync(user);
        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }


        var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:Issuer"],
            audience: _configuration["JWT:Audience"],
            expires: DateTime.UtcNow.AddMinutes(1),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));


        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);


        if (rToken != null)
        {

            var rTokenResponse = new AuthResultVM()
            {
                Token = jwtToken,
                RefreshToken = rToken.Token,
                ExpiresAt = token.ValidTo
            };
            return rTokenResponse;
        }

        // generate refresh token starts
        var refreshToken = new RefreshToken()
        {
            JwtId = token.Id,
            IsResolved = false,
            UserId = user.Id,
            DateAdded = DateTime.UtcNow,
            DateExpire = DateTime.UtcNow.AddMonths(6),
            Token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString()
        };
        //generate refresh token ends.
        // And Save the Generated Referesh Token at Database Table named RefreshTokens.
        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();



        var response = new AuthResultVM()
        {
            Token = jwtToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = token.ValidTo
        };

        return response;
    }



}

