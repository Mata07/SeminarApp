using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeminarApp.Data;

namespace SeminarApp.Controllers
{
    public class EnrollController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EnrollController(ApplicationDbContext context)
        {
            _context = context;
        }

        //GET: Enrollments
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            //sort by title and date columns
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            //za search
            ViewData["CurrentFilter"] = searchString;

            //get courses from dbContext
            var courses = from c in _context.Courses
                          select c;

            //select only courses that are not full
            courses = courses.Where(c => c.IsFull == false);

            //search
            if (!String.IsNullOrEmpty(searchString))
            {
                courses = courses.Where(c => c.Title.Contains(searchString));
            }

            //sort by title and date columns
            switch (sortOrder)
            {
                case "title_desc":
                    courses = courses.OrderByDescending(c => c.Title);
                    break;
                case "Date":
                    courses = courses.OrderBy(c => c.StartDate);
                    break;
                case "date_desc":
                    courses = courses.OrderByDescending(c => c.StartDate);
                    break;
                default:
                    courses = courses.OrderBy(c => c.Title);
                    break;
            }

            return View(await courses.AsNoTracking().ToListAsync());
        }
    }
}