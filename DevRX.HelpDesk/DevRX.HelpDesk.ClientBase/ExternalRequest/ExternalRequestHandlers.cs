using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using DevRX.HelpDesk.ExternalRequest;

namespace DevRX.HelpDesk
{
  partial class ExternalRequestClientHandlers
  {

    public virtual void ContactValueInput(DevRX.HelpDesk.Client.ExternalRequestContactValueInputEventArgs e)
    {
      var x = e.NewValue.Company;
      if (e.NewValue.Company != null)
        _obj.Organization = e.NewValue.Company;
    }

    public virtual void OrganizationValueInput(DevRX.HelpDesk.Client.ExternalRequestOrganizationValueInputEventArgs e)
    {
       if (e.NewValue != e.OldValue)
        _obj.Contact = null;
    }

  }
}