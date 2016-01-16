using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCO.TransferCost.Main
{
    public class Constants
    {
        public const string Hash = "#";
    }

    public static class MenuUid
    {
        public const string DELETE = "774";
        public const string COPY = "772";
        public const string PASTE = "773";
        public const string CUT = "771";
        public const string MAXIMIZEMINIMIZE = "8802";
        public const string COPY_TABLE = "784";

        public const string ADD = "1282";
        public const string NEXT_RECORD = "1288";
        public const string PREVIOUS_RECORD = "1289";
        public const string FIRST_RECORD = "1290";
        public const string LAST_RECORD = "1291";
        public const string ADD_ROW = "1282";
        public const string CANCEL = "1284";
        public const string DELETEROW = "1293";
        public const string REMOVE = "1283";

        public const string ADMINISTRATION = "3328";
    }

    public static class TableNames
    {
        public const string TransferItems = "T_TRANSFERITEMS";
        public const string TransferItemsLines = "T_TRANSFERITEMLINES";
        public const string TransferItemsSetup = "T_ITMTRNSFRSTUP";
        public const string TransferAddCost = "T_TRANSFERADDCOST";
    }

    public static class FieldNames
    {
        public const string IssueItem = "IssueItem";
        public const string ReceiptItem = "ReceiptItem";
        public const string AvgPrice = "AvgPrice";
        public const string FromWareHouse = "From";
        public const string ToWareHouse = "ToWhs";
        public const string Quantity = "Quantity";
        public const string AdditionalCost = "AddCost";
        public const string Type = "Type";
        public const string Name = "Name";
        public const string UoMType = "UoMType";
        public const string Cost = "Cost";
        public const string Code = "Code";
        public const string IssueDescription = "IssueDesc";
        public const string ReceiptDescription = "ReceiptDesc";
    }
}
