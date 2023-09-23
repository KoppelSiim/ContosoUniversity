using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;

namespace ContosoUniversity.Controllers
{
    public class StudentsController : Controller
    {
        private readonly SchoolContext _context;

        public StudentsController(SchoolContext context)
        {
            _context = context;
        }

        // Implementing sorting functionality.
        // Receave sortOrder parameter from the query string in the URL.
        public async Task<IActionResult> Index(string sortOrder)
        {
            // Sets a value in the ViewData dictionary, if sortOrder is null or empty,
            // it sets it to "name_desc", otherwise, it sets it to an empty string.
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            // Sets the sorting parameter for the "Date" column. If sortOrder is equal to "Date,"
            // it sets it to "date_desc", otherwise, it sets it to "Date."
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            // Select all students and store them in the students variable.
            var students = from s in _context.Students
                           select s;


            // The first time the Index page is requested, there's no query string.
            // The students are displayed in ascending order by last name, which is the default
            // as established by the fall-through case in the switch statement.
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.EnrollmentDate);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:
                    students = students.OrderBy(s => s.LastName);
                    break;
            }
            return View(await students.AsNoTracking().ToListAsync());
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            // The Include and ThenInclude methods cause the context to load the
            // Student.Enrollments navigation property, and within each enrollment
            // the Enrollment.Course navigation property. 
            
            var student = await _context.Students
                // Include student enrollments
                .Include(s => s.Enrollments)
                    // Include student course
                    .ThenInclude(e => e.Course)
                //Improves performance in scenarios where the entities returned won't be updated in the current context's lifetime
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);


            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
        [Bind("EnrollmentDate,FirstMidName,LastName")] Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(student);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException ex)
            {

                Console.WriteLine($"Exception: {ex}");
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var studentToUpdate = await _context.Students.FirstOrDefaultAsync(s => s.ID == id);
            if (await TryUpdateModelAsync<Student>(
                studentToUpdate,
                "",
                s => s.FirstMidName, s => s.LastName, s => s.EnrollmentDate))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex )
                {
                    Console.WriteLine(ex);
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            return View(studentToUpdate);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if (student == null)
            {
                return NotFound();
            }
            /*
             This parameter is false when the HttpGet Delete method is called without a previous failure.
             When it's called by the HttpPost Delete method in response to a database update error,
             the parameter is true and an error message is passed to the view.
             */
            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
           
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Students.Remove(student);
                // Generates SQL DELETE
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            catch (DbUpdateException  ex )
            {
                Console.WriteLine(ex);
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }

        private bool StudentExists(int id)
        {
          return (_context.Students?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
