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
using System.ComponentModel;

namespace Calib3D.Marker {

  /// <summary>
  /// Detects MarkerPattern patterns in images.
  /// </summary>
  /// <remarks>
  /// The algorithm works by first binarizing the image into black and white. On this image the contours are extracted.
  /// For each extracted contour an poly-line through the contour is approximated. If the poly-line has exactly four vertices,
  /// the algorithm treats the poly-line as a possible marker border. For each marker hypothesis, the pixels contained in 
  /// inside the poly-line are back-projected to a square image of the same size (in pixels) as the marker image. This image 
  /// is called 'warped image'. Since the contour points do not empose a constant ordering, there are four possibilities for 
  /// the warped image to align with the marker image, namely: a rotation by 0°, 90°, 180°, 270°. For possible configuration 
  /// the marker image is matched with the transformed warped image. The best one (in terms of the normed sum of squared 
  /// pixel-intensity differences) is chosen if the error is below a predefined error. The best orientation is then used
  /// to order the corner pixels accordingly.
  /// </remarks>
  /// <remarks>
  /// Based on code from the Parsley project.
  /// http://parsley.googlecode.com
  /// </remarks>
  [Calib3D.SupportedPattern(typeof(MarkerPattern))]
  public class MarkerDetector : Calib3D.PatternDetector {
    private Emgu.CV.Image<Emgu.CV.Structure.Gray, byte> _binary_marker, _warped, _tmp;
    private int _binary_threshold;
    private float _max_error_normed;
    private System.Drawing.PointF[] _warp_dest;

    /// <summary>
    /// Construct with default parameters.
    /// </summary>
    public MarkerDetector() {
      _max_error_normed = 0.4f;
      _binary_threshold = 60;
    }

    /// <summary>
    /// Set the threshold for blackness, used to binarize the input image.
    /// All values less-than or equal to the treshold are made black, all others
    /// white.
    /// </summary>
    [Description("Set the threshold for blackness, used to binarize the input image." +
                 " All values less-than or equal to the treshold are made black, all others white")]
    public int BinaryThreshold {
      get { return _binary_threshold; }
      set { _binary_threshold = value; }
    }

    /// <summary>
    /// Get/Set the maximum allowed error for pattern detection.
    /// </summary>
    [Description("Get/Set the maximum allowed error for pattern detection.")]
    public float MaximumErrorNormed {
      get { return _max_error_normed; }
      set { _max_error_normed = value; }
    }
    
    public override DetectionResult FindPattern(Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> i) {
      if (this.Pattern == null)
        throw new ArgumentNullException("No pattern specified");

      DetectionResult dr = new DetectionResult(this, this.Pattern);
      dr.Success = false;

      float best_error = _max_error_normed;

      // Find contour points in black/white image
      Emgu.CV.Contour<System.Drawing.Point> contour_points;
      Emgu.CV.Image<Emgu.CV.Structure.Gray, byte> my_img = i.Convert<Emgu.CV.Structure.Gray, Byte>();

      using (Emgu.CV.Image<Emgu.CV.Structure.Gray, byte> binary = new Emgu.CV.Image<Emgu.CV.Structure.Gray, byte>(my_img.Size)) {
        Emgu.CV.CvInvoke.cvThreshold(
          my_img, binary,
          _binary_threshold, 255,
          Emgu.CV.CvEnum.THRESH.CV_THRESH_BINARY | Emgu.CV.CvEnum.THRESH.CV_THRESH_OTSU);
        binary._Not(); // Contour is searched on white points, marker envelope is black.
        contour_points = binary.FindContours();
      }

      // Loop over all contours

      Emgu.CV.MemStorage _contour_storage = new Emgu.CV.MemStorage();
      while (contour_points != null) {
        // Approximate contour points by poly-lines.
        // For our marker-envelope should generate a poly-line consisting of four vertices.
        Emgu.CV.Contour<System.Drawing.Point> c = contour_points.ApproxPoly(contour_points.Perimeter * 0.05, _contour_storage);
        if (c.Total != 4 || c.Perimeter < 200) {
          contour_points = contour_points.HNext;
          continue;
        }

        // Warp content of poly-line as if looking at it from the top
        System.Drawing.PointF[] warp_source = new System.Drawing.PointF[] { 
          new System.Drawing.PointF(c[0].X, c[0].Y),
          new System.Drawing.PointF(c[1].X, c[1].Y),
          new System.Drawing.PointF(c[2].X, c[2].Y),
          new System.Drawing.PointF(c[3].X, c[3].Y)
        };

        Emgu.CV.Matrix<float> warp_matrix = new Emgu.CV.Matrix<float>(3, 3);
        Emgu.CV.CvInvoke.cvGetPerspectiveTransform(warp_source, _warp_dest, warp_matrix);
        Emgu.CV.CvInvoke.cvWarpPerspective(
          my_img, _warped, warp_matrix,
          (int)Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC + (int)Emgu.CV.CvEnum.WARP.CV_WARP_FILL_OUTLIERS,
          new Emgu.CV.Structure.MCvScalar(0)
        );

        Emgu.CV.CvInvoke.cvThreshold(
          _warped, _warped,
          _binary_threshold, 255,
          Emgu.CV.CvEnum.THRESH.CV_THRESH_BINARY | Emgu.CV.CvEnum.THRESH.CV_THRESH_OTSU);

        // Perform a template matching with the stored pattern in order to
        // determine if content of the envelope matches the stored pattern and
        // determine the orientation of the pattern in the image.
        // Orientation is encoded
        // 0: 0°, 1: 90°, 2: 180°, 3: 270°
        float error;
        int orientation;
        TemplateMatch(out error, out orientation);

        if (error < best_error) {
          best_error = error;
          int id_0 = orientation;
          int id_1 = (orientation + 1) % 4;
          int id_2 = (orientation + 2) % 4;
          int id_3 = (orientation + 3) % 4;

          // ids above are still counterclockwise ordered, we need to permute them
          // 0   3    0   1
          // +---+    +---+
          // |   | -> |   |
          // +---+    +---+
          // 1   2    2   3

          dr.Success = true;
          dr.ImagePoints.Clear();
          dr.ImagePoints.Add(c[id_0]);
          dr.ImagePoints.Add(c[id_3]);
          dr.ImagePoints.Add(c[id_1]);
          dr.ImagePoints.Add(c[id_2]);
        }

        contour_points = contour_points.HNext;
      } // End of contours

      return dr;
    }

    private void TemplateMatch(out float error, out int orientation) {
      // 0 degrees
      orientation = 0;
      error = (float)_warped.MatchTemplate(_binary_marker, Emgu.CV.CvEnum.TM_TYPE.CV_TM_SQDIFF_NORMED)[0, 0].Intensity;

      // 90 degrees
      Emgu.CV.CvInvoke.cvTranspose(_warped, _tmp);
      Emgu.CV.CvInvoke.cvFlip(_tmp, IntPtr.Zero, 1); // y-axis 
      float err = (float)_tmp.MatchTemplate(_binary_marker, Emgu.CV.CvEnum.TM_TYPE.CV_TM_SQDIFF_NORMED)[0, 0].Intensity;
      if (err < error) {
        error = err;
        orientation = 1;
      }

      // 180 degrees
      Emgu.CV.CvInvoke.cvFlip(_warped, _tmp, -1);
      err = (float)_tmp.MatchTemplate(_binary_marker, Emgu.CV.CvEnum.TM_TYPE.CV_TM_SQDIFF_NORMED)[0, 0].Intensity;
      if (err < error) {
        error = err;
        orientation = 2;
      }

      // 270 degrees
      Emgu.CV.CvInvoke.cvTranspose(_warped, _tmp);
      Emgu.CV.CvInvoke.cvFlip(_tmp, IntPtr.Zero, 0); // x-axis 
      err = (float)_tmp.MatchTemplate(_binary_marker, Emgu.CV.CvEnum.TM_TYPE.CV_TM_SQDIFF_NORMED)[0, 0].Intensity;
      if (err < error) {
        error = err;
        orientation = 3;
      }
    }

    /// <summary>
    /// Prepare for pattern
    /// </summary>
    protected override void PatternUpdated() {
      base.PatternUpdated();

      MarkerPattern mp = this.Pattern as MarkerPattern;

      if (mp == null)
        return;

      _binary_marker = mp.MarkerImage.Convert<Emgu.CV.Structure.Gray, byte>();
      int marker_image_size = _binary_marker.Width;

      // Warp points are specified in counter-clockwise order
      // since cvApproxpoly seems to return poly-points in counter-clockwise order.
      //
      // 0   3
      // +---+
      // |   |
      // +---+
      // 1   2
      _warp_dest = new System.Drawing.PointF[] { 
          new System.Drawing.PointF(0, 0),
          new System.Drawing.PointF(0, marker_image_size),
          new System.Drawing.PointF(marker_image_size, marker_image_size),
          new System.Drawing.PointF(marker_image_size, 0)
        };

      // Storage to hold warped image
      _warped = new Emgu.CV.Image<Emgu.CV.Structure.Gray, byte>(marker_image_size, marker_image_size);
      // Storage to test orientations
      _tmp = new Emgu.CV.Image<Emgu.CV.Structure.Gray, byte>(marker_image_size, marker_image_size);
    }


  }
}
