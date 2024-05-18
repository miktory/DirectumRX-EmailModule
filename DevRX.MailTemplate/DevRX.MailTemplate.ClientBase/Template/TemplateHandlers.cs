using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using DevRX.MailTemplate.Template;

namespace DevRX.MailTemplate
{
  partial class TemplateClientHandlers
  {

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      if (_obj.IsSystem.Equals(true))
        _obj.State.Properties.Name.IsEnabled = false;
    }
  }


}