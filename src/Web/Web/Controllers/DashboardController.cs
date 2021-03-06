﻿using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CodeMooc.Web.Data;
using CodeMooc.Web.InputModel;
using CodeMooc.Web.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace CodeMooc.Web.Controllers {

    [Route("dashboard")]
    [Authorize(Policy = Startup.MembersOnlyPolicyName)]
    public class DashboardController : Controller {

        protected readonly DataContext Database;
        protected readonly ILogger<DashboardController> Logger;
        protected readonly IConfiguration Configuration;

        public DashboardController(
            DataContext database,
            ILogger<DashboardController> logger,
            IConfiguration configuration
        ) {
            Database = database;
            Logger = logger;
            Configuration = configuration;
        }

        protected int? GetUserId() {
            if(!HttpContext.User.Identity.IsAuthenticated) {
                Logger.LogError("User not authenticated on dashboard");
                return null;
            }

            var sId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(!int.TryParse(sId, out int id)) {
                Logger.LogError("Cannot parse user ID '{0}'", sId);
                return null;
            }

            return id;
        }

        protected T GetViewModel<T>()
            where T : DashboardBaseViewModel {

            T model = TempData.Get<T>() ?? (T)Activator.CreateInstance(typeof(T));

            var userId = GetUserId();
            if(!userId.HasValue) {
                return null;
            }

            model.LoggedUser = (from r in Database.Registrations
                                where r.Id == userId.Value
                                select r).SingleOrDefault();
            Logger.LogDebug("Loaded user information for user {0} {1}", model.LoggedUser.Id, model.LoggedUser.Name);

            return model;
        }

        public IActionResult Index() {
            var model = GetViewModel<DashboardIndexViewModel>();
            if(model == null) {
                return Forbid();
            }

            var pathProfile = GetProfilePicPath(model.LoggedUser.Id);
            model.ProfilePictureFilename = System.IO.File.Exists(pathProfile) ? Path.GetFileName(pathProfile) : null;

            return View(model);
        }

        [HttpGet("donazioni")]
        public IActionResult ShowDonations() {
            var model = GetViewModel<DashboardDonationsViewModel>();
            if(model == null) {
                return Forbid();
            }

            var emails = (from e in Database.Emails
                          where e.RegistrationId == model.LoggedUser.Id
                          select e)
                          .Include(e => e.AssociatedDonations)
                          .ToList();
            model.Donations = emails.SelectMany(e => e.AssociatedDonations).ToList();

            return View("Donations", model);
        }

        [HttpGet("stato")]
        public IActionResult ShowAssociationStatus() {
            var model = GetViewModel<DashboardStatusViewModel>();
            if(model == null) {
                return Forbid();
            }

            model.IsRegistrationConfirmed = model.LoggedUser.ConfirmationTimestamp.HasValue;

            var emails = (from e in Database.Emails
                          where e.RegistrationId == model.LoggedUser.Id
                          select e)
                          .Include(e => e.AssociatedBadges)
                          .ToList();

            model.Emails = emails;
            model.PrimaryEmail = emails.Where(e => e.IsPrimary).Single();

            model.Badges = emails.SelectMany(e => e.AssociatedBadges).ToList();

            model.IsAssociateForCurrentYear = model.Badges.Any(b => b.Year.Year == DateTime.Now.Year && b.Type == BadgeType.Member);

            model.ConfirmationEmailAddress = Environment.GetEnvironmentVariable("CONFIRMATION_MAIL_BCC");

            return View("Status", model);
        }

        protected string GetCurriculumPath(int userId) {
            var pathsConf = Configuration.GetSection("Paths");
            string pathCurricula = pathsConf["Curricula"];

            return Path.Combine(pathCurricula, $"{userId}.pdf");
        }

        protected string GetProfilePicPath(int userId) {
            var pathsConf = Configuration.GetSection("Paths");            
            string pathProfilePics = pathsConf["ProfilePics"];

            return Path.Combine(pathProfilePics, $"{userId}.jpg");
        }

        [HttpGet("curriculum")]
        public IActionResult ShowCvUpload() {
            var model = GetViewModel<DashboardUploadViewModel>();
            var userId = model.LoggedUser.Id;

            var emails = (from e in Database.Emails
                          where e.RegistrationId == userId
                          select e)
                          .Include(e => e.AssociatedBadges)
                          .ToList();
            var badges = emails.SelectMany(e => e.AssociatedBadges).ToList();
            model.IsAssociateForCurrentYear = badges.Any(b => b.Year.Year == DateTime.Now.Year && b.Type == BadgeType.Member);

            var pathProfile = GetProfilePicPath(userId);
            model.ProfilePictureFilename = System.IO.File.Exists(pathProfile) ? Path.GetFileName(pathProfile) : null;
            Logger.LogDebug("Path {0} exists {1}", pathProfile, model.ProfilePictureFilename != null);

            var pathCurriculum = GetCurriculumPath(userId);
            model.CurriculumFilename = System.IO.File.Exists(pathCurriculum) ? Path.GetFileName(pathCurriculum) : null;
            Logger.LogDebug("Path {0} exists {1}", pathCurriculum, model.ProfilePictureFilename != null);

            return View("Upload", model);
        }

        [HttpPost("curriculum")]
        public async Task<IActionResult> ProcessCvUpload(IFormFile profilePic, IFormFile curriculum) {
            var userId = GetUserId();
            if(!userId.HasValue) {
                return RedirectToAction(nameof(ShowCvUpload));
            }

            if(profilePic != null) {
                ProcessProfilePicture(userId.Value, profilePic);
            }
            if(curriculum != null) {
                await ProcessCurriculum(userId.Value, curriculum);
            }

            return RedirectToAction(nameof(ShowCvUpload));
        }

        protected void ProcessProfilePicture(int userId, IFormFile file) {
            Logger.LogDebug("Processing profile picture, name {0} size {1}", file.FileName, file.Length);

            var pathProfile = GetProfilePicPath(userId);
            using(var source = file.OpenReadStream()) {
                using(var img = Image.Load(source)) {
                    Logger.LogTrace("Resizing input image");
                    img.Mutate(x => x
                        .AutoOrient()
                        .Resize(new ResizeOptions {
                            Mode = ResizeMode.Crop,
                            Size = new SixLabors.Primitives.Size(640, 640)
                        })
                    );

                    using(var output = new FileStream(pathProfile, FileMode.Create, FileAccess.Write)) {
                        Logger.LogTrace("Writing file to {0}", pathProfile);
                        img.SaveAsJpeg(output);
                    }
                }
            }

            Logger.LogInformation("Profile picture for user {0} uploaded to {1}", userId, pathProfile);
        }

        protected async Task ProcessCurriculum(int userId, IFormFile file) {
            Logger.LogDebug("Processing curriculum, name {0} size {1}", file.FileName, file.Length);

            if(Path.GetExtension(file.FileName).Equals("pdf", StringComparison.InvariantCultureIgnoreCase)) {
                Logger.LogError("File extension is not PDF");
                TempData.Put(new DashboardUploadViewModel {
                    CurriculumFailure = DashboardUploadViewModel.CurriculumUploadFailure.WrongExtension
                });
                return;
            }

            var pathCurriculum = GetCurriculumPath(userId);
            using(var output = new FileStream(pathCurriculum, FileMode.Create, FileAccess.Write)) {
                Logger.LogTrace("Writing file to {0}", pathCurriculum);
                await file.CopyToAsync(output);
            }

            Logger.LogInformation("Curriculum for user {0} uploaded to {1}", userId, pathCurriculum);
        }

        [HttpGet("pigna-registration")]
        public async Task<IActionResult> ShowPignaNotebooksRegistration() {
            var userId = GetUserId();
            if(!userId.HasValue) {
                return NotFound();
            }

            var emails = await (from e in Database.Emails
                                where e.RegistrationId == userId.Value
                                select e)
                               .Include(e => e.AssociatedBadges)
                               .ToListAsync();
            var badges = emails.SelectMany(e => e.AssociatedBadges).ToList();
            var isAssociate = badges.Any(b => b.Year.Year == DateTime.Now.Year && b.Type == BadgeType.Member);
            if(!isAssociate) {
                return View("AssociatesOnly");
            }

            var registration = await (from reg in Database.PignaNotebookRegistrations
                                      where reg.RegistrationId == userId.Value
                                      select reg).SingleOrDefaultAsync();
            if(registration != null && registration.IsPersonalAddressComplete()) {
                return View("PignaRegistrationConfirmation");
            }

            return View("PignaRegistration", new PignaRegistrationInputModel {
                Email = registration?.Email ?? emails.FirstOrDefault(e => e.IsPrimary)?.Address,
                MeccanographicCode = registration?.MeccanographicCode,
                SchoolName = registration?.SchoolName,
                SchoolAddress = registration?.SchoolAddress,
                SchoolCap = registration?.SchoolCap,
                SchoolCity = registration?.SchoolCity,
                SchoolProvince = registration?.SchoolProvince,
                PersonalAddress = registration?.PersonalAddress,
                PersonalCap = registration?.PersonalCap,
                PersonalCity = registration?.PersonalCity,
                PersonalProvince = registration?.PersonalProvince,
                PhoneNumber = registration?.PhoneNumber
            });
        }

        [HttpPost("pigna-registration")]
        public async Task<IActionResult> ProcessPignaNotebooksRegistration(
            [FromForm] PignaRegistrationInputModel input
        ) {
            if(!ModelState.IsValid) {
                return View("PignaRegistration", input);
            }

            Logger.LogInformation("Registering user {0}", input.Email);

            var id = GetUserId().Value;
            var user = await Database.PignaNotebookRegistrations.Where(r => r.RegistrationId == id).SingleOrDefaultAsync();
            if(user == null) {
                user = new PignaNotebookRegistration {
                    RegistrationId = id
                };
                Database.PignaNotebookRegistrations.Add(user);
            }

            user.Email = input.Email;
            user.MeccanographicCode = input.MeccanographicCode;
            user.SchoolName = input.SchoolName;
            user.SchoolAddress = input.SchoolAddress;
            user.SchoolCap = input.SchoolCap;
            user.SchoolCity = input.SchoolCity;
            user.SchoolProvince = input.SchoolProvince;
            user.PersonalAddress = input.PersonalAddress;
            user.PersonalCap = input.PersonalCap;
            user.PersonalCity = input.PersonalCity;
            user.PersonalProvince = input.PersonalProvince;
            user.PhoneNumber = input.PhoneNumber;
            user.RegisteredOn = DateTime.UtcNow;

            if(await Database.SaveChangesAsync() != 1) {
                Logger.LogError("Failed to write to database, changes != 1");
            }

            return RedirectToAction(nameof(ShowPignaNotebooksRegistration));
        }

    }

}
