
using NPOI;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Action
{
    public static class NpoiExtendAction
    {
    
        public static Dictionary<int, List<XSSFPictureData>> GetAllXSSFPictureDics(XSSFWorkbook wk)
        {
          
            var data = GetPicMap(wk);
            return data;
        }  

        /**
        * 获取文字
        * key = 文字所在行; value = 文字内容
        * @param wb
        * @return Map(row,text)
        */
        private static Dictionary<int, string> getText(XSSFWorkbook wb)
        {

            Dictionary<int, string> map = new();
            //得到Excel工作表对象
            XSSFSheet HSSsheet = (XSSFSheet)wb.GetSheetAt(0);
            //得到Excel工作表的行 
            for (int i = 0; i <= HSSsheet.LastRowNum; i++)
            {
                // 读取左上端单元格
                IRow row =  HSSsheet.GetRow(i);
                // 行不为空
                if (row != null)
                {
                    // 获取到Excel文件中的所有的列
                    int cells = row.FirstCellNum;
                    // 遍历列
                    for (int j = 0; j < cells; j++)
                    {
                        // 获取到列的值
                        ICell cell = row.GetCell(j);
                        map.Add(i, cell.StringCellValue);
                    }
                }
            }
            return map;
        }
        /**
        * 
        * 获取图片
        * key = 图片所在行 value = 图片对象
        * @param workbook
        * @return Map(row,image);
*/
        private static Dictionary<int, List<XSSFPictureData>> GetPicMap(XSSFWorkbook wb)
        {


            Dictionary<int, List<XSSFPictureData>> picMap = new();
            var pictures = wb.GetAllPictures();
            XSSFSheet sheet = (XSSFSheet)wb.GetSheetAt(0);
            foreach (XSSFShape shape in sheet.GetDrawingPatriarch().GetShapes())
            {
                XSSFClientAnchor anchor = (XSSFClientAnchor)shape.GetAnchor();
                List<XSSFPictureData> list = new();


                if (shape is XSSFPicture) {
                    XSSFPicture pic = (XSSFPicture)shape;
                    var picture = (XSSFPicture)shape; 

                    XSSFPictureData picData = (XSSFPictureData)picture.PictureData;
                    //如果通过本次获取的行能得到value，说明本行已存在， 
                    if (picMap.Where(x => x.Key == anchor.Row1).Count() > 0)
                    {
                        //找到本行的集合
                        List<XSSFPictureData> list2 = picMap.Where(x=>x.Key==anchor.Row1).FirstOrDefault().Value;
                        //将图片存储进去 
                        list2.Add(picData);
                        //进入下一轮循环
                        continue;
                    }
                    //添加本次循环的图片对象
                    list.Add(picData);
                    //将本张图片的行信息作为key存入map，将图片对象作为值存储
                    picMap.Add(anchor.Row1, list);
                }
                else
                {
                    //非图片数据则插入null
                    picMap.Add(anchor.Row1, null);
                }

            } 
         return picMap;
    } 
    }
}
