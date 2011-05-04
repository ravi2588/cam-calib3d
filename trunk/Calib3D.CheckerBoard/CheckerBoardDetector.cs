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
using System.ComponentModel.Composition;

namespace Calib3D.CheckerBoard {

  /// <summary>
  /// Detects checkerboard patterns in images.
  /// </summary>
  /// <remarks>
  /// Based on code from the Parsley project.
  /// http://parsley.googlecode.com
  /// </remarks>
  [Calib3D.SupportedPattern(typeof(CheckerBoardPattern))]
  public class CheckerBoardDetector : Calib3D.PatternDetector {

    public override Calib3D.DetectionResult FindPattern(Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> i) {

      CheckerBoardPattern my_p = this.Pattern as CheckerBoardPattern;
      System.Drawing.PointF[] image_points;

      Emgu.CV.Image<Emgu.CV.Structure.Gray, byte> my_img = i.Convert<Emgu.CV.Structure.Gray, Byte>();
      my_img._EqualizeHist();

      bool found = Emgu.CV.CameraCalibration.FindChessboardCorners(
        my_img,
        my_p.CornerCount,
        Emgu.CV.CvEnum.CALIB_CB_TYPE.ADAPTIVE_THRESH |
        Emgu.CV.CvEnum.CALIB_CB_TYPE.FILTER_QUADS |
        Emgu.CV.CvEnum.CALIB_CB_TYPE.NORMALIZE_IMAGE,
        out image_points
      );

      if (found) {
        my_img.FindCornerSubPix(
          new System.Drawing.PointF[][] { image_points },
          new System.Drawing.Size(5, 5),
          new System.Drawing.Size(-1, -1),
          new Emgu.CV.Structure.MCvTermCriteria(0.001));

        Calib3D.SingleViewCorrespondences vc = new SingleViewCorrespondences(image_points, my_p.ModelPoints);

        return new Calib3D.DetectionResult(this, my_p, true, vc);
      } else {
        return new Calib3D.DetectionResult(this, my_p, false);
      }
    }
  }
}
