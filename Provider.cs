using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DependencyInjectionContainerLibrary.Exceptions;

namespace DependencyInjectionContainerLibrary
{
    public class DependencyProvider
    {
        private DependencyConfiguration _configuration;

        public List<Type> listInstances;
        public List<object> listPoint;
        
        public DependencyProvider(DependencyConfiguration configuration)
        {
            _configuration = configuration;
            listInstances = new List<Type>();
            listPoint = new List<object>();
        }
        
        public Object Resolve<TType>() where TType : class
        {
            Type type = typeof(TType);
            Type mainType = null;
            if (typeof(IEnumerable).IsAssignableFrom(type))
            { 
                Type genericType = type.GetGenericArguments()[0];
                if (_configuration.HasType(genericType))
                {
                    List<Object> list = new List<object>();
                    foreach (var implementation in _configuration.GetAllImplementations(genericType))
                    {
                        list.Add(Resolve(implementation));
                    }

                    return list.AsEnumerable();
                }
            }

            if ((mainType = _configuration.HasImplType(type)) != null)
            {
                Implementation implementation = _configuration.GetFirstImplementation(mainType);
                object obj = Resolve(implementation);
                searchPointInstance();
                return obj as TType;
            } 
            
            if (_configuration.HasType(type))
            {
                Implementation implementation = _configuration.GetFirstImplementation(type);
                object obj = Resolve(implementation);
                searchPointInstance();
                return obj as TType;
            }

            if (type.IsGenericType)
            {
                Type genericTypeDefinition = type.GetGenericTypeDefinition();
                Type genericArgument = type.GetGenericArguments()[0];
                if (_configuration.HasType(genericTypeDefinition) && _configuration.HasType(genericArgument))
                {
                    return ResolveOpenGeneric(_configuration.GetFirstImplementation(genericTypeDefinition).Type,
                        genericArgument) as TType;
                }
            }

            throw new UnsupportedTypeException("Unsupported type: " + type.FullName);
        }

        private Object CreateObject(Type type)
        {
            ConstructorInfo[] constructors = type.GetConstructors();

            foreach (var constructorInfo in constructors)
            {
                ParameterInfo[] parameters = constructorInfo.GetParameters();
                if (parameters.All(param => _configuration.HasType(param.ParameterType)))
                {
                    Object value;
                    
                    if (instanceWasCreate(parameters))
                    {
                        value = constructors[1].Invoke(null);
                        listPoint.Add(value);
                    }
                    else
                    {
                        value = constructorInfo.Invoke(parameters.Select(param =>
                            Resolve(_configuration.GetFirstImplementation(param.ParameterType))).ToArray());
                    }

                    return value;
                }
            }

            throw new NoSuitableConstructorException("No suitable constructor for type: " + type.FullName);
        }

        private bool instanceWasCreate(ParameterInfo[] parameters)
        {
            foreach (var param in parameters)
            {
                Type type = _configuration.GetFirstImplementation(param.ParameterType).Type;
                if (listInstances.Contains(type))
                {
                    return true;
                }
            }
            return false;
        }

        private object ResolveOpenGeneric(Type baseType, Type genericArgumentType)
        {
            return CreateObject(baseType.MakeGenericType(genericArgumentType));
        }

        private object Resolve(Implementation implementation)
        {
            Type type = implementation.Type;
            bool flag = false; 
            if (implementation.IsSingleton && implementation.Value != null)
            {
                return implementation.Value;
            }
            
            listInstances.Add(type);
            
            object value = CreateObject(type);

            if (implementation.IsSingleton)
            {
                implementation.Value = value;
            }

            return value;
        }

        private void searchPointInstance()
        {
            for(int i = 0; i < listPoint.Count; i++) {
                PropertyInfo[] properties = listPoint[i].GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var property in properties)
                {
                    Implementation impl = _configuration.GetFirstImplementation(property.PropertyType);
                    property.SetValue(listPoint[i],Resolve(impl));
                }
            }

            listPoint.Clear();
        }
    }
}