using System.Collections;

namespace JsonToObject.Tests
{
    /// <summary>
    /// Perform tests about json structured types.
    /// </summary>
    public class StructuredTypes
    {
        [Fact]
        public void TestObject()
        {
            string simpleJsonObject = @"{""null"": null, ""boolean"": true, ""number"":1, ""string"": ""a"", ""array"": [], ""object"": {}}";
            object? o = JsonToObjectConverter.ConvertToObject(simpleJsonObject);
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
    }
}