using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Calib3D.Feature {

  /// <summary>
  /// Pattern that describes a rectangular image region
  /// </summary>
  public class TexturedRectanglePattern : Calib3D.Pattern {
    private Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> _image;
    private System.Drawing.SizeF _rect_size;

    /// <summary>
    /// Create a new instance of the TexturedRectanglePattern class.
    /// </summary>
    public TexturedRectanglePattern() {
      _rect_size = new System.Drawing.SizeF(1, 1);
      _image = null;
    }

    /// <summary>
    /// Get/Set the image used as pattern
    /// </summary>
    [Description("Get/Set the marker image used as pattern")]
    public Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> Image {
      get { return _image; }
      set {
        _image = value;
        this.TriggerPropertyChanged("Image");
      }
    }

    /// <summary>
    /// Access the the size of the image in calibration units.
    /// </summary>
    [Description("Get/Set the marker image used as pattern")]
    public System.Drawing.SizeF Size {
      get { return _rect_size; }
      set {
        _rect_size = value;
        this.TriggerPropertyChanged("Size");
      }
    }
  
  }
}
