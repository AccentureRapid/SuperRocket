using System.Drawing;


namespace Mello.ImageGallery.Helpers
{
    /// <summary>
    /// Image manipulation helper functionality.
    /// </summary>
	public static class ImageExtensions
    {
        /// <summary>
        /// Re-sizes an image in size maintaing best possible quality.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        public static Image HiqhQualityResize(this Image image, int width, int height)
        {
            Image imgResized = new Bitmap(width, height);
	        
			using (Graphics graphic = Graphics.FromImage(imgResized)) {
		        //Ensure high quality
		        graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
		        graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
		        graphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
		        graphic.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
		        //Resize
		        graphic.DrawImage(image, 0, 0, width, height);
	        }

	        return imgResized;
        }

        /// <summary>
        /// Crops an image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="cropWidth">Width of the crop.</param>
        /// <param name="cropHeight">Height of the crop.</param>
        /// <param name="startXPoint">The start X point.</param>
        /// <param name="startYPoint">The start Y point.</param>
        /// <returns></returns>
        public static Image Crop(this Image image, int cropWidth, int cropHeight, int startXPoint, int startYPoint)
        {
            Image imgCropped = new Bitmap(cropWidth, cropHeight);
	        
			using (Graphics graphic = Graphics.FromImage(imgCropped)) {
		        //Crop
		        //graphic.DrawImage(image, startXPoint, startYPoint, cropWidth, cropHeight)
		        graphic.DrawImage(image, new Rectangle(0, 0, cropWidth, cropHeight), startXPoint, startYPoint, cropWidth, cropHeight, GraphicsUnit.Pixel);
	        }

	        return imgCropped;
        }

        /// <summary>
        /// Calculates the top left point (X, Y co-ordinates) where a crop must occur to provide as much of the central image as possible.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="cropWidth">Crop width.</param>
        /// <param name="cropHeight">Crop height.</param>
        /// <returns>Point with X, Y co-ordinates.</returns>
        public static Point GetTopLeftStartCropPointForKeepCenter(this Image image, int cropWidth, int cropHeight)
        {
            Point pointStart = new Point();
            //X
            pointStart.X = (int)(image.Width - cropWidth) / 2;
            if (cropWidth + (pointStart.X * 2) < image.Width) pointStart.X -= 1;
            //Y
            pointStart.Y = (int)(image.Height - cropHeight) / 2;
            if (cropHeight + (pointStart.Y * 2) < image.Height) pointStart.Y -= 1;
            
			return pointStart;
        }
    }
}