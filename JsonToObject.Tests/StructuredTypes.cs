using System.Reflection;

namespace JsonToObject.Tests
{
    /// <summary>
    /// Perform tests about json structured types.
    /// </summary>
    [Collection(Constants.CollectionDefinitions.JsonToObject)]
    public class StructuredTypes
    {
        private readonly JsonToObjectFixture jsonToObjectFixture;
        public StructuredTypes(JsonToObjectFixture jsonToObjectFixture)
        {
            this.jsonToObjectFixture = jsonToObjectFixture;
        }

        [Fact]
        public void TestSimpleObject()
        {
            string simpleJsonObject = @"{""null"": null, ""boolean"": true, ""number"":1, ""string"": ""a"", ""array"": [], ""object"": {}}";
            object? o = jsonToObjectFixture.JsonToObjectConverter.ConvertToObject(simpleJsonObject);
            Assert.NotNull(o);

            var propertyNames = o!
                .GetType()
                .GetProperties()
                .Select(p => p.Name)
                ;

            Assert.Contains("null", propertyNames);
            Assert.Contains("boolean", propertyNames);
            Assert.Contains("number", propertyNames);
            Assert.Contains("string", propertyNames);
            Assert.Contains("array", propertyNames);
            Assert.Contains("object", propertyNames);
        }

        [Fact]
        public void TestNestedObjects()
        {
            string simpleJsonObject = @"
                {
                    ""firstLevel"": {
                        ""secondLevel"": {
                            ""thirdLevel"": 1
                        }
                    }
                }
                ";
            object? o = jsonToObjectFixture.JsonToObjectConverter.ConvertToObject(simpleJsonObject);
            Assert.NotNull(o);
            Assert.Contains("firstLevel", o!.GetType().GetProperties().Select(p => p.Name));

            PropertyInfo property1 = o!.GetType().GetProperty("firstLevel")!;
            object? nested1 = property1.GetValue(o!);
            Assert.NotNull(nested1);
            Assert.Contains("secondLevel", nested1!.GetType().GetProperties().Select(p => p.Name));

            PropertyInfo property2 = nested1!.GetType().GetProperty("secondLevel")!;
            object? nested2 = property2.GetValue(nested1!);
            Assert.NotNull(nested2);
            Assert.Contains("thirdLevel", nested2!.GetType().GetProperties().Select(p => p.Name));

            PropertyInfo property3 = nested2!.GetType().GetProperty("thirdLevel")!;
            object? nested3 = property3.GetValue(nested2!);
            Assert.NotNull(nested3);
            Assert.IsType<double>(nested3);
        }
    }
}