using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITCO.TransferCost.Main
{
    public class B1Helper
    {
        private static bool returnValue;

        private static SAPbobsCOM.Company diCompany = null;
        public static SAPbobsCOM.Company DiCompany
        {
            get
            {
                if (diCompany == null)
                {
                    diCompany = Application.SBO_Application.Company.GetDICompany() as SAPbobsCOM.Company;
                    diCompany.Connect();
                    return diCompany;
                }
                else
                {
                    return diCompany;
                }
            }
        }

        #region UDO METHODS
        public static bool CreateUdo(string udoName, string description, string tableName, SAPbobsCOM.BoUDOObjType nObjectType, List<string> findFields, params string[] childTables)
        {
            var oUserObjectMD = B1Helper.DiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD) as SAPbobsCOM.UserObjectsMD;
            try
            {
                if (!oUserObjectMD.GetByKey(udoName))
                {
                    oUserObjectMD.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES;
                    oUserObjectMD.CanClose = SAPbobsCOM.BoYesNoEnum.tYES;
                    oUserObjectMD.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO;
                    oUserObjectMD.CanDelete = SAPbobsCOM.BoYesNoEnum.tYES;
                    oUserObjectMD.CanFind = SAPbobsCOM.BoYesNoEnum.tYES;
                    oUserObjectMD.FindColumns.ColumnAlias = "DocEntry";

                    if (findFields != null)
                    {
                        for (int i = 0; i < findFields.Count; i++)
                        {
                            if (i + 1 != findFields.Count)
                                oUserObjectMD.FindColumns.Add();

                            oUserObjectMD.FindColumns.SetCurrentLine(i);
                            oUserObjectMD.FindColumns.ColumnAlias = findFields[i];
                            
                        }
                    }

                    oUserObjectMD.CanLog = SAPbobsCOM.BoYesNoEnum.tNO;
                    oUserObjectMD.LogTableName = string.Empty;
                    oUserObjectMD.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO;
                    oUserObjectMD.ExtensionName = string.Empty;

                    if (childTables != null)
                    {
                        for (int i = 0; i < childTables.Length; i++)
                        {
                            //oUserObjectMD.ChildTables.TableName = childTables[i];
                            if (i + 1 != childTables.Length)
                                oUserObjectMD.ChildTables.Add();

                            oUserObjectMD.ChildTables.SetCurrentLine(i);
                            oUserObjectMD.ChildTables.TableName = childTables[i];

                        }
                    }

                    oUserObjectMD.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO;
                    oUserObjectMD.Code = udoName;
                    oUserObjectMD.Name = description;
                    oUserObjectMD.ObjectType = nObjectType;
                    oUserObjectMD.TableName = tableName;

                    if (oUserObjectMD.Add() != 0)
                    {
                        var err = Utilities.GetErrorMessage();
                        returnValue = false;
                    }
                    else
                        returnValue = true;

                }
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
            }
            finally
            {
                oUserObjectMD.ReleaseObject();
            }

            return returnValue;
        }

        public static bool CreateUdo(string udoName, string description, string tableName, List<string> findFields, params string[] childTables)
        {
            return CreateUdo(udoName, description, tableName, SAPbobsCOM.BoUDOObjType.boud_Document, findFields, childTables);
        }

        public static bool CreateUdo(string udoName, string description, string tableName, params string[] childTables)
        {
            return CreateUdo(udoName, description, tableName, null, childTables);
        }

        public static SAPbobsCOM.GeneralData GetUdoObject(string udoName, string keyName, string keyValue)
        {
            SAPbobsCOM.GeneralService oClassSubjectsGeneralService;
            SAPbobsCOM.GeneralData oClassSubjectsHeaderGeneralData;
            SAPbobsCOM.GeneralDataParams oClassSubjectGeneralCollectionParams;
            try
            {
                DiCompany.StartTransaction();

                SAPbobsCOM.UserObjectsMD UserObjectMD = (SAPbobsCOM.UserObjectsMD)DiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD);

                oClassSubjectsGeneralService = (SAPbobsCOM.GeneralService)DiCompany.GetCompanyService().GetGeneralService(udoName);
                oClassSubjectsHeaderGeneralData = (SAPbobsCOM.GeneralData)oClassSubjectsGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData);

                oClassSubjectGeneralCollectionParams = (SAPbobsCOM.GeneralDataParams)oClassSubjectsGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams);
                oClassSubjectGeneralCollectionParams.SetProperty(keyName, keyValue);

                oClassSubjectsHeaderGeneralData = oClassSubjectsGeneralService.GetByParams(oClassSubjectGeneralCollectionParams);

                if (DiCompany.InTransaction)
                {
                    DiCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                }
                return oClassSubjectsHeaderGeneralData;
            }
            catch (Exception Ex)
            {
                Application.SBO_Application.SetStatusBarMessage(Ex.Message, SAPbouiCOM.BoMessageTime.bmt_Medium, true);
                return null;
            }
        }

        #endregion

        #region Table Methods
        public static void AddTable(string tableName, string description, SAPbobsCOM.BoUTBTableType tableType)
        {
            var oUserTablesMD = B1Helper.DiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables) as SAPbobsCOM.UserTablesMD;
            try
            {
                if (!oUserTablesMD.GetByKey(tableName))
                {
                    oUserTablesMD.TableName = tableName;
                    oUserTablesMD.TableDescription = description;
                    oUserTablesMD.TableType = tableType;
                    if (oUserTablesMD.Add() != 0)
                    {
                        var error = B1Helper.DiCompany.GetLastErrorDescription();
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
            }
            finally
            {
                oUserTablesMD.ReleaseObject();
            }
        }

        #endregion

        #region FIELD METHODS

        /// <summary>
        /// A method for adding new field to B1 table
        /// </summary>
        /// <param name="name">Field Name</param>
        /// <param name="description">Field description</param>
        /// <param name="tableName">Table the field will be added to</param>
        /// <param name="fieldType">Field Type</param>
        /// <param name="size">Field size in the database</param>
        /// <param name="subType"></param>
        /// <param name="mandatory"></param>
        /// <param name="addedToUDT">If this field will be added to system table or User defined table</param>
        /// <param name="valiedValue">The default selected value</param>
        /// <param name="validValues">Add the values seperated by comma "," for value and description ex:(Value,Description)</param>
        public static void AddField(string name, string description, string tableName, SAPbobsCOM.BoFieldTypes fieldType, Nullable<int> size, SAPbobsCOM.BoYesNoEnum mandatory, SAPbobsCOM.BoFldSubTypes subType, bool addedToUDT, string validValue, params string[] validValues)
        {
           
            var objUserFieldMD = B1Helper.diCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields) as SAPbobsCOM.UserFieldsMD;
            try
            {
                if (addedToUDT)
                    tableName = string.Format("@{0}", tableName);
                if (!IsFieldExists(name, tableName))
                {
                    objUserFieldMD.TableName = tableName;
                    objUserFieldMD.Name = name;
                    objUserFieldMD.Description = description;
                    objUserFieldMD.Type = fieldType;
                    objUserFieldMD.Mandatory = mandatory;

                    if (size == null || size <= 0)
                        size = 10;

                    if (fieldType != SAPbobsCOM.BoFieldTypes.db_Numeric)
                        objUserFieldMD.Size = (int)size;
                    else
                        objUserFieldMD.EditSize = (int)size;


                    objUserFieldMD.SubType = subType;

                    if (validValue != null)
                        objUserFieldMD.DefaultValue = validValue;

                    if (validValues != null)
                    {
                        foreach (string s in validValues)
                        {
                            var valuesAttributes = s.Split(',');
                            if (valuesAttributes.Length == 2)
                                objUserFieldMD.ValidValues.Description = valuesAttributes[1];
                            objUserFieldMD.ValidValues.Value = valuesAttributes[0];
                            objUserFieldMD.ValidValues.Add();
                        }
                    }

                    if (objUserFieldMD.Add() != 0)
                    {
                        var error = Utilities.GetErrorMessage();
                        Utilities.LogException(error);
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
            }
            finally
            {
                objUserFieldMD.ReleaseObject();
            }
        }

        /// <summary>
        /// A method for adding new field to B1 table
        /// </summary>
        /// <param name="name">Field Name</param>
        /// <param name="description">Field description</param>
        /// <param name="tableName">Table the field will be added to</param>
        /// <param name="fieldType">Field Type</param>
        /// <param name="size">Field size in the database</param>       
        /// <param name="mandatory">bool: if the value is mandatory to be filled</param>
        /// <param name="subType"></param>
        /// <param name="addedToUDT">If this field will be added to system table or User defined table</param>
        public static void AddField(string name, string description, string tableName, SAPbobsCOM.BoFieldTypes fieldType, Nullable<int> size, SAPbobsCOM.BoYesNoEnum mandatory, SAPbobsCOM.BoFldSubTypes subType, bool addedToUDT)
        {
            AddField(name, description, tableName, fieldType, size, mandatory, subType, addedToUDT, null);
        }

        /// <summary>
        /// A method for adding new field to B1 table
        /// </summary>
        /// <param name="name">Field Name</param>
        /// <param name="description">Field description</param>
        /// <param name="tableName">Table the field will be added to</param>
        /// <param name="fieldType">Field Type</param>
        /// <param name="size">Field size in the database</param>     
        /// <param name="mandatory">bool: if the value is mandatory to be filled</param>
        /// <param name="subType">Sub field type</param>
        public static void AddField(string name, string description, string tableName, SAPbobsCOM.BoFieldTypes fieldType, SAPbobsCOM.BoYesNoEnum mandatory, SAPbobsCOM.BoFldSubTypes subType, bool addedToUDT)
        {
            AddField(name, description, tableName, fieldType, null, mandatory, subType, addedToUDT);
        }

        public static void AddField(string name, string description, string tableName, SAPbobsCOM.BoFieldTypes fieldType, int size, SAPbobsCOM.BoYesNoEnum mandatory, bool addedToUDT)
        {
            AddField(name, description, tableName, fieldType, size, mandatory, 0, addedToUDT);
        }

        /// <summary>
        /// A method for adding new field to B1 table
        /// </summary>
        /// <param name="name">Field Name</param>
        /// <param name="description">Field description</param>
        /// <param name="tableName">Table the field will be added to</param>
        /// <param name="fieldType">Field Type</param>
        /// <param name="size">Field size in the database</param>     
        /// <param name="mandatory">bool: if the value is mandatory to be filled</param>
        public static void AddField(string name, string description, string tableName, SAPbobsCOM.BoFieldTypes fieldType, SAPbobsCOM.BoYesNoEnum mandatory, bool addedToUDT)
        {
            AddField(name, description, tableName, fieldType, null, mandatory, 0, addedToUDT);
        }


        /// <summary>
        /// Check if the field is already created in a table
        /// </summary>
        /// <param name="fieldName">Field name to be checked</param>
        /// <param name="tableName">table to checked the values in</param>
        /// <returns>bool: return the value if teh field is created or not</returns>
        public static bool IsFieldExists(string fieldName, string tableName)
        {
            var recordsSet = B1Helper.DiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;
            try
            {
                StringBuilder query = new StringBuilder("SELECT COUNT('AliasID') AS Count ");
                query.Append("FROM CUFD ");
                query.Append("WHERE AliasID ='{0}' AND TableID = '{1}'");


                recordsSet.DoQuery(string.Format(query.ToString(), fieldName, tableName));
                recordsSet.MoveFirst();
                if (Convert.ToInt32(recordsSet.Fields.Item("Count").Value) > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return true;
            }
            finally
            {
                recordsSet.ReleaseObject();
            }

            //var records = SqlHelper.SBODEMOUSEntities.CUFDs.Where(x => x.AliasID == fieldName && x.TableID == tableName).Count();
            //if (records > 0)
            //    return true;
            //else
            //    return false;
        }

        #endregion
        public static int CreateTransferGoodsIssue(SAPbouiCOM.Application oApplication, string reference, string itemCode, double itemQuantity, string whsCode, System.Data.DataTable batchesTable)
        {
            var goodsIssue = DiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenExit) as SAPbobsCOM.Documents;
            try
            {
                goodsIssue.DocType = SAPbobsCOM.BoDocumentTypes.dDocument_Items;
                goodsIssue.Lines.ItemCode = itemCode;
                goodsIssue.Lines.Quantity = itemQuantity;
                goodsIssue.Lines.WarehouseCode = whsCode;
                goodsIssue.Reference2 = reference;
                if (batchesTable != null)
                {
                    for (int i = 0; i < batchesTable.Rows.Count; i++)
                    {
                        goodsIssue.Lines.BatchNumbers.Quantity = Convert.ToDouble(batchesTable.Rows[i]["Selected Qty"]);
                        goodsIssue.Lines.BatchNumbers.BatchNumber = batchesTable.Rows[i]["Batch"].ToString();
                        goodsIssue.Lines.BatchNumbers.SetCurrentLine(i);
                        goodsIssue.Lines.BatchNumbers.Add();
                    }
                }
                if (goodsIssue.Add() != 0)
                {
                    Utilities.GetErrorMessage();
                    return 0;
                }
                else
                    return goodsIssue.DocEntry;
            }
            catch
            {
                return 0;
            }
            finally
            {
                goodsIssue.ReleaseObject();
            }
        }

        public static int CreateTransferGoodsIssue(SAPbouiCOM.Application oApplication, string reference, string itemCode, double itemQuantity, string whsCode)
        {
            return CreateTransferGoodsIssue(oApplication, reference, itemCode, itemQuantity, whsCode, null);
        }

        public static int CreateTransferGoodsReceipt(SAPbouiCOM.Application oApplication,string reference, string itemCode, double itemQuantity, string whsCode, double totalCost, System.Data.DataTable batchesTable)
        {
            var goodsReceipt = DiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenEntry) as SAPbobsCOM.Documents;
            try
            { 
            goodsReceipt.DocType = SAPbobsCOM.BoDocumentTypes.dDocument_Items;
            goodsReceipt.Lines.ItemCode = itemCode;
            goodsReceipt.Lines.Quantity = itemQuantity;
            goodsReceipt.Lines.WarehouseCode = whsCode;
            goodsReceipt.Lines.UnitPrice = totalCost / itemQuantity;
            goodsReceipt.Reference2 = reference;
            if (batchesTable != null)
            {
                for (int i = 0; i < batchesTable.Rows.Count; i++)
                {
                    goodsReceipt.Lines.BatchNumbers.Quantity = Convert.ToDouble(batchesTable.Rows[i]["Selected Qty"]);
                    goodsReceipt.Lines.BatchNumbers.BatchNumber = batchesTable.Rows[i]["Batch"].ToString();
                    goodsReceipt.Lines.BatchNumbers.SetCurrentLine(i);
                    goodsReceipt.Lines.BatchNumbers.Add();
                }
            }
            if (goodsReceipt.Add() != 0)
            {
                Utilities.GetErrorMessage();
                return 0;
            }
            else
                return goodsReceipt.DocEntry;
            }
            catch (Exception ex)
            {
                return 0;
            }
            finally
            {
                goodsReceipt.ReleaseObject();
            }
        }

        public static int CreateTransferGoodsReceipt(SAPbouiCOM.Application oApplication, string reference, string itemCode, double itemQuantity, string whsCode, double totalCost)
        {
            return CreateTransferGoodsReceipt(oApplication, reference, itemCode, itemQuantity, whsCode, totalCost, null);
        }
        public static SAPbobsCOM.Items GetItemByKey(string key)
        {
            var itm = B1Helper.DiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems) as SAPbobsCOM.Items;
            try
            {
                if (itm.GetByKey(key) == false)
                {
                    B1Helper.DiCompany.GetLastErrorDescription();
                    return null;
                }
                else
                    return itm;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                itm.ReleaseObject();
            }
        }

        public static string GetNextEntryIndex(string tableName)
        {
            var record = B1Helper.DiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;
            try
            {
                record.DoQuery(string.Format("SELECT MAX([DocEntry]) AS DocEntry FROM [dbo].[{0}]", tableName));
                string lastIndex = string.Empty;
                while (record.EoF == false)
                {
                    lastIndex = record.Fields.Item("DocEntry").Value.ToString();
                    record.MoveNext();
                }
                return lastIndex;
            }
            catch (Exception ex)
            {
                return "0";
            }
            finally
            {
                record.ReleaseObject();
            }
        }

        public static void DeleteRecord(int id)
        {
            SAPbobsCOM.Recordset record = B1Helper.DiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;
            try
            {
                record.DoQuery(string.Format("DELETE FROM [@T_TRANSFERITEMLINES] WHERE [DocEntry] = {1}", B1Helper.DiCompany.CompanyDB, id));
            }
            catch (Exception ex)
            {

            }
            finally
            {
                record.ReleaseObject();
            }
        }


        public static decimal GetItemAvgCost(string itemCode)
        {
            decimal avgPrice = 0;
            StringBuilder query = new StringBuilder("SELECT  ISNULL(AvgPrice, 0) AS AvgPrice ");
            query.Append("FROM OITM ");
            query.Append("WHERE ItemCode ='{0}'");

            var recordsSet = B1Helper.DiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;
            try
            {
                recordsSet.DoQuery(string.Format(query.ToString(), itemCode));
                recordsSet.MoveFirst();
                if (!recordsSet.EoF)
                {
                    avgPrice = Convert.ToDecimal(recordsSet.Fields.Item("AvgPrice").Value);
                    recordsSet.MoveNext();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                recordsSet.ReleaseObject();
            }
            return avgPrice;
        }

        public static List<WhsInfo> GetWhsInfoPerItem(string itemCode)
        {
            var recordsSet = B1Helper.DiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;
            try
            {
                StringBuilder query = new StringBuilder("SELECT  ISNULL(OnHand, 0) + ISNULL(IsCommited, 0) AS AvailableValue, ISNULL(WhsCode, '') AS WhsCode  ");
                query.Append("FROM OITW ");
                query.Append("WHERE ItemCode ='{0}' AND IsCommited + OnHand <> 0");

                recordsSet.DoQuery(string.Format(query.ToString(), itemCode));
                var wareHousesList = new List<WhsInfo>();
                recordsSet.MoveFirst();
                while(!recordsSet.EoF)
                {
                    WhsInfo wareHouseInfo = new WhsInfo();
                    wareHouseInfo.AvailableValue = Convert.ToDecimal(recordsSet.Fields.Item("AvailableValue").Value);
                    wareHouseInfo.WhsCode = recordsSet.Fields.Item("WhsCode").Value.ToString();
                    wareHousesList.Add(wareHouseInfo);
                    recordsSet.MoveNext();
                }
                return wareHousesList;
            }
            catch (Exception ex)
            {
                return new List<WhsInfo>();
            }
            finally
            {
                recordsSet.ReleaseObject();
            }
        }

        public static WhsInfo GetWhsItemInfo(string itemCode, string whsCode)
        {
            var recordsSet = B1Helper.DiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;
            try
            {
                StringBuilder query = new StringBuilder("SELECT  ISNULL(OnHand, 0) + ISNULL(IsCommited, 0) AS AvailableValue, ISNULL(WhsCode, '') AS WhsCode  ");
                query.Append("FROM OITW ");
                query.Append("WHERE ItemCode ='{0}'  AND WhsCode = '{1}'");
               
                recordsSet.DoQuery(string.Format(query.ToString(), itemCode, whsCode));
                WhsInfo wareHouseInfo = new WhsInfo();
                if (recordsSet.RecordCount > 0)
                {
                    recordsSet.MoveFirst();
                    wareHouseInfo.AvailableValue = Convert.ToDecimal(recordsSet.Fields.Item("AvailableValue").Value);
                    wareHouseInfo.WhsCode = recordsSet.Fields.Item("WhsCode").Value.ToString();
                }
                return wareHouseInfo;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                recordsSet.ReleaseObject();
            }
        }
    }
}
