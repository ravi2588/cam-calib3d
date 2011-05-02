using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calib3D.Renderer {

  /// <summary>
  /// Visually illustrates the detection result.
  /// </summary>
  public abstract class DetectionResultRenderer {
    private DetectionResult _dr;

    /// <summary>
    /// Construct from result.
    /// </summary>
    /// <param name="dr">Detection result to render</param>
    public DetectionResultRenderer(DetectionResult dr) {
      _dr = dr;
    }

    /// <summary>
    /// Get the associated detection result.
    /// </summary>
    public DetectionResult DetectionResult {
      get { return _dr; }
    }

    /// <summary>
    /// Render result to image.
    /// </summary>
    /// <param name="i">Image to overlay with result</param>
    public abstract void Render(Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> i);
  }
}
