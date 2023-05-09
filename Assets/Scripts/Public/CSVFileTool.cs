using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
#if !Unuse_For_Build
using System.Data;
#endif
#endif
using System.IO;
using System.Text;

public class CSVFileTool
{
	#if UNITY_EDITOR
	
	#if !Unuse_For_Build
	
	public DataTable dataTable;
	/// <summary>
	/// 总列数.
	/// </summary>
	public int colCount;
	/// <summary>
	/// 总行数.
	/// </summary>
	public int rowCount;
	public string filePath;
	
	public CSVFileTool(string path)
	{
		this.filePath = path;
		dataTable = new DataTable();
		OpenCSV();
	}
	
	/// <summary>
	/// 获取某行某列的数据
	/// row:行,row = 1代表第一行
	/// col:列,col = 1代表第一列  
	/// </summary>
	/// <param name="row">Row.</param>
	/// <param name="col">Col.</param>
	public string this[int row, int col]
	{
		get
		{
			if(row - 1 > rowCount || col - 1 > colCount)
				return "";
			
			return dataTable.Rows[row - 1][col - 1].ToString();
		}
		set
		{
			dataTable.Rows[row - 1][col - 1] = value;
		}
	}
	
	public string this[int row, string colName]
	{
		get
		{
			if(row - 1 > rowCount)
				return "";
			
			return dataTable.Rows[row - 1][colName].ToString();
		}
		set
		{
			dataTable.Rows[row - 1][colName] = value;
		}
	}
	
	/// <summary>
	/// 将DataTable中数据写入到CSV文件中
	/// </summary>
	/// <param name="dt">提供保存数据的DataTable</param>
	/// <param name="fileName">CSV的文件路径</param>
	public void SaveCSV()
	{
		FileInfo fi = new FileInfo(filePath);
		if (!fi.Directory.Exists)
		{
			fi.Directory.Create();
		}
		FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
		StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
		string data = "";
		//写出列名称
		for (int i = 0; i < dataTable.Columns.Count; i++)
		{
			data += dataTable.Columns[i].ColumnName.ToString();
			if (i < dataTable.Columns.Count - 1)
			{
				data += ",";
			}
		}
		sw.Write(data+"\r");
		//写出各行数据
		for (int i = 0; i < dataTable.Rows.Count; i++)
		{
			data = "";
			for (int j = 0; j < dataTable.Columns.Count; j++)
			{
				string str = dataTable.Rows[i][j].ToString();
				str = str.Replace("\"", "\"\"");//替换英文双引号 英文冒号需要换成两个双引号
				if (str.Contains(",") || str.Contains("\"") || str.Contains("\r") || str.Contains("\n")) //含逗号 双引号 换行符的需要放到引号中
				{
					str = string.Format("\"{0}\"", str);
				}
				
				data += str;
				if (j < dataTable.Columns.Count - 1)
				{
					data += ",";
				}
			}
			
			//在最后一行不能用WriteLine.
			if(i < dataTable.Rows.Count - 1)
				sw.Write(data+"\r");
			else
				sw.Write(data);
			
		}
		sw.Close();
		fs.Close();
	}
	
	/// <summary>
	/// 将CSV文件的数据读取到DataTable中
	/// </summary>
	/// <param name="fileName">CSV文件路径</param>
	/// <returns>返回读取了CSV数据的DataTable</returns>
	private void OpenCSV()
	{
		FileStream fs = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
		
		StreamReader sr = new StreamReader(fs, Encoding.UTF8);
		//记录每次读取的一行记录
		string strLine = "";
		//记录每行记录中的各字段内容
		string[] aryLine = null;
		string[] tableHead = null;
		//标示是否是读取的第一行
		bool IsFirst = true;
		rowCount = 0;
		//逐行读取CSV中的数据
		while ((strLine = sr.ReadLine()) != null)
		{
			//strLine = Common.ConvertStringUTF8(strLine, encoding);
			//strLine = Common.ConvertStringUTF8(strLine);
			if (IsFirst == true)
			{
				tableHead = strLine.Split(',');
				IsFirst = false;
				colCount = tableHead.Length;
				//创建列
				for (int i = 0; i < colCount; i++)
				{
					DataColumn dc = new DataColumn(tableHead[i]);
					dataTable.Columns.Add(dc);
				}
			}
			else
			{
				aryLine = strLine.Split(',');
				DataRow dr = dataTable.NewRow();
				for (int j = 0; j < colCount; j++)
				{
					dr[j] = aryLine[j];
				}
				dataTable.Rows.Add(dr);
				
				rowCount ++;
			}
			
		}
		
		//		if (aryLine != null && aryLine.Length > 0)
		//		{
		//			dataTable.DefaultView.Sort = tableHead[0] + " " + "asc";
		//		}
		
		sr.Close();
		fs.Close();
	}
	
	public void AddNewRow(params string[] strParams)
	{
		DataRow dr = dataTable.NewRow();
		for (int j = 0; j < colCount; j++)
		{
			dr[j] = strParams[j];
		}
		dataTable.Rows.Add(dr);
		rowCount ++;
	}
	
	public void RemoveRow(int index)
	{
		if(index < 0 || index >= dataTable.Rows.Count)
			return;
		
		dataTable.Rows.RemoveAt(index);
		rowCount --;
	}
	
	#else
	
	
	/// <summary>
	/// 总列数.
	/// </summary>
	public int colCount;
	/// <summary>
	/// 总行数.
	/// </summary>
	public int rowCount;
	public string filePath;
	
	public CSVFileTool(string path)
	{
		
	}
	
	/// <summary>
	/// 获取某行某列的数据
	/// row:行,row = 1代表第一行
	/// col:列,col = 1代表第一列  
	/// </summary>
	/// <param name="row">Row.</param>
	/// <param name="col">Col.</param>
	public string this[int row, int col]
	{
		get
		{
			return "";
		}
		set
		{
			
		}
	}
	
	public string this[int row, string colName]
	{
		get
		{
			
			return "";
		}
		set
		{
			
		}
	}
	
	/// <summary>
	/// 将DataTable中数据写入到CSV文件中
	/// </summary>
	/// <param name="dt">提供保存数据的DataTable</param>
	/// <param name="fileName">CSV的文件路径</param>
	public void SaveCSV()
	{
		
	}
	
	/// <summary>
	/// 将CSV文件的数据读取到DataTable中
	/// </summary>
	/// <param name="fileName">CSV文件路径</param>
	/// <returns>返回读取了CSV数据的DataTable</returns>
	private void OpenCSV()
	{
		
	}
	
	public void AddNewRow(params string[] strParams)
	{
		
	}
	
	public void RemoveRow(int index)
	{
		
	}
	
	
	#endif
	#endif
}

