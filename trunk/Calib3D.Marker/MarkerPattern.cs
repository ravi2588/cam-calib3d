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

namespace Calib3D.Marker {

  /// <summary>
  /// Pattern based on custom marker image.
  /// </summary>
  /// <remarks>
  /// The pattern to be detected is a user-defined square binary image that has a thick black border and 
  /// a non-symmetric black/white interior. The marker is provided as an ordinary image. Additionally, 
  /// the length of pattern in the real world in units of your choice (e.g millimeter) needs to be configured.
  /// The calibration yields four correspondences (corner pixels of the marker) when successful.
  /// 
  /// The algorithm works by first binarizing the image into black and white. On this image the contours are extracted.
  /// For each extracted contour an poly-line through the contour is approximated. If the poly-line has exactly four vertices,
  /// the algorithm treats the poly-line as a possible marker border. For each marker hypothesis, the pixels contained in 
  /// inside the  poly-line are back-projected to a square image of the same size (in pixels) as the marker image. This image 
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
  public class MarkerPattern : Calib3D.Pattern {
    private float _marker_length;
    private Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> _marker_image;

    /// <summary>
    /// Default construct pattern.
    /// </summary>
    public MarkerPattern() {
      _marker_length = 10;
      _marker_image = null;
    }

    /// <summary>
    /// Get/Set the length of the marker
    /// </summary>
    [Description("Length of marker")]
    public float MarkerLength {
      get { return _marker_length; }
      set { 
        _marker_length = value;
        this.UpdateModelPoints();
      }
    }

    /// <summary>
    /// Access the marker image used as pattern
    /// </summary>
    [Description("Get/Set the marker image used as pattern")]
    public Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> MarkerImage {
      get { return _marker_image; }
      set { _marker_image = value; }
    }

    /// <summary>
    /// Update model points.
    /// </summary>
    private void UpdateModelPoints() {
      this.ModelPoints.Clear();

      // 0   1
      // +---+
      // |   |
      // +---+
      // 2   3
      this.ModelPoints.Add(new Emgu.CV.Structure.MCvPoint3D32f(0, 0, 0));
      this.ModelPoints.Add(new Emgu.CV.Structure.MCvPoint3D32f(_marker_length, 0, 0));
      this.ModelPoints.Add(new Emgu.CV.Structure.MCvPoint3D32f(_marker_length, _marker_length, 0));
      this.ModelPoints.Add(new Emgu.CV.Structure.MCvPoint3D32f(0, _marker_length, 0));
    }
  }
}
