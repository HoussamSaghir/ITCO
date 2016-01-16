using System;
using System.Collections.Generic;
using System.Text;
using SAPbouiCOM.Framework;
using ITCO.TransferCost.Main;

namespace ITCO.TransferCost.UI
{
    public class Menu
    {
        public void AddMenuItems()
        {
            try
            {
                SAPbouiCOM.Menus oMenus = null;
                SAPbouiCOM.MenuItem oMenuItem = null;

                oMenus = Application.SBO_Application.Menus;

                SAPbouiCOM.MenuCreationParams oCreationPackage = null;
                oCreationPackage = ((SAPbouiCOM.MenuCreationParams)(Application.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams)));
                oMenuItem = Application.SBO_Application.Menus.Item("43520"); // moudles'

                //oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_POPUP;
                //oCreationPackage.UniqueID = "mnu_Transfer";
                //oCreationPackage.String = "ITCO Transfer";
                //oCreationPackage.Enabled = true;
                //oCreationPackage.Position = -1;
                //oMenus = oMenuItem.SubMenus;
                //oMenus.AddEx(oCreationPackage);

                oMenuItem = Application.SBO_Application.Menus.Item("43540"); 
                //oMenuItem = Application.SBO_Application.Menus.Item("mnu_Transfer");
                oMenus = oMenuItem.SubMenus;
                oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                oCreationPackage.UniqueID = "frm_TransferItems";
                oCreationPackage.String = "Transfer Items";
                oMenus.AddEx(oCreationPackage);

                oMenuItem = Application.SBO_Application.Menus.Item(MenuUid.ADMINISTRATION); // moudles'
                oMenus = oMenuItem.SubMenus;
                oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                oCreationPackage.UniqueID = "frm_TransferSetup";
                oCreationPackage.String = "Transfer Items Setup";
                oMenus.AddEx(oCreationPackage);
            }
            catch (Exception ex)
            {
                Application.SBO_Application.SetStatusBarMessage("Menu Already Exists", SAPbouiCOM.BoMessageTime.bmt_Short, true);
            }
        }

        private void AddMenu()
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
                    oCreationPackage.Enabled = false;
                    oMenus.AddEx(oCreationPackage);
                }

            }
            catch (Exception ex)
            {

            }

        } 

    }
}
