using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.IO;
using System.Data;

namespace ExportExcelByNPOI
{
    public class ExcelHelper : IDisposable
    {
        private string fileName = null; //文件名
        private IWorkbook workbook = null;
        private FileStream fs = null;
        private bool disposed;

        public ExcelHelper(string fileName)
        {
            this.fileName = fileName;
            disposed = false;
        }

        /// <summary>
        /// 将DataTable数据导入到excel中
        /// </summary>
        /// <param name="data">要导入的数据</param>
        /// <param name="isColumnWritten">DataTable的列名是否要导入</param>
        /// <param name="isFreezeHeadLine">是否冻结首行</param>
        /// <param name="sheetName">要导入的excel的sheet的名称</param>
        /// <param name="comments">同时需要导入的批注</param>
        /// <returns>导入数据行数(包含列名那一行)</returns>
        public int DataTableToExcel(DataTable data, string sheetName, bool isColumnWritten, bool isFreezeHeadLine, params Comment[] comments)
        {
            int i = 0;
            int j = 0;
            int count = 0;
            ISheet sheet = null;

            try
            {
                fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                    workbook = new XSSFWorkbook();
                else if (fileName.IndexOf(".xls") > 0) // 2003版本
                    workbook = new HSSFWorkbook();
                if (workbook != null)
                {
                    sheet = workbook.CreateSheet(sheetName);
                }
                else
                {
                    return -1;
                }

                if (isColumnWritten == true) //写入DataTable的列名
                {
                    IRow row = sheet.CreateRow(0);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        ICell cell = row.CreateCell(j);
                        cell.SetCellValue(data.Columns[j].ColumnName);
                        cell.CellStyle = workbook.CreateCellStyle();
                        cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                        cell.CellStyle.Alignment = HorizontalAlignment.Center;
                        sheet.SetColumnWidth(j, (cell.StringCellValue.Length + 4) * 256);
                    }
                    count = 1;
                }
                else
                {
                    count = 0;
                }

                for (i = 0; i < data.Rows.Count; ++i)
                {
                    IRow row = sheet.CreateRow(count);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(data.Rows[i][j].ToString());
                    }
                    ++count;
                }
                if (comments != null && comments.Length > 0)
                {
                    HSSFComment comment;
                    HSSFPatriarch patr = sheet.CreateDrawingPatriarch() as HSSFPatriarch;
                    for (int k = 0; k < comments.Length; k++)
                    {
                        IClientAnchor anchor = workbook.GetCreationHelper().CreateClientAnchor();
                        anchor.Col1 = comments[k].columnIndex + 1;
                        anchor.Col2 = comments[k].columnIndex + 5;
                        anchor.Row1 = comments[k].rowIndex;
                        int lines = 0;
                        for (int l = 0; l < comments[k].comment.Length; l++)
                        {
                            if (comments[k].comment[l] == '\n')
                            {
                                lines++;
                            }
                        }
                        anchor.Row2 = comments[k].rowIndex + lines + 4;
                        comment = patr.CreateCellComment(anchor) as HSSFComment;
                        comment.String = new HSSFRichTextString(comments[k].comment);
                        ICell cell = sheet.GetRow(comments[k].rowIndex).Cells[comments[k].columnIndex];
                        cell.CellComment = comment;
                    }
                }

                if (isFreezeHeadLine)
                {
                    sheet.CreateFreezePane(0, 1);
                }

                workbook.Write(fs); //写入到excel
                return count;
            }
            catch (IOException ioEx)
            {
                UnityEngine.Debug.LogError(string.Format("请先关闭Excel文件 {0}  \n{1}", fileName, ioEx.Message));
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError("Exception: " + ex.Message);
                //Console.WriteLine("Exception: " + ex.Message);
            }
            return -1;
        }

        /// <summary>
        /// 将excel中的数据导入到DataTable中
        /// </summary>
        /// <param name="sheetName">excel工作薄sheet的名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
        /// <returns>返回的DataTable</returns>
        public DataTable ExcelToDataTable(string sheetName, bool isFirstRowColumn)
        {
            ISheet sheet = null;
            DataTable data = new DataTable();
            int startRow = 0;
            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                    workbook = new XSSFWorkbook(fs);
                else if (fileName.IndexOf(".xls") > 0) // 2003版本
                    workbook = new HSSFWorkbook(fs);

                if (sheetName != null)
                {
                    sheet = workbook.GetSheet(sheetName);
                    if (sheet == null) //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                }
                else
                {
                    sheet = workbook.GetSheetAt(0);
                }
                if (sheet != null)
                {
                    IRow firstRow = sheet.GetRow(0);
                    int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数

                    if (isFirstRowColumn)
                    {
                        for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                        {
                            ICell cell = firstRow.GetCell(i);
                            if (cell != null)
                            {
                                string cellValue = cell.StringCellValue;
                                if (cellValue != null)
                                {
                                    DataColumn column = new DataColumn(cellValue);
                                    data.Columns.Add(column);
                                }
                            }
                        }
                        startRow = sheet.FirstRowNum + 1;
                    }
                    else
                    {
                        startRow = sheet.FirstRowNum;
                    }

                    //最后一列的标号
                    int rowCount = sheet.LastRowNum;
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue; //没有数据的行默认是null　　　　　　　

                        DataRow dataRow = data.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            if (row.GetCell(j) != null) //同理，没有数据的单元格都默认是null
                                dataRow[j] = row.GetCell(j).ToString();
                        }
                        data.Rows.Add(dataRow);
                    }
                }

                return data;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log("Exception: " + ex.Message);
                //Console.WriteLine("Exception: " + ex.Message);
                return null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (fs != null)
                        fs.Close();
                }

                fs = null;
                disposed = true;
            }
        }

        public struct Comment
        {
            public int rowIndex;
            public int columnIndex;
            public string comment;

            public Comment(int rowIndex, int columnIndex, string comment)
            {
                this.rowIndex = rowIndex;
                this.columnIndex = columnIndex;
                this.comment = comment;
            }
        }
    }
}