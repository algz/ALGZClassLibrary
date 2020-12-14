using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RegexLibrary
{

    /*
    1.正则表达式的作用：用来描述字符串的特征。

    2.各个匹配字符的含义：
    .   ：表示除\n以外的单个字符
    [ ]  ：表示在字符数组[]中罗列出来的字符任意取单个
    |   ：表示“或”的意思
    ()  ：表示改变优先级或"提取组"
    *   ：限定前面的表达式出现0次或多次
    +   ：限定前面的表达式出现1次或多次
    ？  ：限定前面的表达式出现0次或1次
    ^   ：表示以表达式开头（例：^http表示字符串以“http”开头）
    $   ：表示以表达式结尾 （例：com$表示字符串以“com”结尾）
    \d  ： 小写\d表示0-9之间的数字
    \D  ：大写\D表示除了0-9之外的字符
    \w  ：小写\w表示[a-zA-Z0-9]
    \W  ：大写\W表示除了[a-zA-Z0-9]之外的字符
    \s   ：小写\s表示非可见字符（如空格、tab、\r\n........）
    \S  ：大写\S表示除了非可见字符之外的字符
        
    3. 正则表达式举例
    匹配邮政编码：^[0-9]{6}$
    匹配10~25之间的数字：^(1[0-9]|2[0-5])$
    大致匹配邮箱格式：^[a-zA-Z0-9_]+@[a-zA-Z0-9]+(\.[a-zA-Z0-9]+){1,2}$

    4.使用正则表达式匹配字符串
    需要引用：System.Text.RegularExpressions;
    Regex.IsMatch()方法：来判断给定的字符串是否匹配某个正则表达式
    Regex.Match()方法：从给定的字符串中提取出一个与正则表达式匹配的字符串
    Regex.Matches()方法：从给定的字符串中提取出所有与正则表达式匹配的字符串
    Regex.Replace()方法：替换所有与正则表达式匹配的字符串为另一个字符串
 */
    /// <summary>
    /// </summary>
    public class RegexClass
    {
        /// <summary>
        /// 替换指定之符之间(包括指定字符)的所有内容.(默认为(与)之间的字符)
        /// </summary>
        /// <param name="val"></param>
        /// <param name="startChar"></param>
        /// <param name="endChar"></param>
        /// <param name="replaceChar"></param>
        /// <returns></returns>
        public static string ReplaceCharToChar(string val, string startChar=@"\(", string endChar=@"\)", string replaceChar = "")
        {
            //删除括号内的所有字符串('*'号可替换为任意字符,但在^后必须有一字符)
            Regex reg = new Regex(startChar + "[^*]+" + endChar);
            //Regex reg = new Regex("(HOST="+"[^*]+" + ")\\(");
            string str2 = reg.Replace(val, replaceChar);
            return str2;
        }

        public static string NotReplaceCharToChar(string val, string startChar, string endChar, string replaceChar = "")
        {
            //删除除括号外的所有字符串
            Regex reg1 = new Regex("[^\\(\\)\r\n]+(\\([^\\(\\)]+\\))");
            string str3 = reg1.Replace(val, replaceChar);
            return str3;
        }

        public static string MatcheChar(string val, string startChar = @"\(", string endChar = @"\)")
        {
            //删除括号内的所有字符串('*'号可替换为任意字符,但在^后必须有一字符)
            Regex reg = new Regex(startChar + "[^*]+" + endChar);
            //Regex reg = new Regex("(ST="+"[^*]+" + ")\\(P");
            string str2 = reg.Match(val).ToString();
            return str2;
        }
    }
}
