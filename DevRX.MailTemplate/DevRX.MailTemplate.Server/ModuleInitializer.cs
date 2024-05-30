using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace DevRX.MailTemplate.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      Init();
    }

    /// <summary>
    ///  Инициализация модуля
    /// </summary>
    public void Init()
    {
      PublicFunctions.Template.CreateSystemTemplates();
      GrantAccessRights();
    }
    
    public void GrantAccessRights()
    {
      Templates.AccessRights.Grant(Roles.Administrators, DefaultAccessRightsTypes.FullAccess);
    }
    
  }
}
