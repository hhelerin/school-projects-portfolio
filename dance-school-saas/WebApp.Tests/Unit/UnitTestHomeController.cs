using System;
using System.Threading.Tasks;
using App.DAL.EF;
using App.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using WebApp.Controllers;
using WebApp.ViewModels;
using Xunit;
using Xunit.Abstractions;

namespace WebApp.Tests.Unit;

public class UnitTestHomeController
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly AppDbContext _ctx;

    public UnitTestHomeController(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;

        // set up mock database - inmemory
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            
        // use random guid as db instance id
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        _ctx = new AppDbContext(optionsBuilder.Options);

        // reset db
        _ctx.Database.EnsureDeleted();
        _ctx.Database.EnsureCreated();
    }
    
    [Fact]
    public async Task IndexAction_ReturnsHomeViewModel_ForAnonymousUser()
    {
        // Arrange
        var logger = Mock.Of<ILogger<HomeController>>();
        var userManager = MockUserManager();
        var controller = new HomeController(_ctx, logger, userManager.Object);
        
        // Act
        var result = (await controller.Index()) as ViewResult;
        
        // Assert
        _testOutputHelper.WriteLine(result?.ToString());
        var vm = result?.Model as HomeViewModel;
        Assert.NotNull(vm);
        Assert.False(vm.IsAuthenticated);
        Assert.Empty(vm.UserCompanies);
    }

    private static Mock<UserManager<AppUser>> MockUserManager()
    {
        var store = new Mock<IUserStore<AppUser>>();
        var mgr = new Mock<UserManager<AppUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        mgr.Object.UserValidators.Add(new UserValidator<AppUser>());
        mgr.Object.PasswordValidators.Add(new PasswordValidator<AppUser>());
        return mgr;
    }

}