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
  /// Collection of model/image correspondences for multiple views.
  /// </summary>
  public class MultiViewCorrespondences {
    private List<SingleViewCorrespondences> _view_corrs;
    
    /// <summary>
    /// Construct empty correspondence set.
    /// </summary>
    public MultiViewCorrespondences() {
      _view_corrs = new List<SingleViewCorrespondences>();
    }

    /// <summary>
    /// Add model/image correspondences of a single view.
    /// </summary>
    /// <param name="vc">Correspondences from view</param>
    public void AddView(SingleViewCorrespondences vc) 
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
