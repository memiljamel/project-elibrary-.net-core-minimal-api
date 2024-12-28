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

            group.MapPost("/login", Login)
                .AllowAnonymous();

            group.MapGet("/current", GetAccount)
                .RequireAuthorization("All");

            group.MapPut("/current", UpdateAccount)
                .RequireAuthorization("All")
                .DisableAntiforgery();
        }

        private static async Task<Results<Created<WebResponse<LoginResponse>>, ValidationProblem>> Login(
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
                    new Claim("Id", staff.Id.ToString()),
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

                return TypedResults.Created(string.Empty, new WebResponse<LoginResponse>
                {
                    Code = 201,
                    Status = "Created",
                    Data = new LoginResponse
                    {
                        AccessToken = handler.WriteToken(token)
                    }
                });
            }

            return TypedResults.ValidationProblem(result.ToDictionary());
        }

        private static async Task<Results<Ok<WebResponse<StaffResponse>>, NotFound>> GetAccount(
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

            return TypedResults.Ok(new WebResponse<StaffResponse>
            {
                Code = 200,
                Status = "OK",
                Data = response
            });
        }

        private static async Task<Results<Ok<WebResponse<StaffResponse>>, NotFound, ValidationProblem>> UpdateAccount(
            ClaimsPrincipal claimsPrincipal,
            [FromServices] IValidator<UpdateStaffRequest> validator,
            [FromServices] IUnitOfWork unitOfWork,
            [FromForm] UpdateStaffRequest request)
        {
            var username = claimsPrincipal.Identity?.Name;
            if (username == null)
            {
                return TypedResults.NotFound();
            }

            request.Id = new Guid(claimsPrincipal.FindFirst("Id")!.Value);

            var result = await validator.ValidateAsync(request);

            if (result.IsValid)
            {
                var staff = await unitOfWork.StaffRepository.GetByUsername(username);
                if (staff == null)
                {
                    return TypedResults.NotFound();
                }

                staff.Username = request.Username;
                staff.Name = request.Name;
                staff.StaffNumber = request.StaffNumber;
                staff.AccessLevel = request.AccessLevel;
                staff.UpdatedAt = DateTime.UtcNow;

                if (request.Password != null)
                {
                    staff.Password = BC.HashPassword(request.Password);
                }

                if (request.Image != null)
                {
                    if (staff.ImageUrl != null && File.Exists(Path.Combine("wwwroot", staff.ImageUrl)))
                    {
                        File.Delete(Path.Combine("wwwroot", staff.ImageUrl));
                    }

                    var filename = Path.GetRandomFileName() + Path.GetExtension(request.Image.FileName);

                    await using var stream = new FileStream(Path.Combine("wwwroot", filename), FileMode.Create);
                    await request.Image.CopyToAsync(stream);

                    staff.ImageUrl = filename;
                }

                await unitOfWork.SaveChangesAsync();

                var response = ToStaffResponse(staff);

                return TypedResults.Ok(new WebResponse<StaffResponse>
                {
                    Code = 200,
                    Status = "OK",
                    Data = response
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