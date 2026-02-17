using Microsoft.AspNetCore.Mvc;
using StudentApplication.Data;
using StudentApplication.Models;
using System.Linq;

namespace StudentApplication.Controllers
{
    public class StudentController : Controller
    {
        private readonly AppDbContext _context;

        // Dependency Injection happens here
        public StudentController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Show the list of students
        public IActionResult List()
        {
            var students = _context.Students.ToList();
            return View(students);
        }

        // GET: Shows the blank form to the user
        public IActionResult Create()
        {
            return View();
        }

        // POST: Receives the data when the user clicks "Submit"
        [HttpPost]
        public IActionResult Create(Student student)
        {
            // 1. The Controller passes the Model to the DbContext
            _context.Students.Add(student);

            // 2. DbContext generates the SQL INSERT command and saves it
            _context.SaveChanges();

            // 3. Redirect the user back to the list to see the new addition
            return RedirectToAction("List");
        }

        // --- EDIT LOGIC ---
        // GET: Fetch the specific student and show them in the form
        public IActionResult Edit(int id)
        {
            var student = _context.Students.Find(id);
            if (student == null) return NotFound();
            return View(student);
        }

        // POST: Save the updated details
        [HttpPost]
        public IActionResult Edit(Student student)
        {
            _context.Students.Update(student);
            _context.SaveChanges();
            return RedirectToAction("List");
        }

        // --- DELETE LOGIC ---
        // GET: Show a confirmation page
        public IActionResult Delete(int id)
        {
            var student = _context.Students.Find(id);
            if (student == null) return NotFound();
            return View(student);
        }

        // POST: Actually remove the record from the database
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var student = _context.Students.Find(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                _context.SaveChanges();
            }
            return RedirectToAction("List");
        }
    }
}