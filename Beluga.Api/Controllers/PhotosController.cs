using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using Sendeazy.Api.Dtos;
using Sendeazy.Api.Entities;
using Sendeazy.Api.Helpers;
using Sendeazy.Api.Repositories;
using SendeoApi.Services;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApi.Entities;

namespace Sendeazy.Api.Controllers
{
  [Authorize]
  [Route("api/users/{userid}/photos")]
  [ApiController]
  public class PhotosController : ControllerBase
  {
    private readonly IPhotoDao _photoRepo;
    private readonly IMapper _mapper;
    private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
    private Cloudinary _cloudinary;
    private IUserService _userService;

    public PhotosController(IPhotoDao photoRepo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig, IUserService userService)
    {
      _photoRepo = photoRepo;
      _mapper = mapper;
      _cloudinaryConfig = cloudinaryConfig;
      _userService = userService;

      Account acc = new Account
      (
        _cloudinaryConfig.Value.CloudName,
        _cloudinaryConfig.Value.ApiKey,
        _cloudinaryConfig.Value.ApiSecret

        );

      _cloudinary = new Cloudinary(acc);
    }

    [HttpGet("id", Name = "GetPhoto")]
    public async Task<IActionResult> GetPhoto(int id)
    {
      var photoFromRepo = await _photoRepo.GetPhoto(id);
      var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);
      return Ok(photo);
    }

    [HttpPost]
    public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm] PhotoForCreationDto photoForCreationDto)
    {
      var usrId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
      if (userId != int.Parse(usrId))
      {
        return Unauthorized();
      }
      var userFromRepo = await _userService.GetById(userId);
      var file = photoForCreationDto.File;
      var uploadResult = new ImageUploadResult();

      if (file.Length > 0)
      {
        using (var stream = file.OpenReadStream())
        {
          var uploadParams = new ImageUploadParams()
          {
            File = new FileDescription(file.Name, stream),
            Transformation = new Transformation().Width(5000).Height(5000).Crop("fill").Gravity("face")
          };

          uploadResult = _cloudinary.Upload(uploadParams);
        }
      }
      photoForCreationDto.Url = uploadResult.Uri.ToString();
      photoForCreationDto.PublicId = uploadResult.PublicId;

      var photo = _mapper.Map<Photo>(photoForCreationDto);
      photo.UserId = userFromRepo.Id;
      if (!userFromRepo.Photos.Any(u => u.IsMain))
      {
        photo.IsMain = true;
      }
      userFromRepo.Photos.Add(photo);
      var userToSave = _mapper.Map<User>(userFromRepo);
      if (await _userService.Update(userToSave) > 0)
      {
        var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
        return CreatedAtRoute("GetPhoto", new { Id = photo.Id }, photoToReturn);
      }
      return BadRequest("Could not save the photo");
    }

    [HttpPost("{id}/setMain")]
    public async Task<IActionResult> SetMainPhoto(int userId, int Id)
    {
      var usrId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
      if (userId != int.Parse(usrId))
      {
        return Unauthorized();
      }

      var user = await _userService.GetById(userId);
      if (user.Photos.All(p => p.Id != Id))
      {
        return Unauthorized();
      }

      var photoFromRepo = await _photoRepo.GetPhoto(Id);
      if (photoFromRepo.IsMain)
      {
        return BadRequest("this is already the main photo");
      }

      var currentMainPhoto = await _photoRepo.GetMainPhotoForUser(userId);
      if (currentMainPhoto != null)
      {
        currentMainPhoto.IsMain = false;
        await _photoRepo.UpdatePhoto(currentMainPhoto);
      }
      photoFromRepo.IsMain = true;
      if (await _photoRepo.UpdatePhoto(photoFromRepo))
      {
        return NoContent();
      }

      return BadRequest("Could not set the photo to main");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePhoto(int userId, int id)
    {
      var usrId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
      if (userId != int.Parse(usrId))
      {
        return Unauthorized();
      }

      var user = await _userService.GetById(userId);
      if (user.Photos.All(p => p.Id != id))
      {
        return Unauthorized();
      }

      var photoFromRepo = await _photoRepo.GetPhoto(id);
      if (photoFromRepo.IsMain)
      {
        return BadRequest("this is already the main photo");
      }

      if (photoFromRepo.PublicId != null)
      {
        var deleteParams = new DeletionParams(photoFromRepo.PublicId);
        var result = _cloudinary.Destroy(deleteParams);
        if (result.Result == "ok")
        {
         await _photoRepo.DeletePhoto(photoFromRepo);
        }
      }

      if (photoFromRepo.PublicId == null)
      {
       await _photoRepo.DeletePhoto(photoFromRepo);
      }
      return Ok();
    }
  }
}
