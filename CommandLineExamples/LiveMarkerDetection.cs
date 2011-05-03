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
using Microsoft.Test.CommandLineParsing;
using Calib3D.Util;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Threading;

namespace CommandLineExamples {

  /// <summary>
  /// Marker detection in live stream from camera.
  /// </summary>
  /// <example>
  /// CommandLineExamples.exe run LiveMarkerDetection /MarkerImage=../etc/patterns/marker/marker_a.png
  /// </example>
  public class LiveMarkerDetection : IExample {

    /// <summary>
    /// Supported commandline arguments.
    /// </summary>
    class CommandLineArguments {

      [Description("Print this help")]
      public bool? Help { get; set; }

      [Description("Path to marker image")]
      public string MarkerImage { get; set; }

      [Description("Size of marker")]
      public float? MarkerLength { get; set; }

      [Description("Device ID of camera")]
      public int? HardwareId { get; set; }
    }

    public string Description {
      get { return "Marker detection in live stream from camera."; }
    }

    public void Run(string[] args) {
      CommandLineArguments a = new CommandLineArguments();
      CommandLineParser.ParseArguments(a, args);

      if (a.Help.GetValueOrDefault(false)) {
        CommandLineParser.PrintUsage(a);
        return;
      }

      string image_path = Default.Get(a.MarkerImage, "marker.png");
      int device_id = Default.Get(a.HardwareId, 0);
      float marker_length = Default.Get(a.MarkerLength, 50);

      // Setup pattern and detector
      Calib3D.Marker.MarkerPattern pat = new Calib3D.Marker.MarkerPattern();
      pat.MarkerImage = Calib3D.IO.Images.FromPath(image_path).First();
      pat.MarkerLength = marker_length;

      Calib3D.Marker.MarkerDetector det = new Calib3D.Marker.MarkerDetector();
      det.BinaryThreshold = 40;
      det.MaximumErrorNormed = 0.4f;
      det.Pattern = pat;

      Calib3D.IO.Images.Show(pat.MarkerImage, 1, "Marker");

      // Start capturing from camera.
      Capture capture = new Capture(device_id);

      while (!Console.KeyAvailable) {
        Emgu.CV.Image<Bgr, byte> i = capture.QueryFrame();
        Calib3D.DetectionResult dr = det.FindPattern(i);
        dr.ResultRenderer.Render(i);
        Calib3D.IO.Images.Show(i, 30, "LiveFeed");
      }

    }
  }
}
