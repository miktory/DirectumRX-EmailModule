using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using DevRX.HelpDesk.Request;
using Sungero.Company;

namespace DevRX.HelpDesk
{
  partial class RequestRequestActionsSharedCollectionHandlers
  {

    public virtual void RequestActionsAdded(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      _added.Employee = Employees.Current;
    }
  }

  partial class RequestSharedHandlers
  {

    public virtual void RequestActionsChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      _obj.HoursSum = _obj.RequestActions.Sum(x => x.HoursCount);
    }

  }
}