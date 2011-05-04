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

namespace Calib3D {

  /// <summary>
  /// Perform intrinsic/extrinsic camera calibration from point correspondences.
  /// </summary>
  public class Calibration {

    /// <summary>
    /// Caluculate instrinsic camera parameters from correspondences.
    /// </summary>
    /// <remarks>
    /// Besides the intrinsic camera parameters, extrinsic camera parameters are returned
    /// for each view in the correspondence set.
    /// </remarks>
    /// <param name="c">Model/image correspondences of multiple views</param>
    /// <param name="image_size">Size of source image</param>
    /// <returns>Calibration result</returns>
    public static CalibrationResult GetIntrinsics(MultiViewCorrespondences c, System.Drawing.Size image_size) {
      CalibrationResult cr = new CalibrationResult();
      cr.Intrinsics = new Emgu.CV.IntrinsicCameraParameters();
      PerformIntrinsicCalibration(cr, c, image_size, Emgu.CV.CvEnum.CALIB_TYPE.DEFAULT);
      return cr;
    }

    /// <summary>
    /// Caluculate instrinsic camera parameters from correspondences using a start solution.
    /// </summary>
    /// <remarks>
    /// Besides the intrinsic camera parameters, extrinsic camera parameters are returned
    /// for each view in the correspondence set.
    /// </remarks>
    /// <param name="c">Model/image correspondences of multiple views</param>
    /// <param name="image_size">Size of source image</param>
    /// <param name="intrinsic_guess">Intrinsic parameter guess to use as start solution</param>
    /// <returns>Calibration result</returns>
    public static CalibrationResult GetIntrinsics(
      MultiViewCorrespondences c, 
      System.Drawing.Size image_size, 
      Emgu.CV.Matrix<double> intrinsic_guess) 
    {
      CalibrationResult cr = new CalibrationResult();
      cr.Intrinsics = new Emgu.CV.IntrinsicCameraParameters();
      cr.Intrinsics.IntrinsicMatrix = intrinsic_guess;
      PerformIntrinsicCalibration(cr, c, image_size, Emgu.CV.CvEnum.CALIB_TYPE.CV_CALIB_USE_INTRINSIC_GUESS);
      return cr;
    }

    /// <summary>
    /// Calculate extrinsic camera parameters for correspondences in each view.
    /// </summary>
    /// <param name="c">Model/image correspondences of multiple views</param>
    /// <param name="intrinsics">Intrinsic camera parameter</param>
    /// <returns>Calibration result</returns>
    public static CalibrationResult GetExtrinsics(
      MultiViewCorrespondences c,
      Emgu.CV.IntrinsicCameraParameters intrinsics) 
    {
      if (c.CorrespondenceCount < 4)
        throw new ArgumentException("At least four correspondences are required");

      CalibrationResult cr = new CalibrationResult();
      cr.Intrinsics = intrinsics;

      List<Emgu.CV.ExtrinsicCameraParameters> ecp = new List<Emgu.CV.ExtrinsicCameraParameters>();
      for (int i = 0; i < c.ViewCount; ++i) {
        ecp.Add(
          Emgu.CV.CameraCalibration.FindExtrinsicCameraParams2(
            c.ModelPoints[i], c.ImagePoints[i],
            intrinsics));
      }
      cr.Extrinsics = ecp.ToArray();
      cr.ReprojectionError = GetReprojectionError(intrinsics, cr.Extrinsics, c);
      return cr;
    }

    /// <summary>
    /// Calculate extrinsic camera parameters for single view correspondences
    /// </summary>
    /// <param name="c">Model/image correspondences of view</param>
    /// <param name="intrinsics">Intrinsic camera parameter</param>
    /// <returns>Calibration result</returns>
    public static CalibrationResult GetExtrinsics(
      SingleViewCorrespondences c,
      Emgu.CV.IntrinsicCameraParameters intrinsics) 
    {
      return GetExtrinsics(c.ToMultiViewCorrespondences(), intrinsics);
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
    /// <param name="icp">Intrinsic camera parameters</param>
    /// <param name="ecp">Extrinsic camera parameters for each view</param>
    /// <param name="c">Point correspondences between model and image</param>
    /// <returns>The RMS projection error in pixels</returns>
    public static float GetReprojectionError(
      Emgu.CV.IntrinsicCameraParameters icp,
      Emgu.CV.ExtrinsicCameraParameters[] ecp,
      MultiViewCorrespondences c) 
    {
      double err2 = 0;

      // For each view
      for (int i = 0; i < c.ViewCount; ++i) {
        // Reproject all model points (i.e. back project)
        System.Drawing.PointF[] projected = Emgu.CV.CameraCalibration.ProjectPoints(
          c.ModelPoints[i],
          ecp[i],
          icp);
        
        // Calculate the squared L2 error between each back-projection and 
        // associated image point
        for (int j = 0; j < projected.Length; ++j) {
          double dx = projected[j].X - c.ImagePoints[i][j].X;
          double dy = projected[j].Y - c.ImagePoints[i][j].Y;
          err2 += dx * dx + dy * dy;
        }
      }

      return (float)Math.Sqrt(err2 / c.CorrespondenceCount);
    }

    /// <summary>
    /// Perform intrinsic camera estimation using specific calibration type
    /// </summary>
    /// <param name="cr">Calibration result containing initial parameters</param>
    /// <param name="c">Point correspondences</param>
    /// <param name="image_size">Image size</param>
    /// <param name="calib_type">Calibration algorithm type</param>
    protected static void PerformIntrinsicCalibration(
      CalibrationResult cr,
      MultiViewCorrespondences c,
      System.Drawing.Size image_size,
      Emgu.CV.CvEnum.CALIB_TYPE calib_type) 
    {
      if (c.CorrespondenceCount < 4)
        throw new ArgumentException("At least four correspondences are required");

      Emgu.CV.ExtrinsicCameraParameters[] ext;

      Emgu.CV.CameraCalibration.CalibrateCamera(
        c.ModelPoints, c.ImagePoints, image_size,
        cr.Intrinsics,
        calib_type,
        out ext);

      cr.Extrinsics = ext;
      cr.ReprojectionError = GetReprojectionError(cr.Intrinsics, cr.Extrinsics, c);
    }



  }
}
