using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.Json;

namespace JsonToObject;

public class JsonToObjectConverter
{
    private class Counter
    {
        private ulong count;
        public Counter()
        {
            this.count = 0;
        }

        public ulong Next()
        {
            this.count++;
            return this.count;
        }
    }

    private static ulong assemblyGenerationCounter;

    static JsonToObjectConverter()
    {
        assemblyGenerationCounter = 0;
    }

    public static object? ConvertToObject(string jsonString)
    {
        JsonSerializerOptions opt = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };
        JsonElement rawResult = JsonSerializer.Deserialize<JsonElement>(jsonString, opt);
        object? result = ToStronglyTypedObject(rawResult);
        return result;
    }
    private static object? ToStronglyTypedObject(JsonElement? nullableJsonElement)
    {
        ulong assemblyId = Interlocked.Increment(ref assemblyGenerationCounter);
        string assemblyNameString = $"RuntimeGeneratedAssembly_{assemblyId.ToString(CultureInfo.InvariantCulture)}";
        string moduleName = $"RuntimeGeneratedModule";
        ModuleBuilder moduleBuilder = CreateModuleBuilder(assemblyNameString, moduleName);
        Counter typeGenerationCounter = new Counter();
        var result = ToStronglyTypedObject(nullableJsonElement, moduleBuilder, typeGenerationCounter);
        return result;
    }
    private static object? ToStronglyTypedObject(
        JsonElement? nullableJsonElement,
        ModuleBuilder moduleBuilder,
        Counter typeGenerationCounter
    )
    {
        if (nullableJsonElement == null)
        {
            return null;
        }

        JsonElement jsonElement = nullableJsonElement.Value;

        switch (jsonElement.ValueKind)
        {
            case JsonValueKind.Undefined:
                return null;
            case JsonValueKind.String:
                return jsonElement.GetString();
            case JsonValueKind.False:
                return false;
            case JsonValueKind.True:
                return true;
            case JsonValueKind.Null:
                return null;
            case JsonValueKind.Number:
                {
                    if (jsonElement.TryGetDouble(out var result))
                    {
                        return result;
                    }
                }
                throw new InvalidOperationException($"Unable to parse {jsonElement} as number.");
            case JsonValueKind.Object:
                {
                    ulong typeId = typeGenerationCounter.Next();
                    string typeName = $"RuntimeGeneratedType_{typeId.ToString(CultureInfo.InvariantCulture)}";
                    TypeBuilder typeBuilder = CreateTypeBuilder(moduleBuilder, typeName);
                    Dictionary<string, object?> propertyValues = new Dictionary<string, object?>();
                    foreach (var property in jsonElement.EnumerateObject())
                    {
                        string propertyName = property.Name;
                        object? propertyValue = ToStronglyTypedObject(property.Value, moduleBuilder, typeGenerationCounter);
                        Type propertyValueType;
                        if (null == propertyValue)
                        {
                            propertyValueType = typeof(object);
                        }
                        else
                        {
                            propertyValueType = propertyValue.GetType();
                        }
                        CreateAutoImplementedProperty(typeBuilder, propertyName, propertyValueType);
                        propertyValues.Add(propertyName, propertyValue);
                    }

                    Type resultType = typeBuilder.CreateType()!;
                    object result = Activator.CreateInstance(resultType)!;
                    foreach (var pair in propertyValues)
                    {
                        var propertyInfo = resultType.GetProperty(pair.Key)!;
                        propertyInfo.SetValue(result, pair.Value);
                    }
                    return result;
                }
            case JsonValueKind.Array:
                {
                    List<object?> list = new List<object?>();
                    foreach (var item in jsonElement.EnumerateArray())
                    {
                        object? value = ToStronglyTypedObject(item, moduleBuilder, typeGenerationCounter);
                        list.Add(value);
                    }
                    return list.ToArray();
                }
            default:
                throw new InvalidOperationException($"Value type '{jsonElement.ValueKind}' is not supported");
        }
    }

    private static ModuleBuilder CreateModuleBuilder(
            string assemblyNameString,
            string moduleName
        )
    {
        // create assembly name
        var assemblyName = new AssemblyName(assemblyNameString);
        // create the assembly builder
        AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);

        // create the module builder
        ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName);
        return moduleBuilder;
    }

    private static TypeBuilder CreateTypeBuilder(
            ModuleBuilder moduleBuilder,
            string typeName
        )
    {
        // create the type builder
        TypeBuilder typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Public);
        typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
        return typeBuilder;
    }

    private static void CreateAutoImplementedProperty(
        TypeBuilder builder,
        string propertyName,
        Type propertyType
        )
    {
        const string PrivateFieldPrefix = "m_";
        const string GetterPrefix = "get_";
        const string SetterPrefix = "set_";

        // Generate the field.
        FieldBuilder fieldBuilder = builder.DefineField(
            string.Concat(PrivateFieldPrefix, propertyName),
                          propertyType, FieldAttributes.Private);

        // Generate the property
        PropertyBuilder propertyBuilder = builder.DefineProperty(
            propertyName, PropertyAttributes.HasDefault, propertyType, null);

        // Property getter and setter attributes.
        MethodAttributes propertyMethodAttributes =
            MethodAttributes.Public | MethodAttributes.SpecialName |
            MethodAttributes.HideBySig;

        // Define the getter method.
        MethodBuilder getterMethod = builder.DefineMethod(
            string.Concat(GetterPrefix, propertyName),
            propertyMethodAttributes, propertyType, Type.EmptyTypes);

        // Emit the IL code.
        // ldarg.0
        // ldfld,_field
        // ret
        ILGenerator getterILCode = getterMethod.GetILGenerator();
        getterILCode.Emit(OpCodes.Ldarg_0);
        getterILCode.Emit(OpCodes.Ldfld, fieldBuilder);
        getterILCode.Emit(OpCodes.Ret);

        // Define the setter method.
        MethodBuilder setterMethod = builder.DefineMethod(
            string.Concat(SetterPrefix, propertyName),
            propertyMethodAttributes, null, new Type[] { propertyType });

        // Emit the IL code.
        // ldarg.0
        // ldarg.1
        // stfld,_field
        // ret
        ILGenerator setterILCode = setterMethod.GetILGenerator();
        setterILCode.Emit(OpCodes.Ldarg_0);
        setterILCode.Emit(OpCodes.Ldarg_1);
        setterILCode.Emit(OpCodes.Stfld, fieldBuilder);
        setterILCode.Emit(OpCodes.Ret);

        propertyBuilder.SetGetMethod(getterMethod);
        propertyBuilder.SetSetMethod(setterMethod);
    }
}