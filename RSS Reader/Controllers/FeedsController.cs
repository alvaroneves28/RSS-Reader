using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RSS_Reader.Data;
using RSS_Reader.Entities;
using RSS_Reader.Interfaces;

namespace RSS_Reader.Controllers
{
    /// <summary>
    /// Controller for managing RSS feeds and their articles.
    /// </summary>
    public class FeedsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IRssIngestService _rss;

        /// <summary>
        /// Constructor for FeedsController.
        /// </summary>
        /// <param name="db">Application database context.</param>
        /// <param name="rss">RSS ingest service.</param>
        public FeedsController(AppDbContext db, IRssIngestService rss)
        {
            _db = db;
            _rss = rss;
        }

        /// <summary>
        /// Displays the list of feeds with optional search query.
        /// </summary>
        /// <param name="q">Search query by feed name.</param>
        /// <returns>View with list of feeds.</returns>
        public async Task<IActionResult> Index(string? q)
        {
            var feeds = _db.Feeds.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
                feeds = feeds.Where(f => f.Name.Contains(q));

            var list = await feeds.OrderBy(f => f.Name).ToListAsync();
            ViewBag.Query = q;

            return View(list);
        }

        /// <summary>
        /// Shows the form to create a new feed.
        /// </summary>
        /// <returns>Create feed view.</returns>
        public IActionResult Create() => View(new Feed());

        /// <summary>
        /// Handles POST request to create a new feed.
        /// </summary>
        /// <param name="model">Feed model.</param>
        /// <returns>Redirects to Index on success, or redisplays form on failure.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Feed model)
        {
            if (!Uri.IsWellFormedUriString(model.Url, UriKind.Absolute))
                ModelState.AddModelError("Url", "Invalid URL");

            if (!ModelState.IsValid) return View(model);

            _db.Feeds.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Deletes multiple feeds based on selected IDs.
        /// </summary>
        /// <param name="ids">Array of feed IDs to delete.</param>
        /// <returns>Redirects to Index.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkDelete(int[] ids)
        {
            var feeds = await _db.Feeds.Where(f => ids.Contains(f.Id)).ToListAsync();
            _db.Feeds.RemoveRange(feeds);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Shows confirmation page for deleting a feed.
        /// </summary>
        /// <param name="id">Feed ID.</param>
        /// <returns>Delete confirmation view.</returns>
        public async Task<IActionResult> Delete(int id)
        {
            var feed = await _db.Feeds.FindAsync(id);
            if (feed is null) return NotFound();
            return View(feed);
        }

        /// <summary>
        /// Handles POST request to delete a feed.
        /// </summary>
        /// <param name="id">Feed ID.</param>
        /// <returns>Redirects to Index.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var feed = await _db.Feeds.FindAsync(id);
            if (feed is not null)
            {
                _db.Feeds.Remove(feed);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Shows details of a feed and its articles, with optional filtering.
        /// </summary>
        /// <param name="id">Feed ID.</param>
        /// <param name="from">Filter articles published from this date.</param>
        /// <param name="to">Filter articles published up to this date.</param>
        /// <param name="q">Filter articles by title containing this query.</param>
        /// <returns>Feed details view with filtered articles.</returns>
        public async Task<IActionResult> Details(int id, DateTime? from, DateTime? to, string? q)
        {
            var feed = await _db.Feeds.FindAsync(id);
            if (feed is null) return NotFound();

            var articles = _db.Articles.Where(a => a.FeedId == id);

            if (from.HasValue)
                articles = articles.Where(a => a.PublishedAt >= from.Value);
            if (to.HasValue)
                articles = articles.Where(a => a.PublishedAt <= to.Value);
            if (!string.IsNullOrWhiteSpace(q))
                articles = articles.Where(a => a.Title.Contains(q));

            ViewBag.From = from?.ToString("yyyy-MM-dd");
            ViewBag.To = to?.ToString("yyyy-MM-dd");
            ViewBag.Query = q;
            ViewBag.Feed = feed;

            var list = await articles
                .OrderByDescending(a => a.PublishedAt)
                .ThenByDescending(a => a.Id)
                .ToListAsync();

            return View(list);
        }

        /// <summary>
        /// Reloads the articles of a feed from its RSS URL.
        /// </summary>
        /// <param name="id">Feed ID.</param>
        /// <returns>Redirects to Details view with message about new articles imported.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reload(int id)
        {
            var feed = await _db.Feeds.FindAsync(id);
            if (feed is null) return NotFound();

            var inserted = await _rss.ReloadArticlesAsync(feed);
            TempData["ReloadMessage"] = $"{inserted} new articles imported.";

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
