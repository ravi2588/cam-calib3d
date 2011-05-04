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

namespace Calib3D.IO.Extensions {

  /// <summary>
  /// Provides formatting extensions for various datatypes and formats.
  /// </summary>
  public static class PrettyPrintExtensions {

    /// <summary>
    /// Pretty print a matrix.
    /// </summary>
    /// <param name="matrix">Matrix</param>
    /// <returns>Formatting string</returns>
    public static string PrettyPrint(this Emgu.CV.Matrix<double> matrix) 
    {
      StringBuilder sb = new StringBuilder();

      for (int i = 0; i < matrix.Rows; ++i) {
        for (int j = 0; j < matrix.Cols; ++j) {
          sb.AppendFormat("{0,10:f4} ", matrix[i, j]);
        }
        sb.Remove(sb.Length - 1, 1);
        if (i < matrix.Rows - 1)
          sb.AppendLine();
      }
      return sb.ToString();
    }

    /// <summary>
    /// Pretty print intrinsic matrix values and distortion coefficients.
    /// </summary>
    /// <param name="i"></param>
    /// <returns>Formatting string</returns>
    public static string PrettyPrint(this Emgu.CV.IntrinsicCameraParameters i) 
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("Intrinsic Matrix");
      sb.AppendLine(i.IntrinsicMatrix.PrettyPrint());
      sb.AppendLine("Radial Distortion");
      sb.AppendFormat("  k1={0:f6}, k2={1:f6}", i.DistortionCoeffs[0, 0], i.DistortionCoeffs[1, 0]);
      if (i.DistortionCoeffs.Rows == 5) {
        sb.AppendFormat(", k3={0:f6}", i.DistortionCoeffs[4, 0]);
        sb.AppendLine();
      } else if (i.DistortionCoeffs.Rows == 8) {
        sb.AppendFormat(", k3={0:f6}, k4={0:f6}, k5={1:f6}, k6={1:f6}", 
          i.DistortionCoeffs[4, 0], i.DistortionCoeffs[5, 0], i.DistortionCoeffs[6, 0], i.DistortionCoeffs[7, 0]);
        sb.AppendLine();
      }
      sb.AppendLine("Tangential Distortion");
      sb.AppendFormat("  p1={0:f6}, p2={1:f6}", i.DistortionCoeffs[2, 0], i.DistortionCoeffs[3, 0]);
      
      return sb.ToString();
    }

    /// <summary>
    /// Pretty print intrinsics using human readable values.
    /// </summary>
    /// <param name="i"></param>
    /// <param name="image_width">Image width</param>
    /// <param name="image_height">Image height</param>
    /// <param name="chip_width">Chip width</param>
    /// <param name="chip_height">Chip height</param>
    /// <returns>Formatting string</returns>
    public static string PrettyPrint(
      this Emgu.CV.IntrinsicCameraParameters i, 
      int image_width, int image_height, 
      double chip_width, double chip_height) 
    {    
      double fovx, fovy, focal_length, pixel_aspect_ratio;
      Emgu.CV.Structure.MCvPoint2D64f principal_point;

      i.GetIntrinsicMatrixValues(
        image_width, image_height, chip_width, chip_height,
        out fovx, out fovy, out focal_length, out principal_point, out pixel_aspect_ratio);

      StringBuilder sb = new StringBuilder();
      sb.AppendLine("Focal Length");
      sb.AppendFormat("  f={0:f6}", focal_length);
      sb.AppendLine();
      sb.AppendLine("Principal Point");
      sb.AppendFormat("  x={0:f6} y={0:f6}", principal_point.x, principal_point.y);
      sb.AppendLine();
      sb.AppendLine("Field of View");
      sb.AppendFormat("  x-angle={0:f6}, y-angle{1:f6}", fovx, fovy);

      return sb.ToString();
    }

    /// <summary>
    /// Pretty print extrinsic matrix values.
    /// </summary>
    /// <returns>Formatting string</returns>
    public static string PrettyPrint(this Emgu.CV.ExtrinsicCameraParameters e) {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("Extrinsic Matrix");
      sb.Append(e.ExtrinsicMatrix.PrettyPrint());

      return sb.ToString();
    }

    /// <summary>
    /// Pretty print calibration result.
    /// </summary>
    /// <param name="cr">Calibration result</param>
    /// <returns>Formatting string</returns>
    public static string PrettyPrint(this CalibrationResult cr) {
      return PrettyPrint(cr, true, true);
    }

    /// <summary>
    /// Pretty print calibration result.
    /// </summary>
    /// <param name="cr">Calibration result</param>
    /// <param name="include_intrinsic">Include intrinsic info in output</param>
    /// <param name="include_extrinsic">Include extrinsic info in output</param>
    /// <returns>Formatting string</returns>
    public static string PrettyPrint(this CalibrationResult cr, bool include_intrinsic, bool include_extrinsic) {
      StringBuilder sb = new StringBuilder();
      if (include_intrinsic) {
        sb.AppendLine("Intrinsic Calibration");
        sb.AppendLine("=====================");
        sb.AppendLine(cr.Intrinsics.PrettyPrint());
      }
      if (include_extrinsic) {
        sb.AppendLine();
        sb.AppendLine("Extrinsic Calibration");
        sb.AppendLine("=====================");
        int i = 0;
        foreach (Emgu.CV.ExtrinsicCameraParameters e in cr.Extrinsics) {
          sb.AppendLine(string.Format("View {0}", i));
          sb.AppendLine(e.PrettyPrint());
          sb.AppendLine("-----------");
          i += 1;
        }
      }
      return sb.ToString();
    }

  }
}
