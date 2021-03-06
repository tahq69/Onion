﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Onion.Application.DTOs;
using Onion.Application.Interfaces;
using Onion.Identity.Models;
using Onion.Identity.Services;
using System;
using System.Reflection;
using System.Text;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Onion.Domain.Settings;
using Onion.Identity.Interfaces;
using Onion.Identity.Repositories;
using IdentityDbContext = Onion.Identity.Contexts.IdentityDbContext;

namespace Onion.Identity
{
    /// <summary>
    /// Identity module DI.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds all required services for identity module.
        /// </summary>
        /// <param name="services">DI service.</param>
        /// <param name="configuration">Application configuration.</param>
        public static void AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<IdentityDbContext>(options => options
                    .UseInMemoryDatabase("IdentityDb")
                    .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)));
            }
            else
            {
                services.AddDbContext<IdentityDbContext>(options =>
                    options.UseSqlServer(
                        configuration.GetConnectionString("IdentityConnection"),
                        b =>
                        {
                            Assembly asm = typeof(IdentityDbContext).Assembly;
                            b.MigrationsAssembly(asm.FullName);
                        }));
            }

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<IJwtService, JwtService>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IIdentityResultParser, IdentityResultParser>();

            services.Configure<JwtSettings>(configuration.GetSection("JWTSettings"));
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        ValidIssuer = configuration[$"JWTSettings:{nameof(JwtSettings.Issuer)}"],
                        ValidAudience = configuration[$"JWTSettings:{nameof(JwtSettings.Audience)}"],
                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(configuration[$"JWTSettings:{nameof(JwtSettings.Key)}"])),
                    };
                    o.Events = new JwtBearerEvents()
                    {
                        OnAuthenticationFailed = c =>
                        {
                            c.NoResult();
                            c.Response.StatusCode = 500;
                            c.Response.ContentType = "text/plain";
                            return c.Response.WriteAsync(c.Exception.ToString());
                        },
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(new Response<string>("You are not Authorized"));
                            return context.Response.WriteAsync(result);
                        },
                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = 403;
                            context.Response.ContentType = "application/json";
                            var result =
                                JsonConvert.SerializeObject(
                                    new Response<string>("You are not authorized to access this resource"));
                            return context.Response.WriteAsync(result);
                        },
                    };
                });
        }
    }
}