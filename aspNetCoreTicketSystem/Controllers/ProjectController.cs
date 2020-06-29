using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using aspNetCoreTicketSystem.Models;
using aspNetCoreTicketSystem.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace aspNetCoreTicketSystem.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private readonly ICosmosDbService _cosmosDbService;
        public ProjectController(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [ActionName("Index")]
        public async Task<IActionResult> Index()
        {
            string userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            ViewData["UserEmail"] = userEmail;
            return View(await _cosmosDbService.GetProjectsAsync(userEmail));
        }

        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync(string id)
        {
            return Redirect("/Task/Index?id=" + id);
        }

        [ActionName("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync([Bind("id,ProjectName,ProjectDescription,StartDate,CompletionDate")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.ProjectId = Guid.NewGuid().ToString();
                project.ProjectManager = new List<string>();
                project.ProjectManager.Add( User.Identity.Name + ", " + User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value);
                project.CompletedProj = false;
                project.projectWorkers = new List<string>();
                project.projectWorkers.Add(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value);
                await _cosmosDbService.AddProjectAsync(project);
                return RedirectToAction("Index");
            }

            return View(project);
        }

        [ActionName("Edit")]
        public async Task<ActionResult> EditAsync(string id)
        {
            Project project = await _cosmosDbService.GetProjectAsync(id);
            String userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (project == null)
            {
                return NotFound();
            }

            if (ProjectMethods.isManager(project, userEmail))
            {

                return View(project);
            }

            return Redirect("/Home/Error");
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync([Bind("id,ProjectName,ProjectDescription,CompletedProj,StartDate,CompletionDate,projectWorkers,ProjectManager")] Project project, string id)
        {
            Project projectCompare = await _cosmosDbService.GetProjectAsync(id);
            String userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (project == null)
            {
                return NotFound();
            }

            if (ProjectMethods.isManager(projectCompare, userEmail))
            {
                if (ModelState.IsValid)
                {
                    project.ProjectId = id;
                    await _cosmosDbService.UpdateProjectAsync(project.ProjectId, project);
                    return Redirect("/Project/Index");
                }
                return View(project);
            }

            return Redirect("/Home/Error");
        }

        [ActionName("Delete")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            Project project = await _cosmosDbService.GetProjectAsync(id);
            String userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (project == null)
            {
                return NotFound();
            }

            if (ProjectMethods.isManager(project, userEmail))
            {
                return View(project);
            }

            return Redirect("/Home/Error");
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedAsync([Bind("Id")] string id)
        {
            Project project = await _cosmosDbService.GetProjectAsync(id);
            String userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (project == null)
            {
                return NotFound();
            }

            if (ProjectMethods.isManager(project, userEmail))
            {
                await _cosmosDbService.DeleteProjectAsync(id);
                return Redirect("/Project/Index");
            }

            return Redirect("/Home/Error");
        }

        [ActionName("AddWorker")]
        public async Task<ActionResult> AddWorkerAsync(string id)
        {
            Project project = await _cosmosDbService.GetProjectAsync(id);
            String userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (project == null)
            {
                return NotFound();
            }

            if (ProjectMethods.isManager(project, userEmail))
            {
                List<ProjectTask> taskList = await _cosmosDbService.GetTasksAsync(project.ProjectId);
                Dictionary<string, int> tempDict = TaskMethods.CountCompletionDatesByMonth(taskList);
                List<int> categoryNumbers = TaskMethods.CategorizeTasks(taskList);

                // values for completion and pie charts
                ViewData["completionNumbers"] = TaskMethods.FormatListForView(tempDict.Values.ToList());
                ViewData["completionDates"] = TaskMethods.FormatListForView(tempDict.Keys.ToList());
                ViewData["pieChart"] = TaskMethods.FormatListForView(categoryNumbers);

                return View(project);
            }

            return Redirect("/Home/Error");
        }

        [HttpPost]
        [ActionName("AddWorker")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddWorkerAsync([FromForm] string projectWorker, string projectID, [FromForm] string viewerEmail, [FromForm] string managerEmail)
        {
            Project project = await _cosmosDbService.GetProjectAsync(projectID);
            String userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (project == null)
            {
                return NotFound();
            }

            if (ProjectMethods.isManager(project, userEmail))
            {
                if (projectWorker != null) // action is to remove project viewer
                {
                    if (project.projectWorkers.Contains(viewerEmail))
                    {
                        project.projectWorkers.Remove(viewerEmail);
                        await _cosmosDbService.UpdateProjectAsync(project.ProjectId, project);

                        var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
                        var client = new SendGridClient(apiKey);
                        var from = new EmailAddress(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value, User.Identity.Name);
                        var subject = "You have been taken off of a tickbox project";
                        var to = new EmailAddress(projectWorker, "");
                        var plainTextContent = User.Identity.Name + " has taken you off of project " + project.ProjectName + " on tickbox.";
                        var htmlContent = "link to all of your projects - - https://tickbox.azurewebsites.net/Project/Index";
                        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                        var response = await client.SendEmailAsync(msg);
                    }
                }
                else if ( managerEmail.Length > 0 && managerEmail != null )// action is to make viewer into a manager
                { 
                    if ( !ProjectMethods.isManager( project, managerEmail ) ) // prevent adding an existing manager
                    {
                        project.ProjectManager.Add(managerEmail);

                        await _cosmosDbService.UpdateProjectAsync(project.ProjectId, project);

                        var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
                        var client = new SendGridClient(apiKey);
                        var from = new EmailAddress(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value, User.Identity.Name);
                        var subject = "You have been made a manager of a tickbox project";
                        var to = new EmailAddress(projectWorker, "");
                        var plainTextContent = User.Identity.Name + " has made you a manager for project " + project.ProjectName + " on tickbox.";
                        var htmlContent = "link to all of your projects - - https://tickbox.azurewebsites.net/Project/Index";
                        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                        var response = await client.SendEmailAsync(msg);
                    }
                }
                else // action is to add project viewer
                { 
                    if (!project.projectWorkers.Contains(projectWorker)) // if new worker not in project viewers update list.
                    {
                        project.projectWorkers.Add(projectWorker);
                        await _cosmosDbService.UpdateProjectAsync(project.ProjectId, project);

                        var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
                        var client = new SendGridClient(apiKey);
                        var from = new EmailAddress(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value, User.Identity.Name);
                        var subject = "You have been invited to work on a tickbox project";
                        var to = new EmailAddress(projectWorker, "");
                        var plainTextContent = "Congradulations, " + User.Identity.Name + " has invited you to work on a project at tickbox, click the link to view the project ";
                        var htmlContent = "link to all of your projects - - https://tickbox.azurewebsites.net/Project/Index";
                        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                        var response = await client.SendEmailAsync(msg);
                    }
                }

                return Redirect("/Project/AddWorker/" + project.ProjectId.ToString());
            }

            return Redirect("/Home/Error");
        }

        [ActionName("WorkerDetails")]
        public async Task<ActionResult> WorkerDetailsAsync(string id, string email)
        {
            Project project = await _cosmosDbService.GetProjectAsync(id);
            String userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (project == null)
            {
                return NotFound();
            }

            if (ProjectMethods.isManager(project, userEmail))
            {
                List<ProjectTask> taskList = await _cosmosDbService.GetTasksAsync(id);
                List<ProjectTask> workerSpecificTasks = TaskMethods.FilterTasksByWorkerEmail(taskList, email);

                Dictionary<string, int> tempDict = TaskMethods.CountCompletionDatesByMonth(workerSpecificTasks);

                List<int> categoryNumbers = TaskMethods.CategorizeTasks(workerSpecificTasks);

                ViewData["completionDates"] = TaskMethods.FormatListForView(tempDict.Keys.ToList());
                ViewData["completionNumbers"] = TaskMethods.FormatListForView(tempDict.Values.ToList());
                ViewData["workerEmail"] = email;
                ViewData["projectID"] = project.ProjectId;
                ViewData["projectName"] = project.ProjectName;
                ViewData["tasks"] = taskList;
                ViewData["isManager"] = true;

                ViewData["pieChart"] = TaskMethods.FormatListForView(categoryNumbers);

                return View(workerSpecificTasks);
            }

            return Redirect("/Home/Error");
        }

        [HttpPost]
        [ActionName("WorkerDetails")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> WorkerDetailsAsync(string id, string email, [FromForm] string addedTasks, [FromForm] string removedTasks)
        {
            Project project = await _cosmosDbService.GetProjectAsync(id);
            String userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (project == null)
            {
                return NotFound();
            }

            if (ProjectMethods.isManager(project, userEmail))
            {
                if (addedTasks != null && !addedTasks.Equals(""))  // add tasks
                {
                    List<string> tasksAdd = addedTasks.Split(",").ToList();

                    foreach (string taskId in tasksAdd)
                    {
                        ProjectTask temp = await _cosmosDbService.GetTaskAsync(taskId);

                        if ( temp.taskWorkers == null )
                        {
                            List<string> tempList = new List<string>();

                            tempList.Add(email);

                            temp.taskWorkers = tempList;
                        }
                        else
                        {
                            temp.taskWorkers.Add(email);
                        }

                        await _cosmosDbService.UpdateTaskAsync(temp.Id, temp);

                        var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY"); // send notification email
                        var client = new SendGridClient(apiKey);
                        var from = new EmailAddress(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value, User.Identity.Name);
                        var subject = "You have been assigned a tickbox project task";
                        var to = new EmailAddress(email, "");
                        var plainTextContent = "Congradulations, " + User.Identity.Name + " has assigned you a task for a project at tickbox";
                        var htmlContent = "link to all of your projects - - https://tickbox.azurewebsites.net/Project/Index";
                        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                        var response = await client.SendEmailAsync(msg);
                    }
                }

                if (removedTasks != null && !removedTasks.Equals(""))  // remove tasks
                {
                    List<string> tasksRemove = removedTasks.Split(",").ToList();

                    foreach (string taskId in tasksRemove)
                    {
                        ProjectTask temp = await _cosmosDbService.GetTaskAsync(taskId);

                        temp.taskWorkers.Remove(email);

                        await _cosmosDbService.UpdateTaskAsync(temp.Id, temp);

                        var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY"); // send notification email
                        var client = new SendGridClient(apiKey);
                        var from = new EmailAddress(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value, User.Identity.Name);
                        var subject = "You have been unassigned from a tickbox project task";
                        var to = new EmailAddress(email, "");
                        var plainTextContent = "Dear User, " + User.Identity.Name + " has unassigned you from a task for a project at tickbox";
                        var htmlContent = "link to all of your projects - - https://tickbox.azurewebsites.net/Project/Index";
                        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                        var response = await client.SendEmailAsync(msg);
                    }
                }

                return Redirect("/Project/WorkerDetails/" + project.ProjectId + "?email=" + email);
            }

            return Redirect("/Home/Error");
        }
    }
}