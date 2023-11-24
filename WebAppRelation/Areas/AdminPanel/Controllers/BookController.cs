using WebAppRelation.Areas.AdminPanel.ViewModels;

namespace WebAppRelation.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    public class BookController : Controller
    {
        AppDbContext _db;
        public BookController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Table()
        {
            AdminVM admin = new AdminVM();
            admin.Books = _db.Books
                .Include(x => x.Category)
                .Include(x => x.Brand)
                .ToList();
            return View(admin);
        }
        public async Task<IActionResult> Create()
        {
            ViewData["Categories"] = await _db.Categories.ToListAsync();
            ViewData["Brands"] = await _db.Brands.ToListAsync();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Book book)
        {
            ViewData["Categories"] = await _db.Categories.ToListAsync();
            ViewData["Brands"] = await _db.Brands.ToListAsync();

            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors.Select(p => p.ErrorMessage)).ToList();
                return View();
            }

            if (await _db.Books.AnyAsync(x => x.Title == book.Title))
            {
                ModelState.AddModelError("Title", "Book already exist");
                return View();
            }

            if (await _db.Books.AnyAsync(x => x.BookCode == book.BookCode))
            {
                ModelState.AddModelError("BookCode", "Book already exist");
                return View();
            }

            book.Category = await _db.Categories.FindAsync(book.CategoryId);
            book.Brand = await _db.Brands.FindAsync(book.BrandId);

            _db.Books.AddAsync(book);
            _db.SaveChanges();
            return RedirectToAction("Table");
        }
        public async Task<IActionResult> Update(int Id)
        {
            ViewData["Categories"] = await _db.Categories.ToListAsync();
            ViewData["Brands"] = await _db.Brands.ToListAsync();

            return View(await _db.Books.FindAsync(Id));
        }
        [HttpPost]
        public async Task<IActionResult> Update(Book Book)
        {
            ViewData["Categories"] = await _db.Categories.ToListAsync();
            ViewData["Brands"] = await _db.Brands.ToListAsync();

            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors.Select(p => p.ErrorMessage)).ToList();
                return View();
            }

            if (await _db.Books.AnyAsync(x => x.Title == Book.Title && x.Id != Book.Id))
            {
                ModelState.AddModelError("Title", "Book already exist");
                return View();
            }

            if (await _db.Books.AnyAsync(x => x.BookCode == Book.BookCode && x.Id != Book.Id))
            {
                ModelState.AddModelError("BookCode", "Book already exist");
                return View();
            }

            Book.Category = await _db.Categories.FindAsync(Book.CategoryId);
            Book.Brand = await _db.Brands.FindAsync(Book.BrandId);

            _db.Books.Update(Book);
            _db.SaveChanges();

            return RedirectToAction("Table");
        }
        public IActionResult Delete(int id)
        {
            _db.Books.Remove(_db.Books.Find(id));
            _db.SaveChanges();
            return RedirectToAction("Table");
        }

    }
}
