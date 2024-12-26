using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ELibrary.Entities;
using ELibrary.Models;
using ELibrary.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using BC = BCrypt.Net.BCrypt;
using ValidationFailure = FluentValidation.Results.ValidationFailure;

namespace ELibrary.Endpoint
{
    public static class AccountEndpoint
    {
        public static void Map(WebApplication app)
        {
            var group = app.MapGroup("/api/account")
                .WithTags("Account");

            group.MapGet("/current", GetAccount)
                .RequireAuthorization("All");

            group.MapPost("/login", Login)
                .AllowAnonymous();
        }

        private static async Task<Results<Ok<StaffResponse>, NotFound>> GetAccount(
            ClaimsPrincipal claimsPrincipal,
            [FromServices] IUnitOfWork unitOfWork)
        {
            var username = claimsPrincipal.Identity?.Name;
            if (username == null)
            {
                return TypedResults.NotFound();
            }

            var staff = await unitOfWork.StaffRepository.GetByUsername(username);
            if (staff == null)
            {
                return TypedResults.NotFound();
            }

            var response = ToStaffResponse(staff);

            return TypedResults.Ok(response);
        }

        private static async Task<Results<Created<LoginResponse>, ValidationProblem>> Login(
            [FromServices] IValidator<LoginRequest> validator,
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IConfiguration configuration,
            [FromBody] LoginRequest request)
        {
            var result = await validator.ValidateAsync(request);

            if (result.IsValid)
            {
                var staff = await unitOfWork.StaffRepository.GetByUsername(request.Username);

                if (staff == null || !BC.Verify(request.Password, staff.Password))
                {
                    result.Errors.Add(new ValidationFailure("username", "Invalid login attempt."));

                    return TypedResults.ValidationProblem(result.ToDictionary());
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, staff.Username),
                    new Claim(ClaimTypes.Role, staff.AccessLevel.ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, staff.Username),
                    new Claim(JwtRegisteredClaimNames.Name, staff.Name),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                var credentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                    SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: configuration["Jwt:Issuer"],
                    audience: configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(15),
                    signingCredentials: credentials
                );

                var handler = new JwtSecurityTokenHandler();

                return TypedResults.Created(string.Empty, new LoginResponse
                {
                    AccessToken = handler.WriteToken(token)
                });
            }

            return TypedResults.ValidationProblem(result.ToDictionary());
        }

        private static StaffResponse ToStaffResponse(Staff staff)
        {
            return new StaffResponse
            {
                Id = staff.Id,
                Username = staff.Username,
                Name = staff.Name,
                StaffNumber = staff.StaffNumber,
                AccessLevel = staff.AccessLevel,
                ImageUrl = staff.ImageUrl,
                CreatedAt = staff.CreatedAt,
                UpdatedAt = staff.UpdatedAt
            };
        }
    }
}