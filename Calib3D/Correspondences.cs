/*
 * Calib3D http://code.google.com/p/cam-calib3d/
 * Copyright (c) 2011, Christoph Heindl. All rights reserved.
 * Code license:	New BSD License
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Calib3D {

  /// <summary>
  /// Container to track correspondences between image and model.
  /// </summary>
  public class Correspondences {
    private List<System.Drawing.PointF[]> _image_points;
    private List<Emgu.CV.Structure.MCvPoint3D32f[]> _model_points;

    /// <summary>
    /// Construct empty correspondence set.
    /// </summary>
    public Correspondences() {
      _image_points = new List<System.Drawing.PointF[]>();
      _model_points = new List<Emgu.CV.Structure.MCvPoint3D32f[]>();
    }

    /// <summary>
    /// Add model/image correspondences. 
    /// </summary>
    /// <param name="image_points">Image points</param>
    /// <param name="model_points">Model points</param>
    public void AddView(
      IEnumerable<System.Drawing.PointF> image_points,
      IEnumerable<Emgu.CV.Structure.MCvPoint3D32f> model_points) 
    {
      if (image_points.Count() != model_points.Count())
        throw new ArgumentException("Number of image points must match number of model points");

      _image_points.Add(image_points.ToArray());
      _model_points.Add(model_points.ToArray());
    }

    /// <summary>
    /// Add model/image correspondences from result.
    /// </summary>
    /// <param name="dr"></param>
    public void AddView(DetectionResult dr) {
      if (dr.Success) {
        if (dr.ImagePoints.Count != dr.Pattern.ModelPoints.Count)
          throw new ArgumentException("Number of image points must match number of model points");
        _image_points.Add(dr.ImagePoints.ToArray());
        _model_points.Add(dr.Pattern.ModelPoints.ToArray());
      }
    }

    /// <summary>
    /// Reset to empty state.
    /// </summary>
    public void Clear() {
      _image_points.Clear();
      _model_points.Clear();
    }

    /// <summary>
    /// Access the total number of correspondences.
    /// </summary>
    public int CorrespondenceCount {
      get { return _image_points.Sum(c => c.Length); }
    }

    /// <summary>
    /// Access the total number of views.
    /// </summary>
    public int ViewCount {
      get { return _image_points.Count; }
    }

    /// <summary>
    /// Get image points.
    /// </summary>
    public System.Drawing.PointF[][] ImagePoints {
      get { return _image_points.ToArray(); }
    }

    /// <summary>
    /// Get corresponding model points.
    /// </summary>
    public Emgu.CV.Structure.MCvPoint3D32f[][] ModelPoints {
      get { return _model_points.ToArray(); }
    }

  }
}
