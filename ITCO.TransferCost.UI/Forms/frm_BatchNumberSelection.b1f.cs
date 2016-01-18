using ITCO.TransferCost.Main;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ITCO.TransferCost.UI.Forms
{
    [FormAttribute("frm_BtchSelect", "Forms/frm_BatchNumberSelection.b1f")]
    public class BatchNumberSelection : B1FormBase
    {
        #region PRIVATE VARIABLES

        private SAPbouiCOM.Grid gvDocuments;
        private SAPbouiCOM.Grid gvSelectedBatches;
        private SAPbouiCOM.Grid gvBatches;
        private SAPbouiCOM.Grid gvRecDocuments;
        private SAPbouiCOM.Grid gvRecBatches;

        private SAPbouiCOM.Button btnSave;
        private SAPbouiCOM.Button btnRemove;
        private SAPbouiCOM.Button btnSelect;

        private SAPbouiCOM.DataTable dtDocuments;
        private SAPbouiCOM.DataTable dtBatches;
        private SAPbouiCOM.DataTable dtRecDocuments;
        private SAPbouiCOM.DataTable dtRecBatches;
        private SAPbouiCOM.DataTable dtSelected;

        private SAPbouiCOM.Folder fldrIssueItems;
        private SAPbouiCOM.Folder fldrRecItems;

        private int currentDocumentIndex;
        private string currentItemCode = string.Empty;
        private string currentWhsCode = string.Empty;

        private int currentRecDocumentIndex;
        private string currentRecItemCode = string.Empty;
        private string currentRecWhsCode = string.Empty;

        private TransferItems transferItemsForm = null;
        private List<BatchDataSources> batchDataSources = new List<BatchDataSources>();
        private List<SelectedBatchDataSources> issueBatchDataSources = new List<SelectedBatchDataSources>();
        private List<SelectedBatchDataSources> receiptBatchDataSources = new List<SelectedBatchDataSources>();

        private const string Query = "DECLARE @Selected  INT "+
                                     "SELECT  [OITM].ItemCode,[OBTN].DistNumber ,@Selected AS 'Selected Qty', [OBTQ].Quantity, [OBTQ].WhsCode, [OITM].AvgPrice " +
                                     "FROM [OBTQ] INNER JOIN [OITM] ON [OITM].ItemCode = [OBTQ].ItemCode  "+
                                     "INNER JOIN  [OBTN] ON [OBTN].SysNumber=[OBTQ].SysNumber AND [OBTQ].ItemCode = [OBTN].ItemCode " +
                                     "WHERE [OBTQ].ItemCode='{0}' AND [OBTQ].WhsCode = '{1}'";
        #endregion

        #region PUBLIC PROPERTIES

        public override event UserFormClosedDone OnUserFormClosedDone;

        #endregion

        #region CONSTRUCTORS
        public BatchNumberSelection()
        {
            fldrIssueItems.Select();
        }

        public BatchNumberSelection(TransferItems transferItems, List<BatchRow> issBatchesList, List<BatchRow> recBatchesList)
        {
            fldrIssueItems.Select();
            gvBatches.AutoResizeColumns();
            gvRecDocuments.AutoResizeColumns();
            gvRecBatches.AutoResizeColumns();
            transferItemsForm = transferItems;

            PopulateDocumentsDatatable(issBatchesList, dtDocuments);
            PopulateDocumentsDatatable(recBatchesList, dtRecDocuments);

            PopulateDocumentsGrid(gvRecDocuments);
            PopulateDocumentsGrid(gvDocuments);
        }

        #endregion

        #region INITIALIZATION METHODS

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.dtBatches = this.UIAPIRawForm.DataSources.DataTables.Item("dtBatches");
            this.dtSelected = this.UIAPIRawForm.DataSources.DataTables.Item("dtSelected");
            this.dtDocuments = this.UIAPIRawForm.DataSources.DataTables.Item("dtDocumnts");
            this.dtRecDocuments = this.UIAPIRawForm.DataSources.DataTables.Item("dtRecDocs");
            this.dtRecBatches = this.UIAPIRawForm.DataSources.DataTables.Item("dtRBatches");
            this.gvBatches = ((SAPbouiCOM.Grid)(this.GetItem("Batches").Specific));
            this.btnRemove = ((SAPbouiCOM.Button)(this.GetItem("Remove").Specific));
            this.btnSelect = ((SAPbouiCOM.Button)(this.GetItem("Select").Specific));
            this.btnSave = ((SAPbouiCOM.Button)(this.GetItem("btnSave").Specific));
            this.btnSave.PressedBefore += new SAPbouiCOM._IButtonEvents_PressedBeforeEventHandler(this.btnSave_PressedBefore);
            this.btnSave.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnSave_ClickBefore);
            this.gvSelectedBatches = ((SAPbouiCOM.Grid)(this.GetItem("SlctdBtchs").Specific));
            this.gvDocuments = ((SAPbouiCOM.Grid)(this.GetItem("Documents").Specific));
            this.fldrIssueItems = ((SAPbouiCOM.Folder)(this.GetItem("IssBatches").Specific));
            this.fldrRecItems = ((SAPbouiCOM.Folder)(this.GetItem("recBatches").Specific));
            this.gvRecDocuments = ((SAPbouiCOM.Grid)(this.GetItem("RecDocs").Specific));
            this.gvRecDocuments.ClickAfter += new SAPbouiCOM._IGridEvents_ClickAfterEventHandler(this.gvRecDocuments_ClickAfter);
            this.gvRecBatches = ((SAPbouiCOM.Grid)(this.GetItem("RecBatches").Specific));
            this.gvRecBatches.ValidateAfter += new SAPbouiCOM._IGridEvents_ValidateAfterEventHandler(this.gvRecBatches_ValidateAfter);
            this.gvRecBatches.ValidateBefore += new SAPbouiCOM._IGridEvents_ValidateBeforeEventHandler(this.gvRecBatches_ValidateBefore);
            this.gvRecBatches.KeyDownAfter += new SAPbouiCOM._IGridEvents_KeyDownAfterEventHandler(this.gvRecBatches_KeyDownAfter);
            this.gvDocuments.ClickBefore += new SAPbouiCOM._IGridEvents_ClickBeforeEventHandler(this.gvDocuments_ClickBefore);
            this.gvDocuments.ValidateBefore += new SAPbouiCOM._IGridEvents_ValidateBeforeEventHandler(this.gvDocuments_ValidateBefore);
            this.gvDocuments.ClickAfter += new SAPbouiCOM._IGridEvents_ClickAfterEventHandler(this.gvDocuments_ClickAfter);
            this.btnSave.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.btnSave_ClickAfter);
            this.btnSelect.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.btnSelect_ClickAfter);
            this.btnSelect.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnSelect_ClickBefore);
            this.gvBatches.ClickAfter += new SAPbouiCOM._IGridEvents_ClickAfterEventHandler(this.gvBatches_ClickAfter);
            this.btnRemove.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.btnRemove_ClickAfter);
            this.btnRemove.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnRemove_ClickBefore);
            this.StaticText0 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_8").Specific));
            this.StaticText1 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_9").Specific));
            this.StaticText2 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_10").Specific));
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            this.CloseAfter += new SAPbouiCOM.Framework.FormBase.CloseAfterHandler(this.Form_CloseAfter);
            this.RightClickBefore += new SAPbouiCOM.Framework.FormBase.RightClickBeforeHandler(this.Form_RightClickBefore);
            this.RightClickAfter += new RightClickAfterHandler(this.Form_RightClickAfter);

        }

        private void OnCustomInitialize()
        {
            gvDocuments.AutoResizeColumns();
            gvSelectedBatches.AutoResizeColumns();
        }

        #endregion

        #region ISSUE ITEMS EVENT HANDLERS
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

                if (pVal.MenuUID == "DeleteLine")
                {

                    if (RowToDeleteIndex > 0)
                    {
                        dtRecBatches.Rows.Remove(RowToDeleteIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                Application.SBO_Application.SetStatusBarMessage(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Long, true);
            }
            this.UIAPIRawForm.Freeze(false);
        }

        private void btnSave_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                PopulateBatchTables();
                PopulateRecBatchTables();
                var isManaged = CheckBatchManagementValidity();
                if (isManaged == true)
                {
                    transferItemsForm.BatchForm = this;
                    transferItemsForm.SelectedBatchDataSources = issueBatchDataSources;
                    transferItemsForm.SelectedRecBatchDataSources = receiptBatchDataSources;
                    transferItemsForm.BatchesManaged = true;
                    this.UIAPIRawForm.Close();
                }
            }
            catch(Exception ex)
            {
                Utilities.LogException(ex);
            }
        }

        private void btnSave_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
        }

        private void btnRemove_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
        }

        private void btnSelect_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
        }

        private void gvBatches_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            gvBatches.Rows.IsSelected(pVal.Row);
            gvBatches.Rows.SelectedRows.Add(pVal.Row);
        }

        private void btnSelect_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                var quantityNeeded = Convert.ToDouble(dtDocuments.GetValue("TotalNeeded", currentDocumentIndex));
                //var selectedRows = gvBatches.Rows.SelectedRows.Cast<SAPbouiCOM.GridRows>().AsQueryable();

                for (int i = 0; i < dtBatches.Rows.Count; i++)
                {
                    var requestedBatchesQuantity = Convert.ToDouble(dtBatches.GetValue("Selected Qty", i));
                    var availableBatchesQuantity = Convert.ToDouble(dtBatches.GetValue("Quantity", i));

                    double selectedQuantity = 0;
                    if (gvBatches.Rows.IsSelected(i))
                    {
                        bool isAvailable = false;
                        for (int t = 0; t < dtSelected.Rows.Count; t++)
                        {
                            selectedQuantity = Convert.ToDouble(dtSelected.GetValue("Selected Qty", t));

                            if (dtSelected.GetValue("Batch", t).ToString().Equals(dtBatches.GetValue("DistNumber", i).ToString()))
                            {
                                if (availableBatchesQuantity >= requestedBatchesQuantity)
                                {
                                    if (requestedBatchesQuantity > 0)
                                    {
                                        if (requestedBatchesQuantity < quantityNeeded)
                                        {
                                            dtSelected.SetValue("Selected Qty", t, Convert.ToDouble(selectedQuantity) + Convert.ToDouble(requestedBatchesQuantity));
                                            dtBatches.SetValue("Quantity", i, availableBatchesQuantity - requestedBatchesQuantity);
                                            dtDocuments.SetValue("TotalNeeded", currentDocumentIndex, quantityNeeded - requestedBatchesQuantity);
                                        }
                                        else
                                        {
                                            dtSelected.SetValue("Selected Qty", t, quantityNeeded + selectedQuantity);
                                            dtBatches.SetValue("Quantity", i, availableBatchesQuantity - quantityNeeded);
                                            dtDocuments.SetValue("TotalNeeded", currentDocumentIndex, 0);
                                        }
                                    }
                                }
                                isAvailable = true;
                            }
                        }
                        if (!isAvailable)
                        {
                            if (requestedBatchesQuantity <= availableBatchesQuantity)
                            {
                                if (requestedBatchesQuantity > 0)
                                {
                                    dtSelected.Rows.Add(1);

                                    dtSelected.SetValue("Batch", dtSelected.Rows.Count - 1, dtBatches.GetValue("DistNumber", i));

                                    if (requestedBatchesQuantity < quantityNeeded)
                                    {
                                        dtSelected.SetValue("Selected Qty", dtSelected.Rows.Count - 1, dtBatches.GetValue("Selected Qty", i));
                                        dtBatches.SetValue("Quantity", i, availableBatchesQuantity - requestedBatchesQuantity);
                                        dtDocuments.SetValue("TotalNeeded", currentDocumentIndex, quantityNeeded - requestedBatchesQuantity);
                                    }
                                    else
                                    {
                                        dtSelected.SetValue("Selected Qty", dtSelected.Rows.Count - 1, quantityNeeded);
                                        dtBatches.SetValue("Quantity", i, availableBatchesQuantity - quantityNeeded);
                                        dtDocuments.SetValue("TotalNeeded", currentDocumentIndex, 0);
                                    }
                                }
                            }
                            else
                                Application.SBO_Application.MessageBox("The quantity selected is large.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void btnRemove_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                if (gvBatches.Rows.Count > 0)
                {
                    var selectedRows = gvSelectedBatches.Rows.SelectedRows.Cast<dynamic>();
                    int selectedRow = selectedRows.FirstOrDefault();
                    var selectedBatchName = dtSelected.GetValue("Batch", selectedRow);
                    var selectedQuantity = Convert.ToDouble(dtSelected.GetValue("Selected Qty", selectedRow));
                    var totalNeeded = Convert.ToDouble(dtDocuments.GetValue("TotalNeeded", currentDocumentIndex));

                    for (int i = 0; i < dtBatches.Rows.Count; i++)
                    {
                        if (dtBatches.GetValue("DistNumber", i).ToString() == dtSelected.GetValue("Batch", selectedRow).ToString())
                        {
                            var batchQuantity = Convert.ToDouble(dtBatches.GetValue("Quantity", i));
                            dtBatches.SetValue("Quantity", i, batchQuantity + selectedQuantity);
                            dtDocuments.SetValue("TotalNeeded", currentDocumentIndex, selectedQuantity + totalNeeded);
                            dtSelected.Rows.Remove(selectedRow);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void Form_CloseAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            transferItemsForm.BatchForm = null;
            OnUserFormClosedDone(this);
        }

        private void gvDocuments_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            gvDocuments.Rows.IsSelected(pVal.Row);
            if (currentItemCode != string.Empty)
            {
                PopulateBatchTables();
            }

            if (pVal.Row >= 0)
            {
                string itemCode = dtDocuments.GetValue("ItemCode", pVal.Row).ToString();
                string whsCode = dtDocuments.GetValue("WhsCode", pVal.Row).ToString();
                double avgPrice = Convert.ToDouble(dtDocuments.GetValue("AvgPrice", pVal.Row));
                double addAmount = Convert.ToDouble(dtDocuments.GetValue("AddAmount", pVal.Row));
                double quantity = Convert.ToDouble(dtDocuments.GetValue("Quantity", pVal.Row));

                currentDocumentIndex = pVal.Row;
                currentItemCode = itemCode;
                currentWhsCode = whsCode;

                //CHECK IF THE CURRENT SELECTED ITEM HAS BATCHES TABLE POPULATED BEFORE
                var availableBatches = batchDataSources.Where(x => x.ItemId == currentItemCode && x.WhsCode == currentWhsCode);
                if (availableBatches.Count() > 0)
                {
                    dtBatches.Rows.Clear();
                    var tempBatches = availableBatches.First().AvailableBatches;

                    this.UIAPIRawForm.Freeze(true);
                    for (int x = 0; x < tempBatches.Rows.Count; x++)
                    {
                        dtBatches.Rows.Add(1);
                        dtBatches.SetValue("ItemCode", x, tempBatches.Rows[x]["ItemCode"]);
                        dtBatches.SetValue("DistNumber", x, tempBatches.Rows[x]["DistNumber"]);
                        dtBatches.SetValue("Quantity", x, tempBatches.Rows[x]["Quantity"]);
                        dtBatches.SetValue("WhsCode", x, tempBatches.Rows[x]["WhsCode"]);
                        dtBatches.SetValue("AvgPrice", x, tempBatches.Rows[x]["AvgPrice"]);
                        dtBatches.SetValue("Selected Qty", x, tempBatches.Rows[x]["Selected Qty"]);
                    }
                    this.UIAPIRawForm.Freeze(false);
                }
                else
                {
                    PopulateBatchesGrid(pVal.Row);
                    dtSelected.Rows.Clear();
                    System.Data.DataTable dt = CopyBatchesDataTable(dtBatches);
                    var bds = new BatchDataSources() { ItemId = currentItemCode, WhsCode = currentWhsCode, AvailableBatches = CopyBatchesDataTable(dtBatches) };
                    batchDataSources.Add(bds);
                }

                //CHECK IF THE SELECTED BATCHES TABLE HAS BEEN POPULATED BEFORE
                var selectedBatches = issueBatchDataSources.Where(x => x.ItemCode == currentItemCode && x.WhsCode == currentWhsCode && x.Index == currentDocumentIndex);
                if (selectedBatches.Count() > 0)
                {
                    dtSelected.Rows.Clear();
                    this.UIAPIRawForm.Freeze(true);
                    for (int x = 0; x < selectedBatches.First().SelectedBatches.Rows.Count; x++)
                    {
                        dtSelected.Rows.Add(1);
                        dtSelected.SetValue("Batch", x, selectedBatches.First().SelectedBatches.Rows[x]["Batch"]);
                        dtSelected.SetValue("Selected Qty", x, selectedBatches.First().SelectedBatches.Rows[x]["Selected Qty"]);

                    }
                    this.UIAPIRawForm.Freeze(false);
                }
                else
                {
                    dtSelected.Rows.Clear();
                    System.Data.DataTable dt = CopyBatchesDataTable(dtBatches);
                    var bds = new SelectedBatchDataSources()
                    {
                        AddAmount = addAmount,
                        AvgPrice = Convert.ToDouble(avgPrice),
                        Quantity = Convert.ToDouble(quantity),
                        Index = currentDocumentIndex,
                        ItemCode = currentItemCode,
                        WhsCode = currentWhsCode,
                        SelectedBatches = CopySelectedBatchesDataTable(dtSelected)
                    };
                    issueBatchDataSources.Add(bds);
                }
            }
        }

        private void gvDocuments_ValidateBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
        }

        private void gvDocuments_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
        }
        #endregion

        #region RECEIPT ITEMS EVENT HANDLERS

        private void gvRecDocuments_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            if (currentRecItemCode != string.Empty)
            {
                PopulateRecBatchTables();
            }
            if (pVal.Row >= 0)
            {
                string itemCode = dtRecDocuments.GetValue("ItemCode", pVal.Row).ToString();
                string whsCode = dtRecDocuments.GetValue("WhsCode", pVal.Row).ToString();
                double avgPrice = Convert.ToDouble(dtRecDocuments.GetValue("AvgPrice", pVal.Row));
                double addAmount = Convert.ToDouble(dtRecDocuments.GetValue("AddAmount", pVal.Row));
                double quantity = Convert.ToDouble(dtRecDocuments.GetValue("Quantity", pVal.Row));

                currentRecDocumentIndex = pVal.Row;
                currentRecItemCode = itemCode;
                currentRecWhsCode = whsCode;

                //CHECK IF THE SELECTED BATCHES TABLE HAS BEEN POPULATED BEFORE
                var selectedBatches = receiptBatchDataSources.Where(x => x.ItemCode == currentRecItemCode && x.WhsCode == currentRecWhsCode && x.Index == currentRecDocumentIndex);
                if (selectedBatches.Count() > 0)
                {
                    dtRecBatches.Rows.Clear();
                    this.UIAPIRawForm.Freeze(true);
                    for (int x = 0; x < selectedBatches.First().SelectedBatches.Rows.Count; x++)
                    {
                        dtRecBatches.Rows.Add(1);
                        dtRecBatches.SetValue("Batch", x, selectedBatches.First().SelectedBatches.Rows[x]["Batch"]);
                        dtRecBatches.SetValue("Selected Qty", x, selectedBatches.First().SelectedBatches.Rows[x]["Selected Qty"]);

                    }
                    this.UIAPIRawForm.Freeze(false);
                }
                else
                {
                    dtRecBatches.Rows.Clear();
                    //System.Data.DataTable dt = CopyBatchesDataTable(dtRecBatches);
                    var bds = new SelectedBatchDataSources()
                    {
                        AddAmount = addAmount,
                        AvgPrice = Convert.ToDouble(avgPrice),
                        Quantity = Convert.ToDouble(quantity),
                        Index = currentRecDocumentIndex,
                        ItemCode = currentRecItemCode,
                        WhsCode = currentRecWhsCode,
                        SelectedBatches = CopySelectedBatchesDataTable(dtRecBatches)
                    };
                    receiptBatchDataSources.Add(bds);
                }
            }
            gvRecBatches.AddLine();
        }

        private void gvRecBatches_ValidateBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            var bubbleEventResult = true;
            var enteredValue = dtRecBatches.GetValue(pVal.ColUID, pVal.Row).ToString();
            var totalQuantityNeeded = Convert.ToDouble(dtRecDocuments.GetValue("TotalNeeded", currentRecDocumentIndex));
            double totalSelectedValue = 0;
            if (pVal.ColUID.Equals("Selected Qty"))
            {
                if (enteredValue == string.Empty || !Utilities.IsNumber(enteredValue))
                {
                    Application.SBO_Application.MessageBox("Only numbers allowed");
                    bubbleEventResult = false;
                }
                else
                {
                    for (int i = 0; i < dtRecBatches.Rows.Count; i++)
                    {
                        var selectedQuantity = (string)dtRecBatches.GetValue("Selected Qty", i) == string.Empty ? 0 : Convert.ToDouble(dtRecBatches.GetValue("Selected Qty", i));

                        totalSelectedValue += selectedQuantity;
                    }
                    if (totalSelectedValue > totalQuantityNeeded)
                    {
                        bubbleEventResult = false;
                        Application.SBO_Application.MessageBox("Quantity is over needed.");
                    }
                    else
                    {
                        dtRecDocuments.SetValue("TotalSelected", currentRecDocumentIndex, totalSelectedValue);
                    }
                }
            }
            else
            {
                bubbleEventResult = true;
            }
            BubbleEvent = bubbleEventResult;
        }

        private void gvRecBatches_ValidateAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            var enteredValue = dtRecBatches.GetValue(pVal.ColUID, pVal.Row).ToString();
            if (enteredValue != string.Empty)
            {
                //gvRecBatches.SetCellFocus(pVal.Row, 1);
            }
            else
            {

            }

        }

        #endregion

        #region FORM EVENT HANDLERS

        private void Form_RightClickBefore(ref SAPbouiCOM.ContextMenuInfo eventInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;
            RowToDeleteIndex = eventInfo.Row;
            if (eventInfo.ItemUID == "RecBatches")
                AddDeleteMenu();
        }

        private void Form_RightClickAfter(ref SAPbouiCOM.ContextMenuInfo eventInfo)
        {
            if (eventInfo.ItemUID == "RecBatches")
                RemoveDeleteMenu();
        }

        private void gvRecBatches_KeyDownAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            if (dtRecBatches.Rows.Count == pVal.Row + 1)
            {
                gvRecBatches.AddLine();
            }
        }

        #endregion

        #region PRIVATE METHODS
        /// <summary>
        /// Create a new SAP Datatable and copy the contents from the table passed as parameter
        /// </summary>
        /// <param name="tempBatches">DataTable to copy from</param>
        /// <returns>Return a new SAP  DataTable</returns>
        private System.Data.DataTable CopyBatchesDataTable(SAPbouiCOM.DataTable tempBatches)
        {
            System.Data.DataTable newDataTable = new System.Data.DataTable();
            newDataTable.Columns.Add("ItemCode", typeof(string));
            newDataTable.Columns.Add("DistNumber", typeof(string));
            newDataTable.Columns.Add("Quantity", typeof(string));
            newDataTable.Columns.Add("WhsCode", typeof(string));
            newDataTable.Columns.Add("AvgPrice", typeof(double));
            newDataTable.Columns.Add("Selected Qty", typeof(string));

            for (int x = 0; x < tempBatches.Rows.Count; x++)
            {
                System.Data.DataRow dr = newDataTable.NewRow();

                dr["ItemCode"] = tempBatches.GetValue("ItemCode", x);
                dr["DistNumber"] = tempBatches.GetValue("DistNumber", x);
                dr["Quantity"] = tempBatches.GetValue("Quantity", x);
                dr["WhsCode"] = tempBatches.GetValue("WhsCode", x);
                dr["AvgPrice"] = tempBatches.GetValue("AvgPrice", x);
                dr["Selected Qty"] = tempBatches.GetValue("Selected Qty", x); ;
                newDataTable.Rows.Add(dr);
            }
            
            return newDataTable;
        }

        /// <summary>
        /// Create a new SAP Datatable and copy the contents from the table passed as parameter
        /// </summary>
        /// <param name="tempBatches">DataTable to copy from</param>
        /// <returns>Return a new SAP  DataTable</returns>
        private System.Data.DataTable CopySelectedBatchesDataTable(SAPbouiCOM.DataTable tempBatches)
        {
            System.Data.DataTable newDataTable = new System.Data.DataTable();
            newDataTable.Columns.Add("Batch", typeof(string));
            newDataTable.Columns.Add("Selected Qty", typeof(string));

            for (int x = 0; x < tempBatches.Rows.Count; x++)
            {
                System.Data.DataRow dr = newDataTable.NewRow();
                var batchValue = tempBatches.GetValue("Batch", x);
                var batchQuantity = tempBatches.GetValue("Selected Qty", x);
                if (batchValue.ToString() != string.Empty && batchQuantity.ToString() != string.Empty)
                {
                    dr["Batch"] = tempBatches.GetValue("Batch", x);
                    dr["Selected Qty"] = tempBatches.GetValue("Selected Qty", x);
                    newDataTable.Rows.Add(dr);
                }
            }
            return newDataTable;
        }

        private void PopulateBatchesGrid(int rowIndex)
        {
            string itemCode = dtDocuments.GetValue("ItemCode", rowIndex).ToString();
            string whsCode = dtDocuments.GetValue("WhsCode", rowIndex).ToString();

            var query = string.Format(Query, itemCode, whsCode);
            dtBatches.ExecuteQuery(query);

            gvBatches.SetGridHeaderIndex();

            SAPbouiCOM.EditTextColumn gColumn = gvBatches.Columns.Item("ItemCode") as SAPbouiCOM.EditTextColumn;
            gColumn.LinkedObjectType = "4";

            gColumn = gvBatches.Columns.Item("WhsCode") as SAPbouiCOM.EditTextColumn;
            gColumn.LinkedObjectType = "64";
        }

        private void PopulateDocumentsGrid(SAPbouiCOM.Grid control)
        {
            control.SetGridHeaderIndex();

            SAPbouiCOM.EditTextColumn gColumn = control.Columns.Item("ItemCode") as SAPbouiCOM.EditTextColumn;
            gColumn.LinkedObjectType = "4";

            gColumn = control.Columns.Item("WhsCode") as SAPbouiCOM.EditTextColumn;
            gColumn.LinkedObjectType = "64";
        }

        private int GetCellsSummations(SAPbouiCOM.DataTable dtControl, string ItemCode, string whsCode, string cellName)
        {
            int totalBatches = 0;
            for (int i = 0; i < dtControl.Rows.Count; i++)
            {
                if ((string)dtControl.GetValue("ItemCode", i) == ItemCode && (string)dtControl.GetValue("WhsCode", i) == whsCode)
                {
                    var quantity = Convert.ToInt32(dtControl.GetValue("Quantity", i));
                    totalBatches += quantity;
                }
            }
            return totalBatches;
        }

        /// <summary>
        /// THIS METHOD IS USED TO GET THE USER ENTERED DATA AND POPULATE THE RELATED BATCHES DATA SOURCES ON ITEM SELECTION CHANGED
        /// </summary>
        private void PopulateBatchTables()
        {
            if (dtDocuments.Rows.Count > 0)
            {
                var availableBatches = batchDataSources.Where(x => x.ItemId == currentItemCode && x.WhsCode == currentWhsCode);
                var selectedbatches = issueBatchDataSources.Where(x => x.ItemCode == currentItemCode && x.WhsCode == currentWhsCode && x.Index == currentDocumentIndex);

                if (availableBatches.Count() > 0)
                {
                    availableBatches.First().AvailableBatches = CopyBatchesDataTable(dtBatches);
                }
                else
                {
                    var bds = new BatchDataSources()
                    {
                        ItemId = currentItemCode,
                        WhsCode = currentWhsCode,
                        AvailableBatches = CopyBatchesDataTable(dtBatches)
                    };
                    batchDataSources.Add(bds);
                }
                if (selectedbatches.Count() > 0)
                {
                    selectedbatches.First().SelectedBatches = CopySelectedBatchesDataTable(dtSelected);
                }
                else
                {

                    double avgPrice = Convert.ToDouble(dtDocuments.GetValue("AvgPrice", currentDocumentIndex));
                    double addAmount = Convert.ToDouble(dtDocuments.GetValue("AddAmount", currentDocumentIndex));
                    double quantity = Convert.ToDouble(dtDocuments.GetValue("Quantity", currentDocumentIndex));
                    double totalNeeded = Convert.ToDouble(dtDocuments.GetValue("TotalNeeded", currentRecDocumentIndex));
                    var sbds = new SelectedBatchDataSources()
                    {
                        Quantity = Convert.ToDouble(quantity),
                        AddAmount = addAmount,
                        AvgPrice = Convert.ToDouble(avgPrice),
                        Index = currentDocumentIndex,
                        ItemCode = currentItemCode,
                        WhsCode = currentWhsCode,
                        SelectedBatches = CopySelectedBatchesDataTable(dtSelected),
                        TotalNeeded = totalNeeded
                    };
                    issueBatchDataSources.Add(sbds);
                }
            }
        }

        private void PopulateDocumentsDatatable(List<BatchRow> batchesList, SAPbouiCOM.DataTable control)
        {
            foreach (BatchRow br in batchesList)
            {
                control.Rows.Add(1);
                var rowIndex = control.Rows.Count - 1;
                control.SetValue("ItemCode", rowIndex, br.ItemCode);
                control.SetValue("WhsCode", rowIndex, br.WhsCode);
                control.SetValue("Quantity", rowIndex, br.Quantity);
                control.SetValue("TotalNeeded", rowIndex, br.Quantity);
                control.SetValue("AvgPrice", rowIndex, br.AvgPrice);
                control.SetValue("AddAmount", rowIndex, br.AddAmount);
            }
        }

        private void PopulateRecBatchTables()
        {
            if (dtRecDocuments.Rows.Count > 0)
            {
                var selectedBatches = receiptBatchDataSources.Where(x => x.ItemCode == currentRecItemCode && x.WhsCode == currentRecWhsCode && x.Index == currentRecDocumentIndex);
                double totalSelected = Convert.ToDouble(dtRecDocuments.GetValue("TotalSelected", currentRecDocumentIndex));
                if (selectedBatches.Count() > 0)
                {
                    selectedBatches.First().SelectedBatches = CopySelectedBatchesDataTable(dtRecBatches);
                    selectedBatches.First().TotalSelected = totalSelected;
                }
                else
                {
                    double avgPrice = Convert.ToDouble(dtRecDocuments.GetValue("AvgPrice", currentRecDocumentIndex));
                    double addAmount = Convert.ToDouble(dtRecDocuments.GetValue("AddAmount", currentRecDocumentIndex));
                    double quantity = Convert.ToDouble(dtRecDocuments.GetValue("Quantity", currentRecDocumentIndex));
                    double totalNeeded = Convert.ToDouble(dtRecDocuments.GetValue("TotalNeeded", currentRecDocumentIndex));


                    var sbds = new SelectedBatchDataSources()
                    {
                        Quantity = Convert.ToDouble(quantity),
                        AddAmount = addAmount,
                        AvgPrice = avgPrice,
                        Index = currentRecDocumentIndex,
                        ItemCode = currentRecItemCode,
                        WhsCode = currentRecWhsCode,
                        SelectedBatches = CopySelectedBatchesDataTable(dtRecBatches),
                        TotalNeeded = totalNeeded,
                        TotalSelected = totalNeeded
                    };
                    receiptBatchDataSources.Add(sbds);
                }
            }
        }

        private bool CheckBatchManagementValidity()
        {
            var _isBatchManaged = true;
            for (int i = 0; i < receiptBatchDataSources.Count(); i++)
            {
                if (receiptBatchDataSources[i].SelectedBatches.Rows.Count <= 0 || receiptBatchDataSources[i].TotalSelected != receiptBatchDataSources[i].Quantity)
                {
                    _isBatchManaged = false;
                    Application.SBO_Application.MessageBox("Receipt Items Batches are not managed well");
                    goto Finish;
                }
            } 
            for (int i = 0; i < issueBatchDataSources.Count(); i++)
            {
                if (issueBatchDataSources[i].SelectedBatches.Rows.Count <= 0 || issueBatchDataSources[i].TotalNeeded != 0)
                {
                    Application.SBO_Application.MessageBox("Issue Items Batches are not managed well");
                    _isBatchManaged = false;
                    goto Finish;
                }
            }
        Finish:

            return _isBatchManaged;
        }

        #endregion

        private SAPbouiCOM.StaticText StaticText0;
        private SAPbouiCOM.StaticText StaticText1;
        private SAPbouiCOM.StaticText StaticText2;

        private void btnSave_PressedBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
        }       
    }
}
