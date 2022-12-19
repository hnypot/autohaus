using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using autohaus.Models;

namespace autohaus.Controllers;

public class MarktController : Controller {
    private readonly DBContext _context;

    public MarktController(DBContext context) {
        _context = context;
    }

    // GET: Markt
    public async Task<IActionResult> Index() {
        return View(await _context.Markt.ToListAsync());
    }

    // GET: Markt/Details/5
    public async Task<IActionResult> Details(int? id) {
        if (id == null || _context.Markt == null) {
            return NotFound();
        }

        var markt = await _context.Markt
            .FirstOrDefaultAsync(m => m.Id == id);
        if (markt == null) {
            return NotFound();
        }

        return View(markt);
    }

    // GET: Markt/Create
    public IActionResult Create() {
        if (User.IsInRole("Admin")) {
            return View();
        }
        return RedirectToAction("Denied", "Benutzer");
    }

    // POST: Markt/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Titel,Beschreibung,Preis,Erstellungsdatum,Verkauft")] Markt markt) {
        if (User.IsInRole("Admin")) {
            if (ModelState.IsValid) {
                _context.Add(markt);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(markt);
        }
        return RedirectToAction("Denied", "Benutzer");
    }

    // GET: Markt/Edit/5
    public async Task<IActionResult> Edit(int? id) {
        if (User.IsInRole("Admin")) {
            if (id == null || _context.Markt == null) {
                return NotFound();
            }

            var markt = await _context.Markt.FindAsync(id);
            if (markt == null) {
                return NotFound();
            }
            return View(markt);
        }
        return RedirectToAction("Denied", "Benutzer");
    }

    // POST: Markt/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Titel,Beschreibung,Preis,Erstellungsdatum,Verkauft")] Markt markt) {
        if (User.IsInRole("Admin")) {
            if (id != markt.Id) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    _context.Update(markt);
                    await _context.SaveChangesAsync();
                } catch (DbUpdateConcurrencyException) {
                    if (!MarktExists(markt.Id)) {
                        return NotFound();
                    } else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(markt);
        }
        return RedirectToAction("Denied", "Benutzer");
    }

    // GET: Markt/Delete/5
    public async Task<IActionResult> Delete(int? id) {
        if (User.IsInRole("Admin")) {
            if (id == null || _context.Markt == null) {
                return NotFound();
            }

            var markt = await _context.Markt
                .FirstOrDefaultAsync(m => m.Id == id);
            if (markt == null) {
                return NotFound();
            }

            return View(markt);
        }
        return RedirectToAction("Denied", "Benutzer");
    }

    // POST: Markt/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id) {
        if (User.IsInRole("Admin")) {
            if (_context.Markt == null) {
                return Problem("Entity set 'DBContext.Markt'  is null.");
            }
            var markt = await _context.Markt.FindAsync(id);
            if (markt != null) {
                _context.Markt.Remove(markt);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return RedirectToAction("Denied", "Benutzer");
    }

    public async Task<IActionResult> Buy(int? id, Vertrag vertrag, Benutzer benutzer) {
        if (User.IsInRole("Admin") || User.IsInRole("Kunden")) {
            if (id == null || _context.Markt == null) {
                return NotFound();
            }
            var markt = await _context.Markt.FirstOrDefaultAsync(m => m.Id == id);
            if (markt == null || vertrag == null || benutzer == null) {
                return NotFound();
            }
            vertrag = new Vertrag {
                MarktId = markt.Id,
                BenutzerId = benutzer.Id,
                Beschreibung = "Vielen Dank, dass du bei uns gekauft hast! Du hast " + markt.Titel + " für " + markt.Preis + " gekauft. Du kannst dein Auto bei uns bei der Baselstrasse 40, CH-4053 Basel abholen. Du musst aber eine Identitätskarte mitbringen, damit wir sicher gehen können, dass du der rechtmässige Besitzer bist. Weitere Dokumente erhalten sie bei der Abholung. Wir senden Ihnen ebenfalls noch diese Dokumente per E-Mail zu, sobald sie Ihr Auto abgeholt haben. Wir wünschen dir viel Spass mit deinem neuen Auto!",
                Erstellungsdatum = DateTime.Now,
                Gueltig = true
            };
            markt.Verkauft = true;
            _context.Markt.Update(markt);
            _context.Vertrag.Add(vertrag);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return RedirectToAction("Denied", "Benutzer");
    }

    private bool MarktExists(int id) {
        return _context.Markt.Any(e => e.Id == id);
    }
}