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

namespace Calib3D.Renderer {

  /// <summary>
  /// Renders intrinsic/extrinsic calibration results.
  /// </summary>
  public abstract class CalibrationResultRenderer {
    private CalibrationResult _cr;

    /// <summary>
    /// Construct from result.
    /// </summary>
    /// <param name="dr">Calibration result to render</param>
    public CalibrationResultRenderer(CalibrationResult cr) {
      _cr = cr;
    }

    /// <summary>
    /// Get the associated calibration result.
    /// </summary>
    public CalibrationResult CalibrationResult {
      get { return _cr; }
    }

    /// <summary>
    /// Render result to image.
    /// </summary>
    /// <param name="i">Image to overlay with result</param>
    /// <param name="view_id">View-id to use</param>
    public abstract void Render(Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> i, int view_id);
  }
}
