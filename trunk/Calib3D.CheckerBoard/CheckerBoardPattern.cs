/*
 * Calib3D http://code.google.com/p/cam-calib3d/
 * Copyright (c) 2011, Christoph Heindl. All rights reserved.
 * Code license:	New BSD License
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Composition;

namespace Calib3D.CheckerBoard {

  /// <summary>
  /// Represents a checkerboard used for calibration.
  /// </summary>
  /// <remarks>
  /// A checkerboard as implemented by this class is a rectangular pattern of 
  /// square fields which alternate in color (black/white). It's best to use
  /// a non-square pattern to avoid symmetries in detection. 
  /// 
  /// CheckerBoard is parametrized by the number of inner corner points per
  /// checkerboard row and column. Additionally a field size is specified that
  /// is used to generate object reference points in 3d.
  /// </remarks>
  /// <remarks>
  /// Based on code from the Parsley project.
  /// http://parsley.googlecode.com
  /// </remarks>
  public class CheckerBoardPattern : Calib3D.Pattern {
    private int _inner_corners_x;
    private int _inner_corners_y;
    private float _field_size;

    /// <summary>
    /// Default construct.
    /// </summary>
    public CheckerBoardPattern()
      : this(9, 6, 15.0f) { }

    /// <summary>
    /// Construct from parameters.
    /// </summary>
    /// <param name="inner_corners_x">Number of inner corners in each row</param>
    /// <param name="inner_corners_y">Number of inner corners in each column</param>
    /// <param name="field_size">Size of square</param>
    public CheckerBoardPattern(int inner_corners_x, int inner_corners_y, float field_size) {
      _inner_corners_x = inner_corners_x;
      _inner_corners_y = inner_corners_y;
      _field_size = field_size;
      this.UpdateModelPoints();
    }

    /// <summary>
    /// Get/set the number of inner corners per row and column
    /// </summary>
    [Description("Number of inner corners per row and column")]
    [DefaultValue(typeof(System.Drawing.Size), "9, 6")]
    public System.Drawing.Size CornerCount {
      get { return new System.Drawing.Size(_inner_corners_x, _inner_corners_y); }
      set {
        _inner_corners_x = value.Width;
        _inner_corners_y = value.Height;
        this.UpdateModelPoints();
      }
    }

    /// <summary>
    /// Get/set the size of single square in the checkerboard pattern
    /// </summary>
    [Description("The size of single square in the checkerboard pattern in units of your choice.")]
    [DefaultValue(25.0f)]
    public float SquareLength {
      get { return _field_size; }
      set {
        _field_size = value;
        this.UpdateModelPoints();
      }
    }

    /// <summary>
    /// Update model points.
    /// </summary>
    private void UpdateModelPoints() {
      this.ModelPoints.Clear();

      for (int y = 0; y < _inner_corners_y; ++y) {
        for (int x = 0; x < _inner_corners_x; x++) {
          this.ModelPoints.Add(new Emgu.CV.Structure.MCvPoint3D32f(x * _field_size, y * _field_size, 0));
        }
      }

    }
  }
}
