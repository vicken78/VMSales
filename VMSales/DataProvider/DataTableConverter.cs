﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;
using System.Collections.Specialized;
using System.Reflection;
using System.Diagnostics;

namespace VMSales.ViewModels
{
    public class DataTableConverter
    {
        public static List<T> ConvertTo<T>(DataTable table, List<T> entity)

        {
            entity = null;
            if (table == null)
            {
                Debug.WriteLine("EXPECTED Table is null");
                return null;
            }
            try
            {
                entity = ConvertTo<T>(table.Rows);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return entity;
        }
        static List<T> ConvertTo<T>(DataRowCollection rows)
        {
            List<T> list = null;
            if (rows != null)
            {
                list = new List<T>();

                foreach (DataRow row in rows)
                {
                    T item = CreateItem<T>(row);
                    list.Add(item);
                }
            }
            return list;
        }
        static T CreateItem<T>(DataRow row)
        {
            string str = string.Empty;
            string strObj = string.Empty;
            T obj = default(T);
            if (row != null)
            {
                obj = Activator.CreateInstance<T>();
                strObj = obj.ToString();
                NameValueCollection objDictionary = new NameValueCollection();

                foreach (DataColumn column in row.Table.Columns)
                {
                    PropertyInfo prop = obj.GetType().GetProperty(column.ColumnName);
                    if (prop != null)
                    {
                        str = column.ColumnName;
                        try
                        {
                            objDictionary.Add(str, row[str].ToString());
                            object value = row[column.ColumnName];
                            Type vType = obj.GetType();
                            if (value == DBNull.Value)
                            {
                                if (vType == typeof(int) || vType == typeof(Int16)
                                                                 || vType == typeof(Int32)
                                                                 || vType == typeof(Int64)
                                                                 || vType == typeof(decimal)
                                                                 || vType == typeof(float)
                                                                 || vType == typeof(double))
                                {
                                    value = 0;
                                }
                                else if (vType == typeof(bool))
                                {
                                    value = false;
                                }
                                else if (vType == typeof(DateTime))
                                {
                                    value = DateTime.MaxValue;
                                }
                                else
                                {
                                    value = null;
                                }
                                prop.SetValue(obj, value, null);
                            }
                            else
                            {
                                prop.SetValue(obj, value, null);
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
                PropertyInfo ActionProp = obj.GetType().GetProperty("ActionTemplateValue");
                if (ActionProp != null)
                {
                    object ActionValue = objDictionary;
                    ActionProp.SetValue(obj, ActionValue, null);
                }
            }
            return obj;

        }

    }
}
