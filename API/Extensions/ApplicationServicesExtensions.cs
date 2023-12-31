
using API.Errors;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
  public static class ApplicationServicesExtensions
  {
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration config)
    {
      // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
      services.AddEndpointsApiExplorer();
      services.AddSwaggerGen();

      services.AddDbContext<DataContext>(opt =>
      {
        opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
      });

      services.AddScoped<IProductRepository, ProductRepository>();
      services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
      services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

      services.AddCors(opt =>
      {
        opt.AddPolicy("CorsPolicy", policy =>
  {
    policy.AllowAnyMethod().AllowAnyHeader().WithOrigins("http://localhost:5001");
  });
      });

      services.Configure<ApiBehaviorOptions>(options =>
                 {
                   options.InvalidModelStateResponseFactory = actionContext =>
             {
               var errors = actionContext.ModelState
             .Where(e => e.Value.Errors.Count > 0)
             .SelectMany(x => x.Value.Errors)
             .Select(x => x.ErrorMessage).ToArray();

               var errorResponse = new ApiValidationErrorResponse
               {
                 Errors = errors
               };

               return new BadRequestObjectResult(errorResponse);
             };
                 });

      services.AddCors(opt =>
      {
        opt.AddPolicy("CorsPolicy", policy =>
                    {
                      policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200");
                    });
      });

      return services;
    }
  }
}