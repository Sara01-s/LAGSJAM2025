using System;
using System.Collections.Generic;

public sealed class Services : IDisposable {
	public static Services Instance => _instance ??= new Services();
	private static Services _instance = null;

	private readonly Dictionary<Type, Type> _serviceTypes = new();
	private readonly Dictionary<Type, object> _serviceInstances = new();
	private readonly Dictionary<Type, object> _implementationInstances = new();

	public void RegisterService<TInterface, TImplementation>() where TImplementation : TInterface {
		_serviceTypes[typeof(TInterface)] = typeof(TImplementation);
	}

	public void RegisterInstance<TInterface>(TInterface instance) {
		_serviceInstances[typeof(TInterface)] = instance;
	}

	public void UnregisterService(Type type) {
		_serviceInstances.Remove(type);
		_serviceTypes.Remove(type);
	}

	public void UnregisterService<TInterface>() {
		_serviceInstances.Remove(typeof(TInterface));
		_serviceTypes.Remove(typeof(TInterface));
	}

	public TInterface GetService<TInterface>() {
		return (TInterface)GetService(typeof(TInterface));
	}

	public object GetService(Type serviceType) {
		if (_serviceInstances.TryGetValue(serviceType, out var instance)) {
			return instance;
		}

		if (!_serviceTypes.TryGetValue(serviceType, out var implementationType)) {
			throw new Exception($"Service of type {serviceType} not registered.");
		}

		if (_implementationInstances.TryGetValue(implementationType, out instance)) {
			_serviceInstances[serviceType] = instance;
			return instance;
		}

		var serviceConstructor = implementationType.GetConstructors()[0];
		var constructorParameters = serviceConstructor.GetParameters();
		var parameterInstances = new List<object>();

		// Resolve dependencies for constructor parameters.
		foreach (var parameter in constructorParameters) {
			var service = GetService(parameter.ParameterType);
			parameterInstances.Add(service);
		}

		instance = serviceConstructor.Invoke(parameterInstances.ToArray());

		_implementationInstances[implementationType] = instance;
		_serviceInstances[serviceType] = instance;

		return instance;
	}

	public void Dispose() {
		foreach (var service in _serviceInstances.Values) {
			if (service is IDisposable disposable) {
				disposable.Dispose();
			}
		}

		foreach (var service in _serviceInstances.Values) {
			UnregisterService(service.GetType());
		}
	}

}
