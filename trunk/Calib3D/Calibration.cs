using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calib3D {

  /// <summary>
  /// Perform intrinsic/extrinsic camera calibration from point correspondences.
  /// </summary>
  public static class Calibration {

    /// <summary>
    /// Caluculate instrinsic camera parameters from correspondences.
    /// </summary>
    /// <remarks>
    /// Besides the intrinsic camera parameters, extrinsic camera parameters are returned
    /// for each view in the correspondence set.
    /// </remarks>
    /// <param name="c">Model/image correspondences for multiple views</param>
    /// <param name="image_size">Size of source image</param>
    /// <returns>Calibration result</returns>
    public static CalibrationResult CalculateIntrinsics(Correspondences c, System.Drawing.Size image_size) {
      if (c.CorrespondenceCount < 4)
        throw new ArgumentException("At least four correspondences are required");

      CalibrationResult cr = new CalibrationResult();
      cr.Correspondences = c;
      cr.Intrinsics = new Emgu.CV.IntrinsicCameraParameters();

      Emgu.CV.ExtrinsicCameraParameters[] ext;

      cr.ReprojectionError = (float)Emgu.CV.CameraCalibration.CalibrateCamera(
        c.ModelPoints, c.ImagePoints, image_size,
        cr.Intrinsics,
        Emgu.CV.CvEnum.CALIB_TYPE.DEFAULT,
        out ext);

      cr.Extrinsics = ext;

      return cr;
    }

    /// <summary>
    /// Calculate intrinsic camera parameters for correspondences in each view.
    /// </summary>
    /// <param name="c">Model/image correspondences for multiple views</param>
    /// <param name="intrinsics">Intrinsic camera parameter</param>
    /// <returns>Calibration result</returns>
    public static CalibrationResult CalculateExtrinsics(
      Correspondences c,
      Emgu.CV.IntrinsicCameraParameters intrinsics) 
    {
      if (c.CorrespondenceCount < 4)
        throw new ArgumentException("At least four correspondences are required");

      CalibrationResult cr = new CalibrationResult();
      cr.Correspondences = c;
      cr.Intrinsics = intrinsics;

      List<Emgu.CV.ExtrinsicCameraParameters> ecp = new List<Emgu.CV.ExtrinsicCameraParameters>();
      for (int i = 0; i < c.ViewCount; ++i) {
        ecp.Add(
          Emgu.CV.CameraCalibration.FindExtrinsicCameraParams2(
            c.ModelPoints[i], c.ImagePoints[i],
            intrinsics));
      }
      cr.Extrinsics = ecp.ToArray();

      // to calculate the projection error see
      // http://tech.groups.yahoo.com/group/OpenCV/message/76036

      return cr;
    }

  }
}
