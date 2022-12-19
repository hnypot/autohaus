using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using autohaus.Models;
using System.Security.Claims;

namespace autohaus.Controllers;

public class VertragController : Controller {
    private readonly DBContext _context;

    public VertragController(DBContext context) {
        _context = context;
    }

    // GET: Vertragsliste
    public async Task<IActionResult> Index() {
        if (User.IsInRole("Admin") || User.IsInRole("Kunden")) {
            // Use the user's ID to filter the contracts
            var list = _context.Vertrag
                .Where(v => v.BenutzerId == int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)))
                .Include(v => v.Benutzer)
                .Include(v => v.Markt);

            System.Diagnostics.Debug.WriteLine(User.FindFirstValue(ClaimTypes.NameIdentifier));

            return View(await list.ToListAsync());
        }
        return RedirectToAction("Denied", "Benutzer");
    }

    // GET: Vertrag/Details/5
    public async Task<IActionResult> Details(int? id) {
        if (User.IsInRole("Admin") || User.IsInRole("Kunden")) {
            if (id == null || _context.Vertrag == null) {
                return NotFound();
            }

            var vertrag = await _context.Vertrag
                .Include(v => v.Benutzer)
                .Include(v => v.Markt)
                .FirstOrDefaultAsync(m => m.Vertragnummer == id);
            if (vertrag == null) {
                return NotFound();
            }

            return View(vertrag);
        }
        return RedirectToAction("Denied", "Benutzer");
    }

    // GET: Vertrag/Create
    public IActionResult Create() {
        if (User.IsInRole("Admin")) {
            ViewData["BenutzerId"] = new SelectList(_context.Benutzer, "Id", "Id");
            ViewData["MarktId"] = new SelectList(_context.Markt, "Id", "Id");
            return View();
        }
        return RedirectToAction("Denied", "Benutzer");
    }

    // POST: Vertrag/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Vertragnummer,BenutzerId,MarktId,Beschreibung,Erstellungsdatum")] Vertrag vertrag) {
        if (User.IsInRole("Admin")) {
            if (ModelState.IsValid) {
                _context.Add(vertrag);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BenutzerId"] = new SelectList(_context.Benutzer, "Id", "Id", vertrag.BenutzerId);
            ViewData["MarktId"] = new SelectList(_context.Markt, "Id", "Id", vertrag.MarktId);
            return View(vertrag);
        }
        return RedirectToAction("Denied", "Benutzer");

    }

    // GET: Vertrag/Edit/5
    public async Task<IActionResult> Edit(int? id) {
        if (User.IsInRole("Admin")) {
            if (id == null || _context.Vertrag == null) {
                return NotFound();
            }

            var vertrag = await _context.Vertrag.FindAsync(id);
            if (vertrag == null) {
                return NotFound();
            }
            ViewData["BenutzerId"] = new SelectList(_context.Benutzer, "Id", "Id", vertrag.BenutzerId);
            ViewData["MarktId"] = new SelectList(_context.Markt, "Id", "Id", vertrag.MarktId);
            return View(vertrag);
        }
        return RedirectToAction("Denied", "Benutzer");

    }

    // POST: Vertrag/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Vertragnummer,MarktId,BenutzerId,Beschreibung,Erstellungsdatum")] Vertrag vertrag) {
        if (User.IsInRole("Admin")) {
            if (id != vertrag.Vertragnummer) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    _context.Update(vertrag);
                    await _context.SaveChangesAsync();
                } catch (DbUpdateConcurrencyException) {
                    if (!VertragExists(vertrag.Vertragnummer)) {
                        return NotFound();
                    } else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BenutzerId"] = new SelectList(_context.Benutzer, "Id", "Id", vertrag.BenutzerId);
            ViewData["MarktId"] = new SelectList(_context.Markt, "Id", "Id", vertrag.MarktId);
            return View(vertrag);
        }
        return RedirectToAction("Denied", "Benutzer");

    }

    // GET: Vertrag/Delete/5
    public async Task<IActionResult> Delete(int? id) {
        if (User.IsInRole("Admin")) {
            if (id == null || _context.Vertrag == null) {
                return NotFound();
            }

            var vertrag = await _context.Vertrag
                .Include(v => v.Benutzer)
                .Include(v => v.Markt)
                .FirstOrDefaultAsync(m => m.Vertragnummer == id);
            if (vertrag == null) {
                return NotFound();
            }

            return View(vertrag);
        }
        return RedirectToAction("Denied", "Benutzer");

    }

    // POST: Vertrag/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id) {
        if (User.IsInRole("Admin")) {
            if (_context.Vertrag == null) {
                return Problem("Entity set 'DBContext.Vertrag'  is null.");
            }
            var vertrag = await _context.Vertrag.FindAsync(id);
            if (vertrag != null) {
                _context.Vertrag.Remove(vertrag);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return RedirectToAction("Denied", "Benutzer");
    }

    private bool VertragExists(int id) {
        return _context.Vertrag.Any(e => e.Vertragnummer == id);
    }
}