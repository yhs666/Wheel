﻿using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Wheel.EntityFrameworkCore;

namespace Wheel.EventBus
{
    public static class EventBusExtensions
    {
        public static IServiceCollection AddLocalEventBus(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
                                    .Where(x => !x.Contains("Microsoft.") && !x.Contains("System."))
                                    .Select(x => Assembly.Load(AssemblyName.GetAssemblyName(x))).ToArray());
            });
            return services;
        }
        public static IServiceCollection AddDistributedEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCap(x =>
            {
                x.UseEntityFramework<WheelDbContext>();

                x.UseSqlite(configuration.GetConnectionString("Default"));

                x.UseRabbitMQ(configuration["RabbitMQ:ConnectionString"]);
            });
            return services;
        }
    }
}
