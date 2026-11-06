using ioschools.Application.DTOs;
using ioschools.Application.Interfaces;
using ioschools.Domain.Enums;
using ioschools.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ioschools.Web.Areas.Exams.Controllers
{
    /// <summary>
    /// Exams area controller - migrated from legacy examsController in Areas/exams
    /// Replaces: AreaRegistration → Area attribute routing
    /// </summary>
    [Area("Exams")]
    [Authorize]
    public class ExamsController : Web.Controllers.BaseController
    {
        private readonly IExamService _examService;

        public ExamsController(IExamService examService, ILogger<ExamsController> logger) : base(logger)
        {
            _examService = examService;
        }

        [PermissionFilter(Permission.EXAM_CREATE | Permission.EXAM_ADMIN)]
        public IActionResult Add() => View();

        [HttpPost]
        [PermissionFilter(Permission.EXAM_CREATE | Permission.EXAM_ADMIN)]
        public async Task<IActionResult> Delete(long id)
        {
            if (!SessionId.HasValue) return SendJsonNoPermission();
            var result = await _examService.DeleteExamAsync(id, SessionId.Value, CurrentUserAuth);
            if (!result.IsSuccess) return SendJsonErrorResponse(result.Error ?? "Failed to delete exam");
            return Json(JsonResponse.Ok("Exam deleted successfully"));
        }

        public async Task<IActionResult> Index()
        {
            var exams = await _examService.GetExamsAsync(DateTime.Now.Year);
            return View(exams);
        }

        public async Task<IActionResult> Single(long id)
        {
            var exam = await _examService.GetExamAsync(id);
            if (exam == null) return ReturnNotFoundView();
            return View(exam);
        }

        public async Task<IActionResult> Show(long id)
        {
            var exam = await _examService.GetExamAsync(id);
            if (exam == null) return ReturnNotFoundView();
            return View(exam);
        }

        [PermissionFilter(Permission.EXAM_ADMIN)]
        public IActionResult Edit(long id) => View();

        [PermissionFilter(Permission.EXAM_ADMIN)]
        public IActionResult Remarks(long id) => View();

        [PermissionFilter(Permission.EXAM_ADMIN)]
        public IActionResult ReportCard(long id) => View();
    }

    /// <summary>
    /// Templates controller in exams area - migrated from legacy templatesController
    /// </summary>
    [Area("Exams")]
    [Authorize]
    public class TemplatesController : Web.Controllers.BaseController
    {
        public TemplatesController(ILogger<TemplatesController> logger) : base(logger) { }

        [PermissionFilter(Permission.EXAM_ADMIN)]
        public IActionResult Index() => View();

        [PermissionFilter(Permission.EXAM_ADMIN)]
        public IActionResult Add() => View();

        [PermissionFilter(Permission.EXAM_ADMIN)]
        public IActionResult Details(long id) => View();

        [PermissionFilter(Permission.EXAM_ADMIN)]
        public IActionResult Edit(long id) => View();
    }
}
