using System.Collections.Generic;
using System.Reflection.Emit;
using System;
using System.Linq;
using System.Reflection;

namespace CSI.CastleWindsorHelper.Fake
{
    public static class InterfaceWrapper
    {
        #region Private Fields

        private static IDictionary<string, ModuleBuilder> _builders = new Dictionary<string, ModuleBuilder>(StringComparer.InvariantCultureIgnoreCase);
        private static IDictionary<Type, Type> _types = new Dictionary<Type, Type>();
        private static object _lockObject = new object();

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a type that matches the interface defined by <typeparamref name="T"/>
        /// </summary>
        public static Type CreateType<T>()
        {
            return typeof(T).IsInterface ? GenerateInterfaceType<T>() : GenerateClassType<T>();
        }

        /// <summary>
        /// Creates a type that matches the interface defined by type
        /// </summary>
        public static Type CreateType(Type sourceType)
        {
            return sourceType.IsInterface ? GenerateInterfaceType(sourceType) : GenerateClassType(sourceType);
        }

        /// <summary>
        /// Creates an fake instance that matches the interface defined by <typeparamref name="T"/>
        /// </summary>
        public static T CreateInterfaceInstance<T>()
        {
            var destType = GenerateInterfaceType<T>();
            return (T)Activator.CreateInstance(destType);
        }

        /// <summary>
        /// Creates an fake instance that matches the interface defined by <typeparamref name="T"/>
        /// </summary>
        public static object CreateInterfaceInstance(Type sourceType)
        {
            var destType = GenerateInterfaceType(sourceType);
            return Activator.CreateInstance(destType);
        }

        // Note that calling this method will cause any further
        // attempts to generate an interface to fail
        public static void Save()
        {
            foreach (var builder in _builders.Select(b => b.Value))
            {
                var ass = (AssemblyBuilder)builder.Assembly;
                try
                {
                    ass.Save(ass.GetName().Name + ".dll");
                }
                catch { }
            }
        }

        #endregion

        #region Private Methods

        private static Type GenerateInterfaceType<T>()
        {
            return GenerateInterfaceType(typeof(T));
        }
        private static Type GenerateInterfaceType(Type sourceType)
        {
            #region Cache Fetch

            Type newType;
            if (_types.TryGetValue(sourceType, out newType))
                return newType;

            // Make sure the same interface isn't implemented twice
            lock (_lockObject)
            {
                if (_types.TryGetValue(sourceType, out newType))
                    return newType;

                #endregion

                #region Validation

                if (!sourceType.IsInterface)
                    throw new ArgumentException("Type T is not an interface", "T");

                #endregion

                #region Module and Assembly Creation

                var orginalAssemblyName = sourceType.Assembly.GetName().Name;

                ModuleBuilder moduleBuilder;
                if (!_builders.TryGetValue(orginalAssemblyName, out moduleBuilder))
                {
                    var newAssemblyName = new AssemblyName(Guid.NewGuid() + "." + orginalAssemblyName);

                    var dynamicAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(
                        newAssemblyName,
                        System.Reflection.Emit.AssemblyBuilderAccess.RunAndSave);

                    moduleBuilder = dynamicAssembly.DefineDynamicModule(
                        newAssemblyName.Name,
                        newAssemblyName + ".dll");

                    _builders.Add(orginalAssemblyName, moduleBuilder);
                }

                var assemblyName = moduleBuilder.Assembly.GetName();

                #endregion

                #region Create the TypeBuilder

                var typeBuilder = moduleBuilder.DefineType(
                    sourceType.FullName,
                    TypeAttributes.Public | TypeAttributes.Class,
                    typeof(object),
                    new[] { sourceType });

                #endregion

                #region Enumerate interface inheritance hierarchy

                var interfaces = new List<Type>();
                IEnumerable<Type> subList;

                subList = new[] { sourceType };

                while (subList.Count() != 0)
                {
                    interfaces.AddRange(subList);
                    subList = subList.SelectMany(i => i.GetInterfaces());
                }

                interfaces = interfaces.Distinct().ToList();

                #endregion

                #region Create the methods

                foreach (var method in interfaces.SelectMany(i => i.GetMethods()))
                {
                    // Define the method based on the interfaces definition
                    var newMethod = typeBuilder.DefineMethod(
                        method.Name,
                        method.Attributes ^ MethodAttributes.Abstract,
                        method.CallingConvention,
                        method.ReturnType,
                        method.ReturnParameter.GetRequiredCustomModifiers(),
                        method.ReturnParameter.GetOptionalCustomModifiers(),
                        method.GetParameters().Select(p => p.ParameterType).ToArray(),
                        method.GetParameters().Select(p => p.GetRequiredCustomModifiers()).ToArray(),
                        method.GetParameters().Select(p => p.GetOptionalCustomModifiers()).ToArray()
                        );
                    if (method.IsGenericMethod)
                    {
                        var genericArgs = method.GetGenericArguments();
                        var typeParams = newMethod.DefineGenericParameters(genericArgs.Select(g => g.Name).ToArray());
                    }

                    // Check to see if we have a return type
                    bool hasReturnValue = method.ReturnType != typeof(void);

                    var methodBody = newMethod.GetILGenerator();
                    methodBody.ThrowException(typeof(NotImplementedException));
                }

                #endregion

                #region Create and return the defined type

                newType = typeBuilder.CreateType();

                _types.Add(sourceType, newType);

                return newType;

            }
            #endregion
        }

        private static Type GenerateClassType<T>()
        {
            return GenerateClassType(typeof(T));
        }
        private static Type GenerateClassType(Type sourceType)
        {
            #region Cache Fetch

            Type newType;
            if (_types.TryGetValue(sourceType, out newType))
                return newType;

            // Make sure the same class isn't created twice
            lock (_lockObject)
            {
                if (_types.TryGetValue(sourceType, out newType))
                    return newType;

                #endregion

                #region Validation

                if (!sourceType.IsClass)
                    throw new ArgumentException("Type T is not a class", "T");

                #endregion

                #region Module and Assembly Creation

                var orginalAssemblyName = sourceType.Assembly.GetName().Name;

                ModuleBuilder moduleBuilder;
                if (!_builders.TryGetValue(orginalAssemblyName, out moduleBuilder))
                {
                    var newAssemblyName = new AssemblyName(Guid.NewGuid() + "." + orginalAssemblyName);

                    var dynamicAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(
                        newAssemblyName,
                        System.Reflection.Emit.AssemblyBuilderAccess.RunAndSave);

                    moduleBuilder = dynamicAssembly.DefineDynamicModule(
                        newAssemblyName.Name,
                        newAssemblyName + ".dll");

                    _builders.Add(orginalAssemblyName, moduleBuilder);
                }

                var assemblyName = moduleBuilder.Assembly.GetName();

                #endregion

                #region Enumerate interface inheritance hierarchy

                var interfaces = new List<Type>();
                IEnumerable<Type> subList = sourceType.GetInterfaces();
                while (subList.Count() != 0)
                {
                    interfaces.AddRange(subList);
                    subList = subList.SelectMany(i => i.GetInterfaces());
                }

                interfaces = interfaces.Distinct().ToList();

                #endregion

                #region Create the TypeBuilder

                var typeBuilder = moduleBuilder.DefineType(
                      sourceType.FullName
                    , TypeAttributes.Public | TypeAttributes.Class
                    , sourceType);
                //, interfaces.ToArray());

                #endregion

                #region Create the methods
                foreach (var baseMethod in sourceType.GetMethods())
                {
                    // Define the method based on the interfaces definition

                    if (baseMethod.IsVirtual && false == baseMethod.IsFinal)
                    {
                        var childMethod = typeBuilder.DefineMethod(
                            baseMethod.Name,
                            baseMethod.Attributes,
                            baseMethod.CallingConvention,
                            baseMethod.ReturnType,
                            baseMethod.ReturnParameter.GetRequiredCustomModifiers(),
                            baseMethod.ReturnParameter.GetOptionalCustomModifiers(),
                            baseMethod.GetParameters().Select(p => p.ParameterType).ToArray(),
                            baseMethod.GetParameters().Select(p => p.GetRequiredCustomModifiers()).ToArray(),
                            baseMethod.GetParameters().Select(p => p.GetOptionalCustomModifiers()).ToArray()
                            );
                        // Check to see if we have a return type
                        bool hasReturnValue = baseMethod.ReturnType != typeof(void);

                        var il = childMethod.GetILGenerator();
                        int paramCount = baseMethod.GetParameters().Length;

                        il.Emit(OpCodes.Ldarg_0);
                        for (int i = 0; i < paramCount; i++)
                            il.Emit(OpCodes.Ldarg_S, i + 1);

                        il.Emit(OpCodes.Call, baseMethod);
                        il.Emit(OpCodes.Ret);

                        typeBuilder.DefineMethodOverride(childMethod, baseMethod);
                    }
                }

                #endregion

                #region Create and return the defined type

                newType = typeBuilder.CreateType();

                _types.Add(sourceType, newType);

                return newType;

            }
            #endregion
        }
        #endregion
    }
}