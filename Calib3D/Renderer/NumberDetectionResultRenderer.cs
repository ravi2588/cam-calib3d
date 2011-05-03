///
/// <project>Calib3D http://code.google.com/p/cam-calib3d/ </project>
/// <author>Christoph Heindl</author>
/// <copyright>Copyright (c) 2011, Christoph Heindl</copyright>
/// <license>New BSD License</license>
///

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calib3D.Renderer {

  /// <summary>
  /// Renders red/green circles and numbers for each image point.
  /// </summary>
  public class NumberDetectionResultRenderer : DetectionResultRenderer {

    /// <summary>
    /// Construct from detection result.
    /// </summary>
    /// <param name="dr"></param>
    public NumberDetectionResultRenderer(DetectionResult dr)
      : base(dr) { }

    /// <summary>
    /// Render result to image.
    /// </summary>
    /// <param name="i">Image to overlay with result</param>
    public override void Render(Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> i) {
      if (this.DetectionResult.ImagePoints.Count == 0) {
        return;
      }

      System.Drawing.Color color = this.DetectionResult.Success ? System.Drawing.Color.Green : System.Drawing.Color.Red;
      Emgu.CV.Structure.Bgr bgr = new Emgu.CV.Structure.Bgr(color);
      Emgu.CV.Structure.MCvFont f = new Emgu.CV.Structure.MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_PLAIN, 0.8, 0.8);

      int count = 1;
      foreach (System.Drawing.PointF point in this.DetectionResult.ImagePoints) {
        i.Draw(new Emgu.CV.Structure.CircleF(point, 4), bgr, 2);

        System.Drawing.Point ip = new System.Drawing.Point(
                                      (int)(Math.Round(point.X)),
                                      (int)(Math.Round(point.Y)));
        i.Draw(count.ToString(), ref f, new System.Drawing.Point(ip.X + 5, ip.Y - 5), bgr);

        count++;
      }
    }
  }
}
