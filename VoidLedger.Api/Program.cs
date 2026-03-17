using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VoidLedger.Api.Data;
using VoidLedger.Api.Data.Stores;
using VoidLedger.Core;
using VoidLedger.Core.Services;

namespace VoidLedger.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<IClock, SystemClock>();

            // Enable SQL retry handling for the Azure-hosted database.
            builder.Services.AddDbContext<VoidLedgerDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("VoidLedgerDb"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure()));

            builder.Services.AddScoped<ILedgerStore, EfLedgerStore>();
            builder.Services.AddScoped<ITradeService, TradeService>();
            builder.Services.AddScoped<ILedgerService, LedgerService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            WebApplication app = builder.Build();

            // In non-development, return ProblemDetails instead of raw exception details.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler(handlerApp =>
                {
                    handlerApp.Run(async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        context.Response.ContentType = "application/problem+json";

                        ProblemDetails problem = new ProblemDetails
                        {
                            Title = "UnexpectedError",
                            Status = StatusCodes.Status500InternalServerError,
                            Detail = "An unexpected error occurred."
                        };

                        problem.Extensions["code"] = "Unknown";

                        await context.Response.WriteAsJsonAsync(problem);
                    });
                });
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.MapControllers();

            app.Run();
        }
    }
}