using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

using Orchard.FileSystems.Media;

using Mello.ImageGallery.Models;
using Mello.ImageGallery.Helpers;

using Orchard.MediaLibrary.Services;


namespace Mello.ImageGallery.Services {
    public class ThumbnailService : IThumbnailService {
        private const string ThumbnailFolder = "Thumbnails";

        private readonly ImageFormat _thumbnailImageFormat = ImageFormat.Jpeg;
        private readonly IMediaLibraryService _mediaService;
        private readonly IStorageProvider _storageProvider;

        public ThumbnailService(IMediaLibraryService mediaService, IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
            _mediaService = mediaService;
        }

        protected string GetThumbnailFolder(string mediaPath) {
            // Creates a thumbnail folder if doesn't exists
            if (!_mediaService.GetMediaFolders(mediaPath).Select(o => o.Name).Contains(ThumbnailFolder)) {
                _mediaService.CreateFolder(mediaPath, ThumbnailFolder);
            }

            return _storageProvider.Combine(mediaPath, ThumbnailFolder);
        }

        /// <summary>
        /// Creates an images thumbnail.
        /// </summary>
        /// <param name="image">The image full path on the media storage.</param>
        /// <param name="thumbnailFolderPath">The media path to thumbnails folder.</param>
        /// <param name="imageName">The image name.</param>
        /// <param name="thumbnailWidth">The thumbnail width in pixels.</param>
        /// <param name="thumbnailHeight">The thumbnail height in pixels.</param>
        /// <param name="keepAspectRatio">Indicates whether to keep the original image aspect ratio</param>
        /// <param name="expandToFill">Indicates whether to expand the thumbnail to fill the bounds specified by width and height</param>
        /// <returns>The thumbnail file media path.</returns>
        protected Thumbnail CreateThumbnail(string image, string thumbnailFolderPath, string imageName, int thumbnailWidth,
                                         int thumbnailHeight, bool keepAspectRatio, bool expandToFill) {
            if (thumbnailWidth <= 0) {
                throw new ArgumentException("Thumbnail width must be greater than zero", "thumbnailWidth");
            }

            if (thumbnailHeight <= 0) {
                throw new ArgumentException("Thumbnail height must be greater than zero", "thumbnailHeight");
            }

            string thumbnailFilePath = _storageProvider.Combine(thumbnailFolderPath, imageName);

            IStorageFile imageFile = _storageProvider.GetFile(image);
            using (Stream imageStream = imageFile.OpenRead()) {
                using (Image drawingImage = Image.FromStream(imageStream))
                {
                    bool shouldCreateImage = true;

                    // Verify if the image already has a Thumbnail                    
                    var thumbnailName = _mediaService.GetMediaFiles(thumbnailFolderPath)
                                        .Select(o => o.Name).SingleOrDefault(o => o == imageName);

                    if(thumbnailName != null) {
                        // Verify if the existing thumbnail has the correct size (in case the thumbnail settings have been changed)
                        IStorageFile thumbnailFile = _storageProvider.GetFile(thumbnailFilePath);
                        using (Stream thumnailFileStream = thumbnailFile.OpenRead()) {
                            using (Image thumbnailImage = Image.FromStream(thumnailFileStream)) {
                                if (ImageHasCorrectThumbnail(drawingImage, thumbnailImage, thumbnailWidth, thumbnailHeight, keepAspectRatio, expandToFill))
                                {
                                    shouldCreateImage = false;
                                    thumbnailWidth = thumbnailImage.Width;
                                    thumbnailHeight = thumbnailImage.Height;
                                }
                            }
                        }
                    }

                    if (shouldCreateImage) {
                        using (Image thumbDrawing = CreateThumbnail(drawingImage, thumbnailWidth, thumbnailHeight, keepAspectRatio, expandToFill)) {
                            if (_storageProvider.ListFiles(thumbnailFolderPath).Select(o => o.GetName()).Contains(imageName)) {
                                _storageProvider.DeleteFile(thumbnailFilePath);
                            }

                            IStorageFile thumbFile = _storageProvider.CreateFile(thumbnailFilePath);
                            using (Stream thumbStream = thumbFile.OpenWrite())
                            {
                                thumbDrawing.Save(thumbStream, _thumbnailImageFormat);
                                thumbnailWidth = thumbDrawing.Width;
                                thumbnailHeight = thumbDrawing.Height;
                            }
                        }
                    }
                }
            }

            string thumbnailPublicUrl = _mediaService.GetMediaPublicUrl(Path.GetDirectoryName(thumbnailFilePath), Path.GetFileName(thumbnailFilePath));
            return new Thumbnail {PublicUrl = thumbnailPublicUrl, Width = thumbnailWidth, Height = thumbnailHeight};
        }

        protected Image CreateThumbnail(Image originalImage, int thumbnailWidth, int thumbnailHeight, bool keepAspectRatio, bool expandToFill) {
            if (thumbnailWidth <= 0) {
                throw new ArgumentException("Thumbnail width must be greater than zero", "thumbnailWidth");
            }

            if (thumbnailHeight <= 0) {
                throw new ArgumentException("Thumbnail height must be greater than zero", "thumbnailHeight");
            }

            Image thumbnailImage = null;

            if (keepAspectRatio && expandToFill)
            {
                // Expand/Shrink the image to fill the thumbnail bounds maintaining aspect ratio
                float aspectRatio = (float)originalImage.Width / (float)originalImage.Height;
                int newWidth = thumbnailWidth;
                int newHeight = Convert.ToInt32((float)newWidth / aspectRatio);
                if (newHeight < thumbnailHeight)
                {
                    newHeight = thumbnailHeight;
                    newWidth = Convert.ToInt32((float)newHeight * aspectRatio);
                }
                thumbnailImage = originalImage.HiqhQualityResize(newWidth, newHeight);
                // Then clip to the thumbnail bounds
                Point cropStartPoint = thumbnailImage.GetTopLeftStartCropPointForKeepCenter(thumbnailWidth, thumbnailHeight);
                return thumbnailImage.Crop(thumbnailWidth, thumbnailHeight, cropStartPoint.X, cropStartPoint.Y);               
            }
            else
            {
                int newWidth; int newHeight;
                GetThumbnailSize(originalImage, thumbnailWidth, thumbnailHeight, keepAspectRatio, expandToFill, out newWidth, out newHeight);                
                thumbnailImage = originalImage.HiqhQualityResize(newWidth, newHeight);
            }

            return thumbnailImage;
        }

        protected void GetThumbnailSize(Image originalImage, int thumbnailWidth, int thumbnailHeight, bool keepAspectRatio, bool expandToFill, out int newWidth, out int newHeight)
        {
            if (expandToFill)
            {
                // Regardless of whether we're crop-filling (keeping aspect) or stretch filling, we're only interested in the new thumbnail size
                newWidth = thumbnailWidth;
                newHeight = thumbnailHeight;
            }
            else // Will shrink to fit, but won't expand to fit (in the case where the image is smaller than the thumbnail limits)
            {                
                if (keepAspectRatio)
                {
                    float aspectRatio = (float)originalImage.Width / (float)originalImage.Height;
                    newWidth = originalImage.Width;
                    newHeight = originalImage.Height;
                    if (newWidth > thumbnailWidth)
                    {
                        newWidth = thumbnailWidth;
                        newHeight = Convert.ToInt32((float)newWidth / aspectRatio);
                    }
                    if (newHeight > thumbnailHeight)
                    {
                        newHeight = thumbnailHeight;
                        newWidth = Convert.ToInt32((float)newHeight * aspectRatio);
                    }
                }
                else
                {
                    newWidth = Math.Min(originalImage.Width, thumbnailWidth);
                    newHeight = Math.Min(originalImage.Height, thumbnailHeight);
                }
            }
        }

        private bool ImageHasCorrectThumbnail(Image originalImage, Image thumbnailImage, int thumbnailWidth, int thumbnailHeight, bool keepAspectRatio, bool expandToFill)
        {
          if (thumbnailImage == null)
            return false;
          int newWidth; int newHeight;

          GetThumbnailSize(originalImage, thumbnailWidth, thumbnailHeight, keepAspectRatio, expandToFill, out newWidth, out newHeight);

          return thumbnailImage.Width == newWidth && thumbnailImage.Height == newHeight;
        }

        /// <summary>
        /// Gets a thumbnail for an image.
        /// </summary>
        /// <param name="image">The image full path on the media storage.</param>
        /// <param name="thumbnailWidth">The thumbnail width in pixels.</param>
        /// <param name="thumbnailHeight">The thumbnail height in pixels.</param>
        /// <param name="keepAspectRatio">Indicates whether to keep the original image aspect ratio</param>
        /// <param name="expandToFill">Indicates whether the thumbnail should be expanded to fill the entire thumbnail bounds.</param>
        /// <returns>
        /// The thumbnail full path on the media storage.
        /// </returns>
        /// <remarks>If keepAspectRatio and expandToFill are both true, the thumbnail will be clipped to fit the thumbnail bounds.</remarks>
        public Thumbnail GetThumbnail(string image, int thumbnailWidth, int thumbnailHeight, bool keepAspectRatio, bool expandToFill) {
            if(image == null)  
              throw new ArgumentNullException("image");

            string imageName = Path.GetFileName(image);
            string mediaPath = image.Substring(0, image.Length - imageName.Length - 1);
            string thumbnailFolderPath = GetThumbnailFolder(mediaPath);

            return CreateThumbnail(image, thumbnailFolderPath, imageName, thumbnailWidth, thumbnailHeight, keepAspectRatio, expandToFill);
        }

        /// <summary>
        /// Deletes the thumbnail.
        /// </summary>
        /// <param name="image">The image.</param>
        public void DeleteThumbnail(string image)
        {
            string imageName = Path.GetFileName(image);
            string mediaPath = image.Substring(0, image.Length - imageName.Length - 1);
            string thumbnailFolderPath = GetThumbnailFolder(mediaPath);
            string thumbnailFilePath = _storageProvider.Combine(thumbnailFolderPath, imageName);

            if (_storageProvider.ListFiles(thumbnailFolderPath).Select(o => o.GetName()).Contains(imageName))
            {
                _storageProvider.DeleteFile(thumbnailFilePath);
            }
        }
    }
}