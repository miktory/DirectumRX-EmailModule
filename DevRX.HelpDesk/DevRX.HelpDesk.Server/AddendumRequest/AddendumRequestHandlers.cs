using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using DevRX.HelpDesk.AddendumRequest;

namespace DevRX.HelpDesk
{
  partial class AddendumRequestServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      if (!_obj.AccessRights.CanUpdate())
      {
        // Если нет – выдать права.
        _obj.AccessRights.Grant(_obj.Request.Responsible, DefaultAccessRightsTypes.FullAccess);
        _obj.AccessRights.Save();
      }
    }
  }

}