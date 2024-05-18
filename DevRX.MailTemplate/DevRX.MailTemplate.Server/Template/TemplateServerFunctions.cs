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
    /// <summary>
    /// Добавить в справочник системные HTML-шаблоны.
    /// </summary>
    [Public]
    public void CreateSystemTemplates()
    {
      var templates = Templates.GetAll();
      var mailTemplate = templates.Where(x => x.Name == "MailTemplate" && x.IsSystem.Equals(true)).FirstOrDefault();
      if (mailTemplate == Templates.Null)
      {
        mailTemplate = CreateSystemTemplate("MailTemplate", Sungero.Docflow.Resources.MailTemplate);
        mailTemplate.Save();
      }
      
      var summaryMailMainTemplate = templates.Where(x => x.Name == "SummaryMailMainTemplate" && x.IsSystem.Equals(true)).FirstOrDefault();
      if (summaryMailMainTemplate == Templates.Null)
      {
        summaryMailMainTemplate = CreateSystemTemplate("SummaryMailMainTemplate", Sungero.Docflow.Resources.SummaryMailMainTemplate);
        summaryMailMainTemplate.Save();
      }
      
      var summaryMailBlockContentTemplate = templates.Where(x => x.Name == "SummaryMailBlockContentTemplate" && x.IsSystem.Equals(true)).FirstOrDefault();
      if (summaryMailBlockContentTemplate == Templates.Null)
      {
        summaryMailBlockContentTemplate = CreateSystemTemplate("SummaryMailBlockContentTemplate", Sungero.Docflow.Resources.SummaryMailBlockContentTemplate);
        summaryMailBlockContentTemplate.Save();
      }
      
      var summaryMailGroupContentTemplate = templates.Where(x => x.Name == "SummaryMailGroupContentTemplate" && x.IsSystem.Equals(true)).FirstOrDefault();
      if (summaryMailGroupContentTemplate == Templates.Null)
      {
        summaryMailGroupContentTemplate = CreateSystemTemplate("SummaryMailGroupContentTemplate", Sungero.Docflow.Resources.SummaryMailGroupContentTemplate);
        summaryMailGroupContentTemplate.Save();
      }
    }
    
    public ITemplate CreateSystemTemplate(string name, string htmlText)
    {
      var template = Templates.Create();
      template.HtmlTemplate = htmlText;
      template.Name = name;
      template.IsSystem = true;
      return template;
    }
  }
}