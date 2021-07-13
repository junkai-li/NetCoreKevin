using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Extension
{
    public static class DataTableExtension
    {
        public static DataTable Where(this DataTable dt, string whereCondition)
        {
            if (dt == null)
            {
                return null;
            }
            var dataTable = new DataTable();
            var clonedRow = dt.Select(whereCondition);
            foreach (DataColumn d in dt.Columns)
            {
                dataTable.Columns.Add(d.ColumnName);
            }
            clonedRow.Foreach(r => { dataTable.Rows.Add(r.ItemArray); });
            return dataTable;
        }
    }
}
