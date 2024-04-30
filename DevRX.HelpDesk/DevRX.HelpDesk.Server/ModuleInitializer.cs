using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace DevRX.HelpDesk.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      GrantAllUsersReadAccessRights();
    }

    /// <summary>
    /// 
    /// </summary>

    public void GrantAllUsersReadAccessRights()
    {
      RequestKinds.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      Requests.AccessRights.Grant(Roles.AllUsers, DefaultAccessRightsTypes.Read);
      RequestKinds.AccessRights.Save();
      Requests.AccessRights.Save();
    }
  }
}
