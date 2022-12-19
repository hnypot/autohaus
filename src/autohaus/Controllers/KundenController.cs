using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using autohaus.Models;

namespace autohaus.Controllers;

public class KundenController : Controller {
    private readonly DBContext _context;

    public KundenController(DBContext context) {
        _context = context;
    }

    // GET: Kunden
    public async Task<IActionResult> Index() {
        if (User.IsInRole("Admin")) {
            var dBContext = _context.Kunden.Include(k => k.Benutzer);
            return View(await dBContext.ToListAsync());
        }
        return RedirectToAction("Denied", "Benutzer");
    }

    // GET: Kunden/Details/5
    public async Task<IActionResult> Details(int? id) {
        if (User.IsInRole("Admin")) {
            if (id == null || _context.Kunden == null) {
                return NotFound();
            }

            var kunden = await _context.Kunden
                .Include(k => k.Benutzer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (kunden == null) {
                return NotFound();
            }

            return View(kunden);
        }
        return RedirectToAction("Denied", "Benutzer");
    }

    // GET: Kunden/Create
    public IActionResult Create() {
        if (User.IsInRole("Admin")) {
            ViewData["BenutzerId"] = new SelectList(_context.Benutzer, "Id", "Id");
            return View();
        }
        return RedirectToAction("Denied", "Benutzer");
    }

    // POST: Kunden/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,BenutzerId,Vorname,Nachname,Email,Telefon,Strasse,Plz,Ort")] Kunden kunden) {
        if (User.IsInRole("Admin")) {
            if (ModelState.IsValid) {
                _context.Add(kunden);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BenutzerId"] = new SelectList(_context.Benutzer, "Id", "Id", kunden.BenutzerId);
            return View(kunden);
        }
        return RedirectToAction("Denied", "Benutzer");

    }

    // GET: Kunden/Edit/5
    public async Task<IActionResult> Edit(int? id) {
        if (User.IsInRole("Admin")) {
            if (id == null || _context.Kunden == null) {
                return NotFound();
            }

            var kunden = await _context.Kunden.FindAsync(id);
            if (kunden == null) {
                return NotFound();
            }
            ViewData["BenutzerId"] = new SelectList(_context.Benutzer, "Id", "Id", kunden.BenutzerId);
            return View(kunden);
        }
        return RedirectToAction("Denied", "Benutzer");

    }

    // POST: Kunden/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,BenutzerId,Vorname,Nachname,Email,Telefon,Strasse,Plz,Ort")] Kunden kunden) {
        if (User.IsInRole("Admin")) {
            if (id != kunden.Id) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    _context.Update(kunden);
                    await _context.SaveChangesAsync();
                } catch (DbUpdateConcurrencyException) {
                    if (!KundenExists(kunden.Id)) {
                        return NotFound();
                    } else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BenutzerId"] = new SelectList(_context.Benutzer, "Id", "Id", kunden.BenutzerId);
            return View(kunden);
        }
        return RedirectToAction("Denied", "Benutzer");

    }

    // GET: Kunden/Delete/5
    public async Task<IActionResult> Delete(int? id) {
        if (User.IsInRole("Admin")) {
            if (id == null || _context.Kunden == null) {
                return NotFound();
            }

            var kunden = await _context.Kunden
                .Include(k => k.Benutzer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (kunden == null) {
                return NotFound();
            }

            return View(kunden);
        }
        return RedirectToAction("Denied", "Benutzer");

    }

    // POST: Kunden/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id) {
        if (User.IsInRole("Admin")) {
            if (_context.Kunden == null) {
                return Problem("Entity set 'DBContext.Kunden'  is null.");
            }
            var kunden = await _context.Kunden.FindAsync(id);
            if (kunden != null) {
                _context.Kunden.Remove(kunden);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return RedirectToAction("Denied", "Benutzer");
    }

    private bool KundenExists(int id) {
        return _context.Kunden.Any(e => e.Id == id);
    }
}