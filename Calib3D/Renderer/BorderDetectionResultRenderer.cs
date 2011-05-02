/*
 * Calib3D http://code.google.com/p/cam-calib3d/
 * Copyright (c) 2011, Christoph Heindl. All rights reserved.
 * Code license:	New BSD License
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calib3D.Renderer {

  /// <summary>
  /// Augments image with red/green border.
  /// </summary>
  public class BorderDetectionResultRenderer : DetectionResultRenderer {

    /// <summary>
    /// Construct from detection result.
    /// </summary>
    /// <param name="dr">Detection result</param>
    public BorderDetectionResultRenderer(DetectionResult dr)
      : base(dr) { }

    /// <summary>
    /// Render result to image.
    /// </summary>
    /// <param name="i">Image to overlay with result</param>
    public override void Render(Emgu.CV.Image<Emgu.CV.Structure.Bgr,byte> i)
    {
      System.Drawing.Color color = this.DetectionResult.Success ? System.Drawing.Color.Green : System.Drawing.Color.Red;
      Emgu.CV.Structure.Bgr bgr = new Emgu.CV.Structure.Bgr(color);
      i.Draw(new System.Drawing.Rectangle(new System.Drawing.Point(0, 0), i.Size), bgr, 5);
    }
  }
}
