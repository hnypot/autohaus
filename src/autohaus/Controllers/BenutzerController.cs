using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Security.Cryptography;
using autohaus.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace autohaus.Controllers;

public class BenutzerController : Controller {

    private readonly DBContext _ctx;

    // Dependency Injection

    public BenutzerController(DBContext ctx) {
        _ctx = ctx;
    }

    // GET: Index

    public ActionResult Index() {
        return View();
    }

    // GET: Login

    public ActionResult Login() {
        return View();
    }

    // GET: Register

    public ActionResult Register() {
        return View();
    }

    // GET: AccessDenied

    public ActionResult Denied() {
        TempData["Error"] = "Zugriff verweigert";
        return View();
    }

    // GET: Logout
    public ActionResult Logout() {
        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    //POST: Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Register(KundenBenutzer kb) {
        var checkBenutzer = _ctx.Benutzer.FirstOrDefault(b => b.Benutzername == kb.Benutzer.Benutzername);
        var checkKunden = _ctx.Kunden.FirstOrDefault(k => k.Email == kb.Kunden.Email && k.Telefon == kb.Kunden.Telefon);
        if (checkBenutzer != null && checkKunden != null) {
            TempData["Error"] = "Benutzername, Telefonnummer oder Email existieren bereits";
            Response.WriteAsync("<script>alert('Benutzername, Telefonnummer oder Email existieren bereits')</script>");
            return View();
        }
        kb.Benutzer.Passwort = ComputeSha256Hash(kb.Benutzer.Passwort);
        kb.Benutzer.Admin = false;
        _ctx.Benutzer.Add(kb.Benutzer);
        _ctx.SaveChanges();
        kb.Kunden.BenutzerId = kb.Benutzer.Id;
        _ctx.Kunden.Add(kb.Kunden);
        _ctx.SaveChanges();
        ViewData["BenutzerId"] = new SelectList(_ctx.Benutzer, "Id", "Id", kb.Kunden.BenutzerId);
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Login(Benutzer benutzer) {
        if (!ModelState.IsValid) {
            TempData["Error"] = "Login ist fehlgeschlagen. Bitte versuche es später erneut!";
            await Response.WriteAsync("<script>alert('Login ist fehlgeschlagen. Bitte versuche es später erneut!')</script>");
            return RedirectToAction("Login");
        }
        var verifyPassword = ComputeSha256Hash(benutzer.Passwort);
        // Retrieve the user from the database based on their username and password
        var user = _ctx.Benutzer
            .Where(b => b.Benutzername.Equals(benutzer.Benutzername) && b.Passwort.Equals(verifyPassword))
            .FirstOrDefault();
        if (user == null) {
            TempData["Error"] = "Benutzername oder Passwort falsch!";
            await Response.WriteAsync("<script>alert('Benutzer existiert nicht oder Benutzername bzw. Passwort ist falsch!')</script>");
            return View();
        }
        // Set the ID of the Benutzer object to the user's ID from the database
        benutzer.Id = user.Id;
        var claims = new List<Claim> {
                 new Claim(ClaimTypes.Name, benutzer.Benutzername),
                 new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                 new Claim(ClaimTypes.Role, user.Admin ? "Admin" : "Kunden")
            };
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties();
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);
        System.Diagnostics.Debug.WriteLine(user.Admin ? "Admin" : "Kunden");
        return RedirectToAction("Index");
    }

    private static string ComputeSha256Hash(string rawData) {
        // Create a SHA256
        using (SHA256 sha256Hash = SHA256.Create()) {
            // ComputeHash - returns byte array
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

            // Convert byte array to a string
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++) {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}