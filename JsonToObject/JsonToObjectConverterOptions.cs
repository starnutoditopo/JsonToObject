namespace JsonToObject;

/// <summary>
/// Defines the options to instantiate a <see cref="JsonToObjectConverter" /> object.
/// </summary>
public class JsonToObjectConverterOptions
{
    private const string CONSTANTS_RuntimeGeneratedModuleName = $"RuntimeGeneratedModule";
    private const string CONSTANTS_RuntimeGeneratedAssemblyNameTemplate = "RuntimeGeneratedAssembly_{0}";
    private const string CONSTANTS_RuntimeGeneratedTypeNameTemplate = "RuntimeGeneratedType_{0}";

    /// <summary>Gets or sets the name of the runtime-generated module.</summary>
    /// <value>The name of the runtime-generated module.</value>
    public string RuntimeGeneratedModuleName { get; set; } = CONSTANTS_RuntimeGeneratedModuleName;

    /// <summary>Gets or sets the template to use to generate the name of runtime-generated assemblies.</summary>
    /// <value>The template to use to generate the name of runtime-generated assemblies.</value>
    /// <remarks>Should contain a "{0}" placeholder.</remarks>
    public string RuntimeGeneratedAssemblyNameTemplate { get; set; } = CONSTANTS_RuntimeGeneratedAssemblyNameTemplate;

    /// <summary>Gets or sets the template to use to generate the name of runtime-generated types.</summary>
    /// <value>The template to use to generate the name of runtime-generated types.</value>
    /// <remarks>Should contain a "{0}" placeholder.</remarks>
    public string RuntimeGeneratedTypeNameTemplate { get; set; } = CONSTANTS_RuntimeGeneratedTypeNameTemplate;

    public Func<string, IDictionary<string, object?>, string?>? GetDebuggerDisplayString { get; set; }
}
