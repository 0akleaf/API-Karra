using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using APIKarra.Data;
using APIKarra.Models;
using APIKarra.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace APIKarra
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add controllers without modifying serialization behavior
            builder.Services.AddControllers();

            builder.Services.AddDbContext<KarraDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            builder.Services.AddScoped<ProductService>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<OrderService>();
            builder.Services.AddScoped<RatingService>();
            builder.Services.AddScoped<EventService>();
            builder.Services.AddScoped<CartService>();
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // builder.Services.AddTransient<DataSeeder>();



            // === JWT Token Configuration ===
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = builder.Configuration["Jwt:Issuer"],
                            ValidAudience = builder.Configuration["Jwt:Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                            NameClaimType = ClaimTypes.Name,
                            RoleClaimType = ClaimTypes.Role
                        };
                    });

            builder.Services.Configure<IdentityOptions>(options =>
                {
                    // === Password settings ===
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequiredLength = 6;

                    // === Lockout settings ===
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.AllowedForNewUsers = true;

                    // === User settings ===
                    options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                    options.User.RequireUniqueEmail = false;
                });

            builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<KarraDbContext>()
                .AddDefaultTokenProviders();


            // === Authorization Policies ===
            builder.Services.AddAuthorization();
            builder.Services.AddHttpClient();

            // === SWAGGER ===
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            // === SWAGGER Authorization Input ===
            builder.Services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new() { Title = "Kärra Game Corner API", Version = "v1" });

                    // === JWT Auth setup ===
                    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                        Description = "Skriv 'Bearer' följt av ett mellanslag och din token."
                    });

                    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                    {
                        {
                            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                            {
                                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                                {
                                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}
                        }
                    });
                });


            var app = builder.Build();

            // Seed roles before the app starts
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                await SeedRolesAsync(roleManager);
                await SeedAdminUserAsync(userManager);
            }


            // === SWAGGER ===
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // === Middlewares ===
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            // === Endpoints ===
            app.MapControllers();

            app.Run();
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task SeedAdminUserAsync(UserManager<User> userManager)
        {
            var adminEmail = "admin2@kärra.se";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var admin = new User
                {
                    UserName = "Admin2",
                    Email = adminEmail,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    FirstName = "Admin2",
                    LastName = "Nimda2"
                };

                var result = await userManager.CreateAsync(admin, "Admin321!"); // Change this in production!

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }
    }
}
