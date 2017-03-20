using Mello.ImageGallery.Models;
using Orchard;

namespace Mello.ImageGallery.Services {
    public interface IThumbnailService : IDependency {
        /// <summary>
        /// Gets a thumbnail for an image.
        /// </summary>
        /// <param name="imageMediaPath">The image full path on the media storage.</param>
        /// <param name="thumbnailWidth">The thumbnail width in pixels.</param>
        /// <param name="thumbnailHeight">The thumbnail height in pixels.</param>
        /// <param name="keepAspectRatio">Indicates whether to keep the original image aspect ratio.</param>
        /// <param name="expandToFill">Indicates whether the thumbnail should be expanded to fill the entire thumbnail bounds.</param>
        /// <returns>
        /// The thumbnail full path on the media storage.
        /// </returns>
        Thumbnail GetThumbnail(string imageMediaPath, int thumbnailWidth, int thumbnailHeight, bool keepAspectRatio, bool expandToFill);

        /// <summary>
        /// Deletes the thumbnail.
        /// </summary>
        /// <param name="imageMediaPath">The image full path on the media storage.</param>
        void DeleteThumbnail(string imageMediaPath);
    }
}