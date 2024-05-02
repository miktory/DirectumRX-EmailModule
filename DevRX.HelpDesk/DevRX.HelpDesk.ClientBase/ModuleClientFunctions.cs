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
    
    /// <summary>
    /// Поиск всех обращений компании.
    /// </summary>
    [LocalizeFunction("Найти все обращения компании", "Поиск обращений компании")]
    public virtual void FindRequestByCompany()
    {
      var dialog = Dialogs.CreateInputDialog("Поиск обращений компании");
      var id = dialog.AddSelect("Организация", true, Sungero.Parties.CompanyBases.Null);
      if (dialog.Show() == DialogButtons.Ok)
        Functions.Module.Remote.GetRequestByCompany(id.Value).Show();
    }
    
    /// <summary>
    /// Поиск обращения по номеру.
    /// </summary>
    [LocalizeFunction("Найти обращение по номеру", "Поиск обращения по номеру")]
    public virtual void FindRequestById()
    {
      var dialog = Dialogs.CreateInputDialog("Поиск обращения по номеру");
      var id = dialog.AddInteger("Номер", true);
      if (dialog.Show() == DialogButtons.Ok)
        Functions.Module.Remote.GetRequestById((long)id.Value).Show();
    }
    



  }
}