using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using DevRX.HelpDesk.InternalRequest;

namespace DevRX.HelpDesk.Client
{
  partial class InternalRequestActions
  {
    public virtual void ShowEmployeeRequests(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      Functions.InternalRequest.Remote.GetEmployeeRequests(_obj).Show();
    }

    public virtual bool CanShowEmployeeRequests(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}