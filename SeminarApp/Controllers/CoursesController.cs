using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SeminarApp.Data;
using SeminarApp.Models;

namespace SeminarApp.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
        }

        //GET: Courses
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            var courses = from c in _context.Courses
                          select c;

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

        //GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Course course = await _context.Courses
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        //GET: Courses/Create
        public IActionResult Create()
        {
            return View();
        }

        //POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course course)
        {
            if (await TryUpdateModelAsync<Course>(
                course,
                "course",
                c => c.Title, c => c.Description, c => c.StartDate, c => c.IsFull))
            {
                try
                {
                    _context.Add(course);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persist, " +
                        "see your system administrator.");
                }
            }
            return View(course);
        }

        //GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //provjera id parametra
            if (id == null)
            {
                return NotFound();
            }
            // tražim tečaj po id
            Course course = await _context.Courses.FindAsync(id);
            //provjeravam jel postoji tečaj
            if (course == null)
            {
                return NotFound();
            }
            
            return View(course);
        }

        //POST: Courses/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null) //provjera id
            {
                return NotFound();
            }

            // tražimo course za update po id
            var courseToUpdate = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == id);

            if (await TryUpdateModelAsync<Course>(
                courseToUpdate,
                "",
                c => c.Title, c => c.Description, c => c.StartDate, c => c.IsFull))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException /* ex */)
                {
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persist, " +
                        "see your system administrator.");
                }
            }
            // ako ne uspije vrati View sa unešenim podacima
            return View(courseToUpdate);
        }

        //GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses            
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.CourseId == id);
            if (course == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }
            return View(course);
        }

        //POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }
    }
}