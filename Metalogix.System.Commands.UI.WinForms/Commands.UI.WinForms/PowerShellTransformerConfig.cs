using Metalogix.Actions;
using Metalogix.Commands;
using Metalogix.Transformers;
using Metalogix.Transformers.Attributes;
using Metalogix.Transformers.Interfaces;
using System;
using System.Collections;
using System.Windows.Forms;

namespace Metalogix.Commands.UI.WinForms
{
    [TransformerConfig(new Type[] { typeof(PowerShellTransformer<object, Metalogix.Actions.Action, IEnumerable, IEnumerable>) })]
    public class PowerShellTransformerConfig : ITransformerConfig
    {
        public PowerShellTransformerConfig()
        {
        }

        public bool Configure(TransformerConfigContext context)
        {
            bool dialogResult;
            PowerShellTransformerOptions transformerOptions = context.GetTransformerOptions<PowerShellTransformerOptions>();
            using (PowerShellTranformerDialog powerShellTranformerDialog = new PowerShellTranformerDialog())
            {
                powerShellTranformerDialog.Options = transformerOptions;
                powerShellTranformerDialog.ShowDialog();
                dialogResult = powerShellTranformerDialog.DialogResult == DialogResult.OK;
            }
            return dialogResult;
        }
    }
}