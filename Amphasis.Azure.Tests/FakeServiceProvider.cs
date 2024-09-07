using System.Collections.Concurrent;
using System.Reflection;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;

namespace Amphasis.Azure.Tests
{
	public sealed class FakeServiceProvider : IServiceProvider, IDisposable
	{
		public void AddOrReplace<T>(T service) where T : notnull
		{
			throwIfDisposed();

			addOrReplace(typeof(T), service);
		}

		public object GetService(Type serviceType)
		{
			throwIfDisposed();

			if (_services.TryGetValue(serviceType, out var resolvedService))
			{
				return resolvedService;
			}

			if (serviceType.IsAbstract || serviceType.IsInterface)
			{
				return createFake(serviceType);
			}

			var service = ActivatorUtilities.CreateInstance(this, serviceType);
			addOrReplace(serviceType, service);

			return service;
		}

		public void Dispose()
		{
			if (_isDisposed)
			{
				return;
			}

			var exceptions = new List<Exception>();

			foreach (var (_, service) in _services)
			{
				try
				{
					disposeIfImplemented(service);
				}
				catch (Exception exception)
				{
					exceptions.Add(exception);
				}
			}

			if (exceptions.Count > 0)
			{
				throw new AggregateException(exceptions);
			}

			_isDisposed = true;
			_services.Clear();
		}

		private void throwIfDisposed()
		{
			if (_isDisposed)
			{
				throw new InvalidOperationException("FakeServiceProvider is disposed");
			}
		}

		private void addOrReplace(Type serviceType, object service)
		{
			if (_services.TryGetValue(serviceType, out var registeredService))
			{
				disposeIfImplemented(registeredService);
			}

			_services[serviceType] = service;
		}

		private static void disposeIfImplemented(object service)
		{
			if (service is IDisposable disposable)
			{
				disposable.Dispose();
			}

			if (service is IAsyncDisposable asyncDisposable)
			{
				Task.Run(asyncDisposable.DisposeAsync).RunSynchronously();
			}
		}

		private static object createFake(Type serviceType)
		{
			var factory = FakeFactories.GetOrAdd(
				serviceType,
				type => CreateFakeGenericMethod
					.MakeGenericMethod(type)
					.CreateDelegate<Func<object>>());

			return factory();
		}

		private bool _isDisposed;

		private readonly Dictionary<Type, object> _services = new();

		private static readonly ConcurrentDictionary<Type, Func<object>> FakeFactories = new();

		private static readonly MethodInfo CreateFakeGenericMethod = typeof(A)
			.GetMethods(BindingFlags.Static | BindingFlags.Public)
			.Single(x =>
				x.Name == nameof(A.Fake) &&
				x.GetParameters().Length == 0 &&
				x.IsGenericMethodDefinition);
	}
}