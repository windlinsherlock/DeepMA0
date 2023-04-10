using System;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp.Processing;

namespace PACS.Shared.Commons
{
    public class Util
    {
        public static byte[] GetThumb(byte[] file)
        {
            MemoryStream ms = new MemoryStream(file);
            Image image = Image.Load(ms);

            int width = 0, height = 0;
            if (image.Width > image.Height)
            {
                width = 150;
                height = 150 * image.Height / image.Width;
            }
            else
            {
                height = 150;
                width = 150 * image.Width / image.Height;
            }
            
            image.Mutate(i => i.Resize(width, height));

            using (MemoryStream s = new MemoryStream())
            {
                image.SaveAsJpeg(s);
                return s.ToArray();
            }
        }
/*
        public static async void SaveImage(byte[] file,string name)
        {
            Image image = Image.Load(file);
            try { await image.SaveAsJpegAsync( "Image//"+ name + ".jpg"); }
            catch { }
            
            image.Dispose();
            return;
        }

        public static byte[] LoadImage(string name)
        {
            try
            {
                Image image = Image.Load("Image\\" + name);

                using (MemoryStream s = new MemoryStream())
                {
                    image.SaveAsJpegAsync(s);
                    return s.ToArray();
                }

            }
            catch { return null; }

        }*/
    }
}
