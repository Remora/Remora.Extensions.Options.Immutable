//
//  ReadOnlyConfigureNamedOptions.cs
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
using JetBrains.Annotations;

namespace Remora.Extensions.Options.Immutable;

/// <summary>
/// Implementation of <see cref="IReadOnlyConfigureNamedOptions{TOptions}"/> that configures a read-only options type.
/// </summary>
/// <typeparam name="TOptions">Options type being configured.</typeparam>
[PublicAPI]
public record ReadOnlyConfigureNamedOptions<TOptions>(string? Name, Func<TOptions, TOptions> Function)
    : IReadOnlyConfigureNamedOptions<TOptions> where TOptions : class
{
    /// <inheritdoc />
    public TOptions Configure(TOptions options)
        => Configure(Microsoft.Extensions.Options.Options.DefaultName, options);

    /// <inheritdoc />
    public TOptions Configure(string name, TOptions options)
    {
        // Null name is used to configure all named options.
        if (this.Name is null || name == this.Name)
        {
            return this.Function.Invoke(options);
        }

        return options;
    }
}
