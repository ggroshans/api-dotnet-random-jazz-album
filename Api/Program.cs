using Api.Services;
using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Services.ApiServices;
using Api.Services.ApiServices.Spotify;
using Serilog;
using Api.Services.ApiServices.Spotify.SpotifyAuthService;

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
            builder.Services.AddDbContext<MusicDbContext>(options =>
                options
                    .UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
                    .UseSnakeCaseNamingConvention());

            builder.Services.AddControllers();
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
            builder.Services.AddScoped<StreamingLinksService>();
            builder.Services.AddScoped<PopulateDbService>();

            var allowAny = "_allowAny";
                builder.Services.AddCors(o =>
                {
                    o.AddPolicy(allowAny, b =>
                        b.WithOrigins("https://frontend.vercel.app", "http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
                });
            var app = builder.Build();
            app.UseCors(allowAny);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<MusicDbContext>();
                db.Database.Migrate(); 
            }

            app.UseCors();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}