using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using DevRX.HelpDesk.ExternalRequest;

namespace DevRX.HelpDesk
{
  partial class ExternalRequestContactPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ContactFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if (_obj.Organization != null)
        return query.Where(x => x.Company == _obj.Organization);
      return query;
    }
  }


}