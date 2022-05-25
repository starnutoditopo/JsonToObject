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
        public void TestDefaultUsage()
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
    }
}