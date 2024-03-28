using ConBrain.Controllers.ActionResults;
using ConBrain.Extensions;
using ConBrain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuGet.Configuration;
using System.Linq.Expressions;
using System.Security.Claims;

namespace ConBrain.Controllers
{
    [Authorize]
    public class ImageController : Controller
    {
        public ImageController(IConfiguration configuration, UserDbContext dbContext)
        {
            _dbContext = dbContext;
            _imageSettings = configuration.GetSection("ImageSettings").Get<ImageSettings>() ?? throw new FormatException();
        }

        [Route("edit/avatar")]
        [HttpPost]
        public async Task<IActionResult> EditAvatar(IFormFile file)
        {
            //Авторизированный пользователь
            var person = GetPersonByAuth();
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            var image = await SaveImageAsync(person, file, "avatar", "");
            if (image == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            person.Data.Avatar = image;
            await _dbContext.SaveChangesAsync();
            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        /// <summary>
        ///Метод отправки изображения пользователя
        /// </summary>
        /// <param name="nick">Ник пользователя</param>
        /// <param name="key">Ключ изображения</param>
        /// <returns>Изображение</returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("image")]
        async public Task<IActionResult> Image(int id)
        {
            var person = GetPersonByAuth();
            var image = await _dbContext.Images.Where(i => i.Id == id).FirstOrDefaultAsync();
            if (image == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            if (!image.CanGet(person))
                return new StatusCodeResult(StatusCodes.Status403Forbidden);
            return File(image.Data, image.FileExtension);
        }

        [HttpDelete]
        [AllowAnonymous]
        [Route("image")]
        async public Task<IActionResult> DeleteImage(int id)
        {
            var person = GetPersonByAuth();
            var image = await _dbContext.Images.Where(i => i.Id == id).FirstOrDefaultAsync();
            if (image == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            if (!image.CanGet(person))
                return new StatusCodeResult(StatusCodes.Status403Forbidden);
            try
            {
                _dbContext.Images.Remove(image);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            await _dbContext.SaveChangesAsync();
            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("image/data")]
        async public Task<IActionResult> GetImageData(int id)
        {
            var person = GetPersonByAuth();
            var image = await _dbContext.Images.Where(i => i.Id == id).FirstOrDefaultAsync();
            if (image == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            if (!image.CanGet(person))
                return new StatusCodeResult(StatusCodes.Status403Forbidden);
            return new ImageResult(image);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("images/get")]
        public IActionResult GetImages(string? nick, int[] ignores, int size, string? pattern)
        {
            var authPerson = GetPersonByAuth();
            var target = nick == null || nick == "" ? authPerson : GetPerson(nick);

            if (target == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);

            return new ImagesResult(ignores, size, pattern, target.Images.Where(i => i.CanGet(authPerson)));
        }


        //Метод для добавления изображения авторизированного пользователя
        [HttpPost]
        [Route("image")]
        public async Task<IActionResult> Image(IFormFile file)
        {
            //Авторизированный пользователь
            var person = GetPersonByAuth();
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            var image = await SaveImageAsync(person, file, string.Empty, string.Empty, SecurityLevel.Private);
            if (image == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            return new ImageResult(image);
        }

        [HttpPost]
        [Route("image/edit")]
        async public Task<IActionResult> ImageEdit(int id, string name, string description, SecurityLevel level)
        {
            var person = GetPersonByAuth();
            var image = person?.Images.Where(i => i.Id == id).FirstOrDefault();
            if (image == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            image.Name = name;
            image.Description = description;
            image.SecurityLevel = level;
            await _dbContext.SaveChangesAsync();
            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("{nick}/images")]
        public IActionResult Images(string nick)
        {
            var person = GetPerson(nick);
            if (person == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            return View(person);
        }

        private async Task<Image?> SaveImageAsync(Person person, IFormFile file, string name, string description, SecurityLevel level = 0)
        {

            //Создание файла записи
            if (file == null || !file.CanImage())
                return null;

            //Создание изображение, его обрезка и сохранение на сервере
            try
            {
                using var img = await file.From64bitToImageAsync();
                using var resizeImg = img.Resize(_imageSettings.MaxWidth, _imageSettings.MaxHeight);
                System.Drawing.ImageConverter converter = new();
                var bytes = (byte[])converter.ConvertTo(resizeImg, typeof(byte[]));
                var image = new Image()
                {
                    FileExtension = "image/jpeg",
                    Data = bytes,
                    Date = DateTime.Now,
                    Name = name,
                    Description = description,
                    Owner = person,
                    Size = bytes.Length,
                    SecurityLevel = level
                };
                await _dbContext.Images.AddAsync(image);
                await _dbContext.SaveChangesAsync();
                return image;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private Person? GetPersonByAuth()
        {
            string? namePerson = ControllerContext.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            if (namePerson == null)
                return null;
            return GetPerson(namePerson);
        }

        private Person? GetPerson(string nick)
        {
            var person = _dbContext.People
                 .Include(i => i.Data)
                 .Include(i=>i.Images)
                 .Include(i => i.Friends)
                     .ThenInclude(f => f.Friend)
                        .ThenInclude(f => f.Data)
                 .FirstOrDefault(i => i.Data.Nick == nick);
            return person;
        }

        private readonly UserDbContext _dbContext;
        private readonly ImageSettings _imageSettings;

    }
}
