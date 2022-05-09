using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PayPalPaymentIntergration.Data;
using PayPalPaymentIntergration.Models;

namespace PayPalPaymentIntergration.Controllers
{
    public class BooksController1 : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BooksController1(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        
        // GET: Books
        /*public async Task<IActionResult> Index()
        {
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "Name");

            return View(await _context.Books.ToListAsync());
        }*/


        [AllowAnonymous]
        // GET: Books
        public async Task<IActionResult> Index(int? id)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            if (id != null)
            {
                if (userId == null)
                {
                    return Redirect("/Identity/Account/Login");
                }
                Order order = new Order { UserId = userId, Status = "Unpaid", OrderTime = DateTime.Now };
                var orders = _context.Orders.Where(o => o.UserId == order.UserId && o.Status == "Unpaid" && o.OrderTime > DateTime.Now.AddDays(-1)).Count();
                if (ModelState.IsValid && orders == 0)
                {
                    _context.Add(order);
                    await _context.SaveChangesAsync();
                    //return RedirectToAction(nameof(Index));
                }
                var orderId = _context.Orders.Where(o => o.UserId == order.UserId).Select(s => s.OrderId).OrderBy(i => i).Last();
                if (orderId != 0)
                {
                    Reservation reservation = new Reservation { OrderId = orderId, BookId = (int)id };
                    _context.Add(reservation);
                    await _context.SaveChangesAsync();
                }
            }
            var books = from b in _context.Books
                        join g in _context.Genres on b.GenreId equals g.GenreId
                        select new BookWithGenreName
                        {
                            Title = b.Title,
                            Introduction = b.Introduction,
                            Price = b.Price,
                            Genre = g.Name,
                            ImageUrl = b.ImageUrl
                        };
            return View(await books.ToListAsync());
        }

        [AllowAnonymous]
        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Genre)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        [Authorize(Roles = "Admin")]
        // GET: Books/Create
        public IActionResult Create()
        {
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "Name");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookId,Title,Introduction,GenreId,Price,ImageUrl")] Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "Name", book.GenreId);
            return View(book);
        }

        [Authorize(Roles = "Admin")]
        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "Name", book.GenreId);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookId,Title,Introduction,GenreId,Price,ImageUrl")] Book book)
        {
            if (id != book.BookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.BookId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "Name", book.GenreId);
            return View(book);
        }

        [Authorize(Roles = "Admin")]
        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Genre)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        [Authorize(Roles = "Admin")]
        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);
        }
    }
}
