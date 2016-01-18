using ITCO.TransferCost.Main;
using ITCO.TransferCost.UI.Forms;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITCO.TransferCost.UI
{
    [FormAttribute("frm_TransferItems", "Forms/frm_TransferItems.b1f")]
    public class TransferItems : B1FormBase
    {
        #region PRIVATE VARIABLES

        private SAPbouiCOM.EditText dpDocDate;
        private SAPbouiCOM.EditText txtDocNo;
        private SAPbouiCOM.Button btnSave;
        private SAPbouiCOM.Button btnCancel;
        private SAPbouiCOM.Button btnAddRow;
        private SAPbouiCOM.DBDataSource dtTransferItemLines;
        private SAPbouiCOM.Folder fldrItems;
        private SAPbouiCOM.Folder fldrCosts;
        private SAPbouiCOM.Matrix mtxItems;
        private SAPbouiCOM.Matrix mtxCost;
        //private SAPbouiCOM.Button btnCalculate;
        private SAPbouiCOM.EditText currentControl = null;
        private List<TransferItem> issTransferItemsList = new List<TransferItem>();
        private List<TransferItem> recTransferItemsList = new List<TransferItem>();
        public List<SelectedBatchDataSources> _selectedBatchDataSources = new List<SelectedBatchDataSources>();
        public List<SelectedBatchDataSources> _selectedRecBatchDataSources = new List<SelectedBatchDataSources>();

        #endregion

        #region PUBLIC VARIABLES

        public event SubFormCreated<BatchNumberSelection> OnSubFormCreated;

        public override event UserFormClosedDone OnUserFormClosedDone;
        #endregion

        #region PUBLIC PROPERTIES
        public List<SelectedBatchDataSources> SelectedBatchDataSources { get { return _selectedBatchDataSources; } set { _selectedBatchDataSources = value; } }
        public List<SelectedBatchDataSources> SelectedRecBatchDataSources { get { return _selectedRecBatchDataSources; } set { _selectedRecBatchDataSources = value; } }
        public Nullable<bool> BatchesManaged { get; set; }
        public bool IsMandatoryBatches { get; set; }
        public BatchNumberSelection BatchForm { get; set; }

        #endregion

        #region PUBLIC CONSTRUCTORS
        public TransferItems()
        {
            mtxItems.AutoResizeColumns();
            mtxCost.AutoResizeColumns();
            fldrItems.Select();
            //BatchesManaged = true;
            IsMandatoryBatches = false;
            Action action = new Action(delegate()
            {
                this.UIAPIRawForm.EnableMenu("1283", false);
                this.UIAPIRawForm.EnableMenu("1284", false);
                this.UIAPIRawForm.EnableMenu("1286", false);
                this.UIAPIRawForm.EnableMenu("1288", true);
                this.UIAPIRawForm.EnableMenu("1289", true);
                this.UIAPIRawForm.EnableMenu("1290", true);
                this.UIAPIRawForm.EnableMenu("1291", true);
            });

            Task.Run(action);
            txtDocNo.Item.SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, Convert.ToInt32(SAPbouiCOM.BoAutoFormMode.afm_Find), SAPbouiCOM.BoModeVisualBehavior.mvb_True);
            mtxItems.Item.SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, Convert.ToInt32(SAPbouiCOM.BoAutoFormMode.afm_Ok), SAPbouiCOM.BoModeVisualBehavior.mvb_False);
            mtxCost.Item.SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, Convert.ToInt32(SAPbouiCOM.BoAutoFormMode.afm_Ok), SAPbouiCOM.BoModeVisualBehavior.mvb_False);
            //btnCalculate.Item.SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Visible, Convert.ToInt32(SAPbouiCOM.BoAutoFormMode.afm_Ok), SAPbouiCOM.BoModeVisualBehavior.mvb_False);
            //btnCalculate.Item.SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Visible, Convert.ToInt32(SAPbouiCOM.BoAutoFormMode.afm_Find), SAPbouiCOM.BoModeVisualBehavior.mvb_False);
            btnAddRow.Item.SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Visible, Convert.ToInt32(SAPbouiCOM.BoAutoFormMode.afm_Ok), SAPbouiCOM.BoModeVisualBehavior.mvb_False);
            btnAddRow.Item.SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Visible, Convert.ToInt32(SAPbouiCOM.BoAutoFormMode.afm_Find), SAPbouiCOM.BoModeVisualBehavior.mvb_False);
        }

        #endregion

        #region INITIALIZATION METHODS

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.mtxItems = ((SAPbouiCOM.Matrix)(this.GetItem("mtxItems").Specific));
            this.txtDocNo = ((SAPbouiCOM.EditText)(this.GetItem("txtDocNo").Specific));
            this.dtTransferItemLines = this.UIAPIRawForm.DataSources.DBDataSources.Item(string.Format("@{0}", TableNames.TransferItemsLines));
            this.dpDocDate = ((SAPbouiCOM.EditText)(this.GetItem("dpDocDate").Specific));
            this.mtxItems.ChooseFromListBefore += new SAPbouiCOM._IMatrixEvents_ChooseFromListBeforeEventHandler(this.mtxItems_ChooseFromListBefore);
            this.mtxItems.ChooseFromListAfter += new SAPbouiCOM._IMatrixEvents_ChooseFromListAfterEventHandler(this.mtxItems_ChooseFromListAfter);
            this.mtxItems.ClickBefore += new SAPbouiCOM._IMatrixEvents_ClickBeforeEventHandler(this.mtxItems_ClickBefore);
            this.btnSave = ((SAPbouiCOM.Button)(this.GetItem("1").Specific));
            this.btnSave.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnSave_ClickBefore);
            this.btnSave.PressedBefore += new SAPbouiCOM._IButtonEvents_PressedBeforeEventHandler(this.btnSave_PressedBefore);
            this.btnCancel = ((SAPbouiCOM.Button)(this.GetItem("2").Specific));
            this.fldrItems = ((SAPbouiCOM.Folder)(this.GetItem("Item_5").Specific));
            this.fldrCosts = ((SAPbouiCOM.Folder)(this.GetItem("Item_6").Specific));
            this.mtxCost = ((SAPbouiCOM.Matrix)(this.GetItem("Item_7").Specific));
            this.mtxCost.ChooseFromListAfter += new SAPbouiCOM._IMatrixEvents_ChooseFromListAfterEventHandler(this.mtxCost_ChooseFromListAfter);
            this.mtxCost.ClickBefore += new SAPbouiCOM._IMatrixEvents_ClickBeforeEventHandler(this.mtxCost_ClickBefore);
            //this.btnCalculate = ((SAPbouiCOM.Button)(this.GetItem("btnClc").Specific));
            //this.btnCalculate.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.btnCalculate_ClickAfter);
            //this.btnCalculate.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnCalculate_ClickBefore);
            this.btnAddRow = ((SAPbouiCOM.Button)(this.GetItem("btnAddRow").Specific));
            this.btnAddRow.ClickBefore += btnAddRow_ClickBefore;
            this.btnAddRow.ClickAfter += btnAddRow_ClickAfter;
            this.mtxItems.ValidateBefore += mtxItems_ValidateBefore;
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            this.ActivateAfter += new SAPbouiCOM.Framework.FormBase.ActivateAfterHandler(this.TransferItems_ActivateAfter);
            this.DeactivateAfter += new SAPbouiCOM.Framework.FormBase.DeactivateAfterHandler(this.Form_DeactivateAfter);
            this.ResizeAfter += new SAPbouiCOM.Framework.FormBase.ResizeAfterHandler(this.Form_ResizeAfter);
            this.CloseAfter += new SAPbouiCOM.Framework.FormBase.CloseAfterHandler(this.Form_CloseAfter); ;
            this.RightClickBefore += new SAPbouiCOM.Framework.FormBase.RightClickBeforeHandler(this.Form_RightClickBefore);
            this.DataAddAfter += new SAPbouiCOM.Framework.FormBase.DataAddAfterHandler(this.Form_DataAddAfter);
            this.RightClickBefore += TransferItems_RightClickBefore;
            this.RightClickAfter += TransferItems_RightClickAfter;

        }

        private void OnCustomInitialize()
        {

        }

        #endregion

        #region EVENT HANDLERS

        #region FORM HANDLERS

        private void TransferItems_RightClickAfter(ref SAPbouiCOM.ContextMenuInfo eventInfo)
        {
            try
            {
                RowToDeleteIndex = eventInfo.Row;
                if (eventInfo.ItemUID == "mtxItems" || eventInfo.ItemUID == "mtxCost")
                    RemoveDeleteMenu();
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
            }
        }

        private void TransferItems_RightClickBefore(ref SAPbouiCOM.ContextMenuInfo eventInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                if (eventInfo.ItemUID == "mtxItems" || eventInfo.ItemUID == "mtxCost")
                    AddDeleteMenu();

            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
            }
        }

        private void Form_ResizeAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                mtxItems.AutoResizeColumns();
                mtxCost.AutoResizeColumns();
                var item = this.UIAPIRawForm.Items.Item("Item_4");
                item.Width = mtxItems.Item.Width + 10;
                item.Height = mtxItems.Item.Height + 10;
            }

            catch (Exception ex)
            {
                Utilities.LogException(ex);
            }
        }

        private void Form_RightClickBefore(ref SAPbouiCOM.ContextMenuInfo eventInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                RowToDeleteIndex = eventInfo.Row;
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
            }
        }

        private void Form_DataAddAfter(ref SAPbouiCOM.BusinessObjectInfo pVal)
        {
            try
            {
                var ccc = B1Helper.DiCompany.GetNewObjectKey();
                System.Xml.XmlDocument oXmlDoc = new System.Xml.XmlDocument();
                oXmlDoc.LoadXml(pVal.ObjectKey);
                var udoCode = oXmlDoc.SelectSingleNode("//UDO_ItemsTransferParams/DocEntry").InnerText;
                AddonInfoInfo.TransferItemsProcess(Application.SBO_Application, udoCode, SelectedBatchDataSources, SelectedRecBatchDataSources, issTransferItemsList, recTransferItemsList);
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
            }
            finally
            {
                ClearListsContents();
                BatchesManaged = null;
            }
        }

        private void Form_DeactivateAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
        }

        private void Form_CloseAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                OnUserFormClosedDone(this);
            }
            catch(Exception ex)
            {
                Utilities.LogException(ex);
            }
        }

        private void TransferItems_ActivateAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                if (currentControl != null)
                    currentControl.Active = true;
                currentControl = null;
            }
            catch(Exception ex)
            {
                Utilities.LogException(ex);
            }
        }

        #endregion

        #region GLOBAL HANDLERS
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
                    if (this.UIAPIRawForm.Mode == SAPbouiCOM.BoFormMode.fm_ADD_MODE || this.UIAPIRawForm.Mode == SAPbouiCOM.BoFormMode.fm_OK_MODE)
                    {
                        if (fldrItems.Selected == true)
                            mtxItems.AddLine();
                        else
                            mtxCost.AddLine();
                    }
                }
                if (pVal.MenuUID == "DeleteLine")
                {

                    if (RowToDeleteIndex > 0)
                    {
                        mtxItems.DeleteRow(RowToDeleteIndex);
                        mtxItems.FlushToDataSource();
                    }
                }
            }
            catch (Exception ex)
            {
                Application.SBO_Application.SetStatusBarMessage(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Long, true);
            }
            this.UIAPIRawForm.Freeze(false);
        }

        #endregion

        #region CONTROLS HANDLERS
        public void mtxItems_ValidateBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            if (pVal.ColUID.Equals("Quantity"))
            {

                decimal quantityValue = decimal.MinValue;
                var whsCode = mtxItems.GetCellValue("FromWhs", pVal.Row).ToString();
                var itemCode = mtxItems.GetCellValue("to_Item", pVal.Row).ToString();
                var quantity = mtxItems.GetCellValue("Quantity", pVal.Row);
                if (!quantity.Equals(string.Empty))
                    quantityValue = Convert.ToDecimal(quantity);

                var whsInfo = B1Helper.GetWhsItemInfo(itemCode, whsCode);
                if (quantityValue + GetSelectedItemQuantity(itemCode, whsCode, pVal.Row) > whsInfo.AvailableValue)
                {
                    Application.SBO_Application.MessageBox("The quantity in stock is not enough.");
                    BubbleEvent = false;
                }
                else
                    BubbleEvent = true;
            }
            else
                BubbleEvent = true;
        }

        private void btnSave_PressedBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            try
            {
                if (this.UIAPIRawForm.Mode == SAPbouiCOM.BoFormMode.fm_ADD_MODE)
                {
                    mtxItems.FlushToDataSource();
                    CalculateCostPercentage();

                    mtxItems.GetValuedRows();
                    mtxCost.GetValuedRows();
                    bool isValid = true;

                    if (this.UIAPIRawForm.Mode.Equals(SAPbouiCOM.BoFormMode.fm_ADD_MODE))
                    {
                        if (mtxItems.RowCount == 0 || mtxCost.RowCount == 0)
                        {
                            isValid = false;
                            Application.SBO_Application.MessageBox("Please add a new row.");
                        }
                        else
                            if (!mtxCost.IsMatrixCellsNotEmpty("Type", "Cost", "Code") || !mtxItems.IsMatrixCellsNotEmpty("to_Item", "ToWhs", "rec_Item", "Quantity", "FromWhs", "AvgPrice", "AddAmount"))
                            {
                                isValid = false;
                                Application.SBO_Application.MessageBox("Please fill in all the row cells.");
                            }
                            else
                            {
                                ManageBatches(out isValid);
                            }

                        BubbleEvent = isValid;
                    }
                    else
                    {
                        BubbleEvent = true;
                        IsMandatoryBatches = false;
                    }
                }
                else
                    BubbleEvent = true;
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                BubbleEvent = false;
            }
        }

        private void mtxItems_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            RowToDeleteIndex = pVal.Row;
            mtxItems.FlushToDataSource();

        }

        private void mtxItems_ChooseFromListAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                mtxItems.FlushToDataSource();
                var colUid = pVal.ColUID;
                var row = pVal.Row;

                var selectedCFLItems = base.GetCFLSelectedItem(pVal);               

                if (selectedCFLItems != null)
                {
                    if (pVal.ColUID.Equals("to_Item"))
                    {
                        var selectedCFLItemCode = selectedCFLItems.GetValue("ItemCode", 0).ToString();
                        var selectedCFLItemName = selectedCFLItems.GetValue("ItemName", 0).ToString();

                        if (!selectedCFLItemCode.Equals(string.Empty))
                        {
                            var avgPrice = selectedCFLItems.GetValue("AvgPrice", 0);
                            mtxItems.SetCellWithoutValidation(row, "FromWhs", string.Empty);
                            mtxItems.SetCellWithoutValidation(row, "Quantity", string.Empty);
                            mtxItems.SetCellWithoutValidation(row, "IssueDesc", selectedCFLItemName);
                            mtxItems.SetCellValue("AvgPrice", row, avgPrice);
                            mtxItems.FlushToDataSource();
                        }
                    }
                    else if (pVal.ColUID.Equals("IssueDesc"))
                    {
                        var selectedCFLItemCode = selectedCFLItems.GetValue("ItemCode", 0).ToString();

                        if (!selectedCFLItemCode.Equals(string.Empty))
                        {
                            var avgPrice = selectedCFLItems.GetValue("AvgPrice", 0);
                            mtxItems.SetCellWithoutValidation(row, "FromWhs", string.Empty);
                            mtxItems.SetCellWithoutValidation(row, "Quantity", string.Empty);
                            mtxItems.SetCellWithoutValidation(row, "to_Item", selectedCFLItemCode);
                            mtxItems.SetCellValue("AvgPrice", row, avgPrice);
                            mtxItems.FlushToDataSource();
                        }
                    }
                    else if (pVal.ColUID.Equals("RecDesc"))
                    {
                        var selectedCFLItemCode = selectedCFLItems.GetValue("ItemCode", 0).ToString();
                        if (!selectedCFLItemCode.Equals(string.Empty))
                        {
                            mtxItems.SetCellWithoutValidation(row, "rec_Item", selectedCFLItemCode);
                            mtxItems.FlushToDataSource();
                        }
                    }
                    else if (pVal.ColUID.Equals("rec_Item"))
                    {
                        var selectedCFLItemCode = selectedCFLItems.GetValue("ItemCode", 0).ToString();
                        var selectedCFLItemName = selectedCFLItems.GetValue("ItemName", 0).ToString();
                        if (!selectedCFLItemCode.Equals(string.Empty))
                        {
                            mtxItems.SetCellWithoutValidation(row, "RecDesc", selectedCFLItemName);
                            mtxItems.FlushToDataSource();
                        }
                    }
                }
                //this.UIAPIRawForm.Select();
                //mtxItems.SetCellFocus(pVal.Row, mtxItems.GetColumnIndex(pVal.ColUID));
                currentControl = mtxItems.GetCellSpecific(colUid, row) as SAPbouiCOM.EditText;
                //currentControl.ActivateControl();
            }
            catch (Exception ex)
            {

            }
        }

        private void mtxItems_ChooseFromListBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            try
            {
                if (pVal.ColUID.Equals("FromWhs"))
                {
                    SAPbouiCOM.ChooseFromList cfl = this.UIAPIRawForm.ChooseFromLists.Item("cflWH") as SAPbouiCOM.ChooseFromList;
                    var itemCode = mtxItems.GetCellValue("to_Item", pVal.Row).ToString();
                    var itmWareHouses = B1Helper.GetWhsInfoPerItem(itemCode);

                    var oConditions = Application.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_Conditions) as SAPbouiCOM.Conditions;

                    for (int i = 0; i < itmWareHouses.Count; i++)
                    {
                        var oCondition = oConditions.Add();

                        oCondition.Alias = "WhsCode";
                        oCondition.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                        oCondition.CondVal = itmWareHouses[i].WhsCode;

                        if (i + 1 != itmWareHouses.Count)
                            oCondition.Relationship = SAPbouiCOM.BoConditionRelationship.cr_OR;
                    }
                    if (itmWareHouses.Count == 0)
                    {
                        var oCondition = oConditions.Add();
                        oCondition.Alias = "WhsCode";
                        oCondition.CondVal = string.Empty;
                    }


                    cfl.SetConditions(oConditions);
                }

                BubbleEvent = true;
                mtxItems.FlushToDataSource();
            }
            catch(Exception ex)
            {
                Utilities.LogException(ex);
                BubbleEvent = false;
            }
        }

        private void mtxCost_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            mtxCost.FlushToDataSource();
        }

        private void mtxCost_ChooseFromListAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                var selectedItems = base.GetCFLSelectedItem(pVal);
                if (pVal.ColUID.Equals("Code"))
                {
                    var selectedValue = selectedItems.GetValue("U_Type", 0).ToString();
                    if (!selectedValue.Equals(string.Empty))
                    {
                        mtxCost.SetCellValue("Type", pVal.Row, selectedValue);
                        mtxCost.FlushToDataSource();
                    }
                }
                //SELECT THE CURRENT CONTROL IN ORDER TO ENABLE ON FORM ACTIVATION
                currentControl = mtxCost.GetCellSpecific(pVal.ColUID, pVal.Row) as SAPbouiCOM.EditText;
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
            }

        }

        private void btnAddCost_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            Dictionary<object, object> argsDictionary = new Dictionary<object, object>();
        }

        //private void btnCalculate_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        //{
        //    BubbleEvent = true;
        //    mtxItems.FlushToDataSource();
        //}

        private void btnCalculate_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            //mtxItems.FlushToDataSource();
            //CalculateCostPercentage();
        }

        private void btnSave_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            //try
            //{
            //    if (this.UIAPIRawForm.Mode == SAPbouiCOM.BoFormMode.fm_ADD_MODE)
            //    {
            //        mtxItems.FlushToDataSource();
            //        CalculateCostPercentage();

            //        mtxItems.GetValuedRows();
            //        mtxCost.GetValuedRows();
            //        bool isValid = true;

            //        if (this.UIAPIRawForm.Mode.Equals(SAPbouiCOM.BoFormMode.fm_ADD_MODE))
            //        {
            //            if (mtxItems.RowCount == 0 || mtxCost.RowCount == 0)
            //            {
            //                isValid = false;
            //                Application.SBO_Application.MessageBox("Please add a new row.");
            //            }
            //            else
            //                if (!mtxCost.IsMatrixCellsNotEmpty("Type", "Cost", "Code") || !mtxItems.IsMatrixCellsNotEmpty("to_Item", "ToWhs", "rec_Item", "Quantity", "FromWhs", "AvgPrice", "AddAmount"))
            //                {
            //                    isValid = false;
            //                    Application.SBO_Application.MessageBox("Please fill in all the row cells.");
            //                }
            //                else
            //                {
            //                    ManageBatches(out isValid);
            //                }

            //            BubbleEvent = isValid;
            //        }
            //        else
            //        {
            //            BubbleEvent = true;
            //            BatchesManaged = false;
            //        }
            //    }
            //    else
            //        BubbleEvent = true;
            //}
            //catch (Exception ex)
            //{
            //    Utilities.LogException(ex);
            //    BubbleEvent = false;
            //}
        }

        public void btnAddRow_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (fldrItems.Selected == true)
            {
                if (mtxItems.IsMatrixCellsNotEmpty("to_Item", "ToWhs", "rec_Item", "Quantity", "FromWhs", "AvgPrice"))
                    mtxItems.AddLine();
                else
                    Application.SBO_Application.MessageBox("Please fill in all the row cells.");
            }
            else
            {
                if (mtxCost.IsMatrixCellsNotEmpty("Code", "Type", "Cost"))
                    mtxCost.AddLine();
                else
                    Application.SBO_Application.MessageBox("Please fill in all the row cells.");
            }
        }

        private void btnAddRow_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            mtxCost.FlushToDataSource();
            mtxItems.FlushToDataSource();
        }

        #endregion

        #endregion

        #region PRIVATE METHODS
        private async void CalculateCostPercentage()
        {
            try
            {
                Action action = new Action(delegate()
                {
                    double totalWeight = 0;
                    double totalAmount = 0;
                    double totalQuantity = 0;

                    for (int i = 0; i < mtxCost.RowCount; i++)
                    {
                        var itmType = ((dynamic)mtxCost.GetCellSpecific("Type", i + 1)).Value;
                        var itmCost = ((dynamic)mtxCost.GetCellSpecific("Cost", i + 1)).Value;

                        var itmDoubleCost = itmCost == string.Empty ? 0 : Convert.ToDouble(itmCost);
                        if (itmType.Equals("Weight"))
                            totalWeight += itmDoubleCost;
                        else if (itmType.Equals("Amount"))
                            totalAmount += itmDoubleCost;
                        else
                            totalQuantity += itmDoubleCost;
                    }

                    SAPbobsCOM.Items oItem = AddonInfoInfo.GetNewBOneItem();
                    List<AddonInfoInfo> itemsList = new List<AddonInfoInfo>();

                    double totalItemsAmount = 0;
                    double totalItemsAvgPrice = 0;
                    double totalItemsWeightPrice = 0;
                    double totalItemQuantity = 0;

                    for (int i = 0; i < mtxItems.RowCount; i++)
                    {
                        var itmCode = ((dynamic)mtxItems.GetCellSpecific("to_Item", i + 1)).Value;
                        oItem.GetByKey(itmCode);
                        var oPriceList = oItem.PriceList;
                        var quantity = ((dynamic)mtxItems.GetCellSpecific("Quantity", i + 1)).Value;
                        var itmQuantity = Convert.ToDouble(quantity == string.Empty ? 0 : quantity);
                        var amount = oItem.MovingAveragePrice * Convert.ToDouble(itmQuantity);
                        var avgPrice = oItem.MovingAveragePrice;
                        var itemWeight = oItem.PurchaseUnitWeight;

                        totalItemsAmount += amount;
                        totalItemsAvgPrice += avgPrice;
                        totalItemsWeightPrice += itemWeight;
                        totalItemQuantity += itmQuantity;

                        var itemInfo = new AddonInfoInfo() { Index = i + 1, ItemWeight = itemWeight, AveragePrice = avgPrice, Amount = amount, Quantity = itmQuantity };
                        itemsList.Add(itemInfo);
                    }
                    var ds = this.UIAPIRawForm.DataSources.DBDataSources.Item(string.Format("@{0}", TableNames.TransferItemsLines));

                    foreach (var v in itemsList)
                    {
                        var itemWeightPercentage = totalItemsWeightPrice == 0 ? 0 : v.ItemWeight * totalWeight / totalItemsWeightPrice;
                        var itemAmountPercentage = totalItemsAmount == 0 ? 0 : v.Amount * totalAmount / totalItemsAmount;
                        var itemQuantityPercentage = v.Quantity * totalQuantity / totalItemQuantity;
                        ds.SetValue("U_AddCost", v.Index - 1, (itemWeightPercentage + itemAmountPercentage + itemQuantityPercentage).ToString());
                    }
                    mtxItems.LoadFromDataSource();
                });

                await Task.Run(action);
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
            }
        }

        private bool IsMatrixCellsNotEmpty(SAPbouiCOM.Matrix mtxControl, params string[] fields)
        {
            List<string> ValuesList = new List<string>();
            for (int i = 1; i <= mtxControl.RowCount; i++)
            {
                foreach (var v in fields)
                {
                    ValuesList.Add(mtxControl.GetCellValue(v, i).ToString());
                }
            }
            var emptyValues = ValuesList.Where(x => x.Equals(string.Empty) || x.Equals("0.0")).ToList();
            if (emptyValues.Count == 0)
                return true;
            else
                return false;
            //bool isValid = true;
            //for (int i = 1; i <= mtxItems.RowCount; i++)
            //{
            //    if (mtxItems.GetCellValue("to_Item", i) == string.Empty || mtxItems.GetCellValue("rec_Item", i) == string.Empty || mtxItems.GetCellValue("FromWhs", i) == string.Empty
            //        || mtxItems.GetCellValue("ToWhs", i) == string.Empty || mtxItems.GetCellValue("Quantity", i) == string.Empty || mtxItems.GetCellValue("AvgPrice", i) == string.Empty || mtxItems.GetCellValue("AddAmount", i) == string.Empty || mtxItems.GetCellValue("AddAmount", i) == "0.0")
            //    {
            //        isValid = false;
            //        break;
            //    }
            //}
            //return isValid;
        }

        private bool IsCostMatrixCellsValid()
        {
            bool isValid = true;
            for (int i = 1; i <= mtxCost.RowCount; i++)
            {
                if (mtxCost.GetCellValue("Code", i) as string == string.Empty || mtxCost.GetCellValue("Type", i) as string == string.Empty || mtxCost.GetCellValue("Cost", i) as string == string.Empty)
                {
                    isValid = false;
                    break;
                }
            }
            return isValid;
        }

        private decimal GetSelectedItemQuantity(string itemCode, string whsCode, int rowIndex)
        {
            decimal wholeQuantity = 0;
            try
            {
                for (int i = 1; i <= mtxItems.RowCount; i++)
                {
                    if (i != rowIndex)
                    {
                        if (mtxItems.GetCellValue("to_Item", i).ToString().Equals(itemCode) && mtxItems.GetCellValue("FromWhs", i).ToString().Equals(whsCode))
                        {
                            var stringQuantity = mtxItems.GetCellValue("Quantity", i).ToString();
                            if (stringQuantity.ToString() != string.Empty)
                            {
                                wholeQuantity += Convert.ToDecimal(stringQuantity);
                            }
                        }
                    }
                }
                return wholeQuantity;
            }
            catch(Exception ex)
            {
                Utilities.LogException(ex);
                return 0;
            }
           
        }

        private List<BatchRow> GetIssueSelectedItems(out bool isIssueMandatoryBatches)
        {
            var issueBatchRowsList = new List<BatchRow>();
            isIssueMandatoryBatches = false;
            for (int i = 1; i <= mtxItems.RowCount; i++)
            {
                var issItem = mtxItems.GetCellValue("to_Item", i).ToString();
                var batchItem = B1Helper.DiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems) as SAPbobsCOM.Items;
                batchItem.GetByKey(issItem);
                if (batchItem.ManageBatchNumbers == SAPbobsCOM.BoYesNoEnum.tYES)
                {
                    //BatchesManaged = ChechIfIssueBatchesManaged();
                    isIssueMandatoryBatches = true;

                    BatchRow bRow = new BatchRow()
                    {
                        ItemCode = mtxItems.GetCellValue("to_Item", i).ToString(),
                        WhsCode = mtxItems.GetCellValue("FromWhs", i).ToString(),
                        Quantity = Convert.ToDouble(mtxItems.GetCellValue("Quantity", i)),
                        AvgPrice = Convert.ToDouble(mtxItems.GetCellValue("AvgPrice", i)),
                        AddAmount = Convert.ToDouble(mtxItems.GetCellValue("AddAmount", i))
                    };
                    issueBatchRowsList.Add(bRow);
                }
                else
                {
                    TransferItem tItem = new TransferItem()
                    {
                        AddCost = Convert.ToDouble(mtxItems.GetCellValue("AddAmount", i)),
                        IssueItemCode = mtxItems.GetCellValue("to_Item", i).ToString(),
                        ReceiptItemCode = mtxItems.GetCellValue("rec_Item", i).ToString(),
                        Quantity = mtxItems.GetCellValue("Quantity", i).ToString(),
                        FromWhs = mtxItems.GetCellValue("FromWhs", i).ToString(),
                        ToWhs = mtxItems.GetCellValue("ToWhs", i).ToString(),
                        AvgCost = Convert.ToDouble(mtxItems.GetCellValue("AvgPrice", i))
                    };
                    issTransferItemsList.Add(tItem);
                }

            }
            if (SelectedBatchDataSources.Count != issueBatchRowsList.Count)
                BatchesManaged = null;
            return issueBatchRowsList;
        }

        private List<BatchRow> GetReceiptSelectedItems(out bool isReceiptMandatoryBatches)
        {
            var receiptBatchRowsList = new List<BatchRow>();
            isReceiptMandatoryBatches = false;
            for (int i = 1; i <= mtxItems.RowCount; i++)
            {
                var issItem = mtxItems.GetCellValue("rec_Item", i).ToString();
                var batchItem = B1Helper.DiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems) as SAPbobsCOM.Items;
                batchItem.GetByKey(issItem);
                if (batchItem.ManageBatchNumbers == SAPbobsCOM.BoYesNoEnum.tYES)
                {
                    //BatchesManaged = ChechIfReceiptBatchesManaged();
                    isReceiptMandatoryBatches = true;
                    var batchRows = receiptBatchRowsList.Where(x => x.ItemCode == mtxItems.GetCellValue("rec_Item", i).ToString() && x.WhsCode == mtxItems.GetCellValue("ToWhs", i).ToString());
                    if (batchRows.Count() > 0)
                    {
                        batchRows.First().Quantity += Convert.ToDouble(mtxItems.GetCellValue("Quantity", i));
                        batchRows.First().AddAmount += Convert.ToDouble(mtxItems.GetCellValue("AddAmount", i));
                    }
                    else
                    {
                        BatchRow bRow = new BatchRow()
                        {
                            ItemCode = mtxItems.GetCellValue("rec_Item", i).ToString(),
                            WhsCode = mtxItems.GetCellValue("ToWhs", i).ToString(),
                            Quantity = Convert.ToDouble(mtxItems.GetCellValue("Quantity", i)),
                            AvgPrice = Convert.ToDouble(mtxItems.GetCellValue("AvgPrice", i)),
                            AddAmount = Convert.ToDouble(mtxItems.GetCellValue("AddAmount", i))
                        };
                        receiptBatchRowsList.Add(bRow);
                    }
                }
                else
                {
                    TransferItem tItem = new TransferItem()
                    {
                        AddCost = Convert.ToDouble(mtxItems.GetCellValue("AddAmount", i)),
                        IssueItemCode = mtxItems.GetCellValue("to_Item", i).ToString(),
                        ReceiptItemCode = mtxItems.GetCellValue("rec_Item", i).ToString(),
                        Quantity = mtxItems.GetCellValue("Quantity", i).ToString(),
                        FromWhs = mtxItems.GetCellValue("FromWhs", i).ToString(),
                        ToWhs = mtxItems.GetCellValue("ToWhs", i).ToString(),
                        AvgCost = Convert.ToDouble(mtxItems.GetCellValue("AvgPrice", i))
                    };
                    recTransferItemsList.Add(tItem);
                }

            }
            if (receiptBatchRowsList.Count != SelectedRecBatchDataSources.Count)
                BatchesManaged = null;
            return receiptBatchRowsList;
        }

        private void ManageBatches(out bool BubbleEvent)
        {
            bool isIssueMandatorybatches;
            bool isReceiptMandatoryBatches;

            var issBatchRowsList = GetIssueSelectedItems(out isIssueMandatorybatches);
            var recBatchRowsList = GetReceiptSelectedItems(out isReceiptMandatoryBatches);

            var parameters = new Dictionary<object, object>();
            //if (BatchForm == null && (BatchesManaged == false || BatchesManaged == null) && (isIssueMandatorybatches || isReceiptMandatoryBatches))
            if (BatchForm == null && (BatchesManaged == null) && (isIssueMandatorybatches || isReceiptMandatoryBatches))
            {
                BatchForm = new BatchNumberSelection(this, issBatchRowsList, recBatchRowsList);
                parameters.Add("Instance", BatchForm);
                OnSubFormCreated(this, new FormArgs(parameters));
                ClearListsContents();
                BubbleEvent = false;
            }
            else
                //if (BatchForm != null && (BatchesManaged == false || BatchesManaged == null) && (isIssueMandatorybatches || isReceiptMandatoryBatches))
                if (BatchForm != null && (BatchesManaged == null) && (isIssueMandatorybatches || isReceiptMandatoryBatches))
                {
                    BatchForm.UIAPIRawForm.Select();
                    ClearListsContents();
                    BubbleEvent = false;
                }
                else
                {
                    BubbleEvent = true;
                }
        }

        private bool ChechIfReceiptBatchesManaged()
        {
            bool isManaged = true; ;
            if (SelectedBatchDataSources != null)
            {
                isManaged = true;
                for (int i = 0; i < SelectedRecBatchDataSources.Count; i++)
                {
                    if (SelectedRecBatchDataSources[i].TotalSelected != SelectedRecBatchDataSources[i].Quantity)
                        return false;
                }
            }
            return isManaged;
        }

        private bool ChechIfIssueBatchesManaged()
        {
            bool isManaged = true;
            if (SelectedRecBatchDataSources != null)
            {
                isManaged = true;
                for (int i = 0; i < SelectedRecBatchDataSources.Count; i++)
                {
                    if (SelectedBatchDataSources[i].TotalNeeded != 0)
                        return false;
                }
            }
            return isManaged;
        }

        private void ClearListsContents()
        {
            issTransferItemsList.Clear();
            recTransferItemsList.Clear();
            SelectedBatchDataSources.Clear();
            SelectedRecBatchDataSources.Clear();
        }

        #endregion
    }
}
