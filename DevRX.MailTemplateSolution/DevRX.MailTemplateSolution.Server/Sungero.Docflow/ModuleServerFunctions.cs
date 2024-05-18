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

    /// <summary>
    /// 
    /// </summary>
    [Remote]
    public virtual void Test()
    {
    //  SendSummaryMailNotification();
    var template = MailTemplate.PublicFunctions.Template.GetSelectedTemplate(MailTemplate.Templates.Create());
     var model = new Dictionary<string, object>();
     SendMailByTemplate("distancemezhin@gmail.com",template.HtmlTemplate,model);
    }
    
    public override string GenerateBody(IAssignmentBase assignment, bool isExpired, bool hasSubstitutions)
    {
      if (!Nustache.Core.Helpers.Contains("process_text"))
        Nustache.Core.Helpers.Register("process_text", ProcessText);
      
      var model = this.GenerateBodyModel(assignment, isExpired, hasSubstitutions);
      var template = MailTemplate.PublicFunctions.Template.GetSelectedTemplate(MailTemplate.Templates.Create());
      return this.GetMailBodyAsHtml(template.HtmlTemplate, model);
    }
    
    public Sungero.Core.IEmailMessage CreateMessageWithLogo()
    {
      var message = Mail.CreateMailMessage();
      this.AddLogo(message);
      return message;
    }
    
      [Public]
    public void SendMailByTemplate(string eMail, string template, System.Collections.Generic.Dictionary<string, object> model)
    {
      var message = Mail.CreateMailMessage();
      this.AddLogo(message);
      var body = GetMailBodyAsHtml(template, model);
      //MailTemplateSolution.Module.Docflow.PublicFunctions.Module.GetAttachment();
      message.Body = body;
      message.IsBodyHtml = true;
      message.Subject = "Тестовое Письмо";
      message.To.Add(eMail);
     // message.CC.AddRange(copy);
     
   //MailTemplateSolution.Module.Docflow.PublicFunctions.AddLogo(message);
   //   var fileStream = File.OpenRead(@"C:\Users\DirectumRobot\Downloads\windowsdesktop-runtime-6.0.29-win-x64.exe");
  //    var attachmentName = Path.GetFileName(@"C:\Users\DirectumRobot\Downloads\windowsdesktop-runtime-6.0.29-win-x64.exe");
      // Добавить вложение в виде потока с данными из файла в письмо.
   //   message.AddAttachment(fileStream, attachmentName);
      try {
      Mail.Send(message);
      }
      catch (Exception Ex)
      {
        Mail.Send(message);
      }
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
       
   public override List<Sungero.Docflow.Structures.Module.IEmployeeMailInfo> CreateEmployeesMailInfoToSendSummaryNotification()
    {
      var employees = this.GetEmployeesToSendSummaryNotification();
      var substitutions = this.GetSubstitutionsToSendSummaryNotification(employees);
      employees.AddRange(this.GetSubstitutorNeedSummaryNotificationEmployees(substitutions));
      
      var mailingList = new List<Sungero.Docflow.Structures.Module.IEmployeeMailInfo>();
      foreach (var employee in employees)
      {
        try
        {
          var mailInfo = Sungero.Docflow.Structures.Module.EmployeeMailInfo.Create();
          mailInfo.Id = employee.Id;
          mailInfo.Email = employee.Email;
          mailInfo.EmployeeShortName = Sungero.Company.PublicFunctions.Employee.GetShortName(employee, Sungero.Core.DeclensionCase.Genitive, false);
          mailInfo.LastWorkingDay = Calendar.Now.IsWorkingTime(employee) ? Calendar.Now : Calendar.Now.AddWorkingHours(employee, -1).EndOfWorkingDay();
          mailInfo.PeriodFirstDay = Calendar.GetUserToday(employee).AddWorkingDays(-2).BeginningOfDay();
          mailInfo.PeriodLastDay = Calendar.GetUserToday(employee).AddWorkingDays(this.GetSummaryMailNotificationClosestDaysCount()).EndOfDay();
          mailInfo.SubstitutorEmails = substitutions.Where(s => s.SubstitutedId == employee.Id).Select(s => s.SubstitutorEmail).ToList();
          mailInfo.NeedNotifyAssignmentsSummary = employee.NeedNotifyAssignmentsSummary == true;
          mailInfo.EmployeeCurrentDate = Calendar.GetUserToday(employee);
          mailInfo.AssignmentsAndNotices = new List<Sungero.Docflow.Structures.Module.IWorkflowEntityMailInfo>();
          mailInfo.ActionItems = new List<Sungero.Docflow.Structures.Module.IWorkflowEntityMailInfo>();
          mailInfo.Tasks = new List<Sungero.Docflow.Structures.Module.IWorkflowEntityMailInfo>();
          mailingList.Add(mailInfo);
        }
        catch (Exception ex)
        {
          this.SummaryMailLogError(string.Format("CreateEmployeesMailInfoToSendSummaryNotification. Employee (ID = {0})", employee.Id), ex);
        }
      }
      
      return mailingList;
    }
  
   [Public]
   public override string GetMailBodyAsHtml(string template, System.Collections.Generic.Dictionary<string, object> model)
   {
     if (string.IsNullOrEmpty(template) || model == null)
       return string.Empty;
     
     return Nustache.Core.Render.StringToString(template, model,
                                                new Nustache.Core.RenderContextBehaviour() { OnException = ex => Logger.Error(ex.Message, ex) });
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