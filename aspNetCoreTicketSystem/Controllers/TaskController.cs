﻿using System;
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
            string userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            Project temp = await _cosmosDbService.GetProjectAsync(id);
            List<ProjectTask> sortedTasks = TaskMethods.FilterTasksByWorkerEmail( await _cosmosDbService.GetTasksAsync(id), userEmail );

            List<int> categoryNumbers = TaskMethods.CategorizeTasks(sortedTasks);
            Dictionary<string, int> tempDict = TaskMethods.CountCompletionDatesByMonth(sortedTasks);

            ViewData["completionDates"] = TaskMethods.FormatListForView( tempDict.Keys.ToList() );
            ViewData["completionNumbers"] = TaskMethods.FormatListForView( tempDict.Values.ToList() );

            ViewData["isManager"] = ProjectMethods.isManager(temp,userEmail);
            ViewData["projectName"] = temp.ProjectName;
            ViewData["projectID"] = temp.ProjectId;
            ViewData["pieChart"] = TaskMethods.FormatListForView( categoryNumbers );
            
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
                task.dueDate = task.dueDate.Date;
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

                    if ( !task.checkoutName.Equals(User.Identity.Name))
                    {
                        task.checkoutName = task.checkoutName + ", " + User.Identity.Name;
                    }
                    
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
            ProjectTask currentTask = await _cosmosDbService.GetTaskAsync(id);
            List<Comment> sortedComments = await _cosmosDbService.GetCommentsAsync(id);

            if ( sortedComments != null )
            {
                ViewData["taskComments"] = sortedComments;
            }

            return View(currentTask );
        }

        [HttpPost]
        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync([FromForm] string comment, [FromForm] string taskID)
        {
            Comment tempCom = new Comment();

            tempCom.Id = Guid.NewGuid().ToString();
            tempCom.CommentString = comment;
            tempCom.CreatorName = User.Identity.Name;
            tempCom.TaskID = taskID;
            tempCom.DateTimeCreated = DateTime.Now;

            await _cosmosDbService.AddCommentAsync(tempCom);

            return Redirect("/Task/Details?id=" + taskID);
        }

        [ActionName("AddWorker")]
        public async Task<ActionResult> AddWorkerAsync(string id, string projectID)
        {
            ProjectTask task = await _cosmosDbService.GetTaskAsync(id);
            Project project = await _cosmosDbService.GetProjectAsync(task.ProjectID);
            String userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (ProjectMethods.isManager(project, userEmail))
            {
                foreach (string worker in task.taskWorkers)
                {
                    project.projectWorkers.Remove(worker);
                }

                ViewData["possibleWorkers"] = project.projectWorkers;


                return View(task);
            }

            return Redirect("/Home/Error");
        }

        [HttpPost]
        [ActionName("AddWorker")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddWorkerAsync([FromForm] string taskWorkers, string id, string projectID)
        {
            Project project = await _cosmosDbService.GetProjectAsync(projectID);
            String userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            ProjectTask task = await _cosmosDbService.GetTaskAsync(id);

            if (ProjectMethods.isManager(project, userEmail))
            {
                task.taskWorkers.Add(taskWorkers);

                await _cosmosDbService.UpdateTaskAsync(task.Id, task);

                return Redirect("/Task/Index?id=" + projectID);
            }

            return Redirect("/Home/Error");

        }
    }
}