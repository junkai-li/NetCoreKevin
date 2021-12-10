using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Common
{
    public class XmlHelper
    {


        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static string ObjectToXml(object obj)
        {
            using (MemoryStream memoryStream = new())
            {
                XmlWriterSettings xmlWriterSettings = new();
                xmlWriterSettings.Encoding = Encoding.UTF8;
                xmlWriterSettings.OmitXmlDeclaration = true;

                using (var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings))
                {
                    //去除默认命名空间xmlns:xsd和xmlns:xsi
                    XmlSerializerNamespaces ns = new();
                    ns.Add("", "");

                    XmlSerializer xmlSerializer = new(obj.GetType());
                    xmlSerializer.Serialize(xmlWriter, obj);

                    memoryStream.Position = 0;
                    using (var sr = new StreamReader(memoryStream))
                    {
                        var xmlString = sr.ReadToEnd();

                        return xmlString;
                    }
                }
            }
        }




        /// <summary>
        /// 反序列化为对象
        /// </summary>
        /// <param name="xmlText">对象序列化后的Xml字符串</param>
        /// <returns></returns>
        public static T XmlToObject<T>(string xmlText)
        {
            using (StringReader stringReader = new(xmlText))
            {
                XmlSerializer xmlSerializer = new(typeof(T));

                return (T)xmlSerializer.Deserialize(stringReader);
            }
        }


    }
}
