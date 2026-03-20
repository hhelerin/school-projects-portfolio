namespace App.DAL.EF.Seeding;

public static class InitialData
{
    public static readonly (string roleName, Guid? id)[]
        Roles =
        [
            // System roles
            ("SystemAdmin", null),
            ("SystemSupport", null),
            ("SystemBilling", null),
            // Company roles
            ("CompanyOwner", null),
            ("CompanyAdmin", null),
            ("CompanyManager", null),
            ("CompanyEmployee", null),
        ];

    public static readonly (string name, string password, string firstName, string lastName, Guid? id, string[] roles)[]
        Users =
        [
            ("helehe@taltech.ee", "Root.Beer.101", "Helerin", "Heinsalu", null, ["SystemAdmin", "SystemSupport", "CompanyOwner"])
        ];
}