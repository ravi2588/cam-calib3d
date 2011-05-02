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
    public static CalibrationResult GetIntrinsics(Correspondences c, System.Drawing.Size image_size) {
      if (c.CorrespondenceCount < 4)
        throw new ArgumentException("At least four correspondences are required");

      CalibrationResult cr = new CalibrationResult();
      cr.Correspondences = c;
      cr.Intrinsics = new Emgu.CV.IntrinsicCameraParameters();

      Emgu.CV.ExtrinsicCameraParameters[] ext;

      double err = Emgu.CV.CameraCalibration.CalibrateCamera(
        c.ModelPoints, c.ImagePoints, image_size,
        cr.Intrinsics,
        Emgu.CV.CvEnum.CALIB_TYPE.DEFAULT,
        out ext);

      cr.Extrinsics = ext;
      cr.ReprojectionError = GetReprojectionError(cr);

      return cr;
    }

    /// <summary>
    /// Calculate intrinsic camera parameters for correspondences in each view.
    /// </summary>
    /// <param name="c">Model/image correspondences for multiple views</param>
    /// <param name="intrinsics">Intrinsic camera parameter</param>
    /// <returns>Calibration result</returns>
    public static CalibrationResult GetExtrinsics(
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
      cr.ReprojectionError = GetReprojectionError(cr);
      return cr;
    }

    /// <summary>
    /// Calculate the reprojection error in pixels.
    /// </summary>
    /// <remarks>
    /// Calculates the root mean square error of all correspondences using
    /// the reprojected model points. For each correspondence, the model point
    /// is reprojected to 2d. The squared L2 distance between the reprojected point
    /// and its associated image point is calculated. All squared distances are 
    /// accumulated and divided by the total number of correspondeces. The square
    /// root of this value is returned.
    /// 
    /// Matches current OpenCV implementation. We roll out our own implementation, since
    /// OpenCV supports the reprojection error only for intrinsic camera calibration.
    /// </remarks>
    /// <param name="cr">Calibration result having correspondences, intrinsics and extrinsics</param>
    /// <returns>The RMS projection error in pixels</returns>
    public static float GetReprojectionError(CalibrationResult cr) {
      double err2 = 0;

      // For each view
      for (int i = 0; i < cr.Correspondences.ViewCount; ++i) {
        // Reproject all model points (i.e. back project)
        System.Drawing.PointF[] projected = Emgu.CV.CameraCalibration.ProjectPoints(
          cr.Correspondences.ModelPoints[i],
          cr.Extrinsics[i],
          cr.Intrinsics);
        
        // Calculate the squared L2 error between each back-projection and 
        // associated image point
        for (int j = 0; j < projected.Length; ++j) {
          double dx = projected[j].X - cr.Correspondences.ImagePoints[i][j].X;
          double dy = projected[j].Y - cr.Correspondences.ImagePoints[i][j].Y;
          err2 += dx * dx + dy * dy;
        }
      }

      return (float)Math.Sqrt(err2 / cr.Correspondences.CorrespondenceCount);
    }

  }
}
