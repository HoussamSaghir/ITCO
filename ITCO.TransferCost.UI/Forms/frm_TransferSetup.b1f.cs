using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM.Framework;
using ITCO.TransferCost.Main;

namespace ITCO.TransferCost.UI.Forms
{
    [FormAttribute("frm_TransferSetup", "Forms/frm_TransferSetup.b1f")]
    class TransferSetup : B1FormBase
    {
        private SAPbouiCOM.Button btnSave;
        private SAPbouiCOM.Button btnCancel;
        private SAPbouiCOM.StaticText lblCode;
        private SAPbouiCOM.StaticText lblName;
        private SAPbouiCOM.EditText txtCode;
        private SAPbouiCOM.EditText txtName;
        private SAPbouiCOM.StaticText lblType;
        private SAPbouiCOM.ComboBox cbTypes;
        private SAPbouiCOM.StaticText lblDoc;
        private SAPbouiCOM.EditText txtDocEntry;

        public override event UserFormClosedDone OnUserFormClosedDone;

        public TransferSetup()
        {
            txtDocEntry.Item.SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, Convert.ToInt32(SAPbouiCOM.BoAutoFormMode.afm_Add), SAPbouiCOM.BoModeVisualBehavior.mvb_False);
            txtDocEntry.Item.SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, Convert.ToInt32(SAPbouiCOM.BoAutoFormMode.afm_Ok), SAPbouiCOM.BoModeVisualBehavior.mvb_False);
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.btnSave = ((SAPbouiCOM.Button)(this.GetItem("1").Specific));
            this.btnSave.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.btnSave_ClickBefore);
            this.btnCancel = ((SAPbouiCOM.Button)(this.GetItem("2").Specific));
            this.lblCode = ((SAPbouiCOM.StaticText)(this.GetItem("lblCode").Specific));
            this.lblName = ((SAPbouiCOM.StaticText)(this.GetItem("lblName").Specific));
            this.txtCode = ((SAPbouiCOM.EditText)(this.GetItem("txtCode").Specific));
            this.txtName = ((SAPbouiCOM.EditText)(this.GetItem("txtName").Specific));
            this.lblType = ((SAPbouiCOM.StaticText)(this.GetItem("lblType").Specific));
            this.cbTypes = ((SAPbouiCOM.ComboBox)(this.GetItem("cbTypes").Specific));
            this.lblDoc = ((SAPbouiCOM.StaticText)(this.GetItem("lblDoc").Specific));
            this.txtDocEntry = ((SAPbouiCOM.EditText)(this.GetItem("txtDoc").Specific));
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            this.ResizeAfter += new SAPbouiCOM.Framework.FormBase.ResizeAfterHandler(this.Form_ResizeAfter);
            this.ActivateAfter += new SAPbouiCOM.Framework.FormBase.ActivateAfterHandler(this.Form_ActivateAfter);
            this.CloseAfter += TransferSetup_CloseAfter;
        }

        void TransferSetup_CloseAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            OnUserFormClosedDone(this);
        }


        private void OnCustomInitialize()
        {

        }

        public override void ItemEvent(string FormId, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
        }

        public override void MenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (pVal.MenuUID == "DeleteLine")
            {
                if (RowToDeleteIndex > 0)
                {
                    this.UIAPIRawForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE;
                }
            }
        }

        private void Form_ResizeAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
        }

        private void btnSave_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
        }

        private void Form_ActivateAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
        }      
    }
}
