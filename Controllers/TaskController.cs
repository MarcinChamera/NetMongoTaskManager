using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Models;
using TaskManager.Repositories;

namespace TaskManager.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private readonly ITaskRepository<TaskModel> _taskRepository;

        public TaskController(ITaskRepository<TaskModel> taskRepository)
        {
            _taskRepository = taskRepository;
        }

        // GET: Task
        public ActionResult Index()
        {
            return View(_taskRepository.FindAllActiveTasks());
        }

        // GET: Task/Details/5
        public ActionResult Details(string id)
        {
            return View(_taskRepository.FindById(id));
        }

        // GET: Task/Create
        public ActionResult Create()
        {
            return View(new TaskModel());
        }

        // POST: Task/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TaskModel taskModel)
        {
            _taskRepository.InsertOne(taskModel);

            return RedirectToAction(nameof(Index));
        }

        // GET: Task/Edit/5
        public ActionResult Edit(string id)
        {
            return View(_taskRepository.FindById(id));
        }

        // POST: Task/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, TaskModel taskModel)
        {
            _taskRepository.UpdateTask(id, taskModel);

            return RedirectToAction(nameof(Index));

        }

        // GET: Task/Delete/5
        public ActionResult Delete(string id)
        {
            _taskRepository.DeleteById(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: Task/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id, TaskModel taskModel)
        {
            _taskRepository.DeleteById(id);

            return RedirectToAction(nameof(Index));

        }

        // GET: Task/Done/5
        public ActionResult Done(string id)
        {
            TaskModel task = _taskRepository.FindById(id);
            task.Done = true;
            _taskRepository.UpdateTask(id, task);

            return RedirectToAction(nameof(Index));
        }
    }
}
