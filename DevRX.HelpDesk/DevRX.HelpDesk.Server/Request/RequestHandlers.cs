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
      if (_filter.FlagInWork || _filter.FlagOnControl || _filter.FlagClosed)
        query = query.Where(r => (_filter.FlagInternal && InternalRequests.Is(r)) ||
                            (_filter.FlagExternal && ExternalRequests.Is(r)) ||
                            (_filter.FlagAll && Requests.Is(r)));
      if (_filter.FlagInternal || _filter.FlagExternal || _filter.FlagAll)
        query = query.Where(r => (_filter.FlagInWork && r.LifeCycle.Equals(Request.LifeCycle.InWork)) ||
                            (_filter.FlagOnControl && r.LifeCycle.Equals(Request.LifeCycle.OnControl)) ||
                            (_filter.FlagClosed && r.LifeCycle.Equals(Request.LifeCycle.Closed)));
      if (_filter.FlagMe)
        query = query.Where(r => r.Responsible == Sungero.Company.Employees.Current);
      if (_filter.FlagAllEmployees)
        query = query.Select(r => r);
      if (_filter.FlagSelectedEmployee)
        query = query.Where(r => r.Responsible == _filter.Employee);
      
      
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