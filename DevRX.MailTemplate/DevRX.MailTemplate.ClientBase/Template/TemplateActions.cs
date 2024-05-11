using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using DevRX.MailTemplate.Template;

namespace DevRX.MailTemplate.Client
{
  partial class TemplateActions
  {
    public virtual void SelectTemplate(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      Functions.Template.Remote.ChangeTemplatesSelectionToFalse(_obj);
      _obj.IsSelected = true;
    }

    public virtual bool CanSelectTemplate(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}