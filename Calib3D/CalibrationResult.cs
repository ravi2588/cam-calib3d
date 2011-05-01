﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calib3D {

  /// <summary>
  /// Holds the calibration result.
  /// </summary>
  public class CalibrationResult {
    private Emgu.CV.IntrinsicCameraParameters _intrinsics;
    private Emgu.CV.ExtrinsicCameraParameters[] _extrinsics;
    private Correspondences _correspondences;
    private float _error;

    /// <summary>
    /// Construct empty calibration result.
    /// </summary>
    public CalibrationResult() {
      _intrinsics = null;
      _extrinsics = null;
      _error = float.MaxValue;
    }

    /// <summary>
    /// Get/Set the RMS reprojection error in pixels.
    /// </summary>
    /// <remarks>
    /// The value corresponds to the square root of the sum of squared distances 
    /// between the observed projections (image points) and the projected model points.
    /// </remarks>
    /// <see cref="Calib3D.Calibration.GetReprojectionError"/>
    public float ReprojectionError {
      get { return _error; }
      set { _error = value; }
    }

    /// <summary>
    /// Get/Set intrinsic camera parameters.
    /// </summary>
    public Emgu.CV.IntrinsicCameraParameters Intrinsics {
      get { return _intrinsics; }
      set { _intrinsics = value; }
    }

    /// <summary>
    /// Get/Set extrinsic camera paramters for each view.
    /// </summary>
    public Emgu.CV.ExtrinsicCameraParameters[] Extrinsics {
      get { return _extrinsics; }
      set { _extrinsics = value; }
    }

    /// <summary>
    /// Get/Set the correspondences associated with this result.
    /// </summary>
    public Correspondences Correspondences {
      get { return _correspondences; }
      set { _correspondences = value; }
    }
  }
}