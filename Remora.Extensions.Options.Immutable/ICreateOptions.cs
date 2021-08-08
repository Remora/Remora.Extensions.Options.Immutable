//
//  ICreateOptions.cs
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

namespace Remora.Extensions.Options.Immutable
{
    /// <summary>
    /// Represents something that creates an initial instance of the <typeparamref name="TOptions"/> type.
    /// </summary>
    /// <typeparam name="TOptions">The options type being created.</typeparam>
    public interface ICreateOptions<out TOptions> where TOptions : class
    {
        /// <summary>
        /// Gets the name of the options instance.
        /// </summary>
        string? Name { get; }

        /// <summary>
        /// Invoked to create a <typeparamref name="TOptions"/> instance.
        /// </summary>
        /// <returns>The created instance.</returns>
        TOptions Create();
    }
}
