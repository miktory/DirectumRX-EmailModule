using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace DevRX.HelpDesk.Client
{
  public class ModuleFunctions
  {
    /// <summary>
    /// Создать и отобразить карточку внутреннего обращения.
    /// </summary>
    [LocalizeFunction("Создать внутреннее обращение", "Быстрое создание внутреннего обращения")]
    public virtual void CreateInternalRequest()
    {
      Functions.Module.Remote.CreateInternalRequest().Show();
    }


  }
}