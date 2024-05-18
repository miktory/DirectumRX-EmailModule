using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using CommonLibrary;
using Sungero.Commons;
using Sungero.Company;
using Sungero.Content;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Docflow.ApprovalStage;
using Sungero.Docflow.DocumentKind;
using Sungero.Docflow.Structures.Module;
using Sungero.Docflow.Structures.StampSetting;
using Sungero.Domain;
using Sungero.Domain.LinqExpressions;
using Sungero.Domain.Shared;
using Sungero.Metadata;
using Sungero.Parties;
using Sungero.Workflow;
using Sungero.Docflow.Structures;
using AppMonitoringType = Sungero.Content.AssociatedApplication.MonitoringType;
using ArioTaskStates = Sungero.SmartProcessing.PublicConstants.Module.ProcessingTaskStates;
using ExchDocumentType = Sungero.Exchange.ExchangeDocumentInfoServiceDocuments.DocumentType;
using MessageType = Sungero.Exchange.ExchangeDocumentInfo.MessageType;
using ReportResources = Sungero.Docflow.Reports.Resources;
using SettingsValidationMessageTypes = Sungero.Docflow.Constants.SmartProcessingSetting.SettingsValidationMessageTypes;


namespace DevRX.MailTemplateSolution.Module.Docflow.Server
{
  partial class ModuleFunctions
  {
    public override string GenerateBody(IAssignmentBase assignment, bool isExpired, bool hasSubstitutions)
    {
      if (!Nustache.Core.Helpers.Contains("process_text"))
        Nustache.Core.Helpers.Register("process_text", ProcessText);
      
      var model = this.GenerateBodyModel(assignment, isExpired, hasSubstitutions);
      var template = MailTemplate.PublicFunctions.Template.GetSelectedTemplate(MailTemplate.Templates.Create());
      return this.GetMailBodyAsHtml(template.HtmlTemplate, model);
    }
    
    /// <summary>
    /// Отправить электронное письмо одному или нескольким получателям.
    /// </summary>
    /// <param name="eMail">Почта получателя письма.</param>
    /// <param name="copyRecipients">Список почт получателей копии письма.</param>
    /// <param name="subject">Тема письма.</param>
    /// <param name="template">HTML-шаблон.</param>
    /// <param name="documents">Вложения к письму.</param>
    /// <param name="dataSource">Источник информации для HTML-шаблона.</param>
    /// <returns>True - если письмо отправлено. False - если письмо не отправлено.</returns>
    [Public, Remote]
    public bool SendMailByTemplate(string eMail, List<string> copyRecipients, string subject, string template, List<Sungero.Content.IElectronicDocument> documents, Sungero.Content.IElectronicDocument dataSource)
    {
      var message = Mail.CreateMailMessage();
      this.AddLogo(message);
      var model = new Dictionary<string, object>();
      var body = dataSource == Sungero.Content.ElectronicDocuments.Null ? this.GetMailBodyAsHtml(template, model) : Nustache.Core.Render.StringToString(template, dataSource,
                                                                                                                                                        new Nustache.Core.RenderContextBehaviour() { OnException = ex => Logger.Error(ex.Message, ex) });
      message.Body = body;
      message.IsBodyHtml = true;
      message.Subject = subject;
      message.To.Add(eMail);
      message.CC.AddRange(copyRecipients);
      foreach (var doc in documents)
      {
        try
        {
          message.AddAttachment(doc.LastVersion);
        }
        catch (Exception ex)
        {
          return false;
        }
      }
      
      try 
      {
        Mail.Send(message);
      }
      catch (Exception Ex)
      {
        return false;
      }
      
      return true;
    }
    
      public override void SendSummaryMailNotificationMessages(List<Sungero.Core.IEmailMessage> messages)
    {
      try
      {
        Mail.Send(messages);
      }
      catch (Exception ex)
      {
        this.SummaryMailLogError("Email messages sending failed. Try send messages one by one", ex);
        foreach (var mail in messages)
        {
          try
          {
            // Переполучить сообщение для корректной отправки.
            Mail.Send(this.GetSummaryMailNotificationMessage(mail.Subject, mail.To.ToList(), mail.CC.ToList(), mail.Body));
          }
          catch (Exception exception)
          {
            var mailTo = string.Join(", ", mail.To);
            this.SummaryMailLogError(string.Format("Email message to {0} sending failed", mailTo), exception);
          }
        }
      }
    }
       public override void SendSummaryMailNotification()
    {
      var employeeInfos = this.CreateEmployeesMailInfoToSendSummaryNotification();
      if (!employeeInfos.Any())
      {
        this.SummaryMailLogDebug("There are no employees who need to send a summary");
        return;
      }
      
      var mailMessages = this.GetMailMessages(employeeInfos);
      
      var sentMessagesCount = 0;
      var bunchSize = (int)Sungero.Docflow.PublicFunctions.Module.Remote.GetDocflowParamsNumbericValue(Sungero.Docflow.Constants.Module.SummaryMailNotificationsBunchCountParamName);
      if (bunchSize <= 0)
        bunchSize = Sungero.Docflow.Constants.Module.SummaryMailNotificationsBunchCount;
      
      while (sentMessagesCount < mailMessages.Count())
      {
        var sentMessagesBunch = mailMessages.Skip(sentMessagesCount).Take(bunchSize).ToList();
        SendSummaryMailNotificationMessages(sentMessagesBunch);
        sentMessagesCount += bunchSize;
      }
    }
       
          
   // Поглядеть
     public override string GetSummaryMailNotificationMailBodyAsHtml(Sungero.Docflow.Structures.Module.IEmployeeMailInfo employeeMailInfo)
    {
      var employee = Employees.GetAll().Where(x => x.Id == employeeMailInfo.Id).FirstOrDefault();
      var assignmentsBlockContent = this.GetSummaryMailNotificationAssignmentsAndNoticesContentBlockAsHtml(Sungero.Docflow.Resources.AssignmentsBlockName,
                                                                                                           employeeMailInfo.AssignmentsAndNotices,
                                                                                                           employee,
                                                                                                           employeeMailInfo.PeriodLastDay);
      var actionItemBlockContent = this.GetSummaryMailNotificationTasksContentBlockAsHtml(Sungero.Docflow.Resources.ActionItemsBlockName,
                                                                                          employeeMailInfo.ActionItems);
      var taskBlockContent = this.GetSummaryMailNotificationTasksContentBlockAsHtml(Sungero.Docflow.Resources.TasksBlockName,
                                                                                    employeeMailInfo.Tasks);
      var model = this.GenerateSummaryBodyModel(assignmentsBlockContent, actionItemBlockContent, taskBlockContent);
      var template = MailTemplate.PublicFunctions.Template.GetSelectedTemplate(MailTemplate.Templates.Create());
      return this.GetMailBodyAsHtml(template.HtmlTemplate, model);
    }
  }
}