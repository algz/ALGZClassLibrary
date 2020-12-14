using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SerializeLibrary
{
    public class SerializeXMLClass
    {
        /// <summary>
        /// 序列化DataTable
        /// </summary>
        /// <param name="pDt">包含数据的DataTable</param>
        /// <returns>序列化的DataTable</returns>
        public static string Serialize(DataTable pDt)
        {
            // 序列化DataTable
            StringBuilder sb = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(sb);
            XmlSerializer serializer = new XmlSerializer(typeof(DataTable));
            serializer.Serialize(writer, pDt);
            writer.Close();

            return sb.ToString();
        }

        /// <summary>
        /// 反序列化DataTable
        /// </summary>
        /// <param name="pXml">序列化的DataTable</param>
        /// <returns>DataTable</returns>
        public static DataTable Deserialize(string pXml)
        {

            StringReader strReader = new StringReader(pXml);
            XmlReader xmlReader = XmlReader.Create(strReader);
            XmlSerializer serializer = new XmlSerializer(typeof(DataTable));

            DataTable dt = serializer.Deserialize(xmlReader) as DataTable;

            return dt;
        }
    }
}
