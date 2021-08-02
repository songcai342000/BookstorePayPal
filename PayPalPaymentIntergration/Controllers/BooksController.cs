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
    [AllowAnonymous]
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BooksController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Books
        [HttpGet]
        public async Task<IActionResult> Index(int? id)
        {
            if (id != null)
            {
                Order order = new Order { OrderId = 0, UserId = _userManager.GetUserId(HttpContext.User), Status = "Unpaid", OrderTime = DateTime.Now };
                var orders = _context.Orders.Where(o => o.UserId == order.UserId && o.OrderTime > DateTime.Now.AddDays(-1)).Count();
                if (ModelState.IsValid && orders == 0)
                {
                    _context.Add(order);
                    await _context.SaveChangesAsync();
                    //return RedirectToAction(nameof(Index));
                }
                var orderId = _context.Orders.Where(o => o.UserId == order.UserId).Select(o => o.OrderId).FirstOrDefault();
                if (orderId != null)
                {
                    Reservation reservation = new Reservation { OrderId = orderId, BookId = (int)id };
                    _context.Add(reservation);
                    await _context.SaveChangesAsync();
                }
            }
            return View(await _context.Books.ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookId,Title,Introduction,Genre,Price,ImageUrl")] Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // POST: Books/Index/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int? bookId)
        {
            Order order = new Order { UserId = _userManager.GetUserId(HttpContext.User), Status = "Unpaid", OrderTime = DateTime.Now };
            var orders = _context.Orders.Where(o => o.UserId == order.UserId && o.OrderTime > DateTime.Now.AddDays(-1)).Last();
            if (ModelState.IsValid && orders == null)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
            }
            var orderId = _context.Orders.Where(o => o.UserId == order.UserId).Select(o => o.OrderId).FirstOrDefault();
           
            Reservation reservation = new Reservation { OrderId = orderId, BookId = (int)bookId };
            _context.Add(reservation);
            await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));

            return View(await _context.Books.ToListAsync());
        }*/

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
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookId,Title,Introduction,Genre,Price,ImageUrl")] Book book)
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
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

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
