using ITCO.TransferCost.Main;
using SAPbouiCOM.Framework;
using System;

namespace ITCO.TransferCost.UI
{
    public abstract class B1FormBase : UserFormBase
    {
        #region PUBLIC VARIABLES

        protected int RowToDeleteIndex;

        #endregion

        public abstract event UserFormClosedDone OnUserFormClosedDone;

        #region Protected Methods

        //protected void Form_CloseAfter(SAPbouiCOM.SBOItemEventArg pVal)
        //{
        //    OnUserFormClosedDone(this);
        //}

        protected SAPbouiCOM.DataTable GetCFLSelectedItem(SAPbouiCOM.SBOItemEventArg pVal)
        {
            return (pVal as SAPbouiCOM.ISBOChooseFromListEventArg).SelectedObjects;
        }

        protected void AddDeleteMenu()
        {
            SAPbouiCOM.MenuItem oMenuItem = null;
            SAPbouiCOM.Menus oMenus = null;

            try
            {
                //  CREATE MENU POPUP MYUSERMENU01 AND ADD IT TO TOOLS MENU
                SAPbouiCOM.MenuCreationParams oCreationPackage = null;
                oCreationPackage = ((SAPbouiCOM.MenuCreationParams)(Application.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams)));

                oMenuItem = Application.SBO_Application.Menus.Item("1280"); // Data'

                if (!oMenuItem.SubMenus.Exists("DeleteLine"))
                {
                    oMenus = oMenuItem.SubMenus;
                    oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                    oCreationPackage.UniqueID = "DeleteLine";
                    oCreationPackage.String = "Delete Line";
                    oCreationPackage.Enabled = true;
                    oMenus.AddEx(oCreationPackage);
                }
            }
            catch (Exception ex)
            {

            }
        }

        protected void RemoveDeleteMenu()
        {
            try
            {
                Application.SBO_Application.Menus.RemoveEx("DeleteLine");
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #region ABSTRACT METHODS

        public abstract void ItemEvent(string FormId, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent);

        public abstract void MenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent);

        #endregion

    }
}
