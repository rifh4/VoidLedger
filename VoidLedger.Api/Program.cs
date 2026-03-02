
using VoidLedger.Core;

namespace VoidLedger.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddSingleton<IClock, SystemClock>();
            builder.Services.AddSingleton(_ => new Account(0m));
            builder.Services.AddSingleton<Dictionary<string, decimal>>();
            builder.Services.AddSingleton<Dictionary<string, int>>();
            builder.Services.AddSingleton<List<ActionRecordBase>>();
            builder.Services.AddSingleton(sp =>
                new PriceBook(sp.GetRequiredService<Dictionary<string, decimal>>()));
            builder.Services.AddSingleton(sp =>
                new Portfolio(sp.GetRequiredService<Dictionary<string, int>>()));
            builder.Services.AddSingleton<TradeService>();
            builder.Services.AddSingleton<ILedgerService, LedgerService>();



            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
