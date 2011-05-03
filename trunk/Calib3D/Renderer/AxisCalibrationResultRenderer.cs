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
  /// Augments image with 3d model coordinate frame.
  /// </summary>
  public class AxisCalibrationResultRenderer : CalibrationResultRenderer {

    /// <summary>
    /// Construct from calibration result.
    /// </summary>
    /// <param name="dr">Calibration result</param>
    public AxisCalibrationResultRenderer(CalibrationResult cr)
      : base(cr) { }

    /// <summary>
    /// Render result to image.
    /// </summary>
    /// <param name="i">Image to overlay with result</param>
    /// <param name="view_id">View-id to use</param>
    public override void Render(Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> i, int view_id) 
    {
      float extension = Math.Min(i.Size.Width, i.Size.Height) * 0.4f;
      int thickness = 2;

      // Back-project 3d axis points
      System.Drawing.PointF[] coords = Emgu.CV.CameraCalibration.ProjectPoints(
        new Emgu.CV.Structure.MCvPoint3D32f[] { 
          new Emgu.CV.Structure.MCvPoint3D32f(0, 0, 0),
          new Emgu.CV.Structure.MCvPoint3D32f(extension, 0, 0),
          new Emgu.CV.Structure.MCvPoint3D32f(0, extension, 0),
          new Emgu.CV.Structure.MCvPoint3D32f(0, 0, extension),
        },
        this.CalibrationResult.Extrinsics[view_id], 
        this.CalibrationResult.Intrinsics);

      // Draw colored axis lines
      i.Draw(new Emgu.CV.Structure.LineSegment2DF(coords[0], coords[1]), new Emgu.CV.Structure.Bgr(System.Drawing.Color.Red), thickness);
      i.Draw(new Emgu.CV.Structure.LineSegment2DF(coords[0], coords[2]), new Emgu.CV.Structure.Bgr(System.Drawing.Color.Green), thickness);
      i.Draw(new Emgu.CV.Structure.LineSegment2DF(coords[0], coords[3]), new Emgu.CV.Structure.Bgr(System.Drawing.Color.Blue), thickness);     
    }
  }
}
