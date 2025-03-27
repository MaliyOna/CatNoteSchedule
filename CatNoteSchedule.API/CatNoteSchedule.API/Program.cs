using CatNoteSchedule.API.Middlewares;
using CatNoteSchedule.BLL.DI;
using Serilog;

namespace CatNoteSchedule.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var connection = builder.Configuration.GetConnectionString("DefaultConnection");

        //Log.Logger = new LoggerConfiguration()
        //    .Enrich.FromLogContext()
        //    .WriteTo.MSSqlServer(
        //        connectionString: connection,
        //        sinkOptions: new MSSqlServerSinkOptions
        //        {
        //            TableName = "Logs",
        //        }
        //    )
        //    .CreateLogger();

        builder.Services.AddBLLServices(connection);

        builder.Host.UseSerilog();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.MapControllers();

        app.Run();
    }
}
