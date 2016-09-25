using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pollenalarm.Backend.Helper
{
    public static class DataTableHelper
    {
        // Helper function for ADO.Net Bulkcopy to transfer a IEnumerable list to a datatable
        // Adapted from: http://msdn.microsoft.com/en-us/library/bb396189.aspx
        public static DataTable CopyToDataTable<T>(this IEnumerable<T> source)
        {
            return new DataTableCreator<T>().CreateDataTable(source, null, null);
        }

        public static DataTable CopyToDataTable<T>(this IEnumerable<T> source, DataTable table, LoadOption? options)
        {
            return new DataTableCreator<T>().CreateDataTable(source, table, options);
        }

        public static void BulkCopyToDatabase<T>(this IEnumerable<T> source, System.Data.Linq.DataContext databaseContext) where T : class
        {
            using (var dataTable = CopyToDataTable(source))
            {
                using (var bulkCopy = new SqlBulkCopy(ConfigurationManager.ConnectionStrings["AzureDatabaseConnection"].ConnectionString, SqlBulkCopyOptions.KeepIdentity & SqlBulkCopyOptions.KeepNulls))
                {
                    foreach (DataColumn dc in dataTable.Columns)
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping(dc.ColumnName, dc.ColumnName));

                    //  We could use "dataTable.TableName" in the following line, but this does sometimes have problems, as
                    //  LINQ-to-SQL will drop trailing "s" off table names, so try to insert into [Product], rather than [Products]
                    bulkCopy.DestinationTableName = dataTable.TableName.Replace("Entity", "");
                    bulkCopy.WriteToServer(dataTable);
                }
            }
        }
    }

    public class DataTableCreator<T>
    {
        private FieldInfo[] _fi;
        private PropertyInfo[] _pi;
        private Dictionary<string, int> _ordinalMap;
        private Type _type;

        public DataTableCreator()
        {
            _type = typeof(T);
            _fi = _type.GetFields();
            _pi = _type.GetProperties();
            _ordinalMap = new Dictionary<string, int>();
        }

        public DataTable CreateDataTable(IEnumerable<T> source, DataTable table, LoadOption? options)
        {
            if (typeof(T).IsPrimitive)
            {
                return CreateDatatablePrimitive(source, table, options);
            }


            if (table == null)
            {
                table = new DataTable(typeof(T).Name);
            }

            // now see if need to extend datatable base on the type T + build ordinal map
            table = ExtendTable(table, typeof(T));

            table.BeginLoadData();
            using (IEnumerator<T> e = source.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    if (options != null)
                    {
                        table.LoadDataRow(CreateDataRow(table, e.Current), (LoadOption)options);
                    }
                    else
                    {
                        table.LoadDataRow(CreateDataRow(table, e.Current), true);
                    }
                }
            }
            table.EndLoadData();
            return table;
        }

        public DataTable CreateDatatablePrimitive(IEnumerable<T> source, DataTable table, LoadOption? options)
        {
            if (table == null)
            {
                table = new DataTable(typeof(T).Name);
            }

            if (!table.Columns.Contains("Value"))
            {
                table.Columns.Add("Value", typeof(T));
            }

            table.BeginLoadData();
            using (IEnumerator<T> e = source.GetEnumerator())
            {
                Object[] values = new object[table.Columns.Count];
                while (e.MoveNext())
                {
                    values[table.Columns["Value"].Ordinal] = e.Current;

                    if (options != null)
                    {
                        table.LoadDataRow(values, (LoadOption)options);
                    }
                    else
                    {
                        table.LoadDataRow(values, true);
                    }
                }
            }
            table.EndLoadData();
            return table;
        }

        public DataTable ExtendTable(DataTable table, Type type)
        {
            // value is type derived from T, may need to extend table.
            foreach (FieldInfo f in type.GetFields())
            {
                if (!_ordinalMap.ContainsKey(f.Name))
                {
                    DataColumn dc = table.Columns.Contains(f.Name) ? table.Columns[f.Name]
                        : table.Columns.Add(f.Name, f.FieldType);
                    _ordinalMap.Add(f.Name, dc.Ordinal);
                }
            }
            foreach (PropertyInfo p in type.GetProperties())
            {
                if (!_ordinalMap.ContainsKey(p.Name))
                {
                    Type propType = p.PropertyType;
                    if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        propType = Nullable.GetUnderlyingType(propType);
                    }
                    //if (propType.Namespace == "System")
                    if (p.PropertyType.FullName.ToString().StartsWith("System")
                                && !p.PropertyType.FullName.ToString().StartsWith("System.Linq")
                                && !p.PropertyType.FullName.ToString().StartsWith("System.Data")
                                && !p.Name.ToString().StartsWith("Search"))
                    {
                        DataColumn dc = table.Columns.Contains(p.Name) ? table.Columns[p.Name]
                            : table.Columns.Add(p.Name, propType);
                        _ordinalMap.Add(p.Name, dc.Ordinal);
                    }
                }
            }
            return table;
        }

        public object[] CreateDataRow(DataTable table, T instance)
        {

            FieldInfo[] fi = _fi;
            PropertyInfo[] pi = _pi;

            if (instance.GetType() != typeof(T))
            {
                ExtendTable(table, instance.GetType());
                fi = instance.GetType().GetFields();
                pi = instance.GetType().GetProperties();
            }

            Object[] values = new object[table.Columns.Count];
            foreach (FieldInfo f in fi)
            {
                values[_ordinalMap[f.Name]] = f.GetValue(instance);
            }

            foreach (PropertyInfo p in pi)
            {
                if (p.PropertyType.FullName.ToString().StartsWith("System")
                            && !p.PropertyType.FullName.ToString().StartsWith("System.Linq")
                            && !p.PropertyType.FullName.ToString().StartsWith("System.Data")
                            && !p.Name.ToString().StartsWith("Search"))
                {
                    values[_ordinalMap[p.Name]] = p.GetValue(instance, null);
                }
            }
            return values;
        }
    }
}
