using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Data;
using TaskFlowApi.Dtos.Auth;
using TaskFlowApi.Models;

namespace TaskFlowApi.Services;

public interface IAuthService
{
    Task<User?> RegisterAsync(RegisterDto request);
    Task<User?> LoginAsync(LoginDto request);
}

public class AuthService(TaskFlowDbContext dbContext) : IAuthService
{
    public async Task<User?> RegisterAsync(RegisterDto request)
    {
        var existing = await dbContext.Users.AnyAsync(u => u.Email == request.Email);
        if (existing)
            return null;

        var user = new User
        {
            Email = request.Email,
            DisplayName = request.DisplayName,
            CreatedAt = DateTime.UtcNow,
        };

        // Hash the password using PasswordHasher
        user.PasswordHash = new PasswordHasher<User>().HashPassword(user, request.Password);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        return user;
    }

    public async Task<User?> LoginAsync(LoginDto request)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user is null)
        {
            Console.WriteLine("User not found during login.");
            return null;
        }

        if (!VerifyPassword(request.Password, user))
        {
            Console.WriteLine("Invalid password attempt.");
            return null;
        }

        return user;
    }

    public bool VerifyPassword(string password, User user)
    {
        var result = new PasswordHasher<User>().VerifyHashedPassword(
            user,
            user.PasswordHash,
            password
        );
        return result == PasswordVerificationResult.Success;
    }
}
