/*
 * Calib3D http://code.google.com/p/cam-calib3d/
 * Copyright (c) 2011, Christoph Heindl. All rights reserved.
 * Code license:	New BSD License
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calib3D {

  /// <summary>
  /// Base class for pattern detection results.
  /// </summary>
  public class DetectionResult {
    private bool _success;
    private PatternDetector _detector;
    private Pattern _pattern;
    private List<System.Drawing.PointF> _image_points;

    /// <summary>
    /// Create detection result initialized to success.
    /// </summary>
    public DetectionResult(PatternDetector pd, Pattern p) {
      _detector = pd;
      _pattern = p;
      _success = true;
      _image_points = new List<System.Drawing.PointF>();
    }

    /// <summary>
    /// Create detection result initialized to success.
    /// </summary>
    public DetectionResult(PatternDetector pd, Pattern p, bool success) {
      _detector = pd;
      _pattern = p;
      _success = success;
      _image_points = new List<System.Drawing.PointF>();
    }

    /// <summary>
    /// Construct from paramters.
    /// </summary>
    public DetectionResult(PatternDetector pd, Pattern p, IEnumerable<System.Drawing.PointF> points) {
      _detector = pd;
      _pattern = p;
      _success = true;
      _image_points = new List<System.Drawing.PointF>(points);
    }

    /// <summary>
    /// Construct from paramters.
    /// </summary>
    public DetectionResult(PatternDetector pd, Pattern p, bool success, IEnumerable<System.Drawing.PointF> points) {
      _detector = pd;
      _pattern = p;
      _success = success;
      _image_points = new List<System.Drawing.PointF>(points);
    }

    /// <summary>
    /// Get the pattern detector that produced this result.
    /// </summary>
    public PatternDetector PatternDetector {
      get { return _detector; }
    }

    /// <summary>
    /// Get the pattern.
    /// </summary>
    public Pattern Pattern {
      get { return _pattern; }
    }

    /// <summary>
    /// Get/set a boolean value indication whether the operation 
    /// completed successfully or not.
    /// </summary>
    public bool Success {
      get { return _success; }
      set { _success = value; }
    }

    /// <summary>
    /// Get the list of corresponding points detected in the image.
    /// </summary>
    public IList<System.Drawing.PointF> ImagePoints {
      get { return _image_points; }
    }

    /// <summary>
    /// Provide visual DetectionResult feedback.
    /// </summary>
    public interface IOverlayProvider {
      /// <summary>
      /// Visually overlay an image with the detection result.
      /// </summary>
      /// <param name="i">Image to overlay</param>
      /// <param name="dr">Detection result</param>
      void Overlay(Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> i, DetectionResult dr);
    }
  }
}
