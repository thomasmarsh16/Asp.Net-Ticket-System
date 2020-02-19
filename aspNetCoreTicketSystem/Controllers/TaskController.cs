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
            List<ProjectTask> sortedTasks = await _cosmosDbService.GetTasksAsync(id);

            int numLow = 0;
            int numMed = 0;
            int numHigh = 0;

            foreach( ProjectTask task in sortedTasks )
            {
                switch(task.Importance)
                {
                    case "Low":
                        numLow++;
                        break;
                    case "Medium":
                        numMed++;
                        break;
                    case "High":
                        numHigh++;
                        break;
                }
            }

            ViewData["projectName"] = temp.ProjectName;
            ViewData["projectID"] = temp.ProjectId;
            ViewData["pieChart"] = "[\"" + numLow +"\",\"" + numMed + "\",\"" + numHigh + "\"]";
            
            return View(sortedTasks);
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
        public async Task<ActionResult> CreateAsync([Bind("Id,taskName,Description,Importance,ProjectID,StartDate,dueDate")] ProjectTask task, string id)
        {
            if (ModelState.IsValid)
            {
                task.Id = Guid.NewGuid().ToString();
                task.ProjectID = id;
                task.Completed = false;
                task.taskWorkers = new List<string>();
                task.taskWorkers.Add( User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value );
                await _cosmosDbService.AddTaskAsync( task );
                return Redirect("/Task/Index?id=" + task.ProjectID);
            }

            return View(task);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync([Bind("Id,taskName,Description,Importance,ProjectID,Completed,StartDate,dueDate,taskWorkers")] ProjectTask task)
        {
            if (ModelState.IsValid)
            {
                if (task.Completed == false )
                {
                    task.CompletionDate = new DateTime();
                    task.checkoutName = "";
                }
                else
                {
                    task.CompletionDate = DateTime.Now;
                    task.checkoutName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                }
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

        [ActionName("AddWorker")]
        public async Task<ActionResult> AddWorkerAsync(string id, string projectID)
        {
            ProjectTask task = await _cosmosDbService.GetTaskAsync(id);

            return View(task);
        }

        [HttpPost]
        [ActionName("AddWorker")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddWorkerAsync([FromForm] string taskWorkers, string id, string projectID)
        {
            ProjectTask task = await _cosmosDbService.GetTaskAsync(id);

            task.taskWorkers.Add(taskWorkers);

            await _cosmosDbService.UpdateTaskAsync(task.Id, task);

            return Redirect("/Task/Index?id=" + projectID);
        }
    }
}