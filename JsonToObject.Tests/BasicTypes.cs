using System.Collections;

namespace JsonToObject.Tests
{

    /// <summary>
    /// Perform tests about json basic types.
    /// </summary>
    [Collection(Constants.CollectionDefinitions.JsonToObject)]
    public class BasicTypes
    {
        private readonly JsonToObjectFixture jsonToObjectFixture;
        public BasicTypes(JsonToObjectFixture jsonToObjectFixture)
        {
            this.jsonToObjectFixture = jsonToObjectFixture;
        }

        [Fact]
        public void TestNull()
        {
            string nullJson = @"null";
            object? o = jsonToObjectFixture.JsonToObjectConverter.ConvertToObject(nullJson);
            Assert.Null(o);
        }

        [Fact]
        public void TestNotNull()
        {
            string simpleJsonObject = @"{}";
            object? o = jsonToObjectFixture.JsonToObjectConverter.ConvertToObject(simpleJsonObject);
            Assert.NotNull(o);
        }

        [Fact]
        public void TestNumeric()
        {
            string jsonNumber = @"1234";
            object? o = jsonToObjectFixture.JsonToObjectConverter.ConvertToObject(jsonNumber);
            Assert.NotNull(o);
            Assert.Equal(1234.0, (double)o!);
        }

        [Fact]
        public void TestBooleanTrue()
        {
            string jsonBooleanTrue = @"true";
            object? o = jsonToObjectFixture.JsonToObjectConverter.ConvertToObject(jsonBooleanTrue);
            Assert.NotNull(o);
            Assert.True((bool)o!);
        }

        [Fact]
        public void TestBooleanFalse()
        {
            string jsonBooleanFalse = @"false";
            object? o = jsonToObjectFixture.JsonToObjectConverter.ConvertToObject(jsonBooleanFalse);
            Assert.NotNull(o);
            Assert.False((bool)o!);
        }

        [Fact]
        public void TestEmptyString()
        {
            string emptyJsonString = @"""""";
            object? o = jsonToObjectFixture.JsonToObjectConverter.ConvertToObject(emptyJsonString);
            Assert.NotNull(o);
            Assert.Equal(string.Empty, o!);
        }

        [Fact]
        public void TestString()
        {
            string jsonString = @"""abc""";
            object? o = jsonToObjectFixture.JsonToObjectConverter.ConvertToObject(jsonString);
            Assert.NotNull(o);
            Assert.Equal("abc", o!);
        }

        [Fact]
        public void TestEmptyArray()
        {
            string emptyJsonArray = @"[]";
            object? o = jsonToObjectFixture.JsonToObjectConverter.ConvertToObject(emptyJsonArray);
            Assert.NotNull(o);
            Assert.IsAssignableFrom<IEnumerable>(o!);
            Assert.False(((IEnumerable<object>)o!).Any());
        }

        [Fact]
        public void TestNonEmptyArray()
        {
            string jsonArray = @"[1, ""a"", [], {}]";
            object? o = jsonToObjectFixture.JsonToObjectConverter.ConvertToObject(jsonArray);
            Assert.NotNull(o);
            Assert.IsAssignableFrom<IEnumerable>(o!);
            Assert.True(((IEnumerable<object>)o!).Any());
            Assert.Equal(4, ((IEnumerable<object>)o!).Count());
        }

        [Fact]
        public void TestObject()
        {
            string simpleJsonObject = @"{}";
            object? o = jsonToObjectFixture.JsonToObjectConverter.ConvertToObject(simpleJsonObject);
            Assert.NotNull(o);
            Assert.IsNotType<string>(o!);
            Assert.IsNotType<bool>(o!);
            Assert.IsNotType<double>(o!);
        }
    }
}