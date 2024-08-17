using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Metalogix
{
    public abstract class ImageCache
    {
        private static Dictionary<string, Image> m_data;

        static ImageCache()
        {
            ImageCache.m_data = new Dictionary<string, Image>();
        }

        protected ImageCache()
        {
        }

        public static Image GetIconByExtensionAsImage(string sImageName)
        {
            if (string.IsNullOrEmpty(sImageName))
            {
                return null;
            }

            if (ImageCache.m_data.ContainsKey(sImageName))
            {
                return ImageCache.m_data[sImageName];
            }

            Image bitmap = ImageCache.GetIconByName(sImageName, ImageCache.IconSize.Small, false).ToBitmap();
            ImageCache.m_data.Add(sImageName, bitmap);
            return bitmap;
        }

        public static Icon GetIconByName(string name, ImageCache.IconSize size, bool linkOverlay)
        {
            Metalogix.NativeMethods.SHFILEINFO sHFILEINFO = new Metalogix.NativeMethods.SHFILEINFO();
            uint num = 272;
            if (linkOverlay)
            {
                num += 32768;
            }

            if (ImageCache.IconSize.Small != size)
            {
                num = num;
            }
            else
            {
                num++;
            }

            Metalogix.NativeMethods.SHGetFileInfo(name, 128, ref sHFILEINFO, (uint)Marshal.SizeOf(sHFILEINFO), num);
            Icon icon = (Icon)Icon.FromHandle(sHFILEINFO.hIcon).Clone();
            Metalogix.NativeMethods.DestroyIcon(sHFILEINFO.hIcon);
            return icon;
        }

        public static Image GetImage(string sImageName, Assembly assembly)
        {
            if (string.IsNullOrEmpty(sImageName))
            {
                return null;
            }

            if (ImageCache.m_data.ContainsKey(sImageName))
            {
                return ImageCache.m_data[sImageName];
            }

            Image bitmap = null;
            Stream manifestResourceStream = assembly.GetManifestResourceStream(sImageName);
            if (manifestResourceStream != null)
            {
                string str = sImageName.Substring(sImageName.Length - Math.Min(4, sImageName.Length));
                string str1 = str;
                if (str != null)
                {
                    if (str1 == ".ico")
                    {
                        bitmap = (new Icon(manifestResourceStream)).ToBitmap();
                    }
                    else if (str1 == ".bmp" || str1 == ".png")
                    {
                        bitmap = Image.FromStream(manifestResourceStream);
                        Bitmap bitmap1 = new Bitmap(bitmap.Width, bitmap.Height);
                        bitmap1.SetResolution(bitmap.HorizontalResolution, bitmap.VerticalResolution);
                        using (Graphics graphic = Graphics.FromImage(bitmap1))
                        {
                            graphic.DrawImage(bitmap, 0, 0);
                        }

                        bitmap = bitmap1;
                    }
                }

                manifestResourceStream.Close();
            }

            if (bitmap != null)
            {
                ImageCache.m_data.Add(sImageName, bitmap);
            }

            return bitmap;
        }

        public enum IconSize
        {
            Large,
            Small
        }
    }
}