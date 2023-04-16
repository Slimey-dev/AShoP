using AShoP.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace AShoP.Data;

public class CustomUserManager<TUser> : UserManager<TUser> where TUser : IdentityUser
{
    private readonly ApplicationDbContext _context;

    public CustomUserManager(IUserStore<TUser> store, IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<TUser> passwordHasher, IEnumerable<IUserValidator<TUser>> userValidators,
        IEnumerable<IPasswordValidator<TUser>> passwordValidators, ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<TUser>> logger,
        ApplicationDbContext context) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators,
        keyNormalizer, errors, services, logger)
    {
        _context = context;
    }

    public override async Task<IdentityResult> CreateAsync(TUser user)
    {
        var result = await base.CreateAsync(user);

        if (result.Succeeded)
        {
            var customer = new Customer
            {
                Id = Guid.Parse(user.Id)
            };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
        }

        return result;
    }
}