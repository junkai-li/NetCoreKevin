using Common.Json;
using System.Data;

namespace Kevin.Unit.Test.Kevin.Common
{
    public class JsonHelperTests
    {
        [Theory]
        [InlineData("{\"name\":\"kevin\"}", "name", "kevin")]
        public void GetValueByKey(string json,string key,string expected)
        { 
            Assert.Equal(expected, JsonHelper.GetValueByKey(json, key));
        }

        [Fact]
        public void GetValueByKey_InvalidKey_ThrowsException()
        {
            // Arrange
            string json = "{\"name\":\"John\"}";
            string key = "invalidKey";

            // Act & Assert
            Assert.Throws<Exception>(() => JsonHelper.GetValueByKey(json, key));
        }

        [Fact]
        public void DataRowToJSON_ValidRow_ReturnsJson()
        {
            // Arrange
            DataTable table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Name", typeof(string));
            DataRow row = table.NewRow();
            row["Id"] = 1;
            row["Name"] = "Test"; 
            // Act
            var result = JsonHelper.DataRowToJSON(row); 
            // Assert
            Assert.NotNull(result);
            Assert.Equal("{\"Id\":1,\"Name\":\"Test\"}", result.ToString()); 
        }

        [Fact]
        public void DataRowToObject_ValidRow_ReturnsObject()
        {
            // Arrange
            DataTable table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Name", typeof(string));
            DataRow row = table.NewRow();
            row["Id"] = 1;
            row["Name"] = "Test";

            // Act
            var result = JsonHelper.DataRowToObject<TestModel>(row);

            // Assert
            Assert.Equal(1, result.Id);
            Assert.Equal("Test", result.Name);
        }

        [Fact]
        public void DataTableToList_ValidTable_ReturnsList()
        {
            // Arrange
            DataTable table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Name", typeof(string));
            DataRow row = table.NewRow();
            row["Id"] = 1;
            row["Name"] = "Test";
            table.Rows.Add(row);

            // Act
            var result = JsonHelper.DataTableToList<TestModel>(table);

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0].Id);
            Assert.Equal("Test", result[0].Name);
        }

        [Fact]
        public void JSONToList_ValidJson_ReturnsList()
        {
            // Arrange
            string json = "[{\"Id\":1,\"Name\":\"Test\"}]";

            // Act
            var result = JsonHelper.JSONToList<TestModel>(json);

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0].Id);
            Assert.Equal("Test", result[0].Name);
        }

        [Fact]
        public void ObjectToJSON_ValidObject_ReturnsJson()
        {
            // Arrange
            var obj = new TestModel { Id = 1, Name = "Test" };

            // Act
            var result = JsonHelper.ObjectToJSON(obj);

            // Assert
            Assert.Equal("{\"Id\":1,\"Name\":\"Test\"}", result);
        }

        [Fact]
        public void JSONToObject_ValidJson_ReturnsObject()
        {
            // Arrange
            string json = "{\"Id\":1,\"Name\":\"Test\"}";

            // Act
            var result = JsonHelper.JSONToObject<TestModel>(json);

            // Assert
            Assert.Equal(1, result.Id);
            Assert.Equal("Test", result.Name);
        }

        [Fact]
        public void JsonToArrayList_ValidJson_ReturnsList()
        {
            // Arrange
            string json = "[1,2,3]";

            // Act
            var result = JsonHelper.JsonToArrayList(json);

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal(1, (int)result[0]);
            Assert.Equal(2, (int)result[1]);
            Assert.Equal(3, (int)result[2]);
        }

        private class TestModel
        {
            public int Id { get; set; }
            public required string Name { get; set; }
        }
    }
} 