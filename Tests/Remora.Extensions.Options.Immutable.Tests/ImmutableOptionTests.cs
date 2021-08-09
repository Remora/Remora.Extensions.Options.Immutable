//
//  ImmutableOptionTests.cs
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
using Microsoft.Extensions.Options;
using Remora.Extensions.Options.Immutable.Tests.Data;
using Xunit;

namespace Remora.Extensions.Options.Immutable.Tests
{
    /// <summary>
    /// Tests various functionality of immutable options.
    /// </summary>
    public class ImmutableOptionTests
    {
        /// <summary>
        /// Tests hybrid usage with both immutable and mutable properties.
        /// </summary>
        public class Hybrid
        {
            /// <summary>
            /// Tests whether mutable and immutable configuration calls can be combined.
            /// </summary>
            [Fact]
            public void CanCombineImmutableAndMutableConfigurations()
            {
                var value = "initial";
                var flag = false;

                var services = new ServiceCollection()
                    .Configure(() => new ExplicitOptionsWithMutableProperty(value, flag))
                    .Configure<ExplicitOptionsWithMutableProperty>(opt => opt with { Value = value + value })
                    .Configure<ExplicitOptionsWithMutableProperty>(opt => opt.Mutable = 1)
                    .BuildServiceProvider();

                var options = services.GetRequiredService<IOptions<ExplicitOptionsWithMutableProperty>>().Value;
                Assert.Equal(value + value, options.Value);
                Assert.Equal(flag, options.Flag);
                Assert.Equal(1, options.Mutable);
            }

            /// <summary>
            /// Tests whether mutable and immutable post-configuration calls can be combined.
            /// </summary>
            [Fact]
            public void CanCombineImmutableAndMutablePostConfigurations()
            {
                var value = "initial";
                var flag = false;

                var services = new ServiceCollection()
                    .Configure(() => new ExplicitOptionsWithMutableProperty(value, flag))
                    .PostConfigure<ExplicitOptionsWithMutableProperty>(opt => opt with { Value = value + value })
                    .PostConfigure<ExplicitOptionsWithMutableProperty>(opt => opt.Mutable = 1)
                    .BuildServiceProvider();

                var options = services.GetRequiredService<IOptions<ExplicitOptionsWithMutableProperty>>().Value;
                Assert.Equal(value + value, options.Value);
                Assert.Equal(flag, options.Flag);
                Assert.Equal(1, options.Mutable);
            }
        }

        /// <summary>
        /// Tests options with parameterless constructors.
        /// </summary>
        public class Parameterless
        {
            /// <summary>
            /// Tests whether an option that has a parameterless constructor can be created.
            /// </summary>
            [Fact]
            public void CanCreate()
            {
                var services = new ServiceCollection()
                    .Configure<ParameterlessOptions>(opt => opt with { Flag = true })
                    .BuildServiceProvider();

                var options = services.GetRequiredService<IOptions<ParameterlessOptions>>().Value;
                Assert.Null(options.Value);
                Assert.True(options.Flag);
            }
        }

        /// <summary>
        /// Tests options with all-optional constructors.
        /// </summary>
        public class AllOptional
        {
            /// <summary>
            /// Tests whether an option that has a parameterless constructor can be created.
            /// </summary>
            [Fact]
            public void CanCreate()
            {
                var services = new ServiceCollection()
                    .Configure<AllOptionalOptions>(opt => opt with { Flag = false })
                    .BuildServiceProvider();

                var options = services.GetRequiredService<IOptions<AllOptionalOptions>>().Value;
                Assert.Equal("initial", options.Value);
                Assert.False(options.Flag);
            }
        }

        /// <summary>
        /// Tests explicitly initialized options.
        /// </summary>
        public class ExplicitInitialization
        {
            /// <summary>
            /// Tests whether an option that is explicitly initialized can be created.
            /// </summary>
            [Fact]
            public void CanCreate()
            {
                var value = "initial";
                var flag = false;

                var services = new ServiceCollection()
                    .Configure(() => new ExplicitOptions(value, flag))
                    .BuildServiceProvider();

                var options = services.GetRequiredService<IOptions<ExplicitOptions>>().Value;
                Assert.Equal(value, options.Value);
                Assert.Equal(flag, options.Flag);
            }

            /// <summary>
            /// Tests whether an option that is explicitly initialized can be created.
            /// </summary>
            [Fact]
            public void InstantiationThrowsIfNoInitialStateIsPresent()
            {
                var services = new ServiceCollection()
                    .Configure<ExplicitOptions>(opt => opt with { Flag = true })
                    .BuildServiceProvider();

                Assert.Throws<InvalidOperationException>
                (
                    () => services.GetRequiredService<IOptions<ExplicitOptions>>().Value
                );
            }
        }

        /// <summary>
        /// Tests whether an option that is explicitly initialized can be configured after initial creation.
        /// </summary>
        [Fact]
        public void CanBeConfigured()
        {
            var value = "initial";
            var flag = false;

            var services = new ServiceCollection()
                .Configure(() => new ExplicitOptions(value, flag))
                .Configure<ExplicitOptions>(opt => opt with { Flag = !flag })
                .BuildServiceProvider();

            var options = services.GetRequiredService<IOptions<ExplicitOptions>>().Value;
            Assert.Equal(value, options.Value);
            Assert.Equal(!flag, options.Flag);
        }

        /// <summary>
        /// Tests whether further configuration to an option that is explicitly initialized is cumulative over
        /// multiple configure calls.
        /// </summary>
        [Fact]
        public void ConfigurationsAreCumulative()
        {
            var value = "initial";
            var flag = false;

            var services = new ServiceCollection()
                .Configure(() => new ExplicitOptions(value, flag))
                .Configure<ExplicitOptions>(opt => opt with { Flag = !flag })
                .Configure<ExplicitOptions>(opt => opt with { Value = value + value })
                .BuildServiceProvider();

            var options = services.GetRequiredService<IOptions<ExplicitOptions>>().Value;
            Assert.Equal(value + value, options.Value);
            Assert.Equal(!flag, options.Flag);
        }

        /// <summary>
        /// Tests whether an option that is explicitly initialized can be post-configured after initial creation.
        /// </summary>
        [Fact]
        public void CanBePostConfigured()
        {
            var value = "initial";
            var flag = false;

            var services = new ServiceCollection()
                .Configure(() => new ExplicitOptions(value, flag))
                .PostConfigure<ExplicitOptions>(opt => opt with { Flag = !flag })
                .BuildServiceProvider();

            var options = services.GetRequiredService<IOptions<ExplicitOptions>>().Value;
            Assert.Equal(value, options.Value);
            Assert.Equal(!flag, options.Flag);
        }

        /// <summary>
        /// Tests whether further post-configuration to an option that is explicitly initialized is cumulative over
        /// multiple configure calls.
        /// </summary>
        [Fact]
        public void PostConfigurationsAreCumulative()
        {
            var value = "initial";
            var flag = false;

            var services = new ServiceCollection()
                .Configure(() => new ExplicitOptions(value, flag))
                .PostConfigure<ExplicitOptions>(opt => opt with { Flag = !flag })
                .PostConfigure<ExplicitOptions>(opt => opt with { Value = value + value })
                .BuildServiceProvider();

            var options = services.GetRequiredService<IOptions<ExplicitOptions>>().Value;
            Assert.Equal(value + value, options.Value);
            Assert.Equal(!flag, options.Flag);
        }

        /// <summary>
        /// Tests whether further post-configuration to an option that is explicitly initialized run after normal
        /// configurations.
        /// </summary>
        [Fact]
        public void PostConfigurationsRunAfterConfigurations()
        {
            var value = "initial";
            var flag = false;

            var services = new ServiceCollection()
                .Configure(() => new ExplicitOptions(value, flag))
                .Configure<ExplicitOptions>(opt => opt with { Value = "before" })
                .PostConfigure<ExplicitOptions>(opt =>
                {
                    if (opt.Value == "before")
                    {
                        return opt with { Value = value + value };
                    }

                    return opt;
                })
                .BuildServiceProvider();

            var options = services.GetRequiredService<IOptions<ExplicitOptions>>().Value;
            Assert.Equal(value + value, options.Value);
            Assert.Equal(flag, options.Flag);
        }

        /// <summary>
        /// Tests whether further post-configuration to an option that is explicitly initialized run after normal
        /// configurations, independently of the registration order of the steps.
        /// </summary>
        [Fact]
        public void PostConfigurationsRunAfterConfigurationsIndependentOfRegistrationOrder()
        {
            var value = "initial";
            var flag = false;

            var services = new ServiceCollection()
                .Configure(() => new ExplicitOptions(value, flag))
                .PostConfigure<ExplicitOptions>(opt =>
                {
                    if (opt.Value == "before")
                    {
                        return opt with { Value = value + value };
                    }

                    return opt;
                })
                .Configure<ExplicitOptions>(opt => opt with { Value = "before" })
                .BuildServiceProvider();

            var options = services.GetRequiredService<IOptions<ExplicitOptions>>().Value;
            Assert.Equal(value + value, options.Value);
            Assert.Equal(flag, options.Flag);
        }

        /// <summary>
        /// Tests whether explicitly initialized options can be validated where the validation produces an
        /// unsuccessful result.
        /// </summary>
        [Fact]
        public void CanBeValidatedUnsuccessfully()
        {
            var value = "initial";
            var flag = false;

            var services = new ServiceCollection()
                .Configure(() => new ExplicitOptions(value, flag))
                .AddSingleton<IValidateOptions<ExplicitOptions>>(_ => new TestValidator(value, !flag))
                .BuildServiceProvider();

            Assert.Throws<OptionsValidationException>
            (
                () => services.GetRequiredService<IOptions<ExplicitOptions>>().Value
            );
        }

        /// <summary>
        /// Tests whether explicitly initialized options can be validated where the validation produces a
        /// successful result.
        /// </summary>
        [Fact]
        public void CanBeValidatedSuccessfully()
        {
            var value = "initial";
            var flag = false;

            var services = new ServiceCollection()
                .Configure(() => new ExplicitOptions(value, flag))
                .AddSingleton<IValidateOptions<ExplicitOptions>>(_ => new TestValidator(value, flag))
                .BuildServiceProvider();

            var options = services.GetRequiredService<IOptions<ExplicitOptions>>().Value;
            Assert.Equal(value, options.Value);
            Assert.Equal(flag, options.Flag);
        }

        private record TestValidator(string RequiredValue, bool RequiredFlag) : IValidateOptions<ExplicitOptions>
        {
            /// <inheritdoc/>
            public ValidateOptionsResult Validate(string name, ExplicitOptions options)
            {
                if (options.Value != this.RequiredValue)
                {
                    return ValidateOptionsResult.Fail("Value did not match.");
                }

                if (options.Flag != this.RequiredFlag)
                {
                    return ValidateOptionsResult.Fail("Flag did not match.");
                }

                return ValidateOptionsResult.Success;
            }
        }

        /// <summary>
        /// Tests specific named functionality.
        /// </summary>
        public class Named
        {
            /// <summary>
            /// Tests whether two different options with different names can be created independently.
            /// </summary>
            [Fact]
            public void CanCreateIndependently()
            {
                var first = "first";
                var second = "second";

                var services = new ServiceCollection()
                    .Configure(first, () => new ExplicitOptions(first, false))
                    .Configure(second, () => new ExplicitOptions(second, false))
                    .BuildServiceProvider();

                var accessor = services.GetRequiredService<IOptionsSnapshot<ExplicitOptions>>();
                var firstOptions = accessor.Get(first);
                var secondOptions = accessor.Get(second);

                Assert.Equal(first, firstOptions.Value);
                Assert.Equal(second, secondOptions.Value);
            }

            /// <summary>
            /// Tests whether an option that is explicitly initialized can be configured independently after initial
            /// creation.
            /// </summary>
            [Fact]
            public void CanBeConfiguredIndependently()
            {
                var first = "first";
                var second = "second";

                var services = new ServiceCollection()
                    .Configure(first, () => new ExplicitOptions(first, false))
                    .Configure(second, () => new ExplicitOptions(second, false))
                    .Configure<ExplicitOptions>(first, opt => opt with { Value = first + first })
                    .Configure<ExplicitOptions>(second, opt => opt with { Value = second + second })
                    .BuildServiceProvider();

                var accessor = services.GetRequiredService<IOptionsSnapshot<ExplicitOptions>>();
                var firstOptions = accessor.Get(first);
                var secondOptions = accessor.Get(second);

                Assert.Equal(first + first, firstOptions.Value);
                Assert.Equal(second + second, secondOptions.Value);
            }

            /// <summary>
            /// Tests whether an option that is explicitly initialized can be independently post-configured after initial
            /// creation.
            /// </summary>
            [Fact]
            public void CanBePostConfiguredIndependently()
            {
                var first = "first";
                var second = "second";

                var services = new ServiceCollection()
                    .Configure(first, () => new ExplicitOptions(first, false))
                    .Configure(second, () => new ExplicitOptions(second, false))
                    .PostConfigure<ExplicitOptions>(first, opt => opt with { Value = first + first })
                    .PostConfigure<ExplicitOptions>(second, opt => opt with { Value = second + second })
                    .BuildServiceProvider();

                var accessor = services.GetRequiredService<IOptionsSnapshot<ExplicitOptions>>();
                var firstOptions = accessor.Get(first);
                var secondOptions = accessor.Get(second);

                Assert.Equal(first + first, firstOptions.Value);
                Assert.Equal(second + second, secondOptions.Value);
            }

            /// <summary>
            /// Tests whether configuration applied with ConfigureAll applies to all options.
            /// </summary>
            [Fact]
            public void ConfigureAllAppliesToAll()
            {
                var first = "first";
                var second = "second";
                var flag = false;

                var services = new ServiceCollection()
                    .Configure(first, () => new ExplicitOptions(first, flag))
                    .Configure(second, () => new ExplicitOptions(second, flag))
                    .ConfigureAll<ExplicitOptions>(opt => opt with { Flag = !flag })
                    .BuildServiceProvider();

                var accessor = services.GetRequiredService<IOptionsSnapshot<ExplicitOptions>>();
                var firstOptions = accessor.Get(first);
                var secondOptions = accessor.Get(second);

                Assert.Equal(first, firstOptions.Value);
                Assert.Equal(!flag, firstOptions.Flag);
                Assert.Equal(second, secondOptions.Value);
                Assert.Equal(!flag, secondOptions.Flag);
            }

            /// <summary>
            /// Tests whether configuration applied with PostConfigureAll applies to all options.
            /// </summary>
            [Fact]
            public void PostConfigureAllAppliesToAll()
            {
                var first = "first";
                var second = "second";
                var flag = false;

                var services = new ServiceCollection()
                    .Configure(first, () => new ExplicitOptions(first, flag))
                    .Configure(second, () => new ExplicitOptions(second, flag))
                    .PostConfigureAll<ExplicitOptions>(opt => opt with { Flag = !flag })
                    .BuildServiceProvider();

                var accessor = services.GetRequiredService<IOptionsSnapshot<ExplicitOptions>>();
                var firstOptions = accessor.Get(first);
                var secondOptions = accessor.Get(second);

                Assert.Equal(first, firstOptions.Value);
                Assert.Equal(!flag, firstOptions.Flag);
                Assert.Equal(second, secondOptions.Value);
                Assert.Equal(!flag, secondOptions.Flag);
            }
        }
    }
}
