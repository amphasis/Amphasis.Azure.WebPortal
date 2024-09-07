using Microsoft.Extensions.DependencyInjection;

namespace Amphasis.Azure.Tests;

[TestFixture, Parallelizable(ParallelScope.Fixtures)]
internal abstract class BaseTest
{
	[SetUp]
	public void BaseTestSetUp()
	{
		_serviceProvider = new FakeServiceProvider();
	}

	[TearDown]
	public void BaseTestTearDown()
	{
		_serviceProvider!.Dispose();
		_serviceProvider = null;
	}

	protected void AddOrReplace<T>(T service) where T : notnull
	{
		throwIfServiceProviderNotInitialized();
		_serviceProvider!.AddOrReplace(service);
	}

	protected T Get<T>() where T : notnull
	{
		throwIfServiceProviderNotInitialized();
		return _serviceProvider!.GetRequiredService<T>();
	}

	private void throwIfServiceProviderNotInitialized()
	{
		if (_serviceProvider == null)
		{
			throw new InvalidOperationException("Can not use service provider before initialization");
		}
	}

	private FakeServiceProvider? _serviceProvider;
}