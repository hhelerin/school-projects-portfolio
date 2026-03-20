namespace App.Domain;

public enum SubscriptionTier
{
    Free,
    Standard,
    Premium
}

public enum BillingCycle
{
    DropIn,
    ClassCard,
    MonthlyStyle,
    MonthlyAll
}

public enum CompanyRoleType
{
    SystemAdmin,
    SystemSupport,
    SystemBilling,
    CompanyOwner,
    CompanyAdmin,
    CompanyManager,
    CompanyEmployee
}