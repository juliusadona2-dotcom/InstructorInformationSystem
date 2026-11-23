using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InstructorInformationSystem.Models;

namespace InstructorInformationSystem.Controllers;

[Authorize]
public class InstructorController : Controller
{
    private readonly ApplicationDbContext _context;

    public InstructorController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var instructors = await _context.Instructors
            .AsNoTracking()
            .OrderBy(instructor => instructor.FullName)
            .ToListAsync();

        return View(instructors);
    }

    [HttpGet]
    public IActionResult Create() => View(new Instructor());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("InstructorId,FullName,Age,Address,Designation,Gender,ContactNumber,Majorship,BachelorDegree,EmailAddress")] Instructor instructor)
    {
        if (ModelState.IsValid)
        {
            var email = instructor.EmailAddress.Trim().ToLowerInvariant();
            var duplicateId = await _context.Instructors
                .AnyAsync(item => item.InstructorId == instructor.InstructorId);
            var duplicateEmail = await _context.Instructors
                .AnyAsync(item => item.EmailAddress.ToLower() == email);

            if (duplicateId)
            {
                ModelState.AddModelError(nameof(instructor.InstructorId), "An instructor with this ID already exists.");
            }
            else if (duplicateEmail)
            {
                ModelState.AddModelError(nameof(instructor.EmailAddress), "An instructor with this e-mail address already exists.");
            }
            else
            {
                instructor.InstructorId = instructor.InstructorId.Trim();
                instructor.FullName = instructor.FullName.Trim();
                instructor.EmailAddress = email;
                _context.Instructors.Add(instructor);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Instructor record created successfully.";
                return RedirectToAction(nameof(Index));
            }
        }

        return View(instructor);
    }

    [HttpGet]
    public async Task<IActionResult> Details(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return NotFound();
        }

        var instructor = await _context.Instructors
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.InstructorId == id);

        return instructor is null ? NotFound() : View(instructor);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return NotFound();
        }

        var instructor = await _context.Instructors.AsNoTracking()
            .FirstOrDefaultAsync(item => item.InstructorId == id);
        if (instructor is null)
        {
            return NotFound();
        }

        ViewData["OriginalId"] = id;
        return View(instructor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        string id,
        [Bind("InstructorId,FullName,Age,Address,Designation,Gender,ContactNumber,Majorship,BachelorDegree,EmailAddress")] Instructor instructor)
    {
        // "id" is the ORIGINAL Instructor ID (from the route); instructor.InstructorId is the new value typed in the form.
        ViewData["OriginalId"] = id;

        var existing = await _context.Instructors.FirstOrDefaultAsync(item => item.InstructorId == id);
        if (existing is null)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var newId = instructor.InstructorId.Trim();
            var idChanged = newId != id;
            var email = instructor.EmailAddress.Trim().ToLowerInvariant();

            var duplicateId = idChanged &&
                await _context.Instructors.AnyAsync(item => item.InstructorId == newId);
            var duplicateEmail = await _context.Instructors
                .AnyAsync(item => item.InstructorId != id && item.EmailAddress.ToLower() == email);

            if (duplicateId)
            {
                ModelState.AddModelError(nameof(instructor.InstructorId), "An instructor with this ID already exists.");
            }
            else if (duplicateEmail)
            {
                ModelState.AddModelError(nameof(instructor.EmailAddress), "Another instructor is already using this e-mail address.");
            }
            else
            {
                instructor.InstructorId = newId;
                instructor.FullName = instructor.FullName.Trim();
                instructor.EmailAddress = email;

                if (!idChanged)
                {
                    _context.Entry(existing).CurrentValues.SetValues(instructor);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // InstructorId is the primary key, and EF Core cannot change a key in place,
                    // so replace the old row with a new one inside a single transaction.
                    await using var transaction = await _context.Database.BeginTransactionAsync();
                    _context.Instructors.Remove(existing);
                    await _context.SaveChangesAsync();
                    _context.Instructors.Add(instructor);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }

                TempData["SuccessMessage"] = "Instructor record updated successfully.";
                return RedirectToAction(nameof(Index));
            }
        }

        return View(instructor);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return NotFound();
        }

        var instructor = await _context.Instructors
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.InstructorId == id);

        return instructor is null ? NotFound() : View(instructor);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var instructor = await _context.Instructors.FindAsync(id);
        if (instructor is null)
        {
            return NotFound();
        }

        _context.Instructors.Remove(instructor);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Instructor record deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
