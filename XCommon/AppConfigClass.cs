using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;

namespace XCommon
{
    /// <summary>
    /// 1.在源代码中引用了using System.Configuration，还需在解决方案资源管理器的项目里“引用”中添加“System.Configuration”。
    /// 2.项目添加新建项目->选择"应用配置文件".
    /*
     <!-- app.config -->

<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <appSettings>
      <add key="key1" value="value1" />
  </appSettings>
</configuration>
         */
    /// </summary>
    public class AppConfigClass
    {
        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValueByKey(string key)
        {
            
            return ConfigurationManager.AppSettings[key];
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="strKey"></param>
        /// <param name="value"></param>
        public static void ModifyAppSettings(string strKey, string value)
        {
            //value = ConstructionRealNameSystem.Utilities.CryptoHelper.EncryptAes(value);

            var doc = new XmlDocument();
            //获得配置文件的全路径    
            var strFileName = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            doc.Load(strFileName);


            var appSettingsNode = doc.SelectSingleNode("configuration//appSettings");

            var nodes = appSettingsNode.ChildNodes;

            //找出名称为“add”的所有元素    
            //var nodes = doc.GetElementsByTagName("add");
            int i = 0;
            for (; i < nodes.Count; i++)
            {
                //获得将当前元素的key属性    
                var xmlAttributeCollection = nodes[i].Attributes;
                if (xmlAttributeCollection != null)
                {
                    var att = xmlAttributeCollection["key"];
                    if (att == null) continue;
                    //根据元素的第一个属性来判断当前的元素是不是目标元素    
                    if (att.Value != strKey) continue;
                    //对目标元素中的第二个属性赋值    
                    att = xmlAttributeCollection["value"];
                    att.Value = value;
                }
                break;
            }

            //没有此节点，则新增
            if (i >= nodes.Count)
            {
                XmlElement title = doc.CreateElement("add");
                title.SetAttribute("key", strKey);
                title.SetAttribute("value", value);
                appSettingsNode.AppendChild(title);
            }

            //保存上面的修改    
            doc.Save(strFileName);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
