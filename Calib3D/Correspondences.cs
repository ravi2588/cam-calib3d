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
using System.Collections.ObjectModel;

namespace Calib3D {

  /// <summary>
  /// Dynamic container to hold correspondences between image and model for a single view.
  /// </summary>
  public class ViewCorrespondences {
    private List<System.Drawing.PointF> _image_points;
    private List<Emgu.CV.Structure.MCvPoint3D32f> _model_points;

    public ViewCorrespondences() {
      _image_points = new List<System.Drawing.PointF>();
      _model_points = new List<Emgu.CV.Structure.MCvPoint3D32f>();
    }

    public ViewCorrespondences(
      IEnumerable<System.Drawing.PointF> image_points,
      IEnumerable<Emgu.CV.Structure.MCvPoint3D32f> model_points) 
    {
      _image_points = new List<System.Drawing.PointF>(image_points);
      _model_points = new List<Emgu.CV.Structure.MCvPoint3D32f>(model_points);
    }

    public int Count {
      get { return _image_points.Count; }
    }

    public List<System.Drawing.PointF> ImagePoints
    {
      get { return _image_points; }
    }

    public List<Emgu.CV.Structure.MCvPoint3D32f> ModelPoints {
      get { return _model_points; }
    }

    public Correspondences ToCorrespondences() {
      Correspondences c = new Correspondences();
      c.AddView(this);
      return c;
    }
  };

  /// <summary>
  /// Container to track correspondences between image and model over multiple views.
  /// </summary>
  public class Correspondences {
    private List<ViewCorrespondences> _view_corrs;
    
    /// <summary>
    /// Construct empty correspondence set.
    /// </summary>
    public Correspondences() {
      _view_corrs = new List<ViewCorrespondences>();
    }

    /// <summary>
    /// Add model/image correspondences of a single view.
    /// </summary>
    /// <param name="vc">Correspondences from view</param>
    public void AddView(ViewCorrespondences vc) 
    {
      if (vc.ImagePoints.Count != vc.ModelPoints.Count)
        throw new ArgumentException("Number of image points must match number of model points");

      _view_corrs.Add(vc);
    }

    /// <summary>
    /// Reset to empty state.
    /// </summary>
    public void Clear() {
      _view_corrs.Clear();
    }

    /// <summary>
    /// Access the total number of correspondences.
    /// </summary>
    public int CorrespondenceCount {
      get { return _view_corrs.Sum(c => c.ImagePoints.Count); }
    }

    /// <summary>
    /// Access the total number of views.
    /// </summary>
    public int ViewCount {
      get { return _view_corrs.Count; }
    }

    /// <summary>
    /// Get corresponding image points as array of array.
    /// </summary>
    public System.Drawing.PointF[][] ImagePoints {
      get { return _view_corrs.ConvertAll<System.Drawing.PointF[]>(v => v.ImagePoints.ToArray()).ToArray(); }
    }

    /// <summary>
    /// Get corresponding model points as array of array.
    /// </summary>
    public Emgu.CV.Structure.MCvPoint3D32f[][] ModelPoints {
      get { return _view_corrs.ConvertAll<Emgu.CV.Structure.MCvPoint3D32f[]>(v => v.ModelPoints.ToArray()).ToArray(); }
    }

  }
}
