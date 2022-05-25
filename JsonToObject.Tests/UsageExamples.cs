using Xunit.Abstractions;

namespace JsonToObject.Tests
{
    /// <summary>
    /// Shows some usage examples.
    /// </summary>
    public class UsageExamples
    {
        private readonly ITestOutputHelper logger;

        public UsageExamples(ITestOutputHelper logger)
        {
            this.logger = logger;
        }

        [Fact]
        public void DefaultUsageExample()
        {
            string jsonObject = @"
                {
                    ""property"": {
                        ""subProperty"": {
                            ""numericValue"": 1,
                            ""stringValue"": ""Hi, there!""
                        }
                    },
                    ""tags"": [
                        ""Tag 1"",
                        ""Tag 2"",
                        true
                    ]
                }
                ";

            JsonToObjectConverter jsonToObjectConverter = new JsonToObjectConverter();
            object? o = jsonToObjectConverter.ConvertToObject(jsonObject);
            Assert.NotNull(o);

            // Properties can now be inspected using reflection...

            var propertyNames = o!
                .GetType()
                .GetProperties()
                .Select(p => p.Name)
                ;

            foreach (var propertyName in propertyNames)
            {
                this.logger.WriteLine(propertyName);
            }


            // ... and evaluated.
            object? propertyValue = o!
                .GetType()
                .GetProperty(propertyNames.First())!
                .GetValue(o!);

            Assert.NotNull(propertyValue);
        }

        [Fact]
        public void CustomizedAssemblyModuleAndTypeNames()
        {
            string jsonObject = @"
                {
                    ""property"": {
                        ""subProperty"": {
                            ""numericValue"": 1,
                            ""stringValue"": ""Hi, there!""
                        }
                    },
                    ""tags"": [
                        ""Tag 1"",
                        ""Tag 2"",
                        true
                    ]
                }
                ";

            JsonToObjectConverterOptions options = new JsonToObjectConverterOptions()
            {
                RuntimeGeneratedAssemblyNameTemplate = "MyAssembly_{0}",
                RuntimeGeneratedTypeNameTemplate = "MyType_{0}",
                RuntimeGeneratedModuleName = "MyModule"
            };
            JsonToObjectConverter jsonToObjectConverter = new JsonToObjectConverter(options);
            object? o = jsonToObjectConverter.ConvertToObject(jsonObject);
            Assert.NotNull(o);

            Type type = o!.GetType();
            Assert.StartsWith("MyType", type.Name);
            Assert.StartsWith("MyAssembly", type.Assembly.FullName);

            this.logger.WriteLine(o!.GetType().AssemblyQualifiedName);
        }
    }
}