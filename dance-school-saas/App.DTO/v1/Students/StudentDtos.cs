namespace App.DTO.v1.Students;

public class StudentListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? ContactInfo { get; set; }
    public bool HasAppUser { get; set; }
    public int ActivePackageCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class StudentDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? PersonalId { get; set; }
    public string? ContactInfo { get; set; }
    public string? Details { get; set; }
    public Guid? AppUserId { get; set; }
    public string? AppUserFullName { get; set; }
    public List<PackageSummaryDto> ActivePackages { get; set; } = new();
    public List<TrialRecordSummaryDto> TrialRecords { get; set; } = new();
    public List<ShowcaseEligibilitySummaryDto> ShowcaseEligibilities { get; set; } = new();
    public int AttendanceCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

// Placeholder DTOs - populated in future changes
public class PackageSummaryDto { }

public class TrialRecordSummaryDto { }

public class ShowcaseEligibilitySummaryDto { }
