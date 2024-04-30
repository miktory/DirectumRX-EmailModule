using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using DevRX.HelpDesk.Request;

namespace DevRX.HelpDesk.Client
{
  partial class RequestActions
  {
    public virtual void OpenClosedRequest(Sungero.Domain.Client.ExecuteActionArgs e)
    {
        _obj.LifeCycle = LifeCycle.InWork;
        foreach (var p in _obj.State.Properties)
          p.IsEnabled = true;
        _obj.ClosedDate = null;
    }

    public virtual bool CanOpenClosedRequest(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
       return _obj.LifeCycle == LifeCycle.Closed && !_obj.State.IsChanged;
    }

  }


}