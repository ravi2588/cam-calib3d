using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Calib3D.IO {

  /// <summary>
  /// Simplifies loading of images from files.
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

      if (Directory.Exists(path)) {
        // Load all images from path
        foreach (string file in GetFiles(path, pattern)) {
          yield return new Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte>(file);
        }
      } else if (File.Exists(path)) {
        // Load single image
        yield return new Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte>(path);
      }
    }

    /// <summary>
    /// Get all files in directory matching certain extensions.
    /// </summary>
    /// <param name="path">Path to directory</param>
    /// <param name="searchPattern">Pattern supporting multiple extensions separated by ';'</param>
    /// <returns></returns>
    protected static string[] GetFiles(string path, string pattern) {
      string[] exts = pattern.Split(';');

      List<string> files = new List<string>();
      foreach (string filter in exts) {
        files.AddRange(System.IO.Directory.GetFiles(path, filter));
      }
      return files.ToArray();
    }

  }
}
