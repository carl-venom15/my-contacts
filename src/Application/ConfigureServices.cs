using Application.CQRS.Contacts.Commands;
using Application.Mapper.Profiles;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<InsertContactCommandValidator>();
            services.AddAutoMapper(Assembly.GetAssembly(typeof(ContactProfile)));
            services.AddMediatR(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
