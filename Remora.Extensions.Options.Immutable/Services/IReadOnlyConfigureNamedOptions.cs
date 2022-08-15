//
//  IReadOnlyConfigureNamedOptions.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) Jarl Gullberg
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

using JetBrains.Annotations;

namespace Remora.Extensions.Options.Immutable;

/// <summary>
/// Represents something that configures the <typeparamref name="TOptions"/> type, returning the altered instance.
/// </summary>
/// <typeparam name="TOptions">The options type being configured.</typeparam>
[PublicAPI]
public interface IReadOnlyConfigureNamedOptions<TOptions> : IReadOnlyConfigureOptions<TOptions>
    where TOptions : class
{
    /// <summary>
    /// Invoked to configure a <typeparamref name="TOptions"/> instance.
    /// </summary>
    /// <param name="name">The name of the options instance being configured.</param>
    /// <param name="options">The options instance to configure.</param>
    /// <returns>The options, with the alterations.</returns>
    TOptions Configure(string name, TOptions options);
}
