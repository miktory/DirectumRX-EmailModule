using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using DevRX.HelpDesk.Request;
using Sungero.Company;

namespace DevRX.HelpDesk
{
  partial class RequestClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      if (_obj.LifeCycle == LifeCycle.Closed && !_obj.State.IsChanged)
      {
        foreach (var p in _obj.State.Properties)
          p.IsEnabled = false;
      }
      
      if (Employees.Current != _obj.Responsible)
        _obj.State.Properties.RequestActions.CanDelete = false;
    }

  }
}