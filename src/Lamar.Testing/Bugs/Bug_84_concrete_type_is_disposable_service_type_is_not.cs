using System;
using Lamar.Util;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Lamar.Testing.Bugs
{
    public class Bug_84_concrete_type_is_disposable_service_type_is_not
    {
        public interface ITruck{}

        public class Truck : ITruck, IDisposable
        {
            public void Dispose()
            {
                WasDisposed = true;
            }

            public bool WasDisposed { get; set; }
        }

        [Theory]
        [InlineData(ServiceLifetime.Transient)]
        [InlineData(ServiceLifetime.Singleton)]
        [InlineData(ServiceLifetime.Scoped)]
        public void should_be_disposed_as_constructor_function(ServiceLifetime lifetime)
        {
            var container = Container.For(_ => { _.For<ITruck>().Use<Truck>().Lifetime = lifetime; });

            var nested = container.GetNestedContainer();

            var truck = nested.GetInstance<ITruck>();
            
            nested.Dispose();
            container.Dispose();
            
            truck.As<Truck>().WasDisposed.ShouldBeTrue();
        }
        
        [Theory]
        [InlineData(ServiceLifetime.Transient)]
        [InlineData(ServiceLifetime.Singleton)]
        [InlineData(ServiceLifetime.Scoped)]
        public void should_be_disposed_as_lambda_function(ServiceLifetime lifetime)
        {
            var container = Container.For(_ => { _.For<ITruck>().Use(c => new Truck()).Lifetime = lifetime; });

            var nested = container.GetNestedContainer();

            var truck = nested.GetInstance<ITruck>();
            
            nested.Dispose();
            container.Dispose();
            
            truck.As<Truck>().WasDisposed.ShouldBeTrue();
        }
    }
}