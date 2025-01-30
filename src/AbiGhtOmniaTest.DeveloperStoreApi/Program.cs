using AbiGhtOmniaTest.Application.Commands;
using AbiGhtOmniaTest.Application.Interfaces;
using AbiGhtOmniaTest.Application.Queries;
using AbiGhtOmniaTest.Application.Services;
using AbiGhtOmniaTest.Domain.Authentication;
using AbiGhtOmniaTest.Domain.Entities;
using AbiGhtOmniaTest.Domain.Interfaces;
using AbiGhtOmniaTest.Infraestructure.Data;
using AbiGhtOmniaTest.Infraestructure.Repository;
using MediatR;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog(Log.Logger);
builder.Services.AddAuthorization();

builder.Services.AddDbContext<DeveloperStoreDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Developer Store API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
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
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

var authGroup = app.MapGroup("/auth");

authGroup.MapPost("/login", async (IAuthService authService, LoginRequest request) =>
{
    try
    {
        var response = await authService.LoginAsync(request);
        if (response == null)
        {
            return Results.Unauthorized();
        }
        return Results.Ok(response);
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during login: {Message}", ex.Message);
        return Results.Problem("An unexpected error occurred.");
    }
})
.WithOpenApi()
.Produces<LoginResponse>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status401Unauthorized)
.Produces(StatusCodes.Status500InternalServerError);

var userGroup = app.MapGroup("/users").RequireAuthorization();

userGroup.MapPost("/", async (IUserRepository userRepository, User user) =>
{
    try
    {
        await userRepository.AddUserAsync(user);
        return Results.Created($"/users/{user.Id}", user);
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while adding a user: {Message}", ex.Message);
        return Results.Problem("An unexpected error occurred.");
    }
})
.WithOpenApi()
.Produces<User>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status500InternalServerError);

userGroup.MapGet("/", async (IMediator mediator, int pageNumber, int pageSize) =>
{
    try
    {
        var query = new PaginatedQuery { PageNumber = pageNumber, PageSize = pageSize };
        var users = await mediator.Send(query);
        return Results.Ok(users);
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while retrieving users: {Message}", ex.Message);
        return Results.Problem("An unexpected error occurred.");
    }
})
.WithOpenApi()
.Produces<IEnumerable<User>>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status500InternalServerError);

userGroup.MapGet("/{id}", async (IMediator mediator, Guid id) =>
{
    try
    {
        var query = new GetUserByIdQuery(id);
        var user = await mediator.Send(query);
        return user != null ? Results.Ok(user) : Results.NotFound();
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while retrieving a user by ID: {Message}", ex.Message);
        return Results.Problem("An unexpected error occurred.");
    }
})
.WithOpenApi()
.Produces<User>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status500InternalServerError);

userGroup.MapPut("/{id}", async (IMediator mediator, Guid id, User updatedUser) =>
{
    try
    {
        var command = new UpdateUserCommand();
        var result = await mediator.Send(command);
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while updating a user: {Message}", ex.Message);
        return Results.Problem("An unexpected error occurred.");
    }
})
.WithOpenApi()
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status500InternalServerError);

userGroup.MapDelete("/{id}", async (IMediator mediator, Guid id) =>
{
    try
    {
        var command = new DeleteUserCommand { Id = id };
        await mediator.Send(command);
        return Results.Ok();
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while deleting a user: {Message}", ex.Message);
        return Results.Problem("An unexpected error occurred.");
    }
})
.WithOpenApi()
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status500InternalServerError);


var salesGroup = app.MapGroup("/sales").RequireAuthorization();

salesGroup.MapPost("/", async (IMediator mediator, CreateSaleCommand command) =>
{
    try
    {
        var saleId = await mediator.Send(command);
        return Results.Created($"/sales/{saleId}", saleId);
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while creating a sale: {Message}", ex.Message);
        return Results.Problem("An unexpected error occurred.");
    }
})
.WithOpenApi()
.Produces<Guid>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status500InternalServerError);

salesGroup.MapGet("/", async (IMediator mediator, int pageNumber, int pageSize) =>
{
    try
    {
        var query = new PaginatedQuery { PageNumber = pageNumber, PageSize = pageSize };
        var result = await mediator.Send(query);
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while retrieving sales: {Message}", ex.Message);
        return Results.Problem("An unexpected error occurred.");
    }
})
.WithOpenApi()
.Produces<PaginatedResult<Sale>>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status500InternalServerError);

salesGroup.MapGet("/{id}", async (IMediator mediator, Guid id) =>
{
    try
    {
        var query = new GetSaleQuery(id);
        var sale = await mediator.Send(query);
        return Results.Ok(sale);
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while retrieving a sale by ID: {Message}", ex.Message);
        return Results.Problem("An unexpected error occurred.");
    }
})
.WithOpenApi()
.Produces<Sale>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status500InternalServerError);

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred: {Message}", ex.Message);
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new { Error = "An unexpected error occurred." });
    }
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

await app.RunAsync();
