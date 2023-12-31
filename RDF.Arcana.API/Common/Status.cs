﻿namespace RDF.Arcana.API.Common;

public static class Status
{
    public const string NoFreebies = "For freebie request";
    public const string ForReleasing = "For releasing";
    public const string ForRegular = "For regular";
    public const string Released = "Released";
    public const string ApproverApproval = "Approver Approval";
    public const string UnderReview = "Under review";
    public const string Approved = "Approved";
    public const string Rejected = "Rejected";
    public const string Requested = "Requested";
    public const string Voided = "Voided";
    public const string Archived = "Archived";
    public const string ForListingFeeApproval = "For listing fee approval";
    public const string DirectRegistrationApproval = "Direct Registration Approval";
    public const string ForRegularApproval = "For regular approval";
    public const string ForFreebieApproval = "For freebie approval";
    public const string PendingRegistration = "Pending registration";
}

public static class Origin
{
    public const string Direct = "Direct";
    public const string Prospecting = "Prospecting";
}

public static class CustomerType
{
    public const string Prospect = "Prospect";
    public const string Direct = "Direct";
}