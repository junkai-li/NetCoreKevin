using Kevin.Common.Extension;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Eval;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Kevin.Common.Helper
{
    public class NPOIHelper : ControllerBase
    { 

        #region 从datatable中将数据导出到excel

        /// <summary>
        /// DataTable导出到Excel的MemoryStream
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">表头文本</param>
        public FileStreamResult ExportDTI(DataTable dtSource, string fileName)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            XSSFSheet sheet = workbook.CreateSheet() as XSSFSheet;

            #region 右击文件 属性信息
            //{
            //    DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            //    dsi.Company = "http://www.yongfa365.com/";
            //    workbook.DocumentSummaryInformation = dsi;

            //    SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            //    si.Author = "新增作者"; //填加xls文件作者信息
            //    si.ApplicationName = "NPOI测试程序"; //填加xls文件创建程序信息
            //    si.LastAuthor = "最后作者"; //填加xls文件最后保存者信息
            //    si.Comments = "说明信息"; //填加xls文件作者信息
            //    si.Title = "NPOI测试"; //填加xls文件标题信息
            //    si.Subject = "NPOI测试Demo"; //填加文件主题信息
            //    si.CreateDateTime = DateTime.Now;
            //    workbook.SummaryInformation = si;
            //}

            #endregion

            XSSFCellStyle dateStyle = workbook.CreateCellStyle() as XSSFCellStyle;
            XSSFDataFormat format = workbook.CreateDataFormat() as XSSFDataFormat;
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

            //取得列宽
            int[] arrColWidth = new int[dtSource.Columns.Count];
            foreach (DataColumn item in dtSource.Columns)
            {
                arrColWidth[item.Ordinal] = 25 * 256;
            }

            int rowIndex = 0;

            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表，填充表头，填充列头，样式

                if (rowIndex == 0)
                {
                    #region 表头及样式
                    //{
                    //    XSSFRow headerRow = sheet.CreateRow(0) as XSSFRow;
                    //    headerRow.HeightInPoints = 25;
                    //    headerRow.CreateCell(0).SetCellValue(strHeaderText);

                    //    XSSFCellStyle headStyle = workbook.CreateCellStyle() as XSSFCellStyle;
                    //    headStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
                    //    XSSFFont font = workbook.CreateFont() as XSSFFont;
                    //    font.FontHeightInPoints = 20;
                    //    font.Boldweight = 700;
                    //    headStyle.SetFont(font);

                    //    headerRow.GetCell(0).CellStyle = headStyle;

                    //    //sheet.AddMergedRegion(new Region(0, 0, 0, dtSource.Columns.Count - 1));
                    //    //headerRow.Dispose();
                    //}

                    #endregion


                    #region 列头及样式

                    {
                        XSSFRow headerRow = sheet.CreateRow(0) as XSSFRow;


                        XSSFCellStyle headStyle = workbook.CreateCellStyle() as XSSFCellStyle;
                        headStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                        XSSFFont font = workbook.CreateFont() as XSSFFont;
                        font.FontHeightInPoints = 10;
                        //font.Boldweight = 700;
                        headStyle.SetFont(font);


                        foreach (DataColumn column in dtSource.Columns)
                        {
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                            headerRow.GetCell(column.Ordinal).CellStyle = headStyle;

                            //设置列宽
                            sheet.SetColumnWidth(column.Ordinal, 25 * 256);

                        }
                        //headerRow.Dispose();
                    }

                    #endregion

                    rowIndex = 1;
                }

                #endregion

                #region 填充内容

                XSSFRow dataRow = sheet.CreateRow(rowIndex) as XSSFRow;
                foreach (DataColumn column in dtSource.Columns)
                {
                    XSSFCell newCell = dataRow.CreateCell(column.Ordinal) as XSSFCell;

                    string drValue = row[column].ToString();

                    switch (column.DataType.ToString())
                    {
                        case "System.String": //字符串类型
                            double result;
                            if (isNumeric(drValue, out result))
                            {

                                double.TryParse(drValue, out result);
                                newCell.SetCellValue(result);
                                break;
                            }
                            else
                            {
                                newCell.SetCellValue(drValue);
                                break;
                            }

                        case "System.DateTime": //日期类型
                            DateTime dateV;
                            DateTime.TryParse(drValue, out dateV);
                            newCell.SetCellValue(dateV);

                            newCell.CellStyle = dateStyle; //格式化显示
                            break;
                        case "System.Boolean": //布尔型
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            newCell.SetCellValue(boolV);
                            break;
                        case "System.Int16": //整型
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(drValue, out intV);
                            newCell.SetCellValue(intV);
                            break;
                        case "System.Decimal": //浮点型
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(doubV);
                            break;
                        case "System.DBNull": //空值处理
                            newCell.SetCellValue("");
                            break;
                        default:
                            newCell.SetCellValue("");
                            break;
                    }

                }

                #endregion

                rowIndex++;
            }
            MemoryStream ms = new MemoryStream();
            FileStream file = new FileStream("D://原始动态.xls", FileMode.Create);
            workbook.Write(file);
            file.Close();
            workbook = null;
            ms.Flush();
            ms.Position = 0;
            return null;

            //using (MemoryStream BookStream = new MemoryStream())//定义文件流
            //{
            //    workbook.Write(BookStream);//将工作薄写入文件流
            //    BookStream.Seek(0, SeekOrigin.Begin);//输出之前调用Seek（偏移量，游标位置）方法：获取文件流的长度
            //    return File(BookStream, "application/vnd.ms-excel", fileName); // 文件类型/文件名称/
            //} 
        }

        public static HSSFWorkbook ExportExcelByDataTable(DataTable dataTable, string sheetName = "sheet1")
        {

            HSSFWorkbook wb = new HSSFWorkbook();
            HSSFSheet sheet = (HSSFSheet)wb.CreateSheet(sheetName); //创建工作表
            sheet.CreateFreezePane(0, 1); //冻结列头行
            HSSFRow row_Title = (HSSFRow)sheet.CreateRow(0); //创建列头行
            row_Title.HeightInPoints = 30.5F; //设置列头行高
            HSSFCellStyle cs_Title = (HSSFCellStyle)wb.CreateCellStyle(); //创建列头样式
            cs_Title.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center; //水平居中
            cs_Title.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center; //垂直居中
            HSSFFont cs_Title_Font = (HSSFFont)wb.CreateFont(); //创建字体
            cs_Title_Font.IsBold = true; //字体加粗
            cs_Title_Font.FontHeightInPoints = 14; //字体大小
            cs_Title.SetFont(cs_Title_Font); //将字体绑定到样式

            #region 生成列头
            int ii = 0;
            foreach (DataColumn column in dataTable.Columns)
            {
                HSSFCell cell_Title = (HSSFCell)row_Title.CreateCell(ii); //创建单元格
                cell_Title.CellStyle = cs_Title; //将样式绑定到单元格
                cell_Title.SetCellValue(column.ColumnName);
                sheet.SetColumnWidth(ii, 25 * 256);//设置列宽
                ii++;
            }
            #endregion

            HSSFCellStyle cs_Content = (HSSFCellStyle)wb.CreateCellStyle(); //创建列头样式
            cs_Content.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center; //水平居中
            cs_Content.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center; //垂直居中
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                DataRow row = dataTable.Rows[i];
                HSSFRow row_Content = (HSSFRow)sheet.CreateRow(i + 1); //创建行
                row_Content.HeightInPoints = 20;
                int jj = 0;
                foreach (DataColumn column in dataTable.Columns)
                {
                    string drValue = row[column].ToString();
                    HSSFCell cell_Conent = (HSSFCell)row_Content.CreateCell(jj); //创建单元格
                    cell_Conent.CellStyle = cs_Content;
                    cell_Conent.SetCellValue(drValue);
                    jj++;
                }
            }
            return wb;
        }

        public FileStreamResult ExportExcelFileStream(string fileName, DataTable dataTable, string sheetName = "sheet1")
        {
            var workbook = ExportExcelByDataTable(dataTable, sheetName);
            //把Excel转化为文件流，输出
            MemoryStream BookStream = new MemoryStream();//定义文件流
            workbook.Write(BookStream);//将工作薄写入文件流
            BookStream.Seek(0, SeekOrigin.Begin);//输出之前调用Seek（偏移量，游标位置）方法：获取文件流的长度
            return File(BookStream, "application/vnd.ms-excel", fileName); // 文件类型/文件名称/
        }
        #endregion

        #region 从excel中将数据导出到datatable
        /// <summary>
        /// 读取excel 默认第一行为标头
        /// </summary>
        /// <param name="strFileName">excel文档路径</param>
        /// <returns></returns>
        public static DataTable ImportExceltoDt(string strFileName)
        {
            DataTable dt = new DataTable();
            IWorkbook wb;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                wb = WorkbookFactory.Create(file);
            }
            ISheet sheet = wb.GetSheetAt(0);
            dt = ImportDt(sheet, 0, true);
            return dt;
        }

        /// <summary>
        /// 读取Excel流到DataTable
        /// </summary>
        /// <param name="stream">Excel流</param>
        /// <returns>第一个sheet中的数据</returns>
        public static DataTable ImportExceltoDt(Stream stream)
        {
            try
            {
                DataTable dt = new DataTable();
                IWorkbook wb;
                using (stream)
                {
                    wb = WorkbookFactory.Create(stream);
                }
                ISheet sheet = wb.GetSheetAt(0);
                dt = ImportDt(sheet, 0, true);
                return dt;
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 读取Excel流到DataTable
        /// </summary>
        /// <param name="stream">Excel流</param>
        /// <param name="sheetName">表单名</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns>指定sheet中的数据</returns>
        public static DataTable ImportExceltoDt(Stream stream, string sheetName, int HeaderRowIndex)
        {
            try
            {
                DataTable dt = new DataTable();
                IWorkbook wb;
                using (stream)
                {
                    wb = WorkbookFactory.Create(stream);
                }
                ISheet sheet = wb.GetSheet(sheetName);
                dt = ImportDt(sheet, HeaderRowIndex, true);
                return dt;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 读取Excel流到DataSet
        /// </summary>
        /// <param name="stream">Excel流</param>
        /// <returns>Excel中的数据</returns>
        public static DataSet ImportExceltoDs(Stream stream)
        {
            try
            {
                DataSet ds = new DataSet();
                IWorkbook wb;
                using (stream)
                {
                    wb = WorkbookFactory.Create(stream);
                }
                for (int i = 0; i < wb.NumberOfSheets; i++)
                {
                    DataTable dt = new DataTable();
                    ISheet sheet = wb.GetSheetAt(i);
                    dt = ImportDt(sheet, 0, true);
                    ds.Tables.Add(dt);
                }
                return ds;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 读取Excel流到DataSet
        /// </summary>
        /// <param name="stream">Excel流</param>
        /// <param name="dict">字典参数，key：sheet名，value：列头所在行号，-1表示没有列头</param>
        /// <returns>Excel中的数据</returns>
        public static DataSet ImportExceltoDs(Stream stream, Dictionary<string, int> dict)
        {
            try
            {
                DataSet ds = new DataSet();
                IWorkbook wb;
                using (stream)
                {
                    wb = WorkbookFactory.Create(stream);
                }
                foreach (string key in dict.Keys)
                {
                    DataTable dt = new DataTable();
                    ISheet sheet = wb.GetSheet(key);
                    dt = ImportDt(sheet, dict[key], true);
                    ds.Tables.Add(dt);
                }
                return ds;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 读取excel
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheet">需要导出的sheet</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns></returns>
        public static DataTable ImportExceltoDt(string strFileName, string SheetName, int HeaderRowIndex)
        {
            //HSSFWorkbook workbook;
            IWorkbook wb;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                wb = new HSSFWorkbook(file);
            }
            ISheet sheet = wb.GetSheet(SheetName);
            DataTable table = new DataTable();
            table = ImportDt(sheet, HeaderRowIndex, true);
            //ExcelFileStream.Close();
            //workbook = null;
            sheet = null;
            return table;
        }

        /// <summary>
        /// 读取excel
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheet">需要导出的sheet序号</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns></returns>
        public static DataTable ImportExceltoDt(string strFileName, int SheetIndex, int HeaderRowIndex)
        {
            //HSSFWorkbook workbook;
            IWorkbook wb;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                wb = WorkbookFactory.Create(file);
            }
            ISheet isheet = wb.GetSheetAt(SheetIndex);
            DataTable table = new DataTable();
            table = ImportDt(isheet, HeaderRowIndex, true);
            //ExcelFileStream.Close();
            //workbook = null;
            isheet = null;
            return table;
        }

        /// <summary>
        /// 读取excel
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheet">需要导出的sheet</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns></returns>
        public static DataTable ImportExceltoDt(string strFileName, string SheetName, int HeaderRowIndex, bool needHeader)
        {
            //HSSFWorkbook workbook;
            IWorkbook wb;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                wb = WorkbookFactory.Create(file);
            }
            ISheet sheet = wb.GetSheet(SheetName);
            DataTable table = new DataTable();
            table = ImportDt(sheet, HeaderRowIndex, needHeader);
            //ExcelFileStream.Close();
            //workbook = null;
            sheet = null;
            return table;
        }

        /// <summary>
        /// 读取excel
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheet">需要导出的sheet序号</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns></returns>
        public static DataTable ImportExceltoDt(string strFileName, int SheetIndex, int HeaderRowIndex, bool needHeader)
        {
            //HSSFWorkbook workbook;
            IWorkbook wb;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                wb = WorkbookFactory.Create(file);
            }
            ISheet sheet = wb.GetSheetAt(SheetIndex);
            DataTable table = new DataTable();
            table = ImportDt(sheet, HeaderRowIndex, needHeader);
            //ExcelFileStream.Close();
            //workbook = null;
            sheet = null;
            return table;
        }

        /// <summary>
        /// 将制定sheet中的数据导出到datatable中
        /// </summary>
        /// <param name="sheet">需要导出的sheet</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns></returns>
        static DataTable ImportDt(ISheet sheet, int HeaderRowIndex, bool needHeader)
        {
            DataTable table = new DataTable();
            IRow headerRow;
            int cellCount;

            if (HeaderRowIndex < 0 || !needHeader)
            {
                headerRow = sheet.GetRow(0);
                cellCount = headerRow.LastCellNum;

                for (int i = headerRow.FirstCellNum; i <= cellCount; i++)
                {
                    DataColumn column = new DataColumn(Convert.ToString(i));
                    table.Columns.Add(column);
                }
            }
            else
            {
                headerRow = sheet.GetRow(HeaderRowIndex);
                cellCount = headerRow.LastCellNum;

                for (int i = headerRow.FirstCellNum; i <= cellCount; i++)
                {
                    if (headerRow.GetCell(i) == null)
                    {
                        if (table.Columns.IndexOf(Convert.ToString(i)) > 0)
                        {
                            DataColumn column = new DataColumn(Convert.ToString("重复列名" + i));
                            table.Columns.Add(column);
                        }
                        else
                        {
                            DataColumn column = new DataColumn(Convert.ToString(i));
                            table.Columns.Add(column);
                        }

                    }
                    else if (table.Columns.IndexOf(headerRow.GetCell(i).ToString()) > 0)
                    {
                        DataColumn column = new DataColumn(Convert.ToString("重复列名" + i));
                        table.Columns.Add(column);
                    }
                    else
                    {
                        DataColumn column = new DataColumn(headerRow.GetCell(i).ToString());
                        table.Columns.Add(column);
                    }
                }
            }
            int rowCount = sheet.LastRowNum;
            for (int i = (HeaderRowIndex + 1); i <= sheet.LastRowNum; i++)
            {

                IRow row;
                if (sheet.GetRow(i) == null)
                {
                    row = sheet.CreateRow(i);
                }
                else
                {
                    row = sheet.GetRow(i);
                }

                DataRow dataRow = table.NewRow();

                for (int j = row.FirstCellNum; j <= cellCount; j++)
                {

                    if (row.GetCell(j) != null)
                    {
                        switch (row.GetCell(j).CellType)
                        {
                            case CellType.String:
                                string str = row.GetCell(j).StringCellValue;
                                if (str != null && str.Length > 0)
                                {
                                    dataRow[j] = str.ToString();
                                }
                                else
                                {
                                    dataRow[j] = null;
                                }
                                break;
                            case CellType.Numeric:
                                if (DateUtil.IsCellDateFormatted(row.GetCell(j)))
                                {
                                    dataRow[j] = DateTime.FromOADate(row.GetCell(j).NumericCellValue);
                                }
                                else
                                {
                                    dataRow[j] = Convert.ToDouble(row.GetCell(j).NumericCellValue);
                                }
                                break;
                            case CellType.Boolean:
                                dataRow[j] = Convert.ToString(row.GetCell(j).BooleanCellValue);
                                break;
                            case CellType.Error:
                                dataRow[j] = ErrorEval.GetText(row.GetCell(j).ErrorCellValue);
                                break;
                            case CellType.Formula:
                                switch (row.GetCell(j).CachedFormulaResultType)
                                {
                                    case CellType.String:
                                        string strFORMULA = row.GetCell(j).StringCellValue;
                                        if (strFORMULA != null && strFORMULA.Length > 0)
                                        {
                                            dataRow[j] = strFORMULA.ToString();
                                        }
                                        else
                                        {
                                            dataRow[j] = null;
                                        }
                                        break;
                                    case CellType.Numeric:
                                        dataRow[j] = Convert.ToString(row.GetCell(j).NumericCellValue);
                                        break;
                                    case CellType.Boolean:
                                        dataRow[j] = Convert.ToString(row.GetCell(j).BooleanCellValue);
                                        break;
                                    case CellType.Error:
                                        dataRow[j] = ErrorEval.GetText(row.GetCell(j).ErrorCellValue);
                                        break;
                                    default:
                                        dataRow[j] = "";
                                        break;
                                }
                                break;
                            default:
                                dataRow[j] = "";
                                break;
                        }
                    }

                }
                table.Rows.Add(dataRow);

            }

            return table;
        }

        #endregion

        #region 从List中将数据导出到excel
        public static IWorkbook ExportExcelGroupByList<T>(string fileName, List<T> list, Dictionary<string, string> FieldNames, string sheetName = "sheet1")
        {
            string extension = System.IO.Path.GetExtension(fileName);
            IWorkbook wb;
            if (extension == ".xls")
                wb = new HSSFWorkbook();
            else
                wb = new XSSFWorkbook();// XSSFWorkbook for XLSX
            ISheet sheet = wb.CreateSheet(sheetName); //创建工作表
            sheet.CreateFreezePane(0, 1); //冻结列头行
            IRow row_Title = sheet.CreateRow(0); //创建列头行
            row_Title.HeightInPoints = 30.5F; //设置列头行高
            ICellStyle cs_Title = wb.CreateCellStyle(); //创建列头样式
            cs_Title.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center; //水平居中
            cs_Title.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center; //垂直居中
            IFont cs_Title_Font = wb.CreateFont(); //创建字体
            cs_Title_Font.IsBold = true; //字体加粗
            cs_Title_Font.FontHeightInPoints = 14; //字体大小
            cs_Title.SetFont(cs_Title_Font); //将字体绑定到样式

            //获取 实体类 类型对象
            Type t = typeof(T); // model.GetType();
                                //获取 实体类 所有的 公有属性
            List<PropertyInfo> proInfos = t.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            //创建 实体属性 字典集合
            Dictionary<string, PropertyInfo> dictPros = new Dictionary<string, PropertyInfo>();
            //将 实体属性 中要修改的属性名 添加到 字典集合中 键：属性名  值：属性对象
            List<string> hiddenPropertyList = new List<string>();
            var mgKey = "";
            proInfos.ForEach(p =>
            {
                var mgKeyAttr = p.GetCustomAttribute(typeof(MergedGroupKeyAttribute), false) as MergedGroupKeyAttribute;
                if (mgKeyAttr != null)
                {
                    mgKey = p.Name;
                }
                var exportAttr = p.GetCustomAttribute(typeof(ExportFieldAttribute), false) as ExportFieldAttribute;
                if ((exportAttr != null && !exportAttr.Display))
                {
                    hiddenPropertyList.Add(p.Name);
                }
                if (FieldNames.Values.Contains(p.Name))
                {
                    dictPros.Add(p.Name, p);
                }
            });

            #region 生成列头
            int ii = 0;
            foreach (string key in FieldNames.Keys)
            {
                if (!hiddenPropertyList.Any(p => p == FieldNames[key]))
                {
                    ICell cell_Title = row_Title.CreateCell(ii); //创建单元格
                    cell_Title.CellStyle = cs_Title; //将样式绑定到单元格
                    cell_Title.SetCellValue(key);
                    sheet.SetColumnWidth(ii, 25 * 256);//设置列宽
                    ii++;
                }
            }

            #endregion

            ICellStyle cs_Content = wb.CreateCellStyle(); //创建列头样式
            cs_Content.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center; //水平居中
            cs_Content.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center; //垂直居中
            List<CellRangeAddress> cellRangeAddresses = new List<CellRangeAddress>();
            Dictionary<string, string> dict_mgKey_column = new Dictionary<string, string>();
            for (int i = 0; i < list.Count; i++)
            {
                var row = i + 1;
                IRow row_Content = sheet.CreateRow(row); //创建行
                var rowspanMax = row;
                row_Content.HeightInPoints = 20;
                int jj = -1;
                var key_cell_value = "";
                foreach (string proName in FieldNames.Values)
                {
                    if (dictPros.ContainsKey(proName))
                    {   //如果存在，则取出要属性对象
                        PropertyInfo proInfo = dictPros[proName];
                        //获取对应属性的值
                        object value = proInfo.GetValue(list[i], null); //object newValue = model.uName;
                        string cell_value = value == null ? "" : value.ToString();
                        if (!hiddenPropertyList.Any(p => p == proInfo.Name))
                        {
                            jj++;
                            ICell cell_Conent = row_Content.CreateCell(jj); //创建单元格
                            cell_Conent.CellStyle = cs_Content;
                            cell_Conent.SetCellValue(cell_value);
                        }
                        if (proName == mgKey)
                        {
                            key_cell_value = cell_value;
                            var c = list.Where(o => GetPropValue<string, T>(o, mgKey) == cell_value).Count() - 1;
                            //logger.Info($"cell_value={cell_value}，c={c}");
                            rowspanMax = c + row;
                        }
                        if ((rowspanMax - row) > 0)
                        {
                            var mgattr = proInfo.GetCustomAttribute(typeof(MergedColumnAttribute), false) as MergedColumnAttribute;
                            var mgKey_val_col = key_cell_value + "_" + jj;
                            if (!dict_mgKey_column.ContainsKey(mgKey_val_col) && mgattr != null)
                            {
                                dict_mgKey_column.Add(mgKey_val_col, "");
                                cellRangeAddresses.Add(new CellRangeAddress(row, rowspanMax, jj, jj));
                            }
                        }


                    }

                }
            }
            foreach (var item in cellRangeAddresses)
            {

                sheet.AddMergedRegion(new CellRangeAddress(item.FirstRow, item.LastRow, item.FirstColumn, item.LastColumn));
            }
            return wb;
        }

        public FileStreamResult ExportExcelFileGroupStream<T>(string fileName, List<T> list, string sheetName = "sheet1")
        {
            var keyValuePairs = NPOIHelper.GetExportField<T>();
            var workbook = ExportExcelGroupByList(fileName, list, keyValuePairs, sheetName);
            //把Excel转化为文件流，输出
            MemoryStreamHelper BookStream = new MemoryStreamHelper();//定义文件流
            workbook.Write(BookStream);//将工作薄写入文件流
            BookStream.Seek(0, SeekOrigin.Begin);//输出之前调用Seek（偏移量，游标位置）方法：获取文件流的长度
            return File(BookStream, "application/vnd.ms-excel", fileName); // 文件类型/文件名称/
        }


        public static IWorkbook ExportExcelByList<T>(string fileName, List<T> list, string sheetName = "sheet1", Dictionary<string, string> keyValuePairs = null)
        {
            string extension = System.IO.Path.GetExtension(fileName);
            IWorkbook wb;
            if (extension == ".xls")
                wb = new HSSFWorkbook();
            else
                wb = new XSSFWorkbook();// XSSFWorkbook for XLSX
            if (keyValuePairs == null)
            {
                keyValuePairs = NPOIHelper.GetExportField<T>();
            }
            int page = list.Count % 65530;
            int dataCount = list.Count;
            int pageNo;
            if (page == 0)
            {
                pageNo = dataCount / 65530;
            }
            else
            {
                pageNo = (dataCount / 65530) + 1;
            }
            sheetName = "sheet";
            for (int p = 1; p <= pageNo; p++)
            {
                ISheet sheet = wb.CreateSheet(sheetName + p); //创建工作表
                sheet.CreateFreezePane(0, 1); //冻结列头行
                IRow row_Title = sheet.CreateRow(0); //创建列头行
                row_Title.HeightInPoints = 30.5F; //设置列头行高
                ICellStyle cs_Title = wb.CreateCellStyle(); //创建列头样式
                cs_Title.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center; //水平居中
                cs_Title.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center; //垂直居中
                IFont cs_Title_Font = wb.CreateFont(); //创建字体
                cs_Title_Font.IsBold = true; //字体加粗
                cs_Title_Font.FontHeightInPoints = 14; //字体大小
                cs_Title.SetFont(cs_Title_Font); //将字体绑定到样式
                #region 生成列头
                int ii = 0;
                foreach (string key in keyValuePairs.Keys)
                {
                    ICell cell_Title = row_Title.CreateCell(ii); //创建单元格
                    cell_Title.CellStyle = cs_Title; //将样式绑定到单元格
                    cell_Title.SetCellValue(key);
                    sheet.SetColumnWidth(ii, 25 * 256);//设置列宽
                    ii++;
                }

                #endregion
                //获取 实体类 类型对象
                Type t = typeof(T); // model.GetType();
                                    //获取 实体类 所有的 公有属性
                List<PropertyInfo> proInfos = t.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
                //创建 实体属性 字典集合
                Dictionary<string, PropertyInfo> dictPros = new Dictionary<string, PropertyInfo>();
                //将 实体属性 中要修改的属性名 添加到 字典集合中 键：属性名  值：属性对象
                proInfos.ForEach(p =>
                {
                    if (keyValuePairs.Values.Contains(p.Name))
                    {
                        dictPros.Add(p.Name, p);
                    }
                });

                ICellStyle cs_Content = wb.CreateCellStyle(); //创建列头样式
                cs_Content.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center; //水平居中
                cs_Content.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center; //垂直居中 
                for (int i = 65530 * p - 65530; i < 65530 * p && i < dataCount; i++)
                {

                    IRow row_Content = sheet.CreateRow(i - (65530 * p - 65530) + 1); //创建行
                    row_Content.HeightInPoints = 20;
                    int jj = 0;
                    foreach (string proName in keyValuePairs.Values)
                    {
                        if (dictPros.ContainsKey(proName))
                        {
                            ICell cell_Conent = row_Content.CreateCell(jj); //创建单元格
                            cell_Conent.CellStyle = cs_Content;

                            //如果存在，则取出要属性对象
                            PropertyInfo proInfo = dictPros[proName];
                            //获取对应属性的值
                            object value = proInfo.GetValue(list[i], null); //object newValue = model.uName;
                            string cell_value = value == null ? "" : value.ToString();
                            cell_Conent.SetCellValue(cell_value);
                            jj++;
                        }
                    }
                }
            }
            return wb;
        }

        public FileStreamResult ExportExcelFileStream<T>(string fileName, List<T> list, string sheetName = "sheet1", Dictionary<string, string> keyValuePairs = null)
        {
            if (keyValuePairs == null)
            {
                keyValuePairs = NPOIHelper.GetExportField<T>();
            }
            var workbook = ExportExcelByList(fileName, list, sheetName, keyValuePairs);

            //把Excel转化为文件流，输出
            MemoryStreamHelper BookStream = new MemoryStreamHelper();//定义文件流
            workbook.Write(BookStream);//将工作薄写入文件流
            BookStream.Seek(0, SeekOrigin.Begin);//输出之前调用Seek（偏移量，游标位置）方法：获取文件流的长度
            return File(BookStream, "application/vnd.ms-excel", fileName); // 文件类型/文件名称/


        }

        public class MemoryStreamHelper : MemoryStream
        {
            public MemoryStreamHelper()
            {
                AllowClose = false;
            }
            public bool AllowClose { get; set; }
            public override void Close()
            {
                if (AllowClose)
                    base.Close();
            }
        }


        /// <summary>
        /// 数值类型填充为number类型，其它填充为文本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SheetName"></param>
        /// <param name="list"></param>
        /// <param name="FiedNames"></param>
        /// <returns></returns>
        public static HSSFWorkbook ExportExcelByList2<T>(string SheetName, List<T> list, Dictionary<string, string> FiedNames)
        {
            HSSFWorkbook wb = new HSSFWorkbook();
            HSSFSheet sheet = (HSSFSheet)wb.CreateSheet(SheetName); //创建工作表
            sheet.CreateFreezePane(0, 1); //冻结列头行
            HSSFRow row_Title = (HSSFRow)sheet.CreateRow(0); //创建列头行
            row_Title.HeightInPoints = 30.5F; //设置列头行高
            HSSFCellStyle cs_Title = (HSSFCellStyle)wb.CreateCellStyle(); //创建列头样式
            cs_Title.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center; //水平居中
            cs_Title.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center; //垂直居中
            HSSFFont cs_Title_Font = (HSSFFont)wb.CreateFont(); //创建字体
            cs_Title_Font.IsBold = true; //字体加粗
            cs_Title_Font.FontHeightInPoints = 14; //字体大小
            cs_Title.SetFont(cs_Title_Font); //将字体绑定到样式
            #region 生成列头
            int ii = 0;
            foreach (string key in FiedNames.Keys)
            {
                HSSFCell cell_Title = (HSSFCell)row_Title.CreateCell(ii); //创建单元格
                cell_Title.CellStyle = cs_Title; //将样式绑定到单元格
                cell_Title.SetCellValue(key);
                sheet.SetColumnWidth(ii, 25 * 256);//设置列宽
                ii++;
            }

            #endregion
            //获取 实体类 类型对象
            Type t = typeof(T); // model.GetType();
                                //获取 实体类 所有的 公有属性
            List<PropertyInfo> proInfos = t.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            //创建 实体属性 字典集合
            Dictionary<string, PropertyInfo> dictPros = new Dictionary<string, PropertyInfo>();
            //将 实体属性 中要修改的属性名 添加到 字典集合中 键：属性名  值：属性对象
            proInfos.ForEach(p =>
            {
                if (FiedNames.Values.Contains(p.Name))
                {
                    dictPros.Add(p.Name, p);
                }
            });


            for (int i = 0; i < list.Count; i++)
            {

                HSSFRow row_Content = (HSSFRow)sheet.CreateRow(i + 1); //创建行
                row_Content.HeightInPoints = 20;
                int jj = 0;
                foreach (string proName in FiedNames.Values)
                {
                    if (dictPros.ContainsKey(proName))
                    {
                        HSSFCell cell_Conent = (HSSFCell)row_Content.CreateCell(jj); //创建单元格
                        HSSFCellStyle cs_Content = (HSSFCellStyle)wb.CreateCellStyle(); //创建列头样式
                        cs_Content.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center; //水平居中
                        cs_Content.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center; //垂直居中

                        //如果存在，则取出要属性对象
                        PropertyInfo proInfo = dictPros[proName];
                        //获取对应属性的值
                        object value = proInfo.GetValue(list[i], null); //object newValue = model.uName;
                        string cell_value = value == null ? "" : value.ToString();
                        if (proInfo.PropertyType.FullName == typeof(decimal).FullName
                            || proInfo.PropertyType.FullName == typeof(double).FullName)
                        {

                            cs_Content.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                            cell_Conent.CellStyle = cs_Content;
                            cell_Conent.SetCellValue(Convert.ToDouble(value));
                        }
                        else if (proInfo.PropertyType.FullName == typeof(int).FullName || proInfo.PropertyType.FullName == typeof(long).FullName)
                        {
                            cs_Content.DataFormat = HSSFDataFormat.GetBuiltinFormat("0");
                            cell_Conent.CellStyle = cs_Content;
                            cell_Conent.SetCellValue(Convert.ToDouble(value));
                        }
                        else
                        {
                            cell_Conent.CellStyle = cs_Content;
                            cell_Conent.SetCellValue(cell_value);
                        }
                        jj++;
                    }
                }
            }
            return wb;
        }






        #endregion

        #region 多级表头导出
        public static HSSFWorkbook ExportMultipleTitleExcelByList<T>(string fileName, List<T> list, string fieldName, string headerName, string sheetName = "sheet1")
        {
            var aryFieldName = fieldName.Split(';');
            HSSFWorkbook wb = new HSSFWorkbook();
            HSSFSheet sheet = (HSSFSheet)wb.CreateSheet(sheetName); //创建工作表

            HSSFRow row_Title = (HSSFRow)sheet.CreateRow(0); //创建列头行
            row_Title.HeightInPoints = 30.5F; //设置列头行高
            HSSFCellStyle cs_Title = (HSSFCellStyle)wb.CreateCellStyle(); //创建列头样式
            cs_Title.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center; //水平居中
            cs_Title.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center; //垂直居中
            HSSFFont cs_Title_Font = (HSSFFont)wb.CreateFont(); //创建字体
            cs_Title_Font.IsBold = true; //字体加粗
            cs_Title_Font.FontHeightInPoints = 14; //字体大小
            cs_Title.SetFont(cs_Title_Font); //将字体绑定到样式
            #region 生成列头
            // 获取总行数
            var t_rows = GetRowCount(headerName);
            // 获取总列数
            var t_cols = GetColCount(headerName);

            #region 新建表，填充表头，填充列头，样式
            IRow _row;
            // 构建行
            for (int i = 0; i < t_rows; i++)
            {
                _row = sheet.GetRow(i);
                // 创建行
                if (_row == null)
                    _row = sheet.CreateRow(i);

                for (int j = 0; j < t_cols; j++)
                    _row.CreateCell(j).CellStyle = cs_Title;
            }

            // 取得上一个实体
            NPOIHeader lastRow = null;
            IList<NPOIHeader> hList = GetHeaders(headerName, t_rows, 0);
            // 创建表头
            foreach (NPOIHeader m in hList)
            {
                var data = hList.Where(c => c.firstRow == m.firstRow && c.lastCol == m.firstCol - 1);
                if (data.Count() > 0)
                {
                    lastRow = data.First();
                    if (m.headerName == lastRow.headerName)
                        m.firstCol = lastRow.firstCol;
                }

                // 获取行
                _row = sheet.GetRow(m.firstRow);
                // 合并单元格
                if (m.firstRow != m.lastRow || m.firstCol != m.lastCol)
                {
                    CellRangeAddress region = new CellRangeAddress(m.firstRow, m.lastRow, m.firstCol, m.lastCol);
                    sheet.AddMergedRegion(region);
                }
                // 填充值
                _row.CreateCell(m.firstCol).SetCellValue(m.headerName);
            }

            // 填充表头样式
            for (int i = 0; i < t_rows; i++)
            {
                _row = sheet.GetRow(i);
                for (int j = 0; j < t_cols; j++)
                {
                    _row.GetCell(j).CellStyle = cs_Title;
                    //设置列宽
                    sheet.SetColumnWidth(j, 25 * 256);
                }
            }
            sheet.CreateFreezePane(0, t_rows); //冻结列头行
            #endregion

            #endregion
            //获取 实体类 类型对象
            Type t = typeof(T); // model.GetType();
                                //获取 实体类 所有的 公有属性
            List<PropertyInfo> proInfos = t.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            //创建 实体属性 字典集合
            Dictionary<string, PropertyInfo> dictPros = new Dictionary<string, PropertyInfo>();
            //将 实体属性 中要修改的属性名 添加到 字典集合中 键：属性名  值：属性对象
            proInfos.ForEach(p =>
            {
                dictPros.Add(p.Name, p);
            });

            HSSFCellStyle cs_Content = (HSSFCellStyle)wb.CreateCellStyle(); //创建列头样式
            cs_Content.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center; //水平居中
            cs_Content.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center; //垂直居中
            var c_rows = t_rows - 1;
            for (int i = 0; i < list.Count; i++)
            {
                var childList = new List<JObject>();
                List<string> ChildPropertyList = new List<string>();
                foreach (var proInfo in dictPros.Values)
                {
                    //获取对应属性的值
                    object value = proInfo.GetValue(list[i], null); //object newValue = model.uName;
                    if (value.ToString().Contains("List`1"))
                    {
                        childList = value.ToJson().ToObject<List<JObject>>();
                        break;
                    }
                }
                int childCount = 1;
                if (childList.Count() > 0)
                {
                    childCount = childList.Count();
                    JObject workClumnExtensions = childList.FirstOrDefault();
                    foreach (JProperty jp in workClumnExtensions.Properties())
                    {
                        ChildPropertyList.Add(jp.Name);
                    }
                }
                for (int crow = 0; crow < childCount; crow++)
                {
                    c_rows++;
                    HSSFRow row_Content = (HSSFRow)sheet.CreateRow(c_rows); //创建行
                    row_Content.HeightInPoints = 20;
                    int jj = -1;
                    foreach (string proName in aryFieldName)
                    {  //获取对应属性的值
                        object value = null;
                        if (dictPros.ContainsKey(proName))
                        {
                            //如果存在，则取出要属性对象
                            PropertyInfo proInfo = dictPros[proName];
                            value = proInfo.GetValue(list[i], null);
                        }
                        else if (ChildPropertyList.Contains(proName))
                        {
                            value = childList[crow].GetValue(proName).ToString();
                        }
                        string cell_value = value == null ? "" : value.ToString();

                        jj++;
                        HSSFCell cell_Conent = (HSSFCell)row_Content.CreateCell(jj); //创建单元格
                        cell_Conent.CellStyle = cs_Content;
                        cell_Conent.SetCellValue(cell_value);
                        if (crow == childCount - 1 && dictPros.ContainsKey(proName) & childCount > 1)
                        {
                            sheet.AddMergedRegion(new CellRangeAddress(c_rows - (childCount - 1), c_rows, jj, jj));
                        }


                    }
                }
            }
            return wb;
        }

        public FileStreamResult ExportMultipleTitleExcelStream<T>(string fileName, List<T> list, string fieldName, string headerName, string sheetName = "sheet1")
        {
            var workbook = ExportMultipleTitleExcelByList(fileName, list, fieldName, headerName, sheetName);
            //把Excel转化为文件流，输出
            MemoryStream BookStream = new MemoryStream();//定义文件流
            workbook.Write(BookStream);//将工作薄写入文件流
            BookStream.Seek(0, SeekOrigin.Begin);//输出之前调用Seek（偏移量，游标位置）方法：获取文件流的长度
            return File(BookStream, "application/vnd.ms-excel", fileName); // 文件类型/文件名称/
        }


        #region 辅助方法
        /// <summary>
        /// 表头解析
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="header">表头</param>
        /// <param name="rows">总行数</param>
        /// <param name="addRows">外加行</param>
        /// <param name="addCols">外加列</param>
        /// <returns></returns>
        private static IList<NPOIHeader> GetHeaders(string header, int rows, int addRows)
        {
            // 临时表头数组
            string[] tempHeader;
            string[] tempHeader2;
            // 所跨列数
            int colSpan = 0;
            // 所跨行数
            int rowSpan = 0;
            // 单元格对象
            NPOIHeader model = null;
            // 行数计数器
            int rowIndex = 0;
            // 列数计数器
            int colIndex = 0;
            // 
            IList<NPOIHeader> list = new List<NPOIHeader>();
            // 初步解析
            string[] headers = header.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
            // 表头遍历
            for (int i = 0; i < headers.Length; i++)
            {
                // 行数计数器清零
                rowIndex = 0;
                // 列数计数器清零
                colIndex = 0;
                // 获取所跨行数
                rowSpan = GetRowSpan(headers[i], rows);
                // 获取所跨列数
                colSpan = GetColSpan(headers[i]);

                // 如果所跨行数与总行数相等，则不考虑是否合并单元格问题
                if (rows == rowSpan)
                {
                    colIndex = GetMaxCol(list);
                    model = new NPOIHeader(headers[i],
                        addRows,
                        (rowSpan - 1 + addRows),
                        colIndex,
                        (colSpan - 1 + colIndex),
                        addRows);
                    list.Add(model);
                    rowIndex += (rowSpan - 1) + addRows;
                }
                else
                {
                    // 列索引
                    colIndex = GetMaxCol(list);
                    // 如果所跨行数不相等，则考虑是否包含多行
                    tempHeader = headers[i].Split(new string[] { ">" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < tempHeader.Length; j++)
                    {

                        // 如果总行数=数组长度
                        if (1 == GetColSpan(tempHeader[j]))
                        {
                            if (j == tempHeader.Length - 1 && tempHeader.Length < rows)
                            {
                                model = new NPOIHeader(tempHeader[j],
                                    (j + addRows),
                                    (j + addRows) + (rows - tempHeader.Length),
                                    colIndex,
                                    (colIndex + colSpan - 1),
                                    addRows);
                                list.Add(model);
                            }
                            else
                            {
                                model = new NPOIHeader(tempHeader[j],
                                        (j + addRows),
                                        (j + addRows),
                                        colIndex,
                                        (colIndex + colSpan - 1),
                                        addRows);
                                list.Add(model);
                            }
                        }
                        else
                        {
                            // 如果所跨列数不相等，则考虑是否包含多列
                            tempHeader2 = tempHeader[j].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            for (int m = 0; m < tempHeader2.Length; m++)
                            {
                                // 列索引
                                colIndex = GetMaxCol(list) - colSpan + m;
                                if (j == tempHeader.Length - 1 && tempHeader.Length < rows)
                                {
                                    model = new NPOIHeader(tempHeader2[m],
                                        (j + addRows),
                                        (j + addRows) + (rows - tempHeader.Length),
                                        colIndex,
                                        colIndex,
                                        addRows);
                                    list.Add(model);
                                }
                                else
                                {
                                    model = new NPOIHeader(tempHeader2[m],
                                            (j + addRows),
                                            (j + addRows),
                                            colIndex,
                                            colIndex,
                                            addRows);
                                    list.Add(model);
                                }
                            }
                        }
                        rowIndex += j + addRows;
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 获取最大列
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private static int GetMaxCol(IList<NPOIHeader> list)
        {
            int maxCol = 0;
            if (list.Count > 0)
            {
                foreach (NPOIHeader model in list)
                {
                    if (maxCol < model.lastCol)
                        maxCol = model.lastCol;
                }
                maxCol += 1;
            }

            return maxCol;
        }
        /// <summary>
        /// 获取表头行数
        /// </summary>
        /// <param name="newHeaders">表头文字</param>
        /// <returns></returns>
        private static int GetRowCount(string newHeaders)
        {
            string[] ColumnNames = newHeaders.Split(new char[] { '@' });
            int Count = 0;
            if (ColumnNames.Length <= 1)
                ColumnNames = newHeaders.Split(new char[] { '#' });
            foreach (string name in ColumnNames)
            {
                int TempCount = name.Split(new char[] { '>' }).Length;
                if (TempCount > Count)
                    Count = TempCount;
            }
            return Count;
        }

        /// <summary>
        /// 获取表头列数
        /// </summary>
        /// <param name="newHeaders">表头文字</param>
        /// <returns></returns>
        private static int GetColCount(string newHeaders)
        {
            string[] ColumnNames = newHeaders.Split(new char[] { '@' });
            int Count = 0;
            if (ColumnNames.Length <= 1)
                ColumnNames = newHeaders.Split(new char[] { '#' });
            Count = ColumnNames.Length;
            foreach (string name in ColumnNames)
            {
                int TempCount = name.Split(new char[] { ',' }).Length;
                if (TempCount > 1)
                    Count += TempCount - 1;
            }
            return Count;
        }

        /// <summary>
        /// 列头跨列数
        /// </summary>
        /// <param name="newHeaders">表头文字</param>
        /// <returns></returns>
        private static int GetColSpan(string newHeaders)
        {
            return newHeaders.Split(',').Count();
        }

        /// <summary>
        /// 列头跨行数
        /// </summary> 
        /// <param name="newHeaders">列头文本</param>
        /// <param name="rows">表头总行数</param>
        /// <returns></returns>
        private static int GetRowSpan(string newHeaders, int rows)
        {
            int Count = newHeaders.Split(new string[] { ">" }, StringSplitOptions.RemoveEmptyEntries).Length;
            // 如果总行数与当前表头所拥有行数相等
            if (rows == Count)
                Count = 1;
            else if (Count < rows)
                Count = 1 + (rows - Count);
            else
                throw new Exception("表头格式不正确！");
            return Count;
        }
        #endregion

        /// <summary>
        /// 表头构建类
        /// </summary>
        public class NPOIHeader
        {
            /// <summary>
            /// 表头
            /// </summary>
            public string headerName { get; set; }
            /// <summary>
            /// 起始行
            /// </summary>
            public int firstRow { get; set; }
            /// <summary>
            /// 结束行
            /// </summary>
            public int lastRow { get; set; }
            /// <summary>
            /// 起始列
            /// </summary>
            public int firstCol { get; set; }
            /// <summary>
            /// 结束列
            /// </summary>
            public int lastCol { get; set; }
            /// <summary>
            /// 是否跨行
            /// </summary>
            public int isRowSpan { get; private set; }
            /// <summary>
            /// 是否跨列
            /// </summary>
            public int isColSpan { get; private set; }
            /// <summary>
            /// 外加行
            /// </summary>
            public int rows { get; set; }

            public NPOIHeader() { }
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="headerName">表头</param>
            /// <param name="firstRow">起始行</param>
            /// <param name="lastRow">结束行</param>
            /// <param name="firstCol">起始列</param>
            /// <param name="lastCol">结束列</param>
            /// <param name="rows">外加行</param>
            /// <param name="cols">外加列</param>
            public NPOIHeader(string headerName, int firstRow, int lastRow, int firstCol, int lastCol, int rows = 0)
            {
                this.headerName = headerName;
                this.firstRow = firstRow;
                this.lastRow = lastRow;
                this.firstCol = firstCol;
                this.lastCol = lastCol;
                // 是否跨行判断
                if (firstRow != lastRow)
                    isRowSpan = 1;
                if (firstCol != lastCol)
                    isColSpan = 1;

                this.rows = rows;
            }
        }
        #endregion

        public static void InsertSheet(string outputFile, string sheetname, DataTable dt)
        {
            FileStream readfile = new FileStream(outputFile, FileMode.Open, FileAccess.Read);
            IWorkbook hssfworkbook = WorkbookFactory.Create(readfile);
            //HSSFWorkbook hssfworkbook = new HSSFWorkbook(readfile);
            int num = hssfworkbook.GetSheetIndex(sheetname);
            ISheet sheet1;
            if (num >= 0)
                sheet1 = hssfworkbook.GetSheet(sheetname);
            else
            {
                sheet1 = hssfworkbook.CreateSheet(sheetname);
            }



            if (sheet1.GetRow(0) == null)
            {
                sheet1.CreateRow(0);
            }
            for (int coluid = 0; coluid < dt.Columns.Count; coluid++)
            {
                if (sheet1.GetRow(0).GetCell(coluid) == null)
                {
                    sheet1.GetRow(0).CreateCell(coluid);
                }

                sheet1.GetRow(0).GetCell(coluid).SetCellValue(dt.Columns[coluid].ColumnName);
            }



            for (int i = 1; i <= dt.Rows.Count; i++)
            {

                if (sheet1.GetRow(i) == null)
                {
                    sheet1.CreateRow(i);
                }
                for (int coluid = 0; coluid < dt.Columns.Count; coluid++)
                {
                    if (sheet1.GetRow(i).GetCell(coluid) == null)
                    {
                        sheet1.GetRow(i).CreateCell(coluid);
                    }

                    sheet1.GetRow(i).GetCell(coluid).SetCellValue(dt.Rows[i - 1][coluid].ToString());
                }

            }

            readfile.Close();

            FileStream writefile = new FileStream(outputFile, FileMode.OpenOrCreate, FileAccess.Write);
            hssfworkbook.Write(writefile);
            writefile.Close();

        }

        #region 更新excel中的数据
        /// <summary>
        /// 更新Excel表格
        /// </summary>
        /// <param name="outputFile">需更新的excel表格路径</param>
        /// <param name="sheetname">sheet名</param>
        /// <param name="updateData">需更新的数据</param>
        /// <param name="coluid">需更新的列号</param>
        /// <param name="rowid">需更新的开始行号</param>
        public static void UpdateExcel(string outputFile, string sheetname, string[] updateData, int coluid, int rowid)
        {
            //FileStream readfile = new FileStream(outputFile, FileMode.Open, FileAccess.Read);
            IWorkbook hssfworkbook = null;// WorkbookFactory.Create(outputFile);
                                          //HSSFWorkbook hssfworkbook = new HSSFWorkbook(readfile);
            ISheet sheet1 = hssfworkbook.GetSheet(sheetname);
            for (int i = 0; i < updateData.Length; i++)
            {

                if (sheet1.GetRow(i + rowid) == null)
                {
                    sheet1.CreateRow(i + rowid);
                }
                if (sheet1.GetRow(i + rowid).GetCell(coluid) == null)
                {
                    sheet1.GetRow(i + rowid).CreateCell(coluid);
                }

                sheet1.GetRow(i + rowid).GetCell(coluid).SetCellValue(updateData[i]);

            }

            //readfile.Close();
            FileStream writefile = new FileStream(outputFile, FileMode.OpenOrCreate, FileAccess.Write);
            hssfworkbook.Write(writefile);
            writefile.Close();


        }

        /// <summary>
        /// 更新Excel表格
        /// </summary>
        /// <param name="outputFile">需更新的excel表格路径</param>
        /// <param name="sheetname">sheet名</param>
        /// <param name="updateData">需更新的数据</param>
        /// <param name="coluids">需更新的列号</param>
        /// <param name="rowid">需更新的开始行号</param>
        public static void UpdateExcel(string outputFile, string sheetname, string[][] updateData, int[] coluids, int rowid)
        {
            FileStream readfile = new FileStream(outputFile, FileMode.Open, FileAccess.Read);

            HSSFWorkbook hssfworkbook = new HSSFWorkbook(readfile);
            readfile.Close();
            ISheet sheet1 = hssfworkbook.GetSheet(sheetname);
            for (int j = 0; j < coluids.Length; j++)
            {
                for (int i = 0; i < updateData[j].Length; i++)
                {

                    if (sheet1.GetRow(i + rowid) == null)
                    {
                        sheet1.CreateRow(i + rowid);
                    }
                    if (sheet1.GetRow(i + rowid).GetCell(coluids[j]) == null)
                    {
                        sheet1.GetRow(i + rowid).CreateCell(coluids[j]);
                    }
                    sheet1.GetRow(i + rowid).GetCell(coluids[j]).SetCellValue(updateData[j][i]);

                }
            }

            FileStream writefile = new FileStream(outputFile, FileMode.Create);
            hssfworkbook.Write(writefile);
            writefile.Close();

        }

        /// <summary>
        /// 更新Excel表格
        /// </summary>
        /// <param name="outputFile">需更新的excel表格路径</param>
        /// <param name="sheetname">sheet名</param>
        /// <param name="updateData">需更新的数据</param>
        /// <param name="coluid">需更新的列号</param>
        /// <param name="rowid">需更新的开始行号</param>
        public static void UpdateExcel(string outputFile, string sheetname, double[] updateData, int coluid, int rowid)
        {
            FileStream readfile = new FileStream(outputFile, FileMode.Open, FileAccess.Read);

            HSSFWorkbook hssfworkbook = new HSSFWorkbook(readfile);
            ISheet sheet1 = hssfworkbook.GetSheet(sheetname);
            for (int i = 0; i < updateData.Length; i++)
            {

                if (sheet1.GetRow(i + rowid) == null)
                {
                    sheet1.CreateRow(i + rowid);
                }
                if (sheet1.GetRow(i + rowid).GetCell(coluid) == null)
                {
                    sheet1.GetRow(i + rowid).CreateCell(coluid);
                }

                sheet1.GetRow(i + rowid).GetCell(coluid).SetCellValue(updateData[i]);

            }

            readfile.Close();
            FileStream writefile = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
            hssfworkbook.Write(writefile);
            writefile.Close();


        }

        /// <summary>
        /// 更新Excel表格
        /// </summary>
        /// <param name="outputFile">需更新的excel表格路径</param>
        /// <param name="sheetname">sheet名</param>
        /// <param name="updateData">需更新的数据</param>
        /// <param name="coluids">需更新的列号</param>
        /// <param name="rowid">需更新的开始行号</param>
        public static void UpdateExcel(string outputFile, string sheetname, double[][] updateData, int[] coluids, int rowid)
        {
            FileStream readfile = new FileStream(outputFile, FileMode.Open, FileAccess.Read);

            HSSFWorkbook hssfworkbook = new HSSFWorkbook(readfile);
            readfile.Close();
            ISheet sheet1 = hssfworkbook.GetSheet(sheetname);
            for (int j = 0; j < coluids.Length; j++)
            {
                for (int i = 0; i < updateData[j].Length; i++)
                {

                    if (sheet1.GetRow(i + rowid) == null)
                    {
                        sheet1.CreateRow(i + rowid);
                    }
                    if (sheet1.GetRow(i + rowid).GetCell(coluids[j]) == null)
                    {
                        sheet1.GetRow(i + rowid).CreateCell(coluids[j]);
                    }
                    sheet1.GetRow(i + rowid).GetCell(coluids[j]).SetCellValue(updateData[j][i]);

                }
            }

            FileStream writefile = new FileStream(outputFile, FileMode.Create);
            hssfworkbook.Write(writefile);
            writefile.Close();

        }

        #endregion

        #region 读取Excel到DataTable
        public static DataTable GetData(string excelFilePath)
        {
            string extension = System.IO.Path.GetExtension(excelFilePath);
            IWorkbook workbook;
            using (var stream = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read))
            {
                if (extension == ".xls")
                    workbook = new HSSFWorkbook(stream);
                else
                    workbook = new XSSFWorkbook(stream);// XSSFWorkbook for XLSX
            }

            var sheet = workbook.GetSheetAt(0); // zero-based index of your target sheet
            var dataTable = new DataTable(sheet.SheetName);

            // write the header row
            var headerRow = sheet.GetRow(0);
            foreach (var headerCell in headerRow)
            {
                dataTable.Columns.Add(headerCell.ToString());
            }

            // write the rest
            for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
            {
                var sheetRow = sheet.GetRow(i);
                var dtRow = dataTable.NewRow();
                dtRow.ItemArray = dataTable.Columns
                    .Cast<DataColumn>()
                    .Select(c => sheetRow.GetCell(c.Ordinal, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString())
                    .ToArray();
                dataTable.Rows.Add(dtRow);
            }
            return dataTable;
        }
        public static DataTable GetData(string excelFilePath, string sheetName)
        {
            string extension = System.IO.Path.GetExtension(excelFilePath);
            IWorkbook workbook;
            using (var stream = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read))
            {
                if (extension == ".xls")
                    workbook = new HSSFWorkbook(stream);
                else
                    workbook = new XSSFWorkbook(stream);// XSSFWorkbook for XLSX
            }

            var sheet = workbook.GetSheetAt(0); // zero-based index of your target sheet
            if (!string.IsNullOrWhiteSpace(sheetName))
            {
                sheet = workbook.GetSheet(sheetName);
            }
            var dataTable = new DataTable(sheet.SheetName);

            // write the header row
            var headerRow = sheet.GetRow(0);
            foreach (var headerCell in headerRow)
            {
                dataTable.Columns.Add(headerCell.ToString());
            }

            // write the rest
            for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
            {
                var sheetRow = sheet.GetRow(i);
                var dtRow = dataTable.NewRow();
                dtRow.ItemArray = dataTable.Columns
                    .Cast<DataColumn>()
                    .Select(c => sheetRow.GetCell(c.Ordinal, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString())
                    .ToArray();
                dataTable.Rows.Add(dtRow);
            }
            return dataTable;
        }
        public static DataTable GetDataTable_stream(string fileName, Stream stream, int start_rows = 0)
        {
            IWorkbook workbook;
            if (fileName.Contains(".xls"))
                workbook = new HSSFWorkbook(stream);
            else
                workbook = new XSSFWorkbook(stream);// XSSFWorkbook for XLSX


            var sheet = workbook.GetSheetAt(0); // zero-based index of your target sheet
            var dataTable = new DataTable(sheet.SheetName);

            // write the header row
            var headerRow = sheet.GetRow(start_rows);
            foreach (var headerCell in headerRow)
            {
                dataTable.Columns.Add(headerCell.ToString());
            }

            // write the rest
            for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
            {
                var sheetRow = sheet.GetRow(i);
                var dtRow = dataTable.NewRow();
                dtRow.ItemArray = dataTable.Columns
                    .Cast<DataColumn>()
                    .Select(c => sheetRow.GetCell(c.Ordinal, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString())
                    .ToArray();
                dataTable.Rows.Add(dtRow);
            }
            return dataTable;
        }

        #endregion

        public static bool isNumeric(String message, out double result)
        {
            Regex rex = new Regex(@"^[-]?\d+[.]?\d*$");
            result = -1;
            if (rex.IsMatch(message))
            {
                result = double.Parse(message);
                return true;
            }
            else
                return false;

        }

        /// <summary>
        /// 由DataSet导出Excel
        /// </summary>
        /// <param name="sourceTable">要导出数据的DataTable</param>
        /// <param name="sheetName">工作表名称</param>
        /// <returns>Excel工作表</returns>
        public static MemoryStream ExportDataSetToExcel(DataSet sourceDs, string sheetName)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            MemoryStream ms = new MemoryStream();
            string[] sheetNames = sheetName.Split(',');
            for (int i = 0; i < sheetNames.Length; i++)
            {
                ISheet sheet = workbook.CreateSheet(sheetNames[i]);

                #region 列头
                IRow headerRow = sheet.CreateRow(0);
                HSSFCellStyle headStyle = workbook.CreateCellStyle() as HSSFCellStyle;
                HSSFFont font = workbook.CreateFont() as HSSFFont;
                font.FontHeightInPoints = 10;
                //font.Boldweight = 700;
                headStyle.SetFont(font);

                //取得列宽
                int[] arrColWidth = new int[sourceDs.Tables[i].Columns.Count];
                foreach (DataColumn item in sourceDs.Tables[i].Columns)
                {
                    arrColWidth[item.Ordinal] = Encoding.UTF8.GetBytes(item.ColumnName.ToString()).Length;
                }

                // 处理列头
                foreach (DataColumn column in sourceDs.Tables[i].Columns)
                {
                    headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                    headerRow.GetCell(column.Ordinal).CellStyle = headStyle;
                    //设置列宽
                    sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);

                }
                #endregion

                #region 填充值
                int rowIndex = 1;
                foreach (DataRow row in sourceDs.Tables[i].Rows)
                {
                    IRow dataRow = sheet.CreateRow(rowIndex);
                    foreach (DataColumn column in sourceDs.Tables[i].Columns)
                    {
                        dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                    }
                    rowIndex++;
                }
                #endregion
            }
            workbook.Write(ms);
            ms.Flush();
            ms.Position = 0;
            workbook = null;
            return ms;
        }
        /// <summary>
        /// 导出到本地
        /// </summary>
        /// <param name="sourceDs"></param>
        /// <param name="sheetName"></param>
        /// <param name="path"></param>
        public static void ExportDataSetToExcel2(DataSet sourceDs, string sheetName, string path)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            string[] sheetNames = sheetName.Split(',');
            for (int i = 0; i < sheetNames.Length; i++)
            {
                ISheet sheet = workbook.CreateSheet(sheetNames[i]);

                #region 列头
                IRow headerRow = sheet.CreateRow(0);
                HSSFCellStyle headStyle = workbook.CreateCellStyle() as HSSFCellStyle;
                HSSFFont font = workbook.CreateFont() as HSSFFont;
                font.FontHeightInPoints = 10;
                //font.Boldweight = 700;
                headStyle.SetFont(font);

                //取得列宽
                int[] arrColWidth = new int[sourceDs.Tables[i].Columns.Count];
                foreach (DataColumn item in sourceDs.Tables[i].Columns)
                {
                    arrColWidth[item.Ordinal] = Encoding.UTF8.GetBytes(item.ColumnName.ToString()).Length;
                }

                // 处理列头
                foreach (DataColumn column in sourceDs.Tables[i].Columns)
                {
                    headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                    headerRow.GetCell(column.Ordinal).CellStyle = headStyle;
                    //设置列宽
                    sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);

                }
                #endregion

                #region 填充值
                int rowIndex = 1;
                foreach (DataRow row in sourceDs.Tables[i].Rows)
                {
                    IRow dataRow = sheet.CreateRow(rowIndex);
                    foreach (DataColumn column in sourceDs.Tables[i].Columns)
                    {
                        dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                    }
                    rowIndex++;
                }
                #endregion
            }
            FileStream file = new FileStream(path, FileMode.Create);
            workbook.Write(file);
            file.Close();
        }

        /// <summary>
        /// 验证导入的Excel是否有数据
        /// </summary>
        /// <param name="excelFileStream"></param>
        /// <returns></returns>
        public static bool HasData(Stream excelFileStream)
        {
            using (excelFileStream)
            {
                IWorkbook workBook = new HSSFWorkbook(excelFileStream);
                if (workBook.NumberOfSheets > 0)
                {
                    ISheet sheet = workBook.GetSheetAt(0);
                    return sheet.PhysicalNumberOfRows > 0;
                }
            }
            return false;
        }

        public static Dictionary<string, string> GetExportField<T>()
        {
            Type type = typeof(T);
            var fieldInfos = type.GetProperties();
            Dictionary<string, string> valuePairs = new Dictionary<string, string>();
            foreach (var item in fieldInfos)
            {
                var typeName = item.PropertyType.ToString();
                if (typeName.Contains("List`1"))
                {
                    var classFullName = typeName.Replace("System.Collections.Generic.List`1[", "").Replace("]", "");
                    var childType = getTypeByName(classFullName);
                    var childField = GetExportField(childType);
                    foreach (var k in childField.Keys)
                    {
                        if (!valuePairs.ContainsKey(k)) valuePairs.Add(k, childField[k]);
                    }
                }
                else
                {
                    var dictField = GetExportField(item);
                    foreach (var f in dictField)
                    {
                        valuePairs.Add(f.Key, f.Value);
                    }
                }
            }
            return valuePairs;
        }

        public static Dictionary<string, string> GetExportField2<T>()
        {
            Type type = typeof(T);
            var fieldInfos = type.GetProperties();
            Dictionary<string, string> valuePairs = new Dictionary<string, string>();
            foreach (var item in fieldInfos)
            {
                var typeName = item.PropertyType.ToString();
                if (typeName.Contains("List`1"))
                {
                    var classFullName = typeName.Replace("System.Collections.Generic.List`1[", "").Replace("]", "");
                    var childType = getTypeByName(classFullName);
                    var childField = GetExportField(childType);
                    foreach (var k in childField.Keys)
                    {
                        if (!valuePairs.ContainsKey(k)) valuePairs.Add(k, childField[k]);
                    }
                }

                var dictField = GetExportField(item);
                foreach (var f in dictField)
                {
                    valuePairs.Add(f.Key, f.Value);
                }

            }
            return valuePairs;
        }

        public static Dictionary<string, string> GetExportField(Type type)
        {
            var fieldInfos = type.GetProperties();
            Dictionary<string, string> valuePairs = new Dictionary<string, string>();
            foreach (var item in fieldInfos)
            {
                var dictField = GetExportField(item);
                foreach (var f in dictField)
                {
                    valuePairs.Add(f.Key, f.Value);
                }

            }
            return valuePairs;
        }

        public static Dictionary<string, string> GetExportField(PropertyInfo propertyInfo)
        {
            Dictionary<string, string> valuePairs = new Dictionary<string, string>();
            var exportFileAttr = propertyInfo.GetCustomAttribute(typeof(ExportFieldAttribute), false) as ExportFieldAttribute;
            var exportFile2Attr = propertyInfo.GetCustomAttribute(typeof(MergedGroupKeyAttribute), false) as MergedGroupKeyAttribute;
            if ((exportFileAttr != null && !string.IsNullOrWhiteSpace(exportFileAttr.Name)) || exportFile2Attr != null)
            {
                valuePairs.Add(exportFileAttr.Name, propertyInfo.Name);
            }
            return valuePairs;
        }

        /// <summary>
        /// Gets a all Type instances matching the specified class name with just non-namespace qualified class name.
        /// </summary>
        /// <param name="className">Name of the class sought.</param>
        /// <returns>Types that have the class name specified. They may not be in the same namespace.</returns>
        public static Type getTypeByName(string className)
        {
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] assemblyTypes = a.GetTypes();
                for (int j = 0; j < assemblyTypes.Length; j++)
                {
                    if (assemblyTypes[j].FullName == className)
                    {
                        return assemblyTypes[j];
                    }
                }
            }
            return null;
        }
        public static R GetPropValue<R, T>(T t, string expName)
        {
            Type tp = typeof(T);
            PropertyInfo pInfo = tp.GetProperty(expName, BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance);
            return (R)pInfo.GetValue(t, null);
        }

        /// <summary>
        /// 导出excel模板
        /// </summary>
        /// <param name="columnHeaders">列头模板</param>
        /// <returns></returns>
        public FileStreamResult ExportExcelTemplate(List<string> columnHeaders, string fileName, string sheetName = "Sheet1", string contentType = "application/vnd.ms-excel")
        {
            if (columnHeaders.Count > 0)
            {
                string extension = System.IO.Path.GetExtension(fileName);
                IWorkbook wb;
                if (extension == ".xls")
                    wb = new HSSFWorkbook();
                else
                    wb = new XSSFWorkbook();// XSSFWorkbook for XLSX  
                ISheet sheet = wb.CreateSheet(sheetName); //创建工作表
                sheet.CreateFreezePane(0, 1); //冻结列头行
                IRow row_Title = sheet.CreateRow(0); //创建列头行
                row_Title.HeightInPoints = 30.5F; //设置列头行高
                ICellStyle cs_Title = wb.CreateCellStyle(); //创建列头样式
                cs_Title.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center; //水平居中
                cs_Title.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center; //垂直居中
                IFont cs_Title_Font = wb.CreateFont(); //创建字体
                cs_Title_Font.IsBold = true; //字体加粗
                cs_Title_Font.FontHeightInPoints = 14; //字体大小
                cs_Title.SetFont(cs_Title_Font); //将字体绑定到样式
                #region 生成列头
                int ii = 0;
                foreach (string key in columnHeaders)
                {
                    ICell cell_Title = row_Title.CreateCell(ii); //创建单元格
                    cell_Title.CellStyle = cs_Title; //将样式绑定到单元格
                    cell_Title.SetCellValue(key);
                    sheet.SetColumnWidth(ii, 25 * 256);//设置列宽
                    ii++;
                }

                #endregion
                //获取 实体类 类型对象
                Type t = typeof(T); // model.GetType();
                                    //获取 实体类 所有的 公有属性 
                ICellStyle cs_Content = wb.CreateCellStyle(); //创建列头样式
                cs_Content.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center; //水平居中
                cs_Content.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center; //垂直居中  
                                                                                           //把Excel转化为文件流，输出
                MemoryStreamHelper BookStream = new MemoryStreamHelper();//定义文件流
                wb.Write(BookStream);//将工作薄写入文件流
                BookStream.Seek(0, SeekOrigin.Begin);//输出之前调用Seek（偏移量，游标位置）方法：获取文件流的长度
                return File(BookStream, contentType, fileName); // 文件类型/文件名称/ 
            }
            return default;
        }
    }





    public class ExportFieldAttribute : Attribute
    {
        private string _Name;
        /// <summary>
        /// 导出Excel的列名
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public bool Display { get; set; } = true;
    }

    public class MergedGroupKeyAttribute : Attribute
    {

    }

    /// <summary>
    /// 合并列属性
    /// </summary>
    public class MergedColumnAttribute : Attribute
    {

    }
}
