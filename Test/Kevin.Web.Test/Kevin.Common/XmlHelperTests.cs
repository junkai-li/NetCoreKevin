using Common;

namespace Kevin.Unit.Test.Kevin.Common
{ 
        public class XmlHelperTests
        {
            [Fact]
            public void ObjectToXml_ShouldSerializeObjectToXmlString()
            {
                var testObj = new TestClass { Id = 1, Name = "Test" };
                var xml = XmlHelper.ObjectToXml(testObj);

                Assert.Contains("<Id>1</Id>", xml);
                Assert.Contains("<Name>Test</Name>", xml);
            }

            [Fact]
            public void ObjectToXml_ShouldHandleNullObject()
            {
                Assert.Throws<ArgumentNullException>(() => XmlHelper.ObjectToXml(null));
            }

            [Fact]
            public void XmlToObject_ShouldDeserializeXmlToObject()
            {
                string xml = "<TestClass><Id>1</Id><Name>Test</Name></TestClass>";
                var obj = XmlHelper.XmlToObject<TestClass>(xml);

                Assert.Equal(1, obj.Id);
                Assert.Equal("Test", obj.Name);
            }

            [Fact]
            public void XmlToObject_ShouldHandleInvalidXml()
            {
                string invalidXml = "Invalid XML";
                Assert.Throws<InvalidOperationException>(() => XmlHelper.XmlToObject<TestClass>(invalidXml));
            }

            [Fact]
            public void XmlToObject_ShouldHandleNullXml()
            {
                Assert.Throws<ArgumentNullException>(() => XmlHelper.XmlToObject<TestClass>(null));
            }
        }

        public class TestClass
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
  
}
