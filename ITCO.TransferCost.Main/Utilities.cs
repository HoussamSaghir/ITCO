using SAPbouiCOM.Framework;
using System;
using System.Text.RegularExpressions;

namespace ITCO.TransferCost.Main
{
    public static class Utilities
    {
        public static void LogException(Exception ex)
        {
            Application.SBO_Application.SetStatusBarMessage(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, true);
        }

        public static void LogException(string ex)
        {
            Application.SBO_Application.SetStatusBarMessage(ex, SAPbouiCOM.BoMessageTime.bmt_Short, true);
        }

        public static string GetErrorMessage()
        {
            string ErrMsg = string.Empty;
            int errCode = int.MinValue;
            B1Helper.DiCompany.GetLastError(out errCode, out ErrMsg);
            return ErrMsg;
        }

        public static bool IsNumber(string key)
        {
            return Regex.IsMatch(key, @"^[0-9]*\.?[0-9]+$");
        }
    }
}