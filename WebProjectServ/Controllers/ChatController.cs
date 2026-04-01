using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebProjectServ.Models;


public class ChatController : Controller
{

    private readonly MyDataContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ChatController(MyDataContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public async Task<IActionResult> Private(string userId)
    {
        var currentUserId = _userManager.GetUserId(User);

        var users = _userManager.Users
            .Where(u => u.Id != currentUserId)
            .ToList();

        ViewBag.Users = users;

        if (string.IsNullOrEmpty(userId))
        {
            userId = users.FirstOrDefault()?.Id;
        }

        var messages = _context.Messages
            .Where(m =>
                (m.SenderId == currentUserId && m.ReceiverId == userId) ||
                (m.SenderId == userId && m.ReceiverId == currentUserId))
            .OrderBy(m => m.SentAt)
            .ToList();

        ViewBag.ReceiverId = userId;

        return View("Private", messages);
    }

    [HttpPost]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file");

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files");

        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return Ok(new { fileName = file.FileName, fileUrl = "/files/" + fileName });
    }
}

