using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using DevRX.MailTemplate.Template;

namespace DevRX.MailTemplate
{
  partial class TemplateServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      if (_obj.IsSystem == null)
        _obj.IsSystem = false;
    }
  }

}