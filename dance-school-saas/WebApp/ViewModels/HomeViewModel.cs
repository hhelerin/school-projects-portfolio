using System.Collections.Generic;

namespace WebApp.ViewModels;

public class HomeViewModel
{
    public bool IsAuthenticated { get; set; }
    public string? UserFirstName { get; set; }
    public bool IsRootUser { get; set; }
    public List<CompanySummaryViewModel> UserCompanies { get; set; } = new();
}

public class CompanySummaryViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}