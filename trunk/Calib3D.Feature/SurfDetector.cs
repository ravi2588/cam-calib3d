using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Calib3D.IO.Extensions;

namespace Calib3D.Feature {

  /// <summary>
  /// Detects TexturedRectanglePattern patterns in images using SURF features.
  /// </summary>
  [Calib3D.SupportedPattern(typeof(TexturedRectanglePattern))]
  public class SurfDetector : Calib3D.PatternDetector {
    private Emgu.CV.Features2D.Features2DTracker _tracker;
    private Emgu.CV.Features2D.SURFDetector _feature_provider;
    private System.Drawing.SizeF _scalings;
    private double _hessian_th;
    private EDescriptorSize _desc_size;

    /// <summary>
    /// SURF descriptor size in number of elements.
    /// </summary>
    public enum EDescriptorSize {
      Basic64,
      Extended128
    };

    public SurfDetector() {
      _desc_size = SurfDetector.EDescriptorSize.Basic64;
      _hessian_th = 500;
    }

    /// <summary>
    /// Get/Set the threshold that keypoints must pass to be considered as features.
    /// </summary>
    public double HessianThreshold {
      get { return _hessian_th; }
      set {
        _hessian_th = value;
        PatternUpdated();
      }
    }

     /// <summary>
    /// Get/Set the threshold that keypoints must pass to be considered as features.
    /// </summary>
    public EDescriptorSize DescriptorSize {
      get { return _desc_size; }
      set {
        _desc_size = value;
        PatternUpdated();
      }
    }

    public override DetectionResult FindPattern(Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> i) {
      if (this.Pattern == null)
        throw new ArgumentNullException("No pattern specified");

      Emgu.CV.Image<Emgu.CV.Structure.Gray, byte> g = i.Convert<Emgu.CV.Structure.Gray, byte>();
      Emgu.CV.Features2D.ImageFeature[] image_features = _feature_provider.DetectFeatures(g, null);

      if (image_features.Length == 0) {
        return new DetectionResult(this, this.Pattern, false);
      }

      // Recognitions seems to degenerate over multiple images.
      // See: http://www.emgu.com/forum/viewtopic.php?f=7&t=1609
      // See: http://www.emgu.com/bugs/show_bug.cgi?id=38

      Emgu.CV.Features2D.Features2DTracker.MatchedImageFeature[] matched_features = _tracker.MatchFeature(image_features, 2);
      matched_features = Emgu.CV.Features2D.Features2DTracker.VoteForUniqueness(matched_features, 0.8);
      matched_features = Emgu.CV.Features2D.Features2DTracker.VoteForSizeAndOrientation(matched_features, 1.5, 20);

      Emgu.CV.HomographyMatrix homography = Emgu.CV.Features2D.Features2DTracker.GetHomographyMatrixFromMatchedFeatures(matched_features);

      if (homography != null) {
        // Homography was probably estimated based on RANSAC. Therefore, we need to separate 
        // all features into inliers and outlier. Only inliers will make it into the DetectionResult.

        System.Drawing.PointF[] image_points = new System.Drawing.PointF[matched_features.Length];
        System.Drawing.PointF[] model_points = new System.Drawing.PointF[matched_features.Length];

        for (int j = 0; j < matched_features.Length; ++j) {
          image_points[j] = matched_features[j].ObservedFeature.KeyPoint.Point;
          model_points[j] = matched_features[j].SimilarFeatures[0].Feature.KeyPoint.Point;
        }

        homography.ProjectPoints(model_points);

        // Only those image/model point pairs are considered inliers, that remain close
        // after projection.
        DetectionResult dr = new DetectionResult(this, this.Pattern);
        SingleViewCorrespondences c = dr.ViewCorrespondences;

        for (int j = 0; j < matched_features.Length; ++j) {
          float dx = image_points[j].X - model_points[j].X;
          float dy = image_points[j].Y - model_points[j].Y;
          float d2 = dx * dx + dy * dy;

          if (d2 < 100) {
            c.ImagePoints.Add(image_points[j]);
            c.ModelPoints.Add(ConvertModelPoint(matched_features[j].SimilarFeatures[0].Feature.KeyPoint.Point));
          }
        }

        dr.Success = c.Count >= 4;

        return dr;
      }
      else {
        // No homography found
        return new DetectionResult(this, this.Pattern, false);
      }
    }

    /// <summary>
    /// Convert point in pixels to model point using appropriate scaling.
    /// </summary>
    /// <param name="p">Point</param>
    /// <returns>Point</returns>
    private Emgu.CV.Structure.MCvPoint3D32f ConvertModelPoint(System.Drawing.PointF p) {
      return new Emgu.CV.Structure.MCvPoint3D32f(p.X * _scalings.Width, p.Y * _scalings.Height, 0);
    }

    /// <summary>
    /// Recalculate model features and update tracker
    /// </summary>
    protected override void PatternUpdated() {
      TexturedRectanglePattern pat = this.Pattern as TexturedRectanglePattern;
      if (pat == null) {
        return;
      }

      _feature_provider = new Emgu.CV.Features2D.SURFDetector(
        this.HessianThreshold,
        this.DescriptorSize == EDescriptorSize.Basic64 ? false : true);

      Emgu.CV.Image<Emgu.CV.Structure.Gray, byte> g = pat.Image.Convert<Emgu.CV.Structure.Gray, byte>();
      Emgu.CV.Features2D.ImageFeature[] model_features = _feature_provider.DetectFeatures(g, null);
      _tracker = new Emgu.CV.Features2D.Features2DTracker(model_features);

      _scalings = new System.Drawing.SizeF(pat.Size.Width / pat.Image.Width, pat.Size.Height / pat.Image.Height);
    }
  }

}
