//
//  ReadOnlyOptionsFactory.cs
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
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;

using MSOptions = Microsoft.Extensions.Options.Options;

namespace Remora.Extensions.Options.Immutable
{
    /// <summary>
    /// Implementation of <see cref="IOptionsFactory{TOptions}"/> that can create read-only option instances.
    /// </summary>
    /// <typeparam name="TOptions">The type of options being requested.</typeparam>
    [PublicAPI]
    public class ReadOnlyOptionsFactory<TOptions> : IOptionsFactory<TOptions> where TOptions : class
    {
        private readonly IEnumerable<ICreateOptions<TOptions>> _creators;

        private readonly IEnumerable<IReadOnlyConfigureOptions<TOptions>> _readOnlyConfigures;
        private readonly IEnumerable<IReadOnlyPostConfigureOptions<TOptions>> _readOnlyPostConfigures;

        private readonly IEnumerable<IConfigureOptions<TOptions>> _configures;
        private readonly IEnumerable<IPostConfigureOptions<TOptions>> _postConfigures;

        private readonly IEnumerable<IValidateOptions<TOptions>>? _validations;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyOptionsFactory{TOptions}"/> class.
        /// </summary>
        /// <param name="creators">The initial creation functions.</param>
        /// <param name="readOnlyConfigures">The configuration functions to run.</param>
        /// <param name="readOnlyPostConfigures">The initialization functions to run.</param>
        /// <param name="configures">The configuration actions to run.</param>
        /// <param name="postConfigures">The initialization actions to run.</param>
        /// <param name="validations">The validations to run.</param>
        public ReadOnlyOptionsFactory
        (
            IEnumerable<ICreateOptions<TOptions>> creators,
            IEnumerable<IReadOnlyConfigureOptions<TOptions>> readOnlyConfigures,
            IEnumerable<IReadOnlyPostConfigureOptions<TOptions>> readOnlyPostConfigures,
            IEnumerable<IConfigureOptions<TOptions>> configures,
            IEnumerable<IPostConfigureOptions<TOptions>> postConfigures,
            IEnumerable<IValidateOptions<TOptions>>? validations = null
        )
        {
            _creators = creators;
            _readOnlyConfigures = readOnlyConfigures;
            _readOnlyPostConfigures = readOnlyPostConfigures;
            _configures = configures;
            _postConfigures = postConfigures;
            _validations = validations;
        }

        /// <inheritdoc />
        public TOptions Create(string name)
        {
            TOptions options = GetCreator(name).Create();
            foreach (var configure in _readOnlyConfigures)
            {
                if (configure is IReadOnlyConfigureNamedOptions<TOptions> namedConfigure)
                {
                    options = namedConfigure.Configure(name, options);
                }
                else if (name == MSOptions.DefaultName)
                {
                    options = configure.Configure(options);
                }
            }

            foreach (var post in _readOnlyPostConfigures)
            {
                options = post.PostConfigure(name, options);
            }

            foreach (var configure in _configures)
            {
                if (configure is IConfigureNamedOptions<TOptions> namedSetup)
                {
                    namedSetup.Configure(name, options);
                }
                else if (name == MSOptions.DefaultName)
                {
                    configure.Configure(options);
                }
            }

            foreach (var post in _postConfigures)
            {
                post.PostConfigure(name, options);
            }

            if (_validations == null)
            {
                return options;
            }

            var failures = new List<string>();
            foreach (var validate in _validations)
            {
                var result = validate.Validate(name, options);
                if (result.Failed)
                {
                    failures.AddRange(result.Failures);
                }
            }

            if (failures.Count > 0)
            {
                throw new OptionsValidationException(name, typeof(TOptions), failures);
            }

            return options;
        }

        /// <summary>
        /// Gets a type that can create an initial instance of the <typeparamref name="TOptions"/> type.
        /// </summary>
        /// <param name="name">The name of the options.</param>
        /// <returns>The creator.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if no named creator exists, and the options type does not define a parameterless or all-optional
        /// constructor.
        /// </exception>
        protected virtual ICreateOptions<TOptions> GetCreator(string name)
        {
            var namedCreator = _creators.FirstOrDefault(c => c.Name == name);
            if (namedCreator is not null)
            {
                return namedCreator;
            }

            var constructors = typeof(TOptions).GetConstructors();

            var parameterless = constructors.FirstOrDefault(c => c.GetParameters().Length == 0);
            if (parameterless is not null)
            {
                return new CreateOptions<TOptions>(name, () => (TOptions)parameterless.Invoke(Array.Empty<object>()));
            }

            var allDefaults = constructors.FirstOrDefault(c => c.GetParameters().All(p => p.HasDefaultValue));
            if (allDefaults is null)
            {
                throw new InvalidOperationException
                (
                    $"{typeof(TOptions)} does not define a parameterless or all-optional constructor."
                );
            }

            var args = allDefaults.GetParameters().Select(p => p.DefaultValue).ToArray();
            return new CreateOptions<TOptions>(name, () => (TOptions)allDefaults.Invoke(args));
        }
    }
}
