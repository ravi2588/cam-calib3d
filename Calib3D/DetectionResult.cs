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
  /// Base class for pattern detection results.
  /// </summary>
  public class DetectionResult {
    private bool _success;
    private PatternDetector _detector;
    private Pattern _pattern;
    private List<System.Drawing.PointF> _image_points;
    private Renderer.DetectionResultRenderer _renderer;

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
    /// Get/Set the default renderer associated with this result.
    /// </summary>
    public Renderer.DetectionResultRenderer ResultRenderer {
      get {
        if (_renderer == null) {
          _renderer = new Renderer.NumberDetectionResultRenderer(this);
        }
        return _renderer;
      }
      set {
        _renderer = value;
      }
    }

    /// <summary>
    /// Get the list of corresponding points detected in the image.
    /// </summary>
    public IList<System.Drawing.PointF> ImagePoints {
      get { return _image_points; }
    }

    /// <summary>
    /// Get correspondences for this detection result.
    /// </summary>
    /// <returns>Image/model correspondences.</returns>
    public Correspondences GetCorrespondences() {
      Correspondences c = new Correspondences();
      c.AddView(this);
      return c;
    }
  }
}
