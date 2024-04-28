using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using DevRX.HelpDesk.Request;
using Sungero.Company;

namespace DevRX.HelpDesk
{
  partial class RequestServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      if (_obj.LifeCycle == LifeCycle.Closed)
        _obj.ClosedDate = Calendar.Today;
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.Number = _obj.Id;
      _obj.Responsible = Employees.Current;
      _obj.LifeCycle = LifeCycle.InWork;
      _obj.CreatedDate = Calendar.Today;
    }
  }

}