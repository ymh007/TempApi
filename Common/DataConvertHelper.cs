using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Seagull2.YuanXin.AppApi
{
	/// <summary>
	/// 数据转换类
	/// </summary>
	public class DataConvertHelper<T> where T : new()
	{
		/// <summary>
		/// 将 DataRow 转换为 Model
		/// </summary>
		public static T ConvertToModel(DataRow dr)
		{
			T t = new T();
			PropertyInfo[] propertys = t.GetType().GetProperties();
			foreach (PropertyInfo pi in propertys)
			{
				if (dr.Table.Columns.Contains(pi.Name) && pi.CanWrite)
				{
					object value = dr[pi.Name];
					if (value != null && value != DBNull.Value)
					{
						pi.SetValue(t, value);
					}
				}
			}
			return t;
		}

		/// <summary>
		/// 将 DataTable 转换为 Model
		/// </summary>
		/// <param name="dt">DataTable</param>
		/// <param name="keyIndex">字段名所在列的索引</param>
		/// <param name="dataIndex">数据所在列的索引</param>
		public static T ConvertToModel(DataTable dt, int keyIndex, int dataIndex)
		{
			T t = new T();
			PropertyInfo[] propertys = t.GetType().GetProperties();

			var key = dt.Columns[keyIndex].ColumnName;
			var data = dt.Columns[dataIndex].ColumnName;

			foreach (PropertyInfo pi in propertys)
			{
				if (pi.CanWrite)
				{
					DataRow[] drs = dt.Select("[" + key + "]='" + pi.Name + "'");
					if (drs.Length > 0)
					{
						object value = drs[0][data];
						if (value != null && value != DBNull.Value)
						{
							pi.SetValue(t, value, null);
						}
					}
				}
			}
			return t;
		}

		/// <summary>
		/// 将 DataTable 转换为 List
		/// </summary>
		public static List<T> ConvertToList(DataTable dt)
		{
			List<T> list = new List<T>();
			foreach (DataRow dr in dt.Rows)
			{
				T t = new T();
				PropertyInfo[] propertys = t.GetType().GetProperties();
				foreach (PropertyInfo pi in propertys)
				{
					if (dt.Columns.Contains(pi.Name) && pi.CanWrite)
					{
						object value = dr[pi.Name];
						if (value != null && value != DBNull.Value)
						{
							pi.SetValue(t, value, null);
						}
					}
				}
				list.Add(t);
			}
			return list;
		}

		/// <summary>
		/// 将string转基础数据类型
		/// </summary>
		/// <param name="formData"></param>
		/// <returns></returns>
		public static T ConvertToBaseType(string fromData)
		{
			T t = new T();
			if (t.GetType() == typeof(System.Int32))
			{
				Int32.TryParse(fromData, out int result);
				return (T)(object)result;
			}
			if (t.GetType() == typeof(System.Decimal))
			{
				decimal.TryParse(fromData, out decimal result);
				return (T)(object)result;
			}
			if (t.GetType() == typeof(System.DateTime))
			{
				DateTime.TryParse(fromData, out DateTime result);
				return (T)(object)result;
			}
			return default(T);
		}
	}
}