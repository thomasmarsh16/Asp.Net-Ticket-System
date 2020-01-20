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

namespace aspNetCoreTicketSystem.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private readonly ICosmosDbService _cosmosDbService;
        public TaskController(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        
        [ActionName("Index")]
        public async Task<IActionResult> Index( string id )
        {
            Project temp = await _cosmosDbService.GetProjectAsync(id);
            ViewData["projectName"] = temp.ProjectName;
            ViewData["projectID"] = temp.ProjectId;
            return View(await _cosmosDbService.GetTasksAsync("SELECT * FROM c WHERE c.projectID = \"" + id + "\""));
        }

        [ActionName("Create")]
        public async Task<IActionResult> Create(string id)
        {
            Project temp = await _cosmosDbService.GetProjectAsync(id);
            ViewData["projectName"] = temp.ProjectName;
            ViewData["projectID"] = temp.ProjectId;
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync([Bind("Id,taskName,Description,ProjectID,Completed,StartDate,CompletionDate")] ProjectTask task, string id)
        {
            if (ModelState.IsValid)
            {
                task.Id = Guid.NewGuid().ToString();
                task.ProjectID = id;
                await _cosmosDbService.AddTaskAsync( task );
                return Redirect("/Task/Index?id=" + task.ProjectID);
            }

            return View(task);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync([Bind("Id,taskName,Description,ProjectID,Completed,StartDate,CompletionDate")] ProjectTask task, string id)
        {
            if (ModelState.IsValid)
            {
                await _cosmosDbService.UpdateTaskAsync(task.Id, task);
                return Redirect("/Task/Index?id=" + task.ProjectID);
            }
                return View(task);
        }

        [ActionName("Edit")]
        public async Task<ActionResult> EditAsync(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            ProjectTask task = await _cosmosDbService.GetTaskAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        [ActionName("Delete")]
        public async Task<ActionResult> DeleteAsync(string id, string projectID)
        {
            if (id == null)
            {
                return BadRequest();
            }

            ProjectTask task = await _cosmosDbService.GetTaskAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedAsync([Bind("Id")] string id, string projectID)
        {
            await _cosmosDbService.DeleteTaskAsync(id);
            return Redirect("/Task/Index?id=" + projectID);
        }

        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync(string id)
        {
            return View(await _cosmosDbService.GetTaskAsync(id));
        }
    }
}