using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM.Framework;
using Procons.TransferCost.Main;

namespace Procons.TransferCost.UI.Forms
{
    [FormAttribute("frm_ItemAddCost", "Forms/frm_ItemAddCost.b1f")]
    public class ItemAddCost : B1FormBase
    {
        private SAPbouiCOM.Button btnOk;
        private SAPbouiCOM.Button btnAddRow;
        private SAPbouiCOM.Button btnCancel;
        private SAPbouiCOM.DBDataSource ds;
        private SAPbouiCOM.Matrix mtxCost;
        private SAPbouiCOM.EditText currentControl;
        public string TransferDocEntry { get; set; }

        public ItemAddCost()
        {
            ds = this.UIAPIRawForm.DataSources.DBDataSources.Item("@T_HOUSSAM");
            ds.Query(null);
            mtxCost.LoadFromDataSource();
            mtxCost.AutoResizeColumns();
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.btnOk = ((SAPbouiCOM.Button)(this.GetItem("1").Specific));
            this.btnAddRow = ((SAPbouiCOM.Button)(this.GetItem("AddRow").Specific));
            this.btnAddRow.ClickAfter += btnAddRow_ClickAfter;
            this.btnOk.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnOk_ClickBefore);
            this.btnCancel = ((SAPbouiCOM.Button)(this.GetItem("2").Specific));
            this.mtxCost = ((SAPbouiCOM.Matrix)(this.GetItem("mtxCosts").Specific));
            this.mtxCost.ChooseFromListAfter += new SAPbouiCOM._IMatrixEvents_ChooseFromListAfterEventHandler(this.mtxCost_ChooseFromListAfter);
            this.mtxCost.ClickAfter += new SAPbouiCOM._IMatrixEvents_ClickAfterEventHandler(this.mtxCost_ClickAfter);
            this.mtxCost.ClickBefore += new SAPbouiCOM._IMatrixEvents_ClickBeforeEventHandler(this.mtxCost_ClickBefore);
            this.OnCustomInitialize();

        }

        void btnAddRow_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            mtxCost.AddLine();
            //mtxCost.AddRowIndex();
            mtxCost.AddRowIndex();
            mtxCost.FlushToDataSource();
            this.UIAPIRawForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE;
        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            this.ResizeAfter += new SAPbouiCOM.Framework.FormBase.ResizeAfterHandler(this.Form_ResizeAfter);
            this.ActivateAfter += new SAPbouiCOM.Framework.FormBase.ActivateAfterHandler(this.Form_ActivateAfter);
            this.ActivateBefore += new SAPbouiCOM.Framework.FormBase.ActivateBeforeHandler(this.Form_ActivateBefore);
            this.RightClickBefore += new SAPbouiCOM.Framework.FormBase.RightClickBeforeHandler(this.Form_RightClickBefore);
            this.DataAddAfter += new DataAddAfterHandler(this.Form_DataAddAfter);

        }


        private void OnCustomInitialize()
        {

        }

        private void Form_ResizeAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            mtxCost.AutoResizeColumns();
        }

        public override void ItemEvent(string FormId, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
        }

        public override void MenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            this.UIAPIRawForm.Freeze(true);
            try
            {
                if (pVal.MenuUID == "AddLine")
                {
                    if (this.UIAPIRawForm.Mode == SAPbouiCOM.BoFormMode.fm_UPDATE_MODE || this.UIAPIRawForm.Mode == SAPbouiCOM.BoFormMode.fm_OK_MODE ||
                        this.UIAPIRawForm.Mode == SAPbouiCOM.BoFormMode.fm_ADD_MODE)
                    {
                        mtxCost.AddLine();
                        //mtxCost.AddRowIndex();
                        mtxCost.FlushToDataSource();
                    }
                }
                if (pVal.MenuUID == "DeleteLine")
                {
                    if (RowToDeleteIndex > 0)
                    {
                        mtxCost.DeleteRow(RowToDeleteIndex);
                        mtxCost.FlushToDataSource();
                        this.UIAPIRawForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
            }
            finally
            {
                this.UIAPIRawForm.Freeze(false);
            }
        }

        private void Form_DeactivateAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {

        }

        private void Form_ActivateAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {



        }

        private void Form_ActivateBefore(SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
        }

        private void mtxCost_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            mtxCost.FlushToDataSource();

        }

        private void btnOk_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            mtxCost.GetValuedRows();
            mtxCost.FlushToDataSource();
            BubbleEvent = true;
        }

        private void Form_RightClickBefore(ref SAPbouiCOM.ContextMenuInfo eventInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;
            RowToDeleteIndex = eventInfo.Row;

        }

        private void mtxCost_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {

        }

        private void Form_DataAddAfter(ref SAPbouiCOM.BusinessObjectInfo pVal)
        {


        }

        private void mtxCost_ChooseFromListAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            
        }

    }
}
