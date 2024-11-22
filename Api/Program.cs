using Api.Services;
using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Services.ApiServices;
using Api.Services.ApiServices.Spotify;
using Api.Services.AuthServices.Spotify;
using Serilog;

namespace RandomAlbumApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Debug()
                .WriteTo.Console()
                .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog();
            builder.Services.AddSingleton(Log.Logger);

            Log.Logger.Information("Test");

            // Add services to the container.

            builder.Services.AddDbContext<MusicDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("http://localhost:4200")
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .WithHeaders("Content-Type");
                });
            });

            builder.Services.AddScoped<GptApiService>();
            builder.Services.AddScoped<SpotifyApiService>();
            builder.Services.AddScoped<ISpotifyClient, SpotifyClient>();
            builder.Services.AddScoped<PopulateDbService>();
  
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            app.UseCors();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}