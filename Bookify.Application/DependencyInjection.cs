using Bookify.Application.Abstractions.Behaviours;
using Bookify.Domain.Bookings;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Application;

public static class DependencyInjection
{
    #region Public Methods

    public static void AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;
        services.AddMediatR(configuration =>
                            {
                                configuration.RegisterServicesFromAssembly(assembly);

                                configuration.AddOpenBehavior(typeof(LoggingBehaviour<,>));

                                configuration.AddOpenBehavior(typeof(ValidationBehaviour<,>));
                            });

        services.AddAutoMapper(assembly);
        services.AddValidatorsFromAssembly(assembly);

        services.AddTransient<PricingService>();
    }

    #endregion
}
