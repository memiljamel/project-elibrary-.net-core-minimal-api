using ELibrary.Entities;
using ELibrary.Enums;
using ELibrary.Models;
using ELibrary.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using BC = BCrypt.Net.BCrypt;

namespace ELibrary.Endpoint
{
    public static class StaffEndpoint
    {
        public static void Map(WebApplication app)
        {
            var group = app.MapGroup("/api/staffs")
                .WithTags("Staffs");

            group.MapGet("/", GetStaffs)
                .RequireAuthorization("All");

            group.MapGet("/{staffId:guid}", GetStaff)
                .RequireAuthorization("All");

            group.MapPost("/", CreateStaff)
                .DisableAntiforgery()
                .RequireAuthorization("AdministratorOnly");

            group.MapPut("/{staffId:guid}", UpdateStaff)
                .DisableAntiforgery()
                .RequireAuthorization("AdministratorOnly");

            group.MapDelete("/{staffId:guid}", DeleteStaff)
                .RequireAuthorization("AdministratorOnly");
        }

        private static async Task<Results<Ok<WebResponse<PaginatedList<StaffResponse>>>, NotFound>> GetStaffs(
            [FromServices] IUnitOfWork unitOfWork,
            [FromQuery] string? username,
            [FromQuery] string? name,
            [FromQuery] string? staffNumber,
            [FromQuery] AccessLevelEnum? accessLevel,
            [FromQuery(Name = "page")] int pageNumber = 1,
            [FromQuery(Name = "size")] int pageSize = 15)
        {
            if (pageNumber < 1)
            {
                return TypedResults.NotFound();
            }

            var staffs = await unitOfWork.StaffRepository.GetPagedStaffs(
                username,
                name,
                staffNumber,
                accessLevel,
                pageNumber,
                pageSize);

            if (staffs.PageIndex != 1 && pageNumber > staffs.TotalPages)
            {
                return TypedResults.NotFound();
            }

            var response = staffs.Select(s => ToStaffResponse(s))
                .ToList();

            return TypedResults.Ok(new WebResponse<PaginatedList<StaffResponse>>
            {
                Code = 200,
                Status = "Ok",
                Data = new PaginatedList<StaffResponse>(
                    response,
                    staffs.TotalCount,
                    staffs.PageIndex,
                    pageSize),
                Meta = new MetaResponse
                {
                    CurrentPage = staffs.PageIndex,
                    PerPage = staffs.PageSize,
                    Total = staffs.TotalCount,
                    TotalPage = staffs.TotalPages
                }
            });
        }

        private static async Task<Results<Ok<WebResponse<StaffResponse>>, NotFound>> GetStaff(
            [FromServices] IUnitOfWork unitOfWork,
            [FromRoute] Guid staffId)
        {
            var staff = await unitOfWork.StaffRepository.GetById(staffId);
            if (staff == null)
            {
                return TypedResults.NotFound();
            }

            var response = ToStaffResponse(staff);

            return TypedResults.Ok(new WebResponse<StaffResponse>
            {
                Code = 200,
                Status = "Ok",
                Data = response
            });
        }

        private static async Task<Results<Created<WebResponse<StaffResponse>>, ValidationProblem>> CreateStaff(
            [FromServices] IValidator<CreateStaffRequest> validator,
            [FromServices] IUnitOfWork unitOfWork,
            [FromForm] CreateStaffRequest request)
        {
            var result = await validator.ValidateAsync(request);

            if (result.IsValid)
            {
                var staff = new Staff
                {
                    Username = request.Username,
                    Password = BC.HashPassword(request.Password),
                    Name = request.Name,
                    StaffNumber = request.StaffNumber,
                    AccessLevel = request.AccessLevel
                };

                if (request.Image != null)
                {
                    var filename = Path.GetRandomFileName() + Path.GetExtension(request.Image.FileName);

                    await using var stream = new FileStream(Path.Combine("wwwroot", filename), FileMode.Create);
                    await request.Image.CopyToAsync(stream);

                    staff.ImageUrl = filename;
                }

                unitOfWork.StaffRepository.Add(staff);

                await unitOfWork.SaveChangesAsync();

                var response = ToStaffResponse(staff);

                return TypedResults.Created($"/api/staffs/{staff.Id}", new WebResponse<StaffResponse>
                {
                    Code = 201,
                    Status = "Created",
                    Data = response
                });
            }

            return TypedResults.ValidationProblem(result.ToDictionary());
        }

        private static async Task<Results<Ok<WebResponse<StaffResponse>>, NotFound, ValidationProblem>> UpdateStaff(
            [FromServices] IValidator<UpdateStaffRequest> validator,
            [FromServices] IUnitOfWork unitOfWork,
            [FromRoute] Guid staffId,
            [FromForm] UpdateStaffRequest request)
        {
            request.Id = staffId;

            var result = await validator.ValidateAsync(request);

            if (result.IsValid)
            {
                var staff = await unitOfWork.StaffRepository.GetById(request.Id);
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
                    Status = "Ok",
                    Data = response
                });
            }

            return TypedResults.ValidationProblem(result.ToDictionary());
        }

        private static async Task<Results<Ok<WebResponse<object>>, NotFound>> DeleteStaff(
            [FromServices] IUnitOfWork unitOfWork,
            [FromRoute] Guid staffId)
        {
            var staff = await unitOfWork.StaffRepository.GetById(staffId);
            if (staff == null)
            {
                return TypedResults.NotFound();
            }

            if (staff.ImageUrl != null && File.Exists(Path.Combine("wwwroot", staff.ImageUrl)))
            {
                File.Delete(Path.Combine("wwwroot", staff.ImageUrl));
            }

            unitOfWork.StaffRepository.Remove(staff);

            await unitOfWork.SaveChangesAsync();

            return TypedResults.Ok(new WebResponse<object>
            {
                Code = 200,
                Status = "Ok",
                Data = null
            });
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