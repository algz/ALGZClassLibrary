using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;
using static System.Net.Mime.MediaTypeNames;

namespace AppConfigLibrary
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
        /// 采用XML方式保存App.config文件.
        /// (采用Windows自带的方式,由于windows处理逻辑不同,不能更改App.config文件.)
        /// </summary>
        /// <param name="key">add节点的key </param>
        /// <param name="val">add节点的value</param>
        /// <param name="appConfigFilePath"></param>
        public static void SetValue(string key, string val,string appConfigFilePath="")
        {
            XmlDocument doc = new XmlDocument();
            //获得配置文件的全路径  
            //string strFileName = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            // string  strFileName= AppDomain.CurrentDomain.BaseDirectory + "\\exe.config";  
            string strFileName = appConfigFilePath!=""? appConfigFilePath:System.AppDomain.CurrentDomain.BaseDirectory+ "App.config";
            doc.Load(strFileName);
            XmlNode node = doc.SelectSingleNode(@"//appSettings");

            XmlElement ele = (XmlElement)node.SelectSingleNode(@"//add[@key='"+ key + "']");
            ele.SetAttribute("value", val);
            doc.Save(strFileName);
            ////找出名称为“add”的所有元素  
            //XmlNodeList nodes = doc.GetElementsByTagName("add");
            //for (int i = 0; i < nodes.Count; i++)
            //{
            //    //获得将当前元素的key属性  
            //    XmlAttribute att = nodes[i].Attributes["key"];
            //    XmlElement ele = (XmlElement)nodes[i].SelectSingleNode(@"//add[@key='"+ ConnenctionString + "']");

            //    ele.SetAttribute("value", strKey);
            //    doc.Save(strFileName());
            //    //根据元素的第一个属性来判断当前的元素是不是目标元素  
            //    if (att.Value == strKey)
            //    {
            //        //对目标元素中的第二个属性赋值  
            //        att = nodes[i].Attributes["value"];
            //        att.Value = ConnenctionString;
            //        break;
            //    }
            //}
            ////保存上面的修改  
            //doc.Save(strFileName);
            //System.Configuration.ConfigurationManager.RefreshSection("appSettings");
        }

        /// <summary>
        /// 采用XML方式读取App.config文件.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="appConfigFilePath"></param>
        /// <returns></returns>
        public static string GetValue(string key, string appConfigFilePath = "")
        {
            
            XmlDocument xDoc = new XmlDocument();
            try
            {
                
                string curDllPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);//获取当前DLL路径。
                string strFileName = appConfigFilePath != "" ? appConfigFilePath : curDllPath + @"\App.config";//S 
                //System.AppDomain.CurrentDomain.BaseDirectory：获取执行当前程序的路径，如 >python test.py   ，获取的是“python路径”； curDllPath 获取的是test.py 中所调用的“DLL文件路径”。
                //string strFileName = appConfigFilePath != "" ? appConfigFilePath : System.AppDomain.CurrentDomain.BaseDirectory + "App.config";

                xDoc.Load(strFileName);
                XmlNode xNode;
                XmlElement xElem;
                xNode = xDoc.SelectSingleNode("//appSettings");　　　　//补充，需要在你的app.config 文件中增加一下，<appSetting> </appSetting>
                xElem = (XmlElement)xNode.SelectSingleNode("//add[@key='" + key + "']");
                if (xElem != null)
                    return xElem.GetAttribute("value");
                else
                    return "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Obsolete("此方法并不能完全读取App.config文件,请采用GetValue.")]
        public static string GetValueByKey(string key)
        {

            return ConfigurationManager.AppSettings[key];
        }

        /// <summary>
        /// 修改AppSettings中配置
        /// </summary>
        /// <param name="key">key值</param>
        /// <param name="value">相应值</param>
        [Obsolete("此方法并不能完全修改App.config文件,请采用SaveConfig.")]//标记该方法已弃用
        public static bool SetConfigValue(string key, string value)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (config.AppSettings.Settings[key] != null)
                    config.AppSettings.Settings[key].Value = value;
                else
                    config.AppSettings.Settings.Add(key, value);
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="strKey"></param>
        /// <param name="value"></param>
        [Obsolete("此方法并不能完全修改App.config文件,请采用SaveConfig.")]//标记该方法已弃用
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
