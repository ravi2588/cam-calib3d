/*
 * Calib3D http://code.google.com/p/cam-calib3d/
 * Copyright (c) 2011, Christoph Heindl. All rights reserved.
 * Code license:	New BSD License
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calib3D.Overlays {

  /// <summary>
  /// Simple image border overlay.
  /// </summary>
  public class Border : DetectionResult.IOverlayProvider {

    public void Overlay(Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> i, DetectionResult dr) {

      System.Drawing.Color color = dr.Success ? System.Drawing.Color.Green : System.Drawing.Color.Red;
      Emgu.CV.Structure.Bgr bgr = new Emgu.CV.Structure.Bgr(color);
      i.Draw(new System.Drawing.Rectangle(new System.Drawing.Point(0, 0), i.Size), bgr, 5);
    }

  }
}
