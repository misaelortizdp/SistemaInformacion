using CashFlowSystem.Application.Commands.Auth;
using CashFlowSystem.Application.DTOs.Auth;
using CashFlowSystem.Application.Interfaces;
using CashFlowSystem.Domain.Entities;
using CashFlowSystem.Domain.Enums;
using CashFlowSystem.Domain.Interfaces;
using MediatR;

namespace CashFlowSystem.Application.Handlers.Auth;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public RegisterCommandHandler(
        IRepository<User> userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Check if email already exists
        var existingUsers = await _userRepository.GetAllAsync(cancellationToken);
        if (existingUsers.Any(u => u.Email.ToLower() == request.Email.ToLower()))
        {
            throw new InvalidOperationException("Email already registered");
        }

        // Check if username already exists
        if (existingUsers.Any(u => u.Username.ToLower() == request.Username.ToLower()))
        {
            throw new InvalidOperationException("Username already taken");
        }

        // Hash password
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        // Create new user
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHash,
            Role = UserRole.Cashier, // Default role
            IsActive = true
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Generate JWT token
        var token = _jwtService.GenerateToken(user.Id, user.Username, user.Email, user.Role.ToString());

        return new AuthResponse
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role.ToString(),
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };
    }
}
