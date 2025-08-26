using Common;
using System.Data;

namespace Kevin.Unit.Test.Kevin.Common
{
    public class DataHelperTests
    {
        [Fact]
        public void DataTableToListDisplayName_ShouldReturnEmptyList_WhenTableIsNull()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Name", typeof(string));
            // Act
            table.Rows.Add(1, "Test1");
            table.Rows.Add(2, "Test2");
            var result = DataHelper.DataTableToListDisplayName<TestModel>(table);

            // Assert
            Assert.NotNull(result); 
            Assert.Equal(2,result.Count);
        }

        [Fact]
        public void DataTableToListDisplayName_ShouldReturnEmptyList_WhenTableHasNoRows()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Name", typeof(string));

            // Act
            var result = DataHelper.DataTableToListDisplayName<TestModel>(table);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void DataTableToListDisplayName_ShouldMapColumnsCorrectly()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Rows.Add(1, "Test1");
            table.Rows.Add(2, "Test2");

            // Act
            var result = DataHelper.DataTableToListDisplayName<TestModel>(table);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].Id);
            Assert.Equal("Test1", result[0].Name);
            Assert.Equal(2, result[1].Id);
            Assert.Equal("Test2", result[1].Name);
        }

        [Fact]
        public void DataTableToListDisplayName_ShouldHandleMissingColumns()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Rows.Add(1);

            // Act
            var result = DataHelper.DataTableToListDisplayName<TestModel>(table);

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0].Id);
            Assert.Null(result[0].Name);
        }

        private class TestModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
