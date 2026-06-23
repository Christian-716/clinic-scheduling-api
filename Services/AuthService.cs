using ClinicSchedulingApi.Data;
using ClinicSchedulingApi.DTOs;
using ClinicSchedulingApi.Models;
using ClinicSchedulingApi.Security;
using Microsoft.EntityFrameworkCore;

namespace ClinicSchedulingApi.Services;

public class AuthService
{
    private readonly AppDbContext _db;
    private readonly JwtService _jwt;

    public AuthService(AppDbContext db, JwtService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    public async Task<AuthResponse> RegisterAsync(AuthRequest request)
    {
        var user = new User
        {
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return new AuthResponse { Token = _jwt.GenerateToken(user.Username) };
    }

    public async Task<AuthResponse?> LoginAsync(AuthRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        return new AuthResponse { Token = _jwt.GenerateToken(user.Username) };
    }
}
