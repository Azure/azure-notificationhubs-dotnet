using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebPushDemo.Models;

namespace WebPushDemo.Controllers
{
    public class DevicesController : Controller
    {
        private readonly WebPushDemoContext _context;

        private readonly IConfiguration _configuration;

        public DevicesController(WebPushDemoContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: Devices
        public async Task<IActionResult> Index()
        {
            return View(await _context.Devices.ToListAsync());
        }

        // GET: Devices/Create
        public IActionResult Create()
        {
            ViewBag.PublicKey = _configuration.GetSection("VapidKeys")["PublicKey"];

            return View();
        }

        // POST: Devices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,PushEndpoint,PushP256DH,PushAuth")] Devices devices)
        {
            if (ModelState.IsValid)
            {
                _context.Add(devices);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(devices);
        }
       
        // GET: Devices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var devices = await _context.Devices
                .SingleOrDefaultAsync(m => m.Id == id);
            if (devices == null)
            {
                return NotFound();
            }

            return View(devices);
        }

        // POST: Devices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var devices = await _context.Devices.SingleOrDefaultAsync(m => m.Id == id);
            _context.Devices.Remove(devices);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
