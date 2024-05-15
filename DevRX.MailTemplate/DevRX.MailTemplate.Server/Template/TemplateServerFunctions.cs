using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using DevRX.MailTemplate.Template;

namespace DevRX.MailTemplate.Server
{
  partial class TemplateFunctions
  {
    [Remote(IsPure=true)]
    public void ChangeTemplatesSelectionToFalse()
    {
      var templates = Templates.GetAll();
      foreach (var template in templates)
      {
        template.IsSelected = false;
        template.Save();
      }
    }
    
    [Public]
    public ITemplate GetSelectedTemplate()
    {
      var template = Templates.GetAll(r => r.IsSelected == true).First();
      return template;
    }
  }
}