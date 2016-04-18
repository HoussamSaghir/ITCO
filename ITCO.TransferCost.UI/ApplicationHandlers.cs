using ITCO.TransferCost.Main;
using ITCO.TransferCost.UI.Forms;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ITCO.TransferCost.UI
{
    public class ApplicationHandlers
    {
        private List<dynamic> frmInstances = new List<dynamic>();

        public ApplicationHandlers()
        {
            try
            {
                Application.SBO_Application.StatusBar.SetSystemMessage("ApplicationHandlers starts", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                Application.SBO_Application.AppEvent += new SAPbouiCOM._IApplicationEvents_AppEventEventHandler(SBO_Application_AppEvent);
                Application.SBO_Application.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
                Application.SBO_Application.MenuEvent += new SAPbouiCOM._IApplicationEvents_MenuEventEventHandler(SBO_Application_MenuEvent);
                Application.SBO_Application.RightClickEvent += new SAPbouiCOM._IApplicationEvents_RightClickEventEventHandler(SBO_Application_RightClickEvent);
                Application.SBO_Application.StatusBar.SetSystemMessage("ApplicationHandlers Done", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
            }
        }

        void SBO_Application_LayoutKeyEvent(ref SAPbouiCOM.LayoutKeyInfo eventInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;
        }

        public void SBO_Application_AppEvent(SAPbouiCOM.BoAppEventTypes EventType)
        {
            try
            {
                switch (EventType)
                {
                    case SAPbouiCOM.BoAppEventTypes.aet_ShutDown:
                        System.Windows.Forms.Application.Exit();
                        break;
                    case SAPbouiCOM.BoAppEventTypes.aet_CompanyChanged:
                        break;
                    case SAPbouiCOM.BoAppEventTypes.aet_FontChanged:
                        break;
                    case SAPbouiCOM.BoAppEventTypes.aet_LanguageChanged:
                        break;
                    case SAPbouiCOM.BoAppEventTypes.aet_ServerTerminition:
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
            }
        }

        public void SBO_Application_ItemEvent(string FormId, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (pVal.EventType == SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST)
            {
                var selectedssItems = pVal as SAPbouiCOM.IChooseFromListEvent;
            }

            if (!pVal.BeforeAction && (pVal.FormTypeEx == "frm_TransferSetup" || pVal.FormTypeEx == "frm_TransferItems") && pVal.EventType == SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST)
            {
                var currentFormInstance = frmInstances.Where(x => x.UIAPIRawForm.UniqueID == Application.SBO_Application.Forms.ActiveForm.UniqueID).FirstOrDefault();
                if (currentFormInstance != null)
                    currentFormInstance.ItemEvent(FormId, ref  pVal, out  BubbleEvent);
            }
        }

        public void SBO_Application_MenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            var currentForm = Application.SBO_Application.Forms.ActiveForm;

            try
            {
                if (pVal.BeforeAction && pVal.MenuUID == "frm_TransferItems")
                {
                    TransferItems activeForm = new TransferItems();
                    activeForm.Show();
                    activeForm.OnUserFormClosedDone += new UserFormClosedDone(OnUserFormClosed);
                    activeForm.OnSubFormCreated += new SubFormCreated<BatchNumberSelection>(ExecuteCustomHandler<BatchNumberSelection>);
                    frmInstances.Add(activeForm);
                }
                if (pVal.BeforeAction && pVal.MenuUID == "frm_TransferSetup")
                {
                    TransferSetup activeForm = new TransferSetup();
                    activeForm.Show();
                    activeForm.OnUserFormClosedDone += new UserFormClosedDone(OnUserFormClosed);

                    frmInstances.Add(activeForm);
                }

                var currentFormId = Application.SBO_Application.Forms.ActiveForm.TypeEx;

                var formTypes = frmInstances.Where(c => c.UIAPIRawForm.TypeEx == currentFormId);
                if (formTypes.Count() > 0)
                {
                    if (!pVal.BeforeAction)
                    {
                        var currentFormInstance = frmInstances.Where(x => x.UIAPIRawForm.UniqueID == Application.SBO_Application.Forms.ActiveForm.UniqueID).FirstOrDefault();
                        currentFormInstance.MenuEvent(ref pVal, out BubbleEvent);
                    }
                }

                if (pVal.BeforeAction && pVal.MenuUID == "new_3")
                {
                    BatchNumberSelection bselection = new BatchNumberSelection();
                    bselection.Show();
                }
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
            }
        }

        public void SBO_Application_RightClickEvent(ref SAPbouiCOM.ContextMenuInfo eventInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;
        }

        #region CUSTOM HANDLERS

        public void OnUserFormClosed(object sender)
        {
            try
            {
                frmInstances.Remove(sender);
            }
            catch (Exception ex)
            {
                Utilities.LogException(ex);
            }
        }

        public void ExecuteCustomHandler<T>(object sender, FormArgs frmArgs)
        {
            var transferDocEntry = frmArgs.Parameters["Instance"];
            dynamic objectInstance = (T)transferDocEntry;
            objectInstance.Show();
            objectInstance.OnUserFormClosedDone += new UserFormClosedDone(OnUserFormClosed);
            frmInstances.Add(objectInstance);
        }     
       
        #endregion

        private void LoadXMLFiles()
        {
            System.Xml.XmlDocument oXmlDoc;
            string sPath;
            oXmlDoc = new System.Xml.XmlDocument();
            sPath = System.Windows.Forms.Application.StartupPath.Trim() + "\\XML\\XMLFile.xml";

            oXmlDoc.Load(sPath);
            Application.SBO_Application.LoadBatchActions(oXmlDoc.InnerXml);
        }

    }
}
