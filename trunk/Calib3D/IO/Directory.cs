/*
 * Calib3D http://code.google.com/p/cam-calib3d/
 * Copyright (c) 2011, Christoph Heindl. All rights reserved.
 * Code license:	New BSD License
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calib3D.IO {

  /// <summary>
  /// Directory helpers.
  /// </summary>
  public static class Directory {

    /// <summary>
    /// Get all files in directory matching certain extensions.
    /// </summary>
    /// <param name="path">Path to directory</param>
    /// <param name="searchPattern">Pattern supporting multiple extensions separated by ';'</param>
    /// <returns></returns>
    public static string[] GetFiles(string path, string pattern) {
      string[] exts = pattern.Split(';');

      List<string> files = new List<string>();
      foreach (string filter in exts) {
        files.AddRange(System.IO.Directory.GetFiles(path, filter));
      }
      return files.ToArray();
    }

  }
}
