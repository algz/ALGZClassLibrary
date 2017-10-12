using System;
using System.Collections.Generic;
using System.Text;

using System.Collections.Specialized;
using System.IO;
using System.Windows.Forms;

namespace XCommon
{
    public class XStrings : StringCollection
    {
        public XStrings SubStrings(int iStart, int length)
        {
            XStrings xStrs = new XStrings();
            for (int i = 0; i < length; i++)
                xStrs.Add(this[iStart + i]);

            return xStrs;
        }

		public XStrings GetKeywordBlock(string strKeyword, int nBlockLines)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (this[i].Contains(strKeyword))
				{
					int nEnd = i + nBlockLines - 1;
					if (nEnd >= this.Count)
						throw new Exception("在关键字后不足" + nBlockLines + "行");

					return this.SubStrings(i, nBlockLines);
				}
			}
			
			throw new Exception("未找到" + strKeyword);
		}

		public bool ContainsAny(List<string> strKeywords, out string strFirstContainedKeyword)
		{
			strFirstContainedKeyword = string.Empty;

			string strAll = this.ToLongString();
			foreach (string keyword in strKeywords)
			{
				if (strAll.Contains(keyword))
				{
					strFirstContainedKeyword = keyword;
					return true;
				}
			}

			return false;
		}

		public bool ContainsString(string strKeyword)
		{
			List<string> strs = new List<string>();
			strs.Add(strKeyword);

			string strTemp;

			return ContainsAny(strs, out strTemp);
		}

		public bool ContainsAll(List<string> strKeywords, out string strFirstNotContainedKeyword)
		{
			strFirstNotContainedKeyword = string.Empty;
			string strAll = this.ToLongString();
			foreach (string keyword in strKeywords)
			{
				if (!strAll.Contains(keyword))
				{
					strFirstNotContainedKeyword = keyword;
					return false;
				}
			}

			return true;
		}

        public static implicit operator string[](XStrings xStrs)
        {
            string[] strs = new string[xStrs.Count];
            for (int i = 0; i < strs.Length; i++)
                strs[i] = xStrs[i];

            return strs;
        }

        public static implicit operator XStrings(string[] strs)
        {
            XStrings xStrs = new XStrings();
            xStrs.AddRange(strs);

            return xStrs;
        }

        /// <summary>
        /// 添加字符串到XStrings末尾。注意+=操作比Add方法效率稍低
        /// </summary>
        public static XStrings operator +(XStrings xstrs, string strNew)
        {
            XStrings xstrResult = new XStrings();

            xstrResult += xstrs;

            xstrResult.Add(strNew);

            return xstrResult;
        }

        public static XStrings operator +(XStrings xstr1, XStrings xstr2)
        {
            XStrings xstrNew = new XStrings();

            xstrNew.AddRange(xstr1);
            xstrNew.AddRange(xstr2);

            return xstrNew;
        }

        public static XStrings operator +(XStrings xstr1, float number)
        {
            return xstr1 + number.ToString("f");
        }

		public void SaveDos(string strFilePath)
		{
			Save(strFilePath, false);

			//File.WriteAllLines(strFilePath, ToPinString());
		}

		public void SaveUnix(string strFilePath)
		{
			Save(strFilePath, true);

			//File.WriteAllLines(strFilePath, ToPinString());
		}

		public void Save(string strFilePath, bool bUnixFormat)
		{
            if (this.Count < 1)
				throw new Exception("保存UNIX格式文件失败: 内容为空");

			File.WriteAllText(strFilePath, this.ToLongString(bUnixFormat));

		}

		public string ToLongString()
		{
			return ToLongString(false);
		}

		public string ToLongString(bool bUnixFormat)
		{
			string endl = bUnixFormat ? "\n" : Environment.NewLine;

			string strContent = "";

			foreach (string str in this)
			{
				strContent += str + endl;
			}

            // 删掉最后多写的一个"\n"
			return strContent.Remove(strContent.Length - endl.Length);
		}

		public string ToCommaSeparatString()
		{
			if (this.Count < 1)
				return string.Empty;

			string str = string.Empty;

			str += this[0];

			for (int i = 1; i < this.Count; i++)
			{
				str += "," + this[i];
			}

			return str;
		}

		public void Add(float value)
		{
			base.Add(value.ToString());
		}
    }

	public class StringTable : List<XStrings>
	{
		public XStrings Header = new XStrings();

		public XStrings ToCSVString()
		{
			XStrings xstrs = new XStrings();
			xstrs.Add(Header.ToCommaSeparatString());

			foreach (XStrings xstrLine in this)
			{
				xstrs.Add(xstrLine.ToCommaSeparatString());
			}

			return xstrs;
		}

		public void SaveCSV(string strFilePath)
		{
			ToCSVString().SaveDos(strFilePath);
		}

		public void Show(DataGridView dataGrid)
		{
			dataGrid.Columns.Clear();

			foreach (string strCol in this.Header)
			{
				dataGrid.Columns.Add(strCol, strCol);
			}

			foreach (XStrings xstrLine in this)
			{
				dataGrid.Rows.Add(xstrLine);
			}
		}
	}
}

