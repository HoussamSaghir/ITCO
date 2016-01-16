using System;
using System.Collections.Generic;
using System.Linq;

namespace ITCO.TransferCost.Main
{
    public static class ExtentionMethods
    {
        #region MATRIX METHODS
        public static int GetColumnIndex(this SAPbouiCOM.Matrix mtxControl, string columnName)
        {
            var index = 0;
            for (int i = 0; i < mtxControl.Columns.Count; i++)
            {
                if (mtxControl.Columns.Item(i).UniqueID == columnName)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
        public static void AddLine(this SAPbouiCOM.Matrix mtxControl)
        {
            try
            {
                SAPbouiCOM.EditText currentControl;
                if (mtxControl.RowCount <= 0)
                {
                    mtxControl.AddRow();
                    mtxControl.ClearRowData(1);
                }
                else
                {
                    mtxControl.AddRow(1, mtxControl.RowCount + 1);
                    mtxControl.ClearRowData(mtxControl.RowCount); 
                }
                currentControl = mtxControl.GetCellSpecific(1, mtxControl.RowCount) as SAPbouiCOM.EditText;
                currentControl.Active = true;
            }
            catch (Exception ex)
            {

            }
        }

        public static void AddRowIndex(this SAPbouiCOM.Matrix mtxControl)
        {
            //var rec = B1Helper.DiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;
            //rec.DoQuery("SELECT MAX([DocEntry]) AS DocEntry FROM [dbo].[@T_ITMTRNSFRSTUP]");
            //string lastIndex = string.Empty;
            //while (rec.EoF == false)
            //{
            //    lastIndex = rec.Fields.Item("DocEntry").Value.ToString();
            //    rec.MoveNext();
            //}

            var lastIndex = ((dynamic)mtxControl.Columns.Item("#").Cells.Item(mtxControl.RowCount).Specific).Value;
            if (lastIndex == string.Empty)
                lastIndex = "0";
            ((dynamic)mtxControl.Columns.Item("#").Cells.Item(mtxControl.RowCount).Specific).Value = int.Parse(lastIndex) + 1;
        }

        public static void GetValuedRows(this SAPbouiCOM.Matrix mtxControl)
        {
            for (int i = 1; i <= mtxControl.RowCount; i++)
            {
                int c = mtxControl.Columns.Count - 1;
                bool isEmpty = true;
                while (c >= 0)
                {
                    var cellValue = ((dynamic)mtxControl.Columns.Item(c).Cells.Item(i).Specific).Value;
                    if (cellValue != string.Empty && mtxControl.Columns.Item(c).UniqueID != Constants.Hash && cellValue != "0.0")
                    {
                        isEmpty = false;
                        break;
                    }
                    c--;
                }
                if (isEmpty)
                {
                    mtxControl.DeleteRow(i);
                }
            }
        }

        public static object GetCellValue(this SAPbouiCOM.Matrix control, string column, object row)
        {
            return ((dynamic)control.Columns.Item(column).Cells.Item(row).Specific).Value;
        }

        public static void SetCellValue(this SAPbouiCOM.Matrix control, string column, object row,object newValue)
        {
            ((dynamic)control.Columns.Item(column).Cells.Item(row).Specific).Value = newValue; 
        }

        public static bool IsMatrixCellsNotEmpty(this SAPbouiCOM.Matrix mtxControl, params string[] fields)
        {
            bool result = true;
            List<string> ValuesList = new List<string>();
            for (int i = 1; i <= mtxControl.RowCount; i++)
            {
                foreach (var v in fields)
                {
                    ValuesList.Add(mtxControl.GetCellValue(v, i).ToString());
                }
                var emptyValues = ValuesList.Where(x => x.Equals(string.Empty)).ToList();
                if (emptyValues.Count != 0)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        #endregion

        #region DATATABLE METHODS
        #endregion

        #region grid methods

        public static void SetGridHeaderIndex(this SAPbouiCOM.Grid control)
        {
            for (int i = 0; i < control.Rows.Count; i++)
            {
                control.RowHeaders.SetText(i, (i + 1).ToString());
            }
            control.Columns.Item("RowsHeader").TitleObject.Caption = "#";
        }

        #endregion

        public static void AddLine(this SAPbouiCOM.Grid grdControl)
        {
            grdControl.DataTable.Rows.Add(1);
        }

        public static void ReleaseObject(this object ob)
        {
            System.Runtime.InteropServices.Marshal.ReleaseComObject(ob);
            ob = null;
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        public static void ActivateControl(this SAPbouiCOM.EditText control)
        {
            if (control != null)
                control.Active = true;
            control = null;
        }

        public static int GetSelectedRowIndex(this SAPbouiCOM.Grid gvControl)
        {
            var selectedRows = gvControl.Rows.SelectedRows.Cast<dynamic>();
            int selectedRow = selectedRows.FirstOrDefault();
            return selectedRow;
        }
    }
}
