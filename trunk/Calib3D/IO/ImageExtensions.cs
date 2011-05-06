using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calib3D.IO.Extensions {

  /// <summary>
  /// Emgu Image extension methods
  /// </summary>
  public static class ImageExtensions {

    /// <summary>
    /// Show image and continue processing
    /// </summary>
    /// <param name="i">Image</param>
    /// <param name="window_name">Window name</param>
    public static void Show<T>(this Emgu.CV.Image<T, byte> i, string window_name) where T : struct, Emgu.CV.IColor {
      Calib3D.IO.Images.Show<T>(i, 1, window_name);
    }

    /// <summary>
    /// Show image and block current thread until the user closed the window
    /// </summary>
    /// <param name="i">Image</param>
    /// <param name="window_name">Window name</param>
    public static void ShowWait<T>(this Emgu.CV.Image<T, byte> i, string window_name) where T : struct, Emgu.CV.IColor {
      Calib3D.IO.Images.Show<T>(i, 0, window_name);
    }
  }
}
