using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using aspNetCoreTicketSystem.Models;
using aspNetCoreTicketSystem.Services;

namespace aspNetCoreTicketSystem.Controllers
{
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
            return View(await _cosmosDbService.GetProjectsAsync("SELECT * FROM c WHERE c.projectDescription != null"));
        }

        [ActionName("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync([Bind("id,ProjectName,ProjectDescription,CompletedProj,StartDate,CompletionDate")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.ProjectId = Guid.NewGuid().ToString();
                await _cosmosDbService.AddProjectAsync(project);
                return RedirectToAction("Index");
            }

            return View(project);
        }


        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync([Bind("id,ProjectName,ProjectDescription,CompletedProj,StartDate,CompletionDate")] Project project, string id)
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
    }
}