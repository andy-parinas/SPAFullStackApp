using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebAPI.Data;
using WebAPI.Dto;
using WebAPI.Helpers;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/users/{userId}/photos")]
    public class PhotosController : Controller
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySetting> _cloudinaryConfig;

        private Cloudinary _cloudinary;

        public PhotosController(IDatingRepository repo, 
            IMapper mapper, 
            IOptions<CloudinarySetting> cloudinaryConfig)
        {
            _repo = repo;
            _mapper = mapper;
            _cloudinaryConfig = cloudinaryConfig;

            Account acc = new Account(
                    _cloudinaryConfig.Value.CloudName,
                    _cloudinaryConfig.Value.ApiKey,
                    _cloudinaryConfig.Value.ApiSecret
                );

            _cloudinary = new Cloudinary(acc);
        }

        [HttpGet("{id}", Name="GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repo.GetPhoto(id);

            var photo = _mapper.Map<PhotoReturnDto>(photoFromRepo);

            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, PhotoCreateDto photoDto )
        {
            var user = await _repo.GetUser(userId);

            if (user == null)
                return BadRequest("Could not find user");


            int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (currentUserId != user.Id)
                return Unauthorized();


            var file = photoDto.File;

            var uploadResult = new ImageUploadResult();

            if(file.Length > 0)
            {
                using(var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.Name, stream)
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }

            }

            photoDto.UrlString = uploadResult.Uri.ToString();
            photoDto.PublicId = uploadResult.PublicId;

            Photo photo = _mapper.Map<Photo>(photoDto);

            photo.User = user;

            if (!user.Photos.Any(m => m.IsMain))
                photo.IsMain = true;


            user.Photos.Add(photo);

            var photoReturn = _mapper.Map<PhotoReturnDto>(photo);

            if (await _repo.SaveAll())
            {
                return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoReturn);
            }

            return BadRequest("Could not add photo");
        }


        [HttpPost("{id}/main")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();


            var photo = await _repo.GetPhoto(id);

            if (photo == null)
                return NotFound();

            if (photo.IsMain)
                return BadRequest("This is already a main photo");

            var currentMainPhoto = await _repo.GetMainPhoto(userId);

            if (currentMainPhoto != null)
                currentMainPhoto.IsMain = false;

            photo.IsMain = true;

            if (await _repo.SaveAll())
                return NoContent();

            return BadRequest("Could not set photo to main");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {

            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();


            var photo = await _repo.GetPhoto(id);

            if (photo == null)
                return NotFound();

            if (photo.IsMain)
                return BadRequest("This is a main photo");

            if(photo.PublicId != null)
            {
                var deleteParams = new DeletionParams(photo.PublicId);

                var result = _cloudinary.Destroy(deleteParams);

                if (result.Result == "ok")
                    _repo.Delete(photo);
            }

            if(photo.PublicId == null)
            {
                _repo.Delete(photo);
            }


            if (await _repo.SaveAll())
                return Ok();

            return BadRequest("Failed to delete photo");

        }

    }
}