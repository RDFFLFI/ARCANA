﻿using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using RDF.Arcana.API.Data;
using RDF.Arcana.API.Domain;
using RDF.Arcana.API.Features.Authenticate.Exception;
using RDF.Arcana.API.Features.Users.Exceptions;

namespace RDF.Arcana.API.Features.Authenticate;

public abstract class AuthenticateUser
{
    public class AuthenticateUserQuery : IRequest<AuthenticateUserResult>
    {
        public AuthenticateUserQuery(string username)
        {
            Username = username;
        }

        [Required] public string Username { get; set; }

        [Required] public string Password { get; set; }
    }

    public class AuthenticateUserResult
    {
        private AuthenticateUserResult(User user, string token)
        {
            Id = user.Id;
            Fullname = user.Fullname;
            Username = user.Username;
            Token = token;
            RoleName = user.UserRoles?.UserRoleName;
            Permission = user.UserRoles?.Permissions;
            IsPasswordChanged = user.IsPasswordChanged;
        }

        public int Id { get; set; }

        public string Fullname { get; set; }

        public string Username { get; set; }

        public string RoleName { get; set; }

        public ICollection<string> Permission { get; set; }


        public string Token { get; set; }

        public bool IsPasswordChanged { get; set; }
        public bool ForResetPassword { get; set; }

        public class Handler : IRequestHandler<AuthenticateUserQuery, AuthenticateUserResult>
        {
            private readonly IConfiguration _configuration;
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IConfiguration configuration, IMapper mapper)
            {
                _context = context;
                _configuration = configuration;
                _mapper = mapper;
            }

            public async Task<AuthenticateUserResult> Handle(AuthenticateUserQuery command,
                CancellationToken cancellationToken)
            {
                var user = await _context.Users
                    .Include(x => x.UserRoles)
                    .SingleOrDefaultAsync(x => x.Username == command.Username, cancellationToken);

                //Verify if the credentials is correct
                if (user == null || !BCrypt.Net.BCrypt.Verify(command.Password, user.Password))
                {
                    throw new UsernamePasswordIncorrectException();
                }

                if (!user.IsActive)
                {
                    throw new UserNotActiveException();
                }

                await _context.SaveChangesAsync(cancellationToken);

                var token = GenerateJwtToken(user);

                var result = new AuthenticateUserResult(user, token);

                var results = _mapper.Map<AuthenticateUserResult>(result);

                return results;
            }

            private string GenerateJwtToken(User user)
            {
                var key = _configuration.GetValue<string>("JwtConfig:Key");
                var audience = _configuration.GetValue<string>("JwtConfig:Audience");
                var issuer = _configuration.GetValue<string>("JwtConfig:Issuer");
                var keyBytes = Encoding.ASCII.GetBytes(key);
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim("id", user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.Fullname)
                    }),
                    Expires = DateTime.UtcNow.AddDays(1),
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(keyBytes),
                        SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
        }
    }
}