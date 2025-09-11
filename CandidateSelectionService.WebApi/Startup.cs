using CandidateSelectionService.Auth.Middleware;
using CandidateSelectionService.Auth.Service;
using CandidateSelectionService.Core.Entities;
using CandidateSelectionService.Core.Models.Candidate;
using CandidateSelectionService.Core.Repository;
using CandidateSelectionService.Core.Repository.Auth;
using CandidateSelectionService.Core.Service.Impl;
using CandidateSelectionService.Core.Service.Interfaces;
using CandidateSelectionService.Core.Service.Interfaces.Auth;
using CandidateSelectionService.DAL.EF;
using CandidateSelectionService.DAL.EF.Repository;
using CandidateSelectionService.DAL.EF.Repository.Auth;
using CandidateSelectionService.WebApi.HeaderParameterSwagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;

namespace CandidateSelectionService.WebApi
{
    public static class Startup
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CandidateSelectionServiceApi", Version = "v1" });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }

                c.CustomOperationIds(apiDesc =>
                {
                    return apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)
                        ? methodInfo.Name
                        : null;
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });

                c.OperationFilter<AddRefreshTokenHeaderParameter>();
                c.EnableAnnotations();
                c.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] });
                c.DocInclusionPredicate((name, api) => true);
                c.DocInclusionPredicate((docName, apiDesc) => true);
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"])),
                        ValidateIssuer = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = configuration["Jwt:Audience"],
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddControllers();
        }

        public static void ConfigureUserServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITokenService>(_ =>
            {
                return new JwtTokenService(configuration["Jwt:SecretKey"], configuration["Jwt:Issuer"], configuration["Jwt:Audience"], int.Parse(configuration["Jwt:RefreshTokenExpiryDays"]));
            });

            services.AddScoped<IAuthService>(provider =>
            {
                return new AuthService(provider.GetRequiredService<ITokenService>(),
                    provider.GetRequiredService<IUserRepository>(),
                    provider.GetRequiredService<IRefreshTokenRepository>(),
                    int.Parse(configuration["Jwt:AccessTokenExpiryMinute"]));
            });

            services.AddScoped<ICandidateService, CandidateService>();

            services.AddScoped<ISearchService, SearchService>();

            services.AddSingleton<ISendDataService>(_ =>
            {
                return new TelegramSendService.TelegramSendService(configuration["TelegramService:Token"], long.Parse(configuration["TelegramService:ChatId"]));
            });
        }

        public static void ConfigureRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextFactory<AppDbContext>(options =>
                 options.UseNpgsql(configuration.GetConnectionString("HRConnectionString")));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            services.AddScoped<ICandidateRepository, CandidateRepository>();
            services.AddScoped<IDataCandidateRepository, DataCandidateRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<ISocialNetworkRepository, SocialNetworkRepository>();
            services.AddScoped<IWorkScheduleRepository, WorkScheduleRepository>();

            services.AddScoped<IVerificationRepository, VerificationRepository>();
            services.AddScoped<IVerificationEventRepository, VerificationEventRepository>();
            services.AddScoped<IVerificationEventResultRepository, VerificationEventResultRepository>();
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowAllOrigins");
            app.UseMiddleware<TokenRefreshMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public async static void Migrate(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var dbContext = services.GetRequiredService<AppDbContext>();
                await dbContext.Database.MigrateAsync();

                await SeedTestDataAsync(dbContext, serviceProvider);
            }
        }

        private async static Task SeedTestDataAsync(AppDbContext context, IServiceProvider serviceProvider)
        {
            if (!await context.Users.AnyAsync())
            {
                var testUsers = new[]
                {
                    new User { LastName =  "admin", FirstName ="admin", MiddleName="admin", Login="admin", Salt="d039bc55-073c-4813-974a-8fee3e6ee1a8", Password="y4bSRjwAd7HfMq2D2B7IP6NwnYpvdt3qHMQV94cIXmA="},
                    new User { LastName =  "admin1", FirstName ="admin1", MiddleName="admin1", Login="admin1", Salt="ccf4f882-932a-4d25-9ec8-e344ceef93a0", Password="leyVgXPG7OVhwQvmUHTteh/HhLWN3AConHh3G29gKlA="},
                };

                await context.Users.AddRangeAsync(testUsers);
            }

            if (!await context.WorkSchedules.AnyAsync())
            {
                var testWorkSchedules = new[]
                {
                    new WorkSchedule{Title = "Офис"},
                    new WorkSchedule{Title = "Гибрид"},
                    new WorkSchedule{Title = "Удаленка"},
                };

                await context.WorkSchedules.AddRangeAsync(testWorkSchedules);
            }

            if (!await context.WorkSchedules.AnyAsync())
            {
                var testSocialNetworkTypes = new[]
                {
                    new SocialNetworkType{Title = "Telegram"},
                    new SocialNetworkType{Title = "LinkedIn"},
                    new SocialNetworkType{Title = "Email"},
                };

                await context.SocialNetworkTypes.AddRangeAsync(testSocialNetworkTypes);
            }

            await context.SaveChangesAsync();

            if (!context.Candidates.Any())
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var candidateService = services.GetRequiredService<ICandidateService>();

                    var testCandidates = new[]
                    {
                        new CandidateEditDto("Иванов",
                        "Иван",
                        "Иванович",
                        "ivanov@gmail.com",
                        "89998887788",
                        "Брянск",
                        new DateOnly(1999,5,8),
                        1,
                        new List<SocialNetworkModel>
                        {
                            new SocialNetworkModel(null,
                            "Иванов",
                            "Иван",
                            1,
                            false
                            ),
                            new SocialNetworkModel(null,
                            "Иванов",
                            "Иван",
                            2,
                            false
                            ),
                        }
                        ),
                        new CandidateEditDto("Иванов",
                        "Иван",
                        "Андреевич",
                        "ivanov123@gmail.com",
                        "89998877788",
                        "Брянск",
                        new DateOnly(1987,12,6),
                        3,
                        new List<SocialNetworkModel>
                        {
                            new SocialNetworkModel(null,
                            "Иванов",
                            "Иван",
                            1,
                            false
                            ),
                            new SocialNetworkModel(null,
                            "Иванов",
                            "Иван",
                            2,
                            false
                            ),
                        }
                        ),
                        new CandidateEditDto("Петров",
                        "Пётр",
                        "Петрович",
                        "petrov@gmail.com",
                        "89998834788",
                        "Брянск",
                        new DateOnly(2003,7,12),
                        2,
                        new List<SocialNetworkModel>
                        {
                            new SocialNetworkModel(null,
                            "Петров",
                            "Пётр",
                            1,
                            false
                            ),
                            new SocialNetworkModel(null,
                            "Петров",
                            "Пётр",
                            2,
                            false
                            ),
                        }
                        )
                    };
                    for (int i = 0; i < testCandidates.Length; i++)
                    {
                        await candidateService.CreateCandidateAsync(testCandidates[i], (i + 1) % 2 + 1, CancellationToken.None);
                    }
                }
            }

        }
    }
}
