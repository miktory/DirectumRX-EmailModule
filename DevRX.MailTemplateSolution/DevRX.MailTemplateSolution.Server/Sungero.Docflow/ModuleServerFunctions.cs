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
using Sungero.Docflow;
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
    [Public, Remote]
    public void Test1()
    {
        this.SendMailNotification();
    }
    
    public override string GenerateBody(IAssignmentBase assignment, bool isExpired, bool hasSubstitutions)
    {
      if (!Nustache.Core.Helpers.Contains("process_text"))
        Nustache.Core.Helpers.Register("process_text", ProcessText);
      
      var model = this.GenerateBodyModel(assignment, isExpired, hasSubstitutions);
      var template = MailTemplate.Templates.GetAll(r => Equals(r.Name, "MailTemplate")).FirstOrDefault();
      if (template == MailTemplate.Templates.Null)
        return this.GetMailBodyAsHtml(Docflow.Resources.MailTemplate, model);
      else
        return this.GetMailBodyAsHtml(template.HtmlTemplate, model);
    }
       
           /// <summary>
    /// Запустить рассылку по новым заданиям.
    /// </summary>
    /// <param name="previousRun">Дата прошлого запуска рассылки.</param>
    /// <param name="notificationDate">Дата текущей рассылки.</param>
    /// <param name="assignments">Задания, по которым будет выполнена рассылка.</param>
    /// <returns>True, если хотя бы одно письмо было отправлено, иначе - false.</returns>
    public bool? TrySendNewAssignmentsMailing1(DateTime previousRun, DateTime notificationDate, List<IAssignmentBase> assignments)
    {
      Logger.Debug("Checking new assignments for mailing");
      var hasErrors = false;

      var anyMailSent = false;
      foreach (var assignment in assignments)
      {
        var employee = Employees.As(assignment.Performer);
        if (employee == null)
          continue;

        var endDate = assignment.Created.Value.AddDays(-1);
        var substitutions = Substitutions.GetAll(s => Equals(s.User, employee))
          .Where(s => s.IsSystem != true)
          .Where(s => s.StartDate == null || s.StartDate.Value <= assignment.Created)
          .Where(s => s.EndDate == null || s.EndDate.Value >= endDate);
        
        var substitutes = Employees.GetAll(r => r.NeedNotifyNewAssignments == true)
          .Where(e => substitutions.Any(s => Equals(s.Substitute, e)))
          .Where(e => e.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed)
          .ToList();
        
        var subject = this.GetNewAssignmentSubject(assignment);
        var mailSent = this.TrySendMailByAssignment(assignment, subject, false, employee, substitutes);
        if (!mailSent.IsSuccess)
          hasErrors = true;
        if (mailSent.IsSuccess && mailSent.AnyMailSended)
          anyMailSent = true;
      }
      if (!assignments.Any())
        Logger.Debug("No new assignments for mailing");
      else if (!anyMailSent)
        Logger.Debug("No subscribers for new assignments mailing");
      if (!anyMailSent && !hasErrors)
        return null;
      return anyMailSent || !hasErrors;
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
  
       
          
 /// <summary>
    /// Получить тело письма со сводкой по сотруднику в виде HTML.
    /// </summary>
    /// <param name="employeeMailInfo">Информация по сотруднику.</param>
    /// <returns>Тело письма со сводкой по сотруднику в виде HTML.</returns>
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
      var template = MailTemplate.Templates.GetAll(r => Equals(r.Name, "SummaryMailMainTemplate")).FirstOrDefault();
      if (template == MailTemplate.Templates.Null)
        return this.GetMailBodyAsHtml(Docflow.Resources.SummaryMailMainTemplate, model);
      else
        return this.GetMailBodyAsHtml(template.HtmlTemplate, model);
    }
     
       /// <summary>
    /// Получить содержание блока сводки с задачами в виде HTML.
    /// </summary>
    /// <param name="blockName">Заголовок блока.</param>
    /// <param name="tasks">Задачи.</param>
    /// <returns>Содержание блока сводки с задачами в виде HTML.</returns>
    public override string GetSummaryMailNotificationTasksContentBlockAsHtml(string blockName,
                                                                            List<Sungero.Docflow.Structures.Module.IWorkflowEntityMailInfo> tasks)
    {
      if (blockName == null || tasks == null || !tasks.Any())
        return string.Empty;

      var tasksListContent = this.GetSummaryMailNotificationWorkflowEntitiesListContentAsHtml(tasks);
      var model = this.GenerateBlockContentModel(blockName, tasksListContent, tasks.Count, false, 0);
      var template = MailTemplate.Templates.GetAll(r => Equals(r.Name, "SummaryMailBlockContentTemplate")).FirstOrDefault();
      if (template == MailTemplate.Templates.Null)
        return this.GetMailBodyAsHtml(Docflow.Resources.SummaryMailBlockContentTemplate, model);
      else
        return this.GetMailBodyAsHtml(template.HtmlTemplate, model);
    }
     
       /// <summary>
    /// Получить содержание блока сводки с заданиями и уведомлениями в виде HTML.
    /// </summary>
    /// <param name="blockName">Заголовок блока.</param>
    /// <param name="assignmentsAndNotices">Задания и уведомлния.</param>
    /// <param name="employee">Сотрудник для которого формируется сводка.</param>
    /// <param name="periodLastDay">Срок, после которого задание не должно попадать в сводку.</param>
    /// <returns>Содержание блока сводки с заданиями и уведомлениями в виде HTML.</returns>
    public override string GetSummaryMailNotificationAssignmentsAndNoticesContentBlockAsHtml(string blockName,
                                                                                            List<Sungero.Docflow.Structures.Module.IWorkflowEntityMailInfo> assignmentsAndNotices,
                                                                                            IEmployee employee,
                                                                                            DateTime? periodLastDay)
    {
      if (assignmentsAndNotices == null || !assignmentsAndNotices.Any())
        return string.Empty;
      
      var assignmentsAndNoticesCount = 0;
      var groupsContent = new List<string>();
      var groups = this.GetSummaryMailNotificationAssignmentsAndNoticesGroups(assignmentsAndNotices, employee, periodLastDay);
      foreach (var group in groups)
        if (group.Value.Any())
      {
        groupsContent.Add(this.GetSummaryMailNotificationGroupContentAsHtml(group.Key, group.Value.OrderByDescending(a => a.Created).ToList()));
        assignmentsAndNoticesCount += group.Value.Count;
      }
      
      if (!groupsContent.Any())
        return string.Empty;
      
      var groupContent = string.Join(Environment.NewLine, groupsContent);
      var model = this.GenerateBlockContentModel(blockName, groupContent, assignmentsAndNoticesCount, false, 0);
      var template = MailTemplate.Templates.GetAll(r => Equals(r.Name, "SummaryMailBlockContentTemplate")).FirstOrDefault();
      if (template == MailTemplate.Templates.Null)
        return this.GetMailBodyAsHtml(Docflow.Resources.SummaryMailBlockContentTemplate, model);
      else
        return this.GetMailBodyAsHtml(template.HtmlTemplate, model);
    }
    
       /// <summary>
    /// Получить содержание группы в виде HTML.
    /// </summary>
    /// <param name="name">Название группы.</param>
    /// <param name="entities">Список информации о сущностях.</param>
    /// <returns>Содержание группы в виде HTML.</returns>
    public override string GetSummaryMailNotificationGroupContentAsHtml(string name, List<Sungero.Docflow.Structures.Module.IWorkflowEntityMailInfo> entities)
    {
      var entitiesGroupContent = this.GetSummaryMailNotificationWorkflowEntitiesListContentAsHtml(entities);
      var leftMarginSize = this.GetSummaryMailLeftMarginSize();
      var model = this.GenerateBlockContentModel(name, entitiesGroupContent, entities.Count, true, leftMarginSize);
      var template = MailTemplate.Templates.GetAll(r => Equals(r.Name, "SummaryMailGroupContentTemplate")).FirstOrDefault();
      if (template == MailTemplate.Templates.Null)
        return this.GetMailBodyAsHtml(Docflow.Resources.SummaryMailGroupContentTemplate, model);
      else 
        return this.GetMailBodyAsHtml(template.HtmlTemplate, model);
    }
  }
}