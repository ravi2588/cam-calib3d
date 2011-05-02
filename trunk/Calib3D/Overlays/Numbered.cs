﻿/*
 * Calib3D http://code.google.com/p/cam-calib3d/
 * Copyright (c) 2011, Christoph Heindl. All rights reserved.
 * Code license:	New BSD License
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calib3D.Overlays {

  /// <summary>
  /// Simple numbered circle overlay.
  /// </summary>
  public class Numbered : DetectionResult.IOverlayProvider {

    public void Overlay(Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> i, DetectionResult dr) {
      if (dr.ImagePoints.Count == 0) {
        return;
      }

      System.Drawing.Color color = dr.Success ? System.Drawing.Color.Green : System.Drawing.Color.Red;
      Emgu.CV.Structure.Bgr bgr = new Emgu.CV.Structure.Bgr(color);
      Emgu.CV.Structure.MCvFont f = new Emgu.CV.Structure.MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_PLAIN, 0.8, 0.8);

      int count = 1;
      foreach (System.Drawing.PointF point in dr.ImagePoints) {
        i.Draw(new Emgu.CV.Structure.CircleF(point, 4), bgr, 2);

        System.Drawing.Point ip = new System.Drawing.Point(
                                      (int)(Math.Round(point.X)),
                                      (int)(Math.Round(point.Y)));
        i.Draw(count.ToString(), ref f, new System.Drawing.Point(ip.X + 5, ip.Y - 5), bgr);

        count++;
      }
    }

  }
}