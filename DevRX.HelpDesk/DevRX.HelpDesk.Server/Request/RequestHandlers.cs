using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using DevRX.HelpDesk.Request;
using Sungero.Company;

namespace DevRX.HelpDesk
{
  partial class RequestFilteringServerHandler<T>
  {

    public override IQueryable<T> Filtering(IQueryable<T> query, Sungero.Domain.FilteringEventArgs e)
    {
      // Проверка того, что панель фильтрации включена.
      if (_filter == null)
        return query;
      // Контрол "Набор флажков". Фильтр по состоянию обращения.
      // Возможные значения: В работе, На контроле, Закрыто.
      if (_filter.FlagInWork || _filter.FlagOnControl || _filter.FlagClosed)
        query = query.Where(r => (_filter.FlagInWork && r.LifeCycle.Equals(Request.LifeCycle.InWork)) ||
                            (_filter.FlagOnControl && r.LifeCycle.Equals(Request.LifeCycle.OnControl)) ||
                            (_filter.FlagClosed && r.LifeCycle.Equals(Request.LifeCycle.Closed)));
      return query;
    }
  }


  partial class RequestServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      if (_obj.LifeCycle == LifeCycle.Closed)
      {
        if (string.IsNullOrEmpty(_obj.Result))
        {
          e.AddError("Перед закрытием обращения заполните результат.");
          return;
        }
        _obj.ClosedDate = Calendar.Today;
      }
      _obj.Name = Functions.Request.FormatRequestSubject(_obj);
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.Number = _obj.Id;
      _obj.Responsible = Employees.Current;
      _obj.LifeCycle = LifeCycle.InWork;
      _obj.CreatedDate = Calendar.Today;
      _obj.Name = "Тема будет сформирована автоматически.";
    }
  }

}