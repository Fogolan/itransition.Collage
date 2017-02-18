using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CollageTask.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CollageTask.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [Authorize]
        public ActionResult ShowCollages(string user_id = "")
        {
            Session["CurrentUserId"] = user_id;
            List<Collage> usersCollages = new List<Collage>();
            using (var db = new PostEntities())
            {
                string id = User.Identity.GetUserId();
                usersCollages = db.Collages.Where(x => x.UserId == id).ToList();
            }
            return View(usersCollages);
        }
        [Authorize]
        public ActionResult ShowCollage(string Id = "")
        {
            int collageId = Convert.ToInt32(Id);
            Collage collage = new Collage();
            using (var db = new PostEntities())
            {
                string id = User.Identity.GetUserId();
                collage = db.Collages.Where(x => x.Id == collageId).FirstOrDefault();
            }
            return View(collage);
        }
        [Authorize]
        public ActionResult UploadImage(string user_id = "")
        {
            Session["CurrentUserId"] = user_id;
            List<Image> usersImages = new List<Image>();
            using (var db = new PostEntities())
            {
                string id = User.Identity.GetUserId();
                usersImages = db.Images.Where(x => x.UserId == id).ToList();
            }
            return View(usersImages);
        }

        [HttpPost]
        public ActionResult DragImagesNew(string user_id = "")
        {
            Session["CurrentUserId"] = user_id;
            List<Image> list = new List<Image>();
            using (var db = new PostEntities())
            {
                string id = User.Identity.GetUserId();
                list = db.Images.Where(x => x.UserId == id).ToList();
            }
            return PartialView(list);
        }
        
        [Authorize]
        public ActionResult RedactPhoto(string Id = "")
        {
            Image photo;
            int photoId = Convert.ToInt32(Id);
            using (var db = new PostEntities())
            {
                // string id = User.Identity.GetUserId();
                photo = db.Images.Where(x => x.Id == photoId).FirstOrDefault();
            }
            return View(photo);
        }
        [Authorize]
        public ActionResult ImageList(string user_id = "")
        {
            Session["CurrentUserId"] = user_id;
            List<Image> usersImages = new List<Image>();
            using (var db = new PostEntities())
            {
                string id = User.Identity.GetUserId();
                usersImages = db.Images.Where(x => x.UserId == id).ToList();
            }
            return PartialView(usersImages);
        }
        [Authorize]
        public ActionResult SaveUploadedFile()
        {
            bool isSavedSuccessfully = true;
            string fName = "";
            try
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    //Save file content goes here
                    fName = file.FileName;
                    if (file != null && file.ContentLength > 0)
                    {

                        var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\WallImages", Server.MapPath(@"\")));

                        string pathString = System.IO.Path.Combine(originalDirectory.ToString(), "imagepath");

                        var fileName1 = Path.GetFileName(file.FileName);


                        bool isExists = System.IO.Directory.Exists(pathString);

                        if (!isExists)
                            System.IO.Directory.CreateDirectory(pathString);

                        var path = string.Format("{0}\\{1}", pathString, file.FileName);
                        file.SaveAs(path);

                        Account account = new Account(
                            "fogolan",
                            "393293335414884",
                            "N7O41a-Nl9VpX4nDuzGagsUxeFA");
                        Cloudinary cloudinary = new Cloudinary(account);
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(path)
                        };
                        var uploadResult = cloudinary.Upload(uploadParams);
                        var imageuri = uploadResult.Uri;
                        var userid = User.Identity.GetUserId();
                        Models.Image newimage = new Models.Image();

                        FileInfo deletedFile = new FileInfo(path);
                        deletedFile.Delete();

                        newimage.UserId = userid;
                        newimage.Path = uploadResult.Uri.AbsolutePath;
                        using (var db = new PostEntities())
                        {
                            //  db.Tables.Add(newimage);
                            db.Images.Add(newimage);
                            db.SaveChanges();
                        }
                        ;
                        //RedirectToAction("DragImages");
                        // Response.Redirect(Request.RawUrl);
                        //return RedirectToAction("Index", "UploadImage", userid);
                    }

                }

            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
            }


            if (isSavedSuccessfully)
            {
                return Json(new { Message = fName });
            }
            else
            {
                return Json(new { Message = "Error in saving file" });
            }
        }


        [HttpPost]
        public ActionResult Create(string imageData)
        {
            if (imageData != "")
            {
                string filename = DateTime.Now.ToString().Replace("/", "-").Replace(" ", "- ").Replace(":", "") + ".png";

                var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\WallImages", Server.MapPath(@"\")));

                string pathString = System.IO.Path.Combine(originalDirectory.ToString(), "imagepath");

                // var fileName1 = Path.GetFileName(file.FileName);

                bool isExists = System.IO.Directory.Exists(pathString);

                if (!isExists)
                    System.IO.Directory.CreateDirectory(pathString);

                var path = string.Format("{0}\\{1}", pathString, filename);



                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        byte[] data = Convert.FromBase64String(imageData);
                        bw.Write(data);
                        bw.Close();
                    }
                    fs.Close();
                }

                Account account = new Account(
                                "fogolan",
                                "393293335414884",
                                "N7O41a-Nl9VpX4nDuzGagsUxeFA");
                Cloudinary cloudinary = new Cloudinary(account);
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(path)
                };
                var uploadResult = cloudinary.Upload(uploadParams);

                var imageuri = uploadResult.Uri;
                var userid = User.Identity.GetUserId();
                Image newimage = new Image();

                newimage.UserId = userid;
                newimage.Path = uploadResult.Uri.AbsolutePath;
                using (var db = new PostEntities())
                {
                    //  db.Tables.Add(newimage);
                    db.Images.Add(newimage);
                    db.SaveChanges();
                };

                FileInfo deletedFile = new FileInfo(path);
                deletedFile.Delete();
            }
            List<Image> list = new List<Image>();
                using (var db = new PostEntities())
                {
                    string id = User.Identity.GetUserId();
                    list = db.Images.Where(x => x.UserId == id).ToList();
                }

                return View("UploadImage", list);
        }
        [Authorize]
        public void SaveCollage(string imageData)
        {
            string filename = DateTime.Now.ToString().Replace("/", "-").Replace(" ", "- ").Replace(":", "") + ".png";

            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\WallImages", Server.MapPath(@"\")));

            string pathString = System.IO.Path.Combine(originalDirectory.ToString(), "imagepath");

            // var fileName1 = Path.GetFileName(file.FileName);

            bool isExists = System.IO.Directory.Exists(pathString);

            if (!isExists)
                System.IO.Directory.CreateDirectory(pathString);

            var path = string.Format("{0}\\{1}", pathString, filename);



            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    byte[] data = Convert.FromBase64String(imageData);
                    bw.Write(data);
                    bw.Close();
                }
                fs.Close();
            }

            Account account = new Account(
                            "fogolan",
                            "393293335414884",
                            "N7O41a-Nl9VpX4nDuzGagsUxeFA");
            Cloudinary cloudinary = new Cloudinary(account);
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(path)
            };
            var uploadResult = cloudinary.Upload(uploadParams);

            var imageuri = uploadResult.Uri;
            var userid = User.Identity.GetUserId();
            Image newimage = new Image();

            newimage.UserId = userid;
            newimage.Path = uploadResult.Uri.AbsolutePath;
            using (var db = new PostEntities())
            {
                //  db.Tables.Add(newimage);
                db.Images.Add(newimage);
                db.SaveChanges();
            };

            FileInfo deletedFile = new FileInfo(path);
            deletedFile.Delete();
    }
        [Authorize]
        public void DeletePhoto(int id, string user_id = "")
        {
            var db = new PostEntities();
            Image b = db.Images.Find(id);
            if (b != null)
            {
                string[] path = b.Path.Split('/', '.').ToArray();
                Account account = new Account(
                           "fogolan",
                           "393293335414884",
                           "N7O41a-Nl9VpX4nDuzGagsUxeFA");
                Cloudinary cloudinary = new Cloudinary(account);
                cloudinary.DeleteResources(path[5]);
                db.Images.Remove(b);
                db.SaveChanges();
            }


            //return RedirectToAction("UploadImage");
        }
        [Authorize]
        public ActionResult DeleteCollage(int Id)
        {
            var db = new PostEntities();
            Collage b = db.Collages.Find(Id);
            if (b != null)
            {
                string[] path = b.Path.Split('/', '.').ToArray();
                Account account = new Account(
                           "fogolan",
                           "393293335414884",
                           "N7O41a-Nl9VpX4nDuzGagsUxeFA");
                Cloudinary cloudinary = new Cloudinary(account);
                cloudinary.DeleteResources(path[5]);
                db.Collages.Remove(b);
                db.SaveChanges();
            }
            return RedirectToAction("ShowCollages");
        }
        [Authorize]
        public ActionResult CreateCollage(string user_id = "")
        {
            Session["CurrentUserId"] = User.Identity.GetUserId();
            user_id = User.Identity.GetUserId();
            Collage collage = new Collage();
            collage.JSON = "";
            using (var db = new PostEntities())
            {
                collage.Photos = db.Images.Where(x => x.UserId == user_id).ToList();
            }
            return View(collage);
        }
        [Authorize]
        [HttpPost]
        public ActionResult CreateCollage(string imageData, string inputData)
        {
            if (imageData != "")
            {
                string filename = DateTime.Now.ToString().Replace("/", "-").Replace(" ", "- ").Replace(":", "") + ".png";

                var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\WallImages", Server.MapPath(@"\")));

                string pathString = System.IO.Path.Combine(originalDirectory.ToString(), "imagepath");

                bool isExists = System.IO.Directory.Exists(pathString);

                if (!isExists)
                    System.IO.Directory.CreateDirectory(pathString);

                var path = string.Format("{0}\\{1}", pathString, filename);



                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        byte[] data = Convert.FromBase64String(imageData);
                        bw.Write(data);
                        bw.Close();
                    }
                    fs.Close();
                }

                Account account = new Account(
                                "fogolan",
                                "393293335414884",
                                "N7O41a-Nl9VpX4nDuzGagsUxeFA");
                Cloudinary cloudinary = new Cloudinary(account);
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(path)
                };
                var uploadResult = cloudinary.Upload(uploadParams);

                var imageuri = uploadResult.Uri;
                var userid = User.Identity.GetUserId();
                Collage newCollage = new Collage();

                newCollage.UserId = userid;
                newCollage.Path = uploadResult.Uri.AbsolutePath;
                
                using (var db = new PostEntities())
                {
                    newCollage.CollageName = "Collage" + db.Collages.Where(x => x.UserId == userid).Count().ToString();
                    if (inputData != "")
                        newCollage.JSON = inputData;
                    else
                    {
                        newCollage.JSON = "";
                    }
                    db.Collages.Add(newCollage);
                    db.SaveChanges();
                };

                FileInfo deletedFile = new FileInfo(path);
                deletedFile.Delete();
            }
            List<Collage> usersCollages = new List<Collage>();
            using (var db = new PostEntities())
            {
                string id = User.Identity.GetUserId();
                usersCollages = db.Collages.Where(x => x.UserId == id).ToList();
            }
            return View("ShowCollages", usersCollages);
        }
        [Authorize]
        public ActionResult EditCollage(string Id = "")
        {
            int collageId = Convert.ToInt32(Id);
            Session["CurrentUserId"] = User.Identity.GetUserId();
            var user_id = User.Identity.GetUserId();
            Collage collage = new Collage();
            using (var db = new PostEntities())
            {
                collage = db.Collages.Where(x => x.Id == collageId).FirstOrDefault();
                collage.Photos = db.Images.Where(x => x.UserId == user_id).ToList();
            }
            return View("EditCollage",collage);
        }
        [Authorize]
        [HttpPost]
        public ActionResult EditCollage(string imageData, string inputData, string id)
        {
            if (imageData != "")
            {
                string filename = DateTime.Now.ToString().Replace("/", "-").Replace(" ", "- ").Replace(":", "") + ".png";

                var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\WallImages", Server.MapPath(@"\")));

                string pathString = System.IO.Path.Combine(originalDirectory.ToString(), "imagepath");

                bool isExists = System.IO.Directory.Exists(pathString);

                if (!isExists)
                    System.IO.Directory.CreateDirectory(pathString);

                var path = string.Format("{0}\\{1}", pathString, filename);



                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        byte[] data = Convert.FromBase64String(imageData);
                        bw.Write(data);
                        bw.Close();
                    }
                    fs.Close();
                }

                Account account = new Account(
                                "fogolan",
                                "393293335414884",
                                "N7O41a-Nl9VpX4nDuzGagsUxeFA");
                Cloudinary cloudinary = new Cloudinary(account);
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(path)
                };
                var uploadResult = cloudinary.Upload(uploadParams);

                var imageuri = uploadResult.Uri;
                var userid = User.Identity.GetUserId();

                using (var db = new PostEntities())
                {
                    int collageId = Convert.ToInt32(id);
                    var newCollage = db.Collages.Where(x => x.Id == collageId).FirstOrDefault();
                    newCollage.UserId = userid;
                    newCollage.Path = uploadResult.Uri.AbsolutePath;
                    if (inputData != "")
                        newCollage.JSON = inputData;
                    else
                    {
                        newCollage.JSON = "";
                    }
                    db.SaveChanges();
                };

                FileInfo deletedFile = new FileInfo(path);
                deletedFile.Delete();
            }
            List<Collage> usersCollages = new List<Collage>();
            using (var db = new PostEntities())
            {
                string Id = User.Identity.GetUserId();
                usersCollages = db.Collages.Where(x => x.UserId == Id).ToList();
            }
            return View("ShowCollages", usersCollages);
        }
    }
}