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
        public IActionResult Index()
        {
            return View();
        }

        [ActionName("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync([Bind("Id,ProjectName,ProjectDescription,Completed,StartDate,CompletionDate")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.ProjectId = Guid.NewGuid().ToString();
                await _cosmosDbService.AddProjectAsync(project);
                return RedirectToAction("Index");
            }

            return View(project);
        }
    }
}