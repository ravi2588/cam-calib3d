using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Calib3D.IO {

  /// <summary>
  /// Simplifies I/O of images.
  /// </summary>
  public class Images {

    /// <summary>
    /// Load image(s) from path.
    /// </summary>
    /// <param name="path">If path is a directory all images within are loaded, 
    /// else it is assumed to represent a single image.</param>
    /// <returns>Loaded images.</returns>
    public static IEnumerable<Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte>> FromPath(string path) {
      return FromPath(path, "*.png;*.bmp;*.jpg");
    }

    /// <summary>
    /// Load image(s) from path.
    /// </summary>
    /// <param name="path">If path is a directory all images within are loaded, 
    /// else it is assumed to represent a single image.</param>
    /// <param name="pattern">Multiple wildchars separated by ';'</param>
    /// <returns>Loaded images.</returns>
    public static IEnumerable<Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte>> FromPath(string path, string pattern) {

      if (System.IO.Directory.Exists(path)) {
        // Load all images from path
        foreach (string file in IO.Directory.GetFiles(path, pattern)) {
          yield return new Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte>(file);
        }
      } else if (File.Exists(path)) {
        // Load single image
        yield return new Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte>(path);
      }
    }

    /// <summary>
    /// Opens a new window showing the given image.
    /// </summary>
    /// <param name="i">Image to show</param>
    /// <param name="wait_for_close">If set waits for the window to be closed by the user</param>
    public static void Show(Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> i, bool wait_for_close)
    {
      Show(i, wait_for_close, "image");
    }

    /// <summary>
    /// Opens a new window showing the given image.
    /// </summary>
    /// <param name="i">Image to show</param>
    /// <param name="wait_for_close">If set waits for the window to be closed by the user</param>
    /// <param name="window_name">Window title</param>
    public static void Show(Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> i, bool wait_for_close, string window_name)
    {
      Emgu.CV.CvInvoke.cvNamedWindow(window_name);
      Emgu.CV.CvInvoke.cvShowImage(window_name, i.Ptr);
      if (wait_for_close) {
        Emgu.CV.CvInvoke.cvWaitKey(0);
        Emgu.CV.CvInvoke.cvDestroyWindow("x");
      }
    }

  }
}
