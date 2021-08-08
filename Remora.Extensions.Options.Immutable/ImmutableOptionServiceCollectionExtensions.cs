//
//  ImmutableOptionServiceCollectionExtensions.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) 2017 Jarl Gullberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Remora.Extensions.Options.Immutable
{
    /// <summary>
    /// Defines extension methods for the <see cref="IServiceCollection"/> interface.
    /// </summary>
    public static class ImmutableOptionServiceCollectionExtensions
    {
        /// <summary>
        /// Registers a function used to create a particular type of options.
        /// </summary>
        /// <typeparam name="TOptions">The options type to be created.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="creator">the function used to create the options.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection Configure<TOptions>(this IServiceCollection services, Func<TOptions> creator)
            where TOptions : class
            => services.Configure(Microsoft.Extensions.Options.Options.DefaultName, creator);

        /// <summary>
        /// Registers a function used to create a particular type of options.
        /// </summary>
        /// <typeparam name="TOptions">The options type to be created.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="name">The name of the options instance.</param>
        /// <param name="creator">the function used to create the options.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection Configure<TOptions>
        (
            this IServiceCollection services,
            string? name,
            Func<TOptions> creator
        )
            where TOptions : class
        {
            services.AddOptions();
            services.TryAdd(ServiceDescriptor.Transient<IOptionsFactory<TOptions>, ReadOnlyOptionsFactory<TOptions>>());
            services.AddSingleton<ICreateOptions<TOptions>>
            (
                new CreateOptions<TOptions>(name, creator)
            );

            return services;
        }

        /// <summary>
        /// Registers a function used to configure a particular type of options..
        /// </summary>
        /// <remarks>
        /// These are run before all
        /// <see cref="PostConfigure{TOptions}(IServiceCollection, Func{TOptions, TOptions})"/>.
        /// </remarks>
        /// <typeparam name="TOptions">The options type to be configured.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="configureOptions">The function used to configure the options.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection Configure<TOptions>
        (
            this IServiceCollection services,
            Func<TOptions, TOptions> configureOptions
        )
            where TOptions : class
            => services.Configure(Microsoft.Extensions.Options.Options.DefaultName, configureOptions);

        /// <summary>
        /// Registers a function used to configure a particular type of options..
        /// </summary>
        /// <remarks>
        /// These are run before all
        /// <see cref="PostConfigure{TOptions}(IServiceCollection, Func{TOptions, TOptions})"/>.
        /// </remarks>
        /// <typeparam name="TOptions">The options type to be configured.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="name">The name of the options instance.</param>
        /// <param name="configureOptions">The function used to configure the options.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection Configure<TOptions>
        (
            this IServiceCollection services,
            string? name,
            Func<TOptions, TOptions> configureOptions
        )
            where TOptions : class
        {
            services.AddOptions();
            services.TryAddTransient<IOptionsFactory<TOptions>, ReadOnlyOptionsFactory<TOptions>>();
            services.AddSingleton<IReadOnlyConfigureOptions<TOptions>>
            (
                new ReadOnlyConfigureNamedOptions<TOptions>(name, configureOptions)
            );

            return services;
        }

        /// <summary>
        /// Registers a function used to configure all instances of a particular type of options.
        /// </summary>
        /// <typeparam name="TOptions">The options type to be configured.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="configureOptions">The function used to configure the options.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection ConfigureAll<TOptions>
        (
            this IServiceCollection services,
            Func<TOptions, TOptions> configureOptions
        ) where TOptions : class
            => services.Configure(null, configureOptions);

        /// <summary>
        /// Registers a function used to initialize a particular type of options.
        /// </summary>
        /// <remarks>
        /// These are run after all <see cref="Configure{TOptions}(IServiceCollection, Func{TOptions, TOptions})"/>.
        /// </remarks>
        /// <typeparam name="TOptions">The options type to be configured.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="configureOptions">The function used to configure the options.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection PostConfigure<TOptions>
        (
            this IServiceCollection services,
            Func<TOptions, TOptions> configureOptions
        ) where TOptions : class
            => services.PostConfigure(Microsoft.Extensions.Options.Options.DefaultName, configureOptions);

        /// <summary>
        /// Registers a function used to configure a particular type of options.
        /// </summary>
        /// <typeparam name="TOptions">The options type to be configure.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="name">The name of the options instance.</param>
        /// <param name="configureOptions">The function used to configure the options.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection PostConfigure<TOptions>
        (
            this IServiceCollection services,
            string name,
            Func<TOptions, TOptions> configureOptions
        ) where TOptions : class
        {
            services.AddOptions();
            services.AddSingleton<IReadOnlyPostConfigureOptions<TOptions>>
            (
                new ReadOnlyPostConfigureOptions<TOptions>(name, configureOptions)
            );

            return services;
        }

        /// <summary>
        /// Registers a function used to post configure all instances of a particular type of options.
        /// </summary>
        /// <remarks>
        /// These are run after all <seealso cref="Configure{TOptions}(IServiceCollection, Func{TOptions, TOptions})"/>.
        /// </remarks>
        /// <typeparam name="TOptions">The options type to be configured.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="configureOptions">The function used to configure the options.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection PostConfigureAll<TOptions>
        (
            this IServiceCollection services,
            Action<TOptions> configureOptions
        ) where TOptions : class
            => services.PostConfigure(null, configureOptions);
    }
}
