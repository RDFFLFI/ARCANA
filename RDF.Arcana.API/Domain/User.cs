﻿using System.ComponentModel.DataAnnotations.Schema;
using FluentValidation;
using RDF.Arcana.API.Common;

namespace RDF.Arcana.API.Domain;

public class User : BaseEntity
{
    public string FullIdNo { get; set; }
    public string Fullname { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public bool IsPasswordChanged { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public bool IsActive { get; set; } = true;
    public int? CompanyId { get; set; }
    public int? DepartmentId { get; set; }
    public int? LocationId { get; set; }
    public int? UserRolesId { get; set; }
    public string MobileNumber { get; set; }



    [ForeignKey("AddedByUser")] public int? AddedBy { get; set; }

    public string ProfilePicture { get; set; }
    public virtual Company Company { get; set; }
    public virtual Department Department { get; set; }
    public virtual Location Location { get; set; }
    public virtual UserRoles UserRoles { get; set; }
    public virtual User AddedByUser { get; set; }

    public virtual ICollection<Clients> Clients { get; set; }
    public virtual ICollection<Request> RequesterRequests { get; set; }
    public virtual ICollection<Request> ApproverRequests { get; set; }
    public virtual ICollection<Approver> Approver { get; set; }
    public virtual ICollection<Approval> Approvals { get; set; }
    public virtual ICollection<FreebieRequest> FreebieRequests { get; set; }
    public virtual ICollection<ListingFee> ListingFees { get; set; }
    public virtual CdoCluster CdoCluster { get; set; }
}

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(x => x.Id).NotNull();
        RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required!")
            .MinimumLength(3).WithMessage("Username must be at least 3 character long!");
    }
}