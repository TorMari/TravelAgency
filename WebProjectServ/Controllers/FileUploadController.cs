using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebProjectServ.Controllers
{
    //[Authorize]
    //public class FileUploadController : Controller
    //{
    //    private readonly IWebHostEnvironment _env;

    //    public FileUploadController(IWebHostEnvironment env)
    //    {
    //        _env = env;
    //    }

    //    [HttpPost]
    //    public async Task<IActionResult> UploadChatFile(IFormFile file)
    //    {
    //        if (file == null || file.Length == 0) return BadRequest();

    //        var uploads = Path.Combine(_env.WebRootPath, "chat_files");
    //        if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

    //        var uniqueName = Guid.NewGuid().ToString() + "_" + file.FileName;
    //        var filePath = Path.Combine(uploads, uniqueName);

    //        using (var fileStream = new FileStream(filePath, FileMode.Create))
    //        {
    //            await file.CopyToAsync(fileStream);
    //        }

    //        return Json(new { path = "/chat_files/" + uniqueName, name = file.FileName });
    //    }
    //}


    [Authorize]
    [Route("[controller]/[action]")] // Додайте цей атрибут для чіткої маршрутизації
    public class FileUploadController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public FileUploadController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost]
        // Перевірте, чи назва параметра 'file' збігається з formData.append("file", ...)
        public async Task<IActionResult> UploadChatFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не вибрано або порожній");

            try
            {
                var uploads = Path.Combine(_env.WebRootPath, "chat_files");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

                var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploads, uniqueName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Повертаємо об'єкт, який очікує JS
                return Json(new { path = "/chat_files/" + uniqueName, name = file.FileName });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Помилка збереження: {ex.Message}");
            }
        }
    }
}
