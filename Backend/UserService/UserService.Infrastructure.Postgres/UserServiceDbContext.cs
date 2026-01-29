using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Users;

namespace UserService.Infrastructure.Postgres;

public interface IReadDbContext
{
    IQueryable<User> UsersQueryable { get; }
}

public class UserServiceDbContext(DbContextOptions<UserServiceDbContext> options)
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    /*protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(UserServiceDbContext).Assembly);
    }*/
}


// public class UserServiceDbContext(DbContextOptions<UserServiceDbContext> options)
//     : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options) : IdentityDbContext<User, IdentityRole<Guid>, Guid>, IReadDbContext
// {
//     //public IQueryable<User> UsersQueryable => Users.AsQueryable().AsNoTracking();
// }