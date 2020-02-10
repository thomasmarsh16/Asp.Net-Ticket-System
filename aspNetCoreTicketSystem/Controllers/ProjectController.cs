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

            return View(await _cosmosDbService.GetProjectsAsync(userEmail));
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
                project.CompletedProj = false;
                project.projectWorkers = new List<string>();
                project.projectWorkers.Add(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value);
                await _cosmosDbService.AddProjectAsync(project);
                return RedirectToAction("Index");
            }

            return View(project);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync([Bind("id,ProjectName,ProjectDescription,CompletedProj,StartDate,CompletionDate,projectWorkers")] Project project, string id)
        {
            if (ModelState.IsValid)
            {
                project.ProjectId = id;
                await _cosmosDbService.UpdateProjectAsync(project.ProjectId, project);
                return Redirect("/Project/Index");
            }
            return View(project);
        }

        [ActionName("Edit")]
        public async Task<ActionResult> EditAsync(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Project task = await _cosmosDbService.GetProjectAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        [ActionName("Delete")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Project task = await _cosmosDbService.GetProjectAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedAsync([Bind("Id")] string id)
        {
            await _cosmosDbService.DeleteProjectAsync(id);
            return Redirect("/Project/Index");
        }

        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync(string id)
        {
            return Redirect("/Task/Index?id=" + id);
        }

        [ActionName("AddWorker")]
        public async Task<ActionResult> AddWorkerAsync(string id)
        {
            Project project = await _cosmosDbService.GetProjectAsync(id);

            return View(project);
        }

        [HttpPost]
        [ActionName("AddWorker")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddWorkerAsync([FromForm] string projectWorker, string projectID)
        {
            Project project = await _cosmosDbService.GetProjectAsync(projectID);

            if ( !project.projectWorkers.Contains(projectWorker) ) // if new worker not in project viewers update list.
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

            return Redirect("/Project/Index");
        }
    }
}