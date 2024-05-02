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
    public virtual void FindRequestById(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var dialog = Dialogs.CreateInputDialog("Поиск обращения по номеру");
      var id = dialog.AddInteger("Номер", true);
      if (dialog.Show() == DialogButtons.Ok)
        Functions.Request.Remote.GetRequestById(_obj, (long)id.Value).Show();
    }
    


    public virtual bool CanFindRequestById(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void ShowRequestAddendum(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      Functions.Request.Remote.GetAddendumRequests(_obj).Show();
    }

    public virtual bool CanShowRequestAddendum(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !_obj.State.IsInserted;
    }

    public virtual void CreateAddendumRequest(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      Functions.Request.Remote.CreateAddendumRequest(_obj).Show();
    }

    public virtual bool CanCreateAddendumRequest(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !_obj.State.IsInserted;
    }

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