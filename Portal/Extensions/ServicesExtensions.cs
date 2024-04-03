using System.Text;
using Controls.Data.Interceptors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using VoteUp.Portal.Exceptions;
using VoteUp.Portal.GQL.Controllers;
using VoteUp.Portal.GQL.Types;
using VoteUp.Portal.Repositories;
using VoteUp.Portal.Services;
using VoteUp.Portal.Services.Email;
using VoteUp.Portal.Services.Email.Providers;
using VoteUp.Portal.Services.Email.Providers.Sendgrid;
using VoteUp.Portal.Services.Email.Providers.SMTP;
using VoteUp.Portal.Util;
using VoteUp.PortalData;
using VoteUp.PortalData.Helpers;
using VoteUp.PortalData.Interceptors;
using VoteUp.PortalData.Models.Identity;
using VoteUp.PortalData.Models.Identity.Constants;
using VoteUp.PortalData.Models.Interfaces;

namespace VoteUp.Portal.Extensions;

public static class ServicesExtensions
{
	public static IServiceCollection AddVoteUpDb(
		this IServiceCollection services,
		IConfiguration configuration,
		IWebHostEnvironment environment
	)
	{
		services.AddSingleton<AuditInterceptor>();
		services.AddSingleton<SoftDeleteEntitiesInterceptor>();
		services.AddSingleton<CMTimestampInterceptor>();

		services.AddDbContext<VoteUpDbContext>(
			(sp, options) =>
			{
				var auditInterceptor = sp.GetRequiredService<AuditInterceptor>();
				var softDeleteInterceptor = sp.GetRequiredService<SoftDeleteEntitiesInterceptor>();
				var timestampInterceptor = sp.GetRequiredService<CMTimestampInterceptor>();

				options = options.UseLazyLoadingProxies();

				if(environment.IsDevelopment())
					options = options.LogTo(Console.WriteLine);

				options = options.ConfigureWarnings(warnings =>
					warnings.Ignore(CoreEventId.DetachedLazyLoadingWarning)
				);

				if (environment.IsDevelopment())
					options = options.EnableSensitiveDataLogging().EnableDetailedErrors();

				options = options
					.UseNpgsql(configuration.GetConnectionString("Default"))
					.AddInterceptors(timestampInterceptor, softDeleteInterceptor, auditInterceptor)
					.UseSnakeCaseNamingConvention();
			}
		);

		return services;
	}

	public static IServiceCollection AddVoteUpDbPooled(
		this IServiceCollection services,
		IConfiguration configuration,
		IWebHostEnvironment environment
	)
	{
		services.AddSingleton<AuditInterceptor>();
		services.AddSingleton<SoftDeleteEntitiesInterceptor>();
		services.AddSingleton<CMTimestampInterceptor>();

		services.AddPooledDbContextFactory<VoteUpDbContext>(
			(sp, options) =>
			{
				var auditInterceptor = sp.GetRequiredService<AuditInterceptor>();
				var softDeleteInterceptor = sp.GetRequiredService<SoftDeleteEntitiesInterceptor>();
				var timestampInterceptor = sp.GetRequiredService<CMTimestampInterceptor>();

				options = options.UseLazyLoadingProxies();

				if(environment.IsDevelopment())
					options = options.LogTo(Console.WriteLine);

				options = options.ConfigureWarnings(warnings =>
					warnings.Ignore(CoreEventId.DetachedLazyLoadingWarning)
				);

				if (environment.IsDevelopment())
					options = options.EnableSensitiveDataLogging().EnableDetailedErrors();

				options = options
					.UseNpgsql(configuration.GetConnectionString("Default"))
					.AddInterceptors(timestampInterceptor, softDeleteInterceptor, auditInterceptor)
					.UseSnakeCaseNamingConvention();
			}
		);

		services.AddScoped<VoteUpDbContextFactory>();
		services.AddTransient(sp =>
			sp.GetRequiredService<VoteUpDbContextFactory>().CreateDbContext()
		);

		return services;
	}

	public static IServiceCollection AddRepositories(this IServiceCollection services)
	{
		// Register repositories here as transient services

		services.AddTransient<ICityRepository, CityRepository>();
		services.AddTransient<IUserRepository, UserRepository>();
		services.AddTransient<IAuthRepository, AuthRepository>();
		services.AddTransient<IEmailRepository, EmailRepository>();


		return services;
	}
	public static IServiceCollection AddServices(this IServiceCollection services)
    {
		services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ITemplatingService, TemplatingService>();

        return services;
    }

	public static IServiceCollection AddAuthContext(this IServiceCollection services)
	{
		services.AddScoped<IAuthContext>(sp =>
			sp.GetRequiredService<IHttpContextAccessor>().MapToAuthContext()
		);
		return services;
	}

	public static IServiceCollection AddIdentity(
		this IServiceCollection services,
		IConfiguration configuration
	)
	{
		services
			.AddIdentity<User, Role>(options =>
			{
				options.Password.RequireDigit = true;
				options.Password.RequiredLength = 6;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireUppercase = true;
				options.Password.RequireLowercase = true;
				options.User.RequireUniqueEmail = true;

				if (bool.Parse(configuration["Identity:UserLocking"]!))
				{
					options.Lockout.AllowedForNewUsers = true;
					options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
					options.Lockout.MaxFailedAccessAttempts = 6;
				}
			})
			.AddEntityFrameworkStores<VoteUpDbContext>()
			.AddDefaultTokenProviders();

		services.Configure<DataProtectionTokenProviderOptions>(o =>
			o.TokenLifespan = TimeSpan.FromDays(2)
		);

		services.Configure<List<InitialUserConfiguration>>(
			configuration.GetSection("InitialUsers")
		);

		return services;
	}

	public static IServiceCollection AddJwtAuthentication(
		this IServiceCollection services,
		IConfiguration configuration
	)
	{
		var key = Encoding.UTF8.GetBytes(configuration["Tokens:Key"]!);

		services
			.AddAuthentication(x =>
			{
				x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(x =>
			{
				x.RequireHttpsMetadata = true;
				x.SaveToken = true;
				x.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ValidateIssuer = true,
					ValidIssuer = configuration["Tokens:Issuer"],
					ValidAudience = configuration["Tokens:Audience"],
					ValidateAudience = true,
					ValidateLifetime = true
				};
			});

		services.Configure<JwtTokenSettings>(configuration.GetSection("Tokens"));

		return services;
	}

	public static IServiceCollection AddDefaultAuthorization(this IServiceCollection services)
	{
		services.AddAuthorization(options =>
		{
			foreach (string permission in Permission.GetAll())
			{
				options.AddPolicy(
					permission,
					policy =>
						policy.RequireAssertion(context =>
							context.User.HasClaim(c =>
								c.Type == CustomClaimTypes.Permission && c.Value == permission
							)
						)
				);
			}
		});
		return services;
	}


    public static IServiceCollection AddSmtp(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<SmtpSettings>(configuration.GetSection("SMTP"));
        services.AddScoped<IEmailProvider, SmtpEmailProvider>();
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }

    public static IServiceCollection AddSendgrid(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<SendGridSettings>(configuration.GetSection("SENDGRID"));
        services.AddScoped<IEmailProvider, SendGridEmailProvider>();
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }


	public static IServiceCollection ConfigureGraphQL(this IServiceCollection services)
	{
		services
			.AddGraphQLServer()
            .AddAuthorization()
			.AddQueryType<Query>()
			.AddMutationType<Mutation>()
			.AddSubscriptionType<Subscription>()
			.AddType<UserType>()
			.AddTypeExtension<QueryCityResolvers>()
			.AddTypeExtension<MutationCityResolvers>()
			.AddTypeExtension<SubscriptionCityResolvers>()
			.AddTypeExtension<MutationAuthResolvers>()
			.ModifyRequestOptions(opt => opt.IncludeExceptionDetails = true)
			.AddErrorFilter(error =>
			{
				if (error.Exception is ApiException apiException)
				{
					error = error.WithMessage(apiException.Message);

					if (apiException.Code is not null)
						error = error.WithCode(apiException.Code);
				}

				return error;
			});

		return services;
	}

	public static IServiceCollection ConfigureCors(this IServiceCollection services)
	{
		services.AddCors(options =>
		{
			options.AddPolicy(
				"dev",
				pbuilder =>
				{
					pbuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
				}
			);
			options.AddPolicy(
				"prod",
				pbuilder =>
				{
					pbuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); // TODO redefine this later if needed
				}
			);
		});

		return services;
	}
}
