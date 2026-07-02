using WorkoutTrackerApi.Domain;
using WorkoutTrackerApi.Infrastructure;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace WorkoutTrackerApi.Application;

public class UserService {
    private readonly JwtService _jwtService;
    private readonly PasswordHasher _passwordHasher;
    private readonly WorkoutTrackerApiDbContext _context;
    private readonly IMapper _mapper;

    public UserService(JwtService jwtService, PasswordHasher passwordHasher, WorkoutTrackerApiDbContext context, IMapper mapper) {
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
        _context = context;
        _mapper = mapper;
    }

    public async Task<UserResponse> RegisterUser(RegisterUserDto dto) {
        var isEmailExisted = await _context.UserTable
            .AnyAsync(x => x.Email == dto.Email);

        if (isEmailExisted)
            throw new InvalidOperationException("Email already existed");

        User theNewUser = new User {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHashed = dto.Password,
        };
            
        theNewUser.PasswordHashed = _passwordHasher.HashPassword(dto.Password);

        _context.UserTable.Add(theNewUser);
        await _context.SaveChangesAsync();

        return new UserResponse {
            Name = theNewUser.Name,
            Email = theNewUser.Email
        };
    }

}