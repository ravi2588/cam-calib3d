/*
 * Calib3D http://code.google.com/p/cam-calib3d/
 * Copyright (c) 2011, Christoph Heindl. All rights reserved.
 * Code license:	New BSD License
 */

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
    /// <param name="wait_time_ms">Wait time in milliseconds. Use zero or negativ delay to wait forever.</param>
    public static void Show(Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> i, int wait_time_ms)
    {
      Show(i, wait_time_ms, "image");
    }

    /// <summary>
    /// Opens a new window showing the given image.
    /// </summary>
    /// <param name="i">Image to show</param>
    /// <param name="wait_time_ms">Wait time in milliseconds. Use zero or negativ delay to wait forever.</param>
    /// <param name="window_name">Window title</param>
    public static void Show(Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> i, int wait_time_ms, string window_name)
    {
      Emgu.CV.CvInvoke.cvNamedWindow(window_name);
      Emgu.CV.CvInvoke.cvShowImage(window_name, i.Ptr);
      Emgu.CV.CvInvoke.cvWaitKey(wait_time_ms);
    }

  }
}
