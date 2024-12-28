using ELibrary.Entities;
using ELibrary.Models;
using ELibrary.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ELibrary.Endpoint
{
    public static class MemberEndpoint
    {
        public static void Map(WebApplication app)
        {
            var group = app.MapGroup("/api/members")
                .WithTags("Members");

            group.MapGet("/", GetMembers)
                .RequireAuthorization("All");

            group.MapGet("/{memberId:guid}", GetMember)
                .RequireAuthorization("All");

            group.MapPost("/", CreateMember)
                .DisableAntiforgery()
                .RequireAuthorization("All");

            group.MapPut("/{memberId:guid}", UpdateMember)
                .DisableAntiforgery()
                .RequireAuthorization("All");

            group.MapDelete("/{memberId:guid}", DeleteMember)
                .RequireAuthorization("All");
        }

        private static async Task<Results<Ok<WebResponse<PaginatedList<MemberResponse>>>, NotFound>> GetMembers(
            [FromServices] IUnitOfWork unitOfWork,
            [FromQuery] string? memberNumber,
            [FromQuery] string? name,
            [FromQuery] string? address,
            [FromQuery] string? email,
            [FromQuery] string? phone,
            [FromQuery(Name = "page")] int pageNumber = 1,
            [FromQuery(Name = "size")] int pageSize = 15)
        {
            if (pageNumber < 1)
            {
                return TypedResults.NotFound();
            }

            var members = await unitOfWork.MemberRepository.GetPagedMembers(
                memberNumber,
                name,
                address,
                email,
                phone,
                pageNumber,
                pageSize);

            if (members.PageIndex != 1 && pageNumber > members.TotalPages)
            {
                return TypedResults.NotFound();
            }

            var response = members.Select(m => ToMemberResponse(m))
                .ToList();

            return TypedResults.Ok(new WebResponse<PaginatedList<MemberResponse>>
            {
                Code = 200,
                Status = "OK",
                Data = new PaginatedList<MemberResponse>(
                    response,
                    members.TotalCount,
                    members.PageIndex,
                    pageSize),
                Meta = new MetaResponse
                {
                    CurrentPage = members.PageIndex,
                    PerPage = members.PageSize,
                    Total = members.TotalCount,
                    TotalPage = members.TotalPages
                }
            });
        }

        private static async Task<Results<Ok<WebResponse<MemberResponse>>, NotFound>> GetMember(
            [FromServices] IUnitOfWork unitOfWork,
            [FromRoute] Guid memberId)
        {
            var member = await unitOfWork.MemberRepository.GetById(memberId);
            if (member == null)
            {
                return TypedResults.NotFound();
            }

            var response = ToMemberResponse(member);

            return TypedResults.Ok(new WebResponse<MemberResponse>
            {
                Code = 200,
                Status = "OK",
                Data = response
            });
        }

        private static async Task<Results<Created<WebResponse<MemberResponse>>, ValidationProblem>> CreateMember(
            [FromServices] IValidator<CreateMemberRequest> validator,
            [FromServices] IUnitOfWork unitOfWork,
            [FromForm] CreateMemberRequest request)
        {
            var result = await validator.ValidateAsync(request);

            if (result.IsValid)
            {
                await unitOfWork.BeginTransactionAsync();

                try
                {
                    var member = new Member
                    {
                        MemberNumber = request.MemberNumber,
                        Name = request.Name,
                        Address = request.Address,
                        Email = request.Email
                    };

                    if (request.Image != null)
                    {
                        var filename = Path.GetRandomFileName() + Path.GetExtension(request.Image.FileName);

                        await using var stream = new FileStream(Path.Combine("wwwroot", filename), FileMode.Create);
                        await request.Image.CopyToAsync(stream);

                        member.ImageUrl = filename;
                    }

                    unitOfWork.MemberRepository.Add(member);

                    await unitOfWork.SaveChangesAsync();

                    foreach (var phoneNumber in request.Phones)
                    {
                        var phone = new Phone
                        {
                            PhoneNumber = phoneNumber,
                            MemberId = member.Id
                        };
                        unitOfWork.PhoneRepository.Add(phone);

                        await unitOfWork.SaveChangesAsync();
                    }

                    await unitOfWork.CommitAsync();

                    var response = ToMemberResponse(member);

                    return TypedResults.Created($"/api/members/{member.Id}", new WebResponse<MemberResponse>
                    {
                        Code = 201,
                        Status = "Created",
                        Data = response
                    });
                }
                catch (Exception e)
                {
                    await unitOfWork.RollbackAsync();

                    throw new Exception(e.Message);
                }
            }

            return TypedResults.ValidationProblem(result.ToDictionary());
        }

        private static async Task<Results<Ok<WebResponse<MemberResponse>>, NotFound, ValidationProblem>> UpdateMember(
            [FromServices] IValidator<UpdateMemberRequest> validator,
            [FromServices] IUnitOfWork unitOfWork,
            [FromRoute] Guid memberId,
            [FromForm] UpdateMemberRequest request)
        {
            request.Id = memberId;

            var result = await validator.ValidateAsync(request);

            if (result.IsValid)
            {
                await unitOfWork.BeginTransactionAsync();

                try
                {
                    var member = await unitOfWork.MemberRepository.GetById(request.Id);
                    if (member == null)
                    {
                        return TypedResults.NotFound();
                    }

                    unitOfWork.PhoneRepository.RemoveRange(member.Phones);

                    await unitOfWork.SaveChangesAsync();

                    member.MemberNumber = request.MemberNumber;
                    member.Name = request.Name;
                    member.Address = request.Address;
                    member.Email = request.Email;
                    member.UpdatedAt = DateTime.UtcNow;

                    if (request.Image != null)
                    {
                        if (member.ImageUrl != null && File.Exists(Path.Combine("wwwroot", member.ImageUrl)))
                        {
                            File.Delete(Path.Combine("wwwroot", member.ImageUrl));
                        }

                        var filename = Path.GetRandomFileName() + Path.GetExtension(request.Image.FileName);

                        await using var stream = new FileStream(Path.Combine("wwwroot", filename), FileMode.Create);
                        await request.Image.CopyToAsync(stream);

                        member.ImageUrl = filename;
                    }

                    await unitOfWork.SaveChangesAsync();

                    foreach (var phoneNumber in request.Phones)
                    {
                        var phone = new Phone
                        {
                            PhoneNumber = phoneNumber,
                            MemberId = member.Id
                        };
                        unitOfWork.PhoneRepository.Add(phone);

                        await unitOfWork.SaveChangesAsync();
                    }

                    await unitOfWork.CommitAsync();

                    var response = ToMemberResponse(member);

                    return TypedResults.Ok(new WebResponse<MemberResponse>
                    {
                        Code = 200,
                        Status = "OK",
                        Data = response
                    });
                }
                catch (Exception e)
                {
                    await unitOfWork.RollbackAsync();

                    throw new Exception(e.Message);
                }
            }

            return TypedResults.ValidationProblem(result.ToDictionary());
        }

        private static async Task<Results<Ok<WebResponse<object>>, NotFound>> DeleteMember(
            [FromServices] IUnitOfWork unitOfWork,
            [FromRoute] Guid memberId)
        {
            await unitOfWork.BeginTransactionAsync();

            try
            {
                var member = await unitOfWork.MemberRepository.GetById(memberId);
                if (member == null)
                {
                    return TypedResults.NotFound();
                }

                unitOfWork.PhoneRepository.RemoveRange(member.Phones);

                await unitOfWork.SaveChangesAsync();

                if (member.ImageUrl != null && File.Exists(Path.Combine("wwwroot", member.ImageUrl)))
                {
                    File.Delete(Path.Combine("wwwroot", member.ImageUrl));
                }

                unitOfWork.MemberRepository.Remove(member);

                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitAsync();

                return TypedResults.Ok(new WebResponse<object>
                {
                    Code = 200,
                    Status = "OK",
                    Data = null
                });
            }
            catch (Exception e)
            {
                await unitOfWork.RollbackAsync();

                throw new Exception(e.Message);
            }
        }

        private static MemberResponse ToMemberResponse(Member member)
        {
            return new MemberResponse
            {
                Id = member.Id,
                MemberNumber = member.MemberNumber,
                Name = member.Name,
                Address = member.Address,
                Email = member.Email,
                Phones = member.Phones?.Select(p => p.PhoneNumber).ToList(),
                ImageUrl = member.ImageUrl,
                CreatedAt = member.CreatedAt,
                UpdatedAt = member.UpdatedAt
            };
        }
    }
}