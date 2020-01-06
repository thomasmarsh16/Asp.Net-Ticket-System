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
    public class TaskController : Controller
    {
        private readonly ICosmosDbService _cosmosDbService;
        public TaskController(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [ActionName("Index")]
        public async Task<IActionResult> Index( string queryString )
        {
            return View(await _cosmosDbService.GetTasksAsync("SELECT * FROM c WHERE c.Name != \"\" AND c.projectName = \"" + queryString + "\""));
        }

        [ActionName("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync([Bind("Id,Name,Description,ProjectName,Completed,StartDate,CompletionDate")] ProjectTask task)
        {
            if (ModelState.IsValid)
            {
                task.Id = Guid.NewGuid().ToString();
                await _cosmosDbService.AddTaskAsync( task );
                return RedirectToAction("Index");
            }

            return View(task);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync([Bind("Id,Name,Description,ProjectName,Completed,StartDate,CompletionDate")] ProjectTask task)
        {
            if (ModelState.IsValid)
            {
                await _cosmosDbService.UpdateTaskAsync( task.Id, task );
                return RedirectToAction("Index");
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
        public async Task<ActionResult> DeleteAsync(string id)
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
        public async Task<ActionResult> DeleteConfirmedAsync([Bind("Id")] string id)
        {
            await _cosmosDbService.DeleteTaskAsync(id);
            return RedirectToAction("Index");
        }

        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync(string id)
        {
            return View(await _cosmosDbService.GetTaskAsync(id));
        }
    }
}