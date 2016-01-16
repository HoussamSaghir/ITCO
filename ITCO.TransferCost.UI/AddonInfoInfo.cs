using ITCO.TransferCost.Main;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace ITCO.TransferCost.UI
{
    public class AddonInfoInfo
    {
        public SAPbobsCOM.Items TransferItem { get; set; }
        public double Quantity { get; set; }
        public double ItemWeight { get; set; }
        public double Amount { get; set; }
        public double AveragePrice { get; set; }
        public int Index { get; set; }


        public AddonInfoInfo()
        {
        }

        public static void TransferItemsProcess(SAPbouiCOM.Application oApp, string referenceNumber, List<SelectedBatchDataSources> issueBatchDSList, List<SelectedBatchDataSources> receiptBatchDSList, List<TransferItem> issueItemsList, List<TransferItem> receiptItemsList)
        {
            try
            {
                B1Helper.DiCompany.StartTransaction();

                Task t1 = Task.Run(delegate()
                 {
                     foreach (var tItem in issueItemsList)
                     {
                         B1Helper.CreateTransferGoodsIssue(oApp, referenceNumber, tItem.IssueItemCode, Convert.ToDouble(tItem.Quantity), tItem.FromWhs);
                     }
                 });

                Task t2 = Task.Run(delegate()
                         {
                             foreach (var ds in issueBatchDSList)
                             {
                                 B1Helper.CreateTransferGoodsIssue(oApp, referenceNumber, ds.ItemCode, Convert.ToDouble(ds.Quantity), ds.WhsCode, ds.SelectedBatches);
                             }
                         });

                Task t3 = Task.Run(delegate()
                        {
                            foreach (var ds in receiptBatchDSList)
                            {
                                B1Helper.CreateTransferGoodsReceipt(oApp, referenceNumber, ds.ItemCode, Convert.ToDouble(ds.Quantity), ds.WhsCode, ds.AddAmount + ds.AvgPrice, ds.SelectedBatches);
                            }
                        });
                Task t4 = Task.Run(delegate()
               {
                   foreach (var receiptItem in receiptItemsList)
                   {
                       B1Helper.CreateTransferGoodsReceipt(oApp, referenceNumber, receiptItem.ReceiptItemCode, Convert.ToDouble(receiptItem.Quantity), receiptItem.ToWhs, receiptItem.AddCost + receiptItem.AvgCost);
                   }
               });

                Task.WaitAll(t1, t2, t3, t4);
                B1Helper.DiCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
            }
            catch (Exception ex)
            {
                var id = Convert.ToInt32(referenceNumber);
                Utilities.LogException(ex);
                B1Helper.DeleteRecord(id);
                B1Helper.DiCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
            }
        }

        public static SAPbobsCOM.GeneralData GetUserTransferItem(string udoCode, string primaryKeyName, string primaryKeyValue)
        {
            return B1Helper.GetUdoObject(udoCode, primaryKeyName, primaryKeyValue);
        }

        public static bool InstallUDOs()
        {
            try
            {
                B1Helper.DiCompany.StartTransaction();

                B1Helper.AddTable(TableNames.TransferItems, "Transfer Items", SAPbobsCOM.BoUTBTableType.bott_Document);
                B1Helper.AddTable(TableNames.TransferItemsLines, "Transfer Items Lines", SAPbobsCOM.BoUTBTableType.bott_DocumentLines);
                B1Helper.AddTable(TableNames.TransferItemsSetup, "Transfer Items Setup", SAPbobsCOM.BoUTBTableType.bott_Document);
                B1Helper.AddTable(TableNames.TransferAddCost, "Transfer Items Added Cost", SAPbobsCOM.BoUTBTableType.bott_DocumentLines);

                B1Helper.AddField(FieldNames.IssueItem, "Issue Item", TableNames.TransferItemsLines, SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoYesNoEnum.tYES, true);
                B1Helper.AddField(FieldNames.IssueDescription, "Issue Item Description", TableNames.TransferItemsLines, SAPbobsCOM.BoFieldTypes.db_Alpha, 100, SAPbobsCOM.BoYesNoEnum.tNO, true);
                B1Helper.AddField(FieldNames.ReceiptItem, "Receipt Item", TableNames.TransferItemsLines, SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoYesNoEnum.tYES, true);
                B1Helper.AddField(FieldNames.ReceiptDescription, "Receipt Item Description", TableNames.TransferItemsLines, SAPbobsCOM.BoFieldTypes.db_Alpha, 100, SAPbobsCOM.BoYesNoEnum.tNO, true);
                B1Helper.AddField(FieldNames.AvgPrice, "AvgPrice", TableNames.TransferItemsLines, SAPbobsCOM.BoFieldTypes.db_Float, SAPbobsCOM.BoYesNoEnum.tYES, SAPbobsCOM.BoFldSubTypes.st_Price, true);
                B1Helper.AddField(FieldNames.FromWareHouse, "From Whs", TableNames.TransferItemsLines, SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoYesNoEnum.tYES, true);
                B1Helper.AddField(FieldNames.ToWareHouse, "To Whs", TableNames.TransferItemsLines, SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoYesNoEnum.tYES, true);
                B1Helper.AddField(FieldNames.Quantity, "Quantity", TableNames.TransferItemsLines, SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoYesNoEnum.tYES, true);
                B1Helper.AddField(FieldNames.AdditionalCost, "Add. Cost", TableNames.TransferItemsLines, SAPbobsCOM.BoFieldTypes.db_Float, SAPbobsCOM.BoYesNoEnum.tYES, SAPbobsCOM.BoFldSubTypes.st_Price, true);

                //T_TRANSFERADDCOST FIELDS
                B1Helper.AddField(FieldNames.Cost, "Cost", TableNames.TransferAddCost, SAPbobsCOM.BoFieldTypes.db_Numeric, SAPbobsCOM.BoYesNoEnum.tYES, SAPbobsCOM.BoFldSubTypes.st_Price, true);
                B1Helper.AddField(FieldNames.UoMType, "UoM Type", TableNames.TransferAddCost, SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoYesNoEnum.tYES, true);
                B1Helper.AddField(FieldNames.Code, "Code", TableNames.TransferAddCost, SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoYesNoEnum.tYES, true);

                //T_ITMTRNSFRSTUP FIELDS
                B1Helper.AddField(FieldNames.Type, "Type", TableNames.TransferItemsSetup, SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoYesNoEnum.tYES, true);
                B1Helper.AddField(FieldNames.Code, "Code", TableNames.TransferItemsSetup, SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoYesNoEnum.tYES, true);
                B1Helper.AddField(FieldNames.Name, "Name", TableNames.TransferItemsSetup, SAPbobsCOM.BoFieldTypes.db_Alpha, SAPbobsCOM.BoYesNoEnum.tYES, true);

                B1Helper.DiCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                var firstUdo = B1Helper.CreateUdo("UDO_ItemsTransfer", "UDO_ItemsTransfer", TableNames.TransferItems, TableNames.TransferItemsLines, TableNames.TransferAddCost);
                var secondUdo = B1Helper.CreateUdo("UDO_ItmTrnsfrSetup", "UDO_ItmTrnsfrSetup", TableNames.TransferItemsSetup, new List<string> { string.Format("U_{0}", FieldNames.Code), string.Format("U_{0}", FieldNames.Name) });
                if (firstUdo && secondUdo)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
                B1Helper.DiCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                return false;
            }
        }

        public static string GetNextEntryIndex(string tableName)
        {
            try
            {
                var result = B1Helper.GetNextEntryIndex(tableName);
                if (result.Equals(string.Empty))
                    result = "0";
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static SAPbobsCOM.Items GetNewBOneItem()
        {
            return B1Helper.DiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems) as SAPbobsCOM.Items;
        }

        #region STATIC METHODS

        public static void SetFormFilter()
        {
            try
            {
                //SAPbouiCOM.EventFilters objFilters = new SAPbouiCOM.EventFilters();
                //SAPbouiCOM.EventFilter objFilter;

                //objFilter = objFilters.Add(SAPbouiCOM.BoEventTypes.et_FORM_LOAD);
                //objFilter.AddEx("frm_TransferItems");


                //objFilter = objFilters.Add(SAPbouiCOM.BoEventTypes.et_FORM_CLOSE);
                //objFilter.AddEx("frm_TransferItems");



                //objFilter = objFilters.Add(SAPbouiCOM.BoEventTypes.et_MENU_CLICK);
                //objFilter.AddEx("frm_TransferItems");


                //objFilter = objFilters.Add(SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED);
                //objFilter.AddEx("frm_TransferItems");



                //objFilter = objFilters.Add(SAPbouiCOM.BoEventTypes.et_COMBO_SELECT);
                //objFilter.AddEx("frm_TransferItems");

                //objFilter = objFilters.Add(SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST);
                //objFilter.AddEx("frm_TransferItems");


                //objFilter = objFilters.Add(SAPbouiCOM.BoEventTypes.et_KEY_DOWN);
                //objFilter.AddEx("frm_TransferItems");


                //objFilter = objFilters.Add(SAPbouiCOM.BoEventTypes.et_LOST_FOCUS);
                //objFilter.AddEx("frm_TransferItems");


                //objFilter = objFilters.Add(SAPbouiCOM.BoEventTypes.et_VALIDATE);
                //objFilter.AddEx("frm_TransferItems");


                //objFilter = objFilters.Add(SAPbouiCOM.BoEventTypes.et_FORM_DATA_LOAD);
                //objFilter.AddEx("frm_TransferItems");



                //objFilter = objFilters.Add(SAPbouiCOM.BoEventTypes.et_CLICK);
                //objFilter.AddEx("frm_TransferItems");


                //objFilter = objFilters.Add(SAPbouiCOM.BoEventTypes.et_RIGHT_CLICK);
                //objFilter.AddEx("frm_TransferItems");


                //objFilter = objFilters.Add(SAPbouiCOM.BoEventTypes.et_DOUBLE_CLICK);
                //objFilter.AddEx("frm_TransferItems");

                //objFilter = objFilters.Add(SAPbouiCOM.BoEventTypes.et_PICKER_CLICKED);
                //objFilter.AddEx("frm_TransferItems");


                //SetFilter(objFilters);
            }
            catch (Exception ex)
            {

            }
        }

        public static void RemoveMenu(string menuId)
        {
            Application.SBO_Application.Menus.RemoveEx(menuId);
        }

        protected static void SetFilter(SAPbouiCOM.EventFilters Filters)
        {
            Application.SBO_Application.SetFilter(Filters);
        }


        #endregion
    }
}
