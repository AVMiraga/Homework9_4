
using Microsoft.AspNetCore.Mvc;
using WebAppRelation.Areas.AdminPanel.ViewModels;
using WebAppRelation.Models;

namespace WebAppRelation.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    public class BookImagesController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public BookImagesController(AppDbContext db, IWebHostEnvironment environment)
        {
            _db = db;
            _env = environment;
        }

        public IActionResult Table()
        {
            AdminVM admin = new AdminVM();
            admin.BookImages = _db.BookImages
                .Include(x => x.Book)
                .ToList();
            return View(admin);
        }
        public IActionResult Create()
        {
            ViewData["Books"] = _db.Books.ToList();
            return View();
        }
        [HttpPost]
        public IActionResult Create(BookImages BookImage)
        {
            ViewData["Books"] = _db.Books.ToList();

            if (BookImage.ImageFile == null || !BookImage.ImageFile.ContentType.Contains("image") || BookImage.ImageFile.Length > 2097152)
            {
                ModelState.AddModelError("ImageFile", "Please upload an image file that is less than 2MB");
                return View();
            }
            string fileName = Guid.NewGuid().ToString() + BookImage.ImageFile.FileName;

            string filePath = Path.Combine(_env.WebRootPath, "uploads", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                BookImage.ImageFile.CopyTo(stream);
            }

            BookImage.ImgUrl = fileName;

            if (!ModelState.IsValid)
            {
                return View(BookImage);
            }

            BookImage.Book = _db.Books.Find(BookImage.BookId);

            _db.BookImages.Add(BookImage);
            _db.SaveChanges();
            return RedirectToAction("Table");
        }
        public IActionResult Update(int Id)
        {
            BookImages bookImages = _db.BookImages.Find(Id);

            ViewData["Books"] = _db.Books.ToList();

            return View(bookImages);
        }
        [HttpPost]
        public async Task<IActionResult> Update(BookImages BookImage)
        {
            ViewData["Books"] = _db.Books.ToList();

            if (!ModelState.IsValid)
            {
                return View(BookImage);
            }

            BookImages bookImages = await _db.BookImages.FindAsync(BookImage.Id);

            if (BookImage.ImageFile != null)
            {
                System.IO.File.Delete(Path.Combine(_env.WebRootPath, "uploads", bookImages.ImgUrl));

                string fileName = Guid.NewGuid().ToString() + BookImage.ImageFile.FileName;

                string filePath = Path.Combine(_env.WebRootPath, "uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    BookImage.ImageFile.CopyTo(stream);
                }

                bookImages.ImgUrl = fileName;
            }

            bookImages.Book = await _db.Books.FindAsync(BookImage.BookId);

            await _db.SaveChangesAsync();

            return RedirectToAction("Table");
        }
        public IActionResult Delete(int id)
        {
            var bookImage = _db.BookImages.Find(id);

            if (bookImage != null)
            {
                System.IO.File.Delete(Path.Combine(_env.WebRootPath, "uploads", bookImage.ImgUrl));

                _db.BookImages.Remove(bookImage);
                _db.SaveChanges();
            }

            return RedirectToAction("Table");
        }

    }
}