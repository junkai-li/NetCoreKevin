using Common.AliYun;
using NPOI.HSSF.UserModel;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Common
{
    public static class DataHelper
    {


        /// <summary>
        /// 将datatable 转换成 实体List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static IList<T> DataTableToList<T>(DataTable table) where T : class
        {
            ////去除空行
            //List<DataRow> removelist = new List<DataRow>();
            //for (int i = 0; i < table.Rows.Count; i++)
            //{
            //    bool IsNull = true;
            //    for (int j = 0; j < table.Columns.Count; j++)
            //    {
            //        if (!string.IsNullOrEmpty(table.Rows[i][j].ToString().Trim()))
            //        {
            //            IsNull = false;
            //        }
            //    }
            //    if (IsNull)
            //    {
            //        removelist.Add(table.Rows[i]);
            //    }
            //}
            //for (int i = 0; i < removelist.Count; i++)
            //{
            //    table.Rows.Remove(removelist[i]);
            //}

            if (table.Rows.Count == 0)
            {
                return new List<T>();
            }
            else
            {

                IList<T> list = new List<T>();
                T model = default;
                foreach (DataRow dr in table.Rows)
                {
                    model = Activator.CreateInstance<T>();

                    foreach (DataColumn dc in dr.Table.Columns)
                    {
                        object drValue = dr[dc.ColumnName];

                        PropertyInfo pi = model.GetType().GetProperty(dc.ColumnName.Replace("(必填)", "").Replace("（必填）", ""));

                        if (pi != null && pi.CanWrite && (drValue != null && !Convert.IsDBNull(drValue)))
                        {
                            string piFullName = pi.PropertyType.FullName;

                            if (piFullName.Contains("System.DateTime"))
                            {
                                if (pi.PropertyType.FullName.StartsWith("System.Nullable`1[[System.DateTime"))
                                {
                                    if (!string.IsNullOrEmpty(drValue.ToString()))
                                    {
                                        pi.SetValue(model, Convert.ToDateTime(drValue), null);
                                    }
                                    else
                                    {
                                        pi.SetValue(model, null, null);
                                    }

                                }
                                else
                                {
                                    pi.SetValue(model, Convert.ToDateTime(drValue), null);
                                }
                            }
                            else if (piFullName.Contains("System.Boolean"))
                            {
                                pi.SetValue(model, Convert.ToBoolean(drValue), null);
                            }
                            else if (piFullName.Contains("System.Decimal"))
                            {
                                pi.SetValue(model, Convert.ToDecimal(string.IsNullOrEmpty(drValue.ToString()) ? "0.0" : System.Text.RegularExpressions.Regex.Replace(drValue.ToString(), @"[^0-9,.]+", "")), null);
                            }
                            else if (piFullName.Contains("System.Int32"))
                            {
                                pi.SetValue(model, Convert.ToInt32(string.IsNullOrEmpty(drValue.ToString()) ? "0" : System.Text.RegularExpressions.Regex.Replace(drValue.ToString(), @"[^0-9]+", "")), null);
                            }
                            else
                            {
                                pi.SetValue(model, drValue, null);
                            }

                        }
                    }

                    list.Add(model);
                }
                return list;
            }

        }


        /// <summary>
        /// 将datatable 转换成 实体List(加入实体DisplayName表头匹配 )
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static IList<T> DataTableToListDisplayName<T>(DataTable table) where T : class
        {
            if (table.Rows.Count == 0)
            {
                return new List<T>();
            }
            else
            {

                IList<T> list = new List<T>();
                T model = default;
                foreach (DataRow dr in table.Rows)
                {
                    model = Activator.CreateInstance<T>();

                    foreach (DataColumn dc in dr.Table.Columns)
                    {
                        object drValue = dr[dc.ColumnName];

                        var properties = model.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();

                        var displayNamePI = properties.Where(p => p.CustomAttributes.Where(t => t.AttributeType.Name == "DisplayNameAttribute").Select(t => t.ConstructorArguments.Select(v => v.Value.ToString()).FirstOrDefault()).FirstOrDefault() == "用户名").FirstOrDefault();

                        PropertyInfo pi = model.GetType().GetProperty(dc.ColumnName) ?? displayNamePI;

                        if (pi != null && pi.CanWrite && (drValue != null && !Convert.IsDBNull(drValue)))
                        {
                            string piFullName = pi.PropertyType.FullName;

                            if (piFullName.Contains("System.DateTime"))
                            {
                                if (pi.PropertyType.FullName.StartsWith("System.Nullable`1[[System.DateTime"))
                                {
                                    pi.SetValue(model, null, null);
                                }
                                else
                                {
                                    pi.SetValue(model, Convert.ToDateTime(drValue), null);
                                }
                            }
                            else if (piFullName.Contains("System.Boolean"))
                            {
                                pi.SetValue(model, Convert.ToBoolean(drValue), null);
                            }
                            else
                            {
                                pi.SetValue(model, drValue, null);
                            }

                        }
                    }

                    list.Add(model);
                }
                return list;
            }

        }



        /// <summary>
        /// 实体List 转 datatable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ListToDataTable<T>(IList<T> list)
            where T : class
        {
            if (list == null || list.Count <= 0)
            {
                return null;
            }
            DataTable dt = new(typeof(T).Name);
            DataColumn column;
            DataRow row;

            PropertyInfo[] myPropertyInfo = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            int length = myPropertyInfo.Length;
            bool createColumn = true;

            foreach (T t in list)
            {
                if (t == null)
                {
                    continue;
                }

                row = dt.NewRow();
                for (int i = 0; i < length; i++)
                {
                    PropertyInfo pi = myPropertyInfo[i];
                    string name = pi.Name;
                    if (createColumn)
                    {
                        column = new DataColumn(name, pi.PropertyType);
                        dt.Columns.Add(column);
                    }

                    row[name] = pi.GetValue(t, null);
                }

                if (createColumn)
                {
                    createColumn = false;
                }

                dt.Rows.Add(row);
            }
            return dt;

        }



        /// <summary>  
        /// 将excel导入到datatable  
        /// </summary>  
        /// <param name="filePath">excel路径</param>  
        /// <param name="isColumnName">第一行是否是列名</param>  
        /// <returns>返回datatable</returns>  
        public static DataTable ExcelToDataTable(string filePath, bool isColumnName)
        {
            DataTable dataTable = null;
            FileStream fs = null;
            DataColumn column = null;
            DataRow dataRow = null;
            IWorkbook workbook = null;
            ISheet sheet = null;
            IRow row = null;
            ICell cell = null;
            int startRow = 0;
            try
            {
                using (fs = System.IO.File.OpenRead(filePath))
                {
                    // 2007版本  
                    if (filePath.IndexOf(".xlsx") > 0)
                        workbook = new XSSFWorkbook(fs);
                    // 2003版本  
                    else if (filePath.IndexOf(".xls") > 0)
                        workbook = new HSSFWorkbook(fs);

                    if (workbook != null)
                    {
                        sheet = workbook.GetSheetAt(0);//读取第一个sheet，当然也可以循环读取每个sheet  
                        dataTable = new DataTable();
                        if (sheet != null)
                        {
                            int rowCount = sheet.LastRowNum;//总行数  
                            if (rowCount > 0)
                            {
                                IRow firstRow = sheet.GetRow(0);//第一行  
                                int cellCount = firstRow.LastCellNum;//列数  

                                //构建datatable的列  
                                if (isColumnName)
                                {
                                    startRow = 1;//如果第一行是列名，则从第二行开始读取  
                                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                    {
                                        cell = firstRow.GetCell(i);
                                        if (cell != null)
                                        {
                                            if (cell.StringCellValue != null)
                                            {
                                                column = new DataColumn(cell.StringCellValue.Replace("(必填)", "").Replace("（必填）", ""));
                                                dataTable.Columns.Add(column);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                    {
                                        column = new DataColumn("column" + (i + 1));
                                        dataTable.Columns.Add(column);
                                    }
                                }

                                //填充行  
                                for (int i = startRow; i <= rowCount; ++i)
                                {
                                    row = sheet.GetRow(i);
                                    if (row == null) continue;

                                    dataRow = dataTable.NewRow();
                                    //第一行为空直接跳过
                                    if (row.GetCell(0) == null)
                                    {
                                        continue;
                                    }
                                    for (int j = row.FirstCellNum; j < cellCount; ++j)
                                    {
                                        cell = row.GetCell(j);
                                        if (cell == null)
                                        {
                                            dataRow[j] = "";
                                        }
                                        else
                                        {

                                            //CellType(Unknown = -1,Numeric = 0,String = 1,Formula = 2,Blank = 3,Boolean = 4,Error = 5,)  
                                            switch (cell.CellType)
                                            {
                                                case CellType.Blank:
                                                    dataRow[j] = "";
                                                    break;
                                                case CellType.Numeric:
                                                    //NPOI中数字和日期都是NUMERIC类型的，这里对其进行判断是否是日期类型
                                                    if (HSSFDateUtil.IsCellDateFormatted(cell))//日期类型
                                                    {
                                                        dataRow[j] = cell.DateCellValue;
                                                    }
                                                    else//其他数字类型
                                                    {
                                                        dataRow[j] = cell.NumericCellValue;
                                                    }
                                                    break;
                                                case CellType.String:
                                                    dataRow[j] = cell.StringCellValue;
                                                    break;
                                            }
                                        }
                                    }
                                    dataTable.Rows.Add(dataRow);
                                }
                            }
                        }
                    }
                }
                return dataTable;
            }
            catch (Exception)
            {
                if (fs != null)
                {
                    fs.Close();
                }
                return null;
            }
        }




        /// <summary>
        /// 将 List 数据转换为 Excel 文件流
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="exportDic">用于判断字段是否有导出权限</param>
        /// <returns></returns>
        public static IO.NpoiMemoryStream ListToExcel<T>(List<T> list, Dictionary<string, bool> exportDic = null, Dictionary<string, string> FieldList = default) where T : new()
        {
            if (FieldList == null)
            {
                FieldList = new Dictionary<string, string>();
            }
            //创建Excel文件的对象
            NPOI.XSSF.UserModel.XSSFWorkbook book = new();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");

            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);

            T model = new();
            var dict = PropertyHelper.GetPropertiesDisplayName(model);

            int x = 0;
            foreach (var item in dict)
            {
                //判断是否有权限
                if (exportDic != null && exportDic.Count > 0)
                {
                    if (exportDic.ContainsKey(item.Key.ToString()))
                    {
                        //无权限 跳过循环
                        if (exportDic[item.Key.ToString()] == false)
                        {
                            continue;
                        }
                    }
                }
                row1.CreateCell(x).SetCellValue(item.Key.ToString());
                x++;
            }

            var fields = new List<string>();
            //自定义属性数据 
            foreach (var item in FieldList)
            {
                var key = item.Key.ToString().Split("_")[1];
                if (!fields.Contains(key))
                {
                    row1.CreateCell(x).SetCellValue(key);
                    fields.Add(key);
                    x++;
                }
            }
            //将数据逐步写入sheet1各个行

            foreach (var item in list)
            {
                int i = list.IndexOf(item);
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);

                dict = PropertyHelper.GetProperties(item);
                int d = 0;
                var id = dict.Where(p => p.Key.ToString() == "Id").FirstOrDefault().Value;

                foreach (var it in dict)
                {

                    //判断是否有权限
                    if (exportDic != null && exportDic.Count > 0)
                    {
                        if (exportDic.ContainsKey(it.Key.ToString()))
                        {
                            //无权限 跳过循环
                            if (exportDic[it.Key.ToString()] == false)
                            {
                                continue;
                            }
                        }
                    }
                    var val = it.Value == null ? "" : it.Value.ToString();
                    //判断当前值是否是图片的路径并且不是文件，如果是就转成图片
                    Regex fileFilt = new(@".*\.[gif|jpg|php|jsp|jpeg|png]");

                    if (fileFilt.IsMatch(val.ToLower()))//统一路径
                    {
                        MsgInsetImg(sheet1, rowtemp, i + 1, book, val, d);
                    }
                    else
                    {
                        rowtemp.CreateCell(d).SetCellValue(val);
                    }

                    d++;
                }

                //自定义属性数据
                foreach (var item2 in FieldList)
                {
                    var key = item2.Key.ToString();

                    if (key.Contains(id + "_"))
                    {
                        rowtemp.CreateCell(d).SetCellValue(item2.Value == null ? "" : item2.Value.ToString());
                        d++;
                    }
                }
            }




            //写入到客户端 
            IO.NpoiMemoryStream ms = new();
            ms.AllowClose = false;
            book.Write(ms);
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            ms.AllowClose = true;


            //string filename = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "运算结果.xlsx";
            //return File(ms, "application/vnd.ms-excel", filename);

            return ms;


        }

        /// <summary>
        /// 将 List 数据转换为 Excel 文件流
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="exportDic">用于判断字段是否有导出权限</param>
        /// <returns></returns>
        public static IO.NpoiMemoryStream HSSFListToExcel<T>(List<T> list, Dictionary<string, bool> exportDic = null, Dictionary<string, string> FieldList = default) where T : new()
        {
            if (FieldList == null)
            {
                FieldList = new Dictionary<string, string>();
            }
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");

            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);

            T model = new();
            var dict = PropertyHelper.GetPropertiesDisplayName(model);

            int x = 0;
            foreach (var item in dict)
            {
                //判断是否有权限
                if (exportDic != null && exportDic.Count > 0)
                {
                    if (exportDic.ContainsKey(item.Key.ToString()))
                    {
                        //无权限 跳过循环
                        if (exportDic[item.Key.ToString()] == false)
                        {
                            continue;
                        }
                    }
                }
                row1.CreateCell(x).SetCellValue(item.Key.ToString());
                x++;
            }

            var fields = new List<string>();
            //自定义属性数据 
            foreach (var item in FieldList)
            {
                var key = item.Key.ToString().Split("_")[1];
                if (!fields.Contains(key))
                {
                    row1.CreateCell(x).SetCellValue(key);
                    fields.Add(key);
                    x++;
                }
            }
            //将数据逐步写入sheet1各个行

            foreach (var item in list)
            {
                int i = list.IndexOf(item);
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);

                dict = PropertyHelper.GetProperties(item);
                int d = 0;
                var id = dict.Where(p => p.Key.ToString() == "Id").FirstOrDefault().Value;

                foreach (var it in dict)
                {

                    //判断是否有权限
                    if (exportDic != null && exportDic.Count > 0)
                    {
                        if (exportDic.ContainsKey(it.Key.ToString()))
                        {
                            //无权限 跳过循环
                            if (exportDic[it.Key.ToString()] == false)
                            {
                                continue;
                            }
                        }
                    }
                    var val = it.Value == null ? "" : it.Value.ToString();
                    //判断当前值是否是图片的路径并且不是文件，如果是就转成图片
                    Regex fileFilt = new(@".*\.[gif|jpg|php|jsp|jpeg|png]");

                    if (fileFilt.IsMatch(val.ToLower()))//统一路径
                    {
                        HSSFMsgInsetImg(sheet1, rowtemp, i + 1, book, val, d);
                    }
                    else
                    {
                        rowtemp.CreateCell(d).SetCellValue(val);
                    }

                    d++;
                }

                //自定义属性数据
                foreach (var item2 in FieldList)
                {
                    var key = item2.Key.ToString();

                    if (key.Contains(id + "_"))
                    {
                        rowtemp.CreateCell(d).SetCellValue(item2.Value == null ? "" : item2.Value.ToString());
                        d++;
                    }
                }
            }




            //写入到客户端 
            var ms = new IO.NpoiMemoryStream();
            ms.AllowClose = false;
            book.Write(ms);
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            ms.AllowClose = true;


            //string filename = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "运算结果.xlsx";
            //return File(ms, "application/vnd.ms-excel", filename);

            return ms;


        }

        /// <summary>
        /// 将图标保存在Excel上
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="row1"></param>
        /// <param name="i"></param>
        /// <param name="xssfworkbook"></param>
        /// <param name="path"></param>
        /// <param name="colnum"></param>
        public static void MsgInsetImg(ISheet sheet, IRow row1, int i, XSSFWorkbook xssfworkbook, string path, int colnum)
        {
            try
            {

                ////设置图片那的宽高
                //sheet.SetColumnWidth(colnum, 3531);

                //var oos = new OssHelper();
                //path = path[1..(path.Length - 1)];
                //using var original = SKBitmap.Decode(oos.GetObjectStream(path));

                //using var img = original.Resize(new SKImageInfo(100, 145), SKFilterQuality.High);

                ////图片转换为文件流
                //MemoryStream ms = new();
                //using var image = SKImage.FromBitmap(img);
                //using var imageData = image.Encode(SKEncodedImageFormat.Png, 100);
                //imageData.SaveTo(ms);
                ////BinaryReader br = new BinaryReader(ms);
                //var picBytes = ms.ToArray();
                //ms.Close();
                //row1.Height = 2220;
                ////插入图片
                //if (picBytes != null && picBytes.Length > 0)
                //{
                //    var rows = i;
                //    var cols = colnum;
                //    /* Add Picture to Workbook, Specify picture type as PNG and Get an Index */
                //    int pictureIdx = xssfworkbook.AddPicture(picBytes, NPOI.SS.UserModel.PictureType.JPEG);  //添加图片
                //    /* Create the drawing container */
                //    XSSFDrawing drawing = (XSSFDrawing)sheet.CreateDrawingPatriarch();
                //    /* Create an anchor point */
                //    XSSFClientAnchor anchor = new(0, 0, 0, 0, cols, rows, 1, 1);

                //    /* Invoke createPicture and pass the anchor point and ID */
                //    XSSFPicture picture = (XSSFPicture)drawing.CreatePicture(anchor, pictureIdx);
                //    /* Call resize method, which resizes the image */
                //    picture.Resize();

                //    picBytes = null;
                //}
                //}
                //else
                //{

                //    if (System.IO.File.Exists(dPath))//如果有图片就设置高
                //    {
                //        row1.Height = 2220;
                //        using (FileStream fs = new FileStream(dPath, FileMode.Open, FileAccess.Read))

                //        {

                //            using (Image original = Image.FromStream(fs))

                //            {
                //                System.Drawing.Image img = original.GetThumbnailImage(100, 145, null, IntPtr.Zero);

                //                //图片转换为文件流
                //                MemoryStream ms = new MemoryStream();
                //                img.Save(ms, ImageFormat.Bmp);
                //                //BinaryReader br = new BinaryReader(ms);
                //                var picBytes = ms.ToArray();
                //                ms.Close();

                //                //插入图片
                //                if (picBytes != null && picBytes.Length > 0)
                //                {
                //                    var rows = i;
                //                    var cols = colnum;
                //                    /* Add Picture to Workbook, Specify picture type as PNG and Get an Index */
                //                    int pictureIdx = xssfworkbook.AddPicture(picBytes, NPOI.SS.UserModel.PictureType.JPEG);  //添加图片
                //                    /* Create the drawing container */
                //                    XSSFDrawing drawing = (XSSFDrawing)sheet.CreateDrawingPatriarch();
                //                    /* Create an anchor point */
                //                    XSSFClientAnchor anchor = new XSSFClientAnchor(0, 0, 0, 0, cols, rows, 1, 1);

                //                    /* Invoke createPicture and pass the anchor point and ID */
                //                    XSSFPicture picture = (XSSFPicture)drawing.CreatePicture(anchor, pictureIdx);
                //                    /* Call resize method, which resizes the image */
                //                    picture.Resize();

                //                    picBytes = null;
                //                }
                //            }
                //        }
                //    }

                //}

            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 将图标保存在Excel上
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="row1"></param>
        /// <param name="i"></param>
        /// <param name="xssfworkbook"></param>
        /// <param name="path"></param>
        /// <param name="colnum"></param>
        public static void HSSFMsgInsetImg(ISheet sheet, IRow row1, int i, HSSFWorkbook xssfworkbook, string path, int colnum)
        {
            try
            {

                ////设置图片那的宽高
                //sheet.SetColumnWidth(colnum, 3531);
                //var oos = new OssHelper();
                //using var original = SKBitmap.Decode(oos.GetObjectStream(path));
                //using var img = original.Resize(new SKImageInfo(100, 145), SKFilterQuality.High);

                ////图片转换为文件流
                //MemoryStream ms = new();
                //using var image = SKImage.FromBitmap(img);
                //using var imageData = image.Encode(SKEncodedImageFormat.Png, 100);
                //imageData.SaveTo(ms);
                ////BinaryReader br = new BinaryReader(ms);
                //var picBytes = ms.ToArray();
                //ms.Close();
                //row1.Height = 2220;
                ////插入图片
                //if (picBytes != null && picBytes.Length > 0)
                //{
                //    var rows = i;
                //    var cols = colnum;
                //    /* Add Picture to Workbook, Specify picture type as PNG and Get an Index */
                //    int pictureIdx = xssfworkbook.AddPicture(picBytes, NPOI.SS.UserModel.PictureType.JPEG);  //添加图片
                //    /* Create the drawing container */
                //    XSSFDrawing drawing = (XSSFDrawing)sheet.CreateDrawingPatriarch();
                //    /* Create an anchor point */
                //    XSSFClientAnchor anchor = new(0, 0, 0, 0, cols, rows, 1, 1);

                //    /* Invoke createPicture and pass the anchor point and ID */
                //    XSSFPicture picture = (XSSFPicture)drawing.CreatePicture(anchor, pictureIdx);
                //    /* Call resize method, which resizes the image */
                //    picture.Resize();

                //    picBytes = null;
                //}

                //else
                //{

                //    if (System.IO.File.Exists(dPath))//如果有图片就设置高
                //    {
                //        row1.Height = 2220;
                //        using (FileStream fs = new FileStream(dPath, FileMode.Open, FileAccess.Read))

                //        {

                //            using (Image original = Image.FromStream(fs))

                //            {
                //                System.Drawing.Image img = original.GetThumbnailImage(100, 145, null, IntPtr.Zero);

                //                //图片转换为文件流
                //                MemoryStream ms = new MemoryStream();
                //                img.Save(ms, ImageFormat.Bmp);
                //                //BinaryReader br = new BinaryReader(ms);
                //                var picBytes = ms.ToArray();
                //                ms.Close();

                //                //插入图片
                //                if (picBytes != null && picBytes.Length > 0)
                //                {
                //                    var rows = i;
                //                    var cols = colnum;
                //                    /* Add Picture to Workbook, Specify picture type as PNG and Get an Index */
                //                    int pictureIdx = xssfworkbook.AddPicture(picBytes, NPOI.SS.UserModel.PictureType.JPEG);  //添加图片
                //                    /* Create the drawing container */
                //                    XSSFDrawing drawing = (XSSFDrawing)sheet.CreateDrawingPatriarch();
                //                    /* Create an anchor point */
                //                    XSSFClientAnchor anchor = new XSSFClientAnchor(0, 0, 0, 0, cols, rows, 1, 1);

                //                    /* Invoke createPicture and pass the anchor point and ID */
                //                    XSSFPicture picture = (XSSFPicture)drawing.CreatePicture(anchor, pictureIdx);
                //                    /* Call resize method, which resizes the image */
                //                    picture.Resize();

                //                    picBytes = null;
                //                }
                //            }
                //        }
                //    }

                //}

            }
            catch (Exception)
            {
            }
        }



        /// <summary>
        /// 添加下拉列表
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="name"></param>
        /// <param name="firstcol"></param>
        /// <param name="lastcol"></param>
        /// <param name="vals"></param>
        /// <param name="sheetindex"></param>
        public static XSSFWorkbook SetCellDropdownList(XSSFWorkbook workbook, string name, int firstcol, int lastcol, string[] vals)
        {
            ISheet sheet = workbook.GetSheetAt(0);

            //先创建一个Sheet专门用于存储下拉项的值
            ISheet sheet2 = workbook.CreateSheet(name);

            var sheet2Index = workbook.GetSheetIndex(name);

            //隐藏
            workbook.SetSheetHidden(sheet2Index, true);
            int index = 0;
            foreach (var item in vals)
            {
                sheet2.CreateRow(index).CreateCell(0).SetCellValue(item);
                index++;
            }
            //创建的下拉项的区域：
            var rangeName = name + "Range";
            IName range = workbook.CreateName();
            range.RefersToFormula = name + "!$A$1:$A$" + index;
            range.NameName = rangeName;
            CellRangeAddressList regions = new(0, 65535, firstcol, lastcol);


            CT_DataValidation dataValidation = new();

            dataValidation.showErrorMessage = true;
            dataValidation.showDropDown = true;
            dataValidation.showInputMessage = true;
            //XSSFDataValidationHelper dvHelper = new XSSFDataValidationHelper(workbook);
            //XSSFDataValidationConstraint dvConstraint = (XSSFDataValidationConstraint)dvHelper.CreateFormulaListConstraint(name);
            //CellRangeAddressList addressList = new CellRangeAddressList(0, 65535, firstcol, lastcol);
            //XSSFDataValidation validation = (XSSFDataValidation)dvHelper.CreateValidation(dvConstraint, addressList);
            ////XSSFDataValidation dataValidate = new XSSFDataValidation(regions, dataValidation);
            ////dataValidate.CreateErrorBox("输入不合法", "请输入或选择下拉列表中的值。");
            ////dataValidate.EmptyCellAllowed = true;
            ////dataValidate.ShowPromptBox = false;
            ////dataValidate.ShowErrorBox = false; 
            //sheet.AddValidationData(dataValidate);
            return workbook;
        }
        /// <summary>
        /// 添加下拉列表数据
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="name"></param> 
        public static void SetCellValuelist(XSSFWorkbook workbook, string name, string[] vals)
        {


            //先创建一个Sheet专门用于存储下拉项的值
            ISheet sheet2 = workbook.GetSheet(name);
            int index = 0;
            foreach (var item in vals)
            {
                sheet2.CreateRow(index).CreateCell(0).SetCellValue(item);
                index++;
            }

        }

    }

}
