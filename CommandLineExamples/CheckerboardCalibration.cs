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
using Microsoft.Test.CommandLineParsing;
using System.ComponentModel;
using System.IO;
using Calib3D.IO.Extensions;
using Calib3D.Util;

namespace CommandLineExamples {

  /// <summary>
  /// Simple Chessboard calibration
  /// </summary>
  /// <example>
  /// CommandLineExamples.exe run CheckerboardCalibration /ImageDirectory=../etc/images/checkerboard ^ 
  ///                             /PatternSize=9,6 /PatternSquareLength=25 /Verbose
  /// </example>
  public class CheckerboardCalibration : IExample {

    /// <summary>
    /// Supported commandline arguments.
    /// </summary>
    class CommandLineArguments {

      [Description("Print this help")]
      public bool? Help { get; set; }

      [Description("Directory containing the calibration images")]
      public string ImageDirectory { get; set; }

      [Description("File to store calibration result to")]
      public string CalibrationResultOutput { get; set; }

      [Description("Size of a single square in units of your choice")]
      public float? PatternSquareLength { get; set; }

      [Description("Provide verbose output information")]
      public bool? Verbose { get; set; }

      [Description("Number of inner corners per row and column")]
      public System.Drawing.Size? PatternSize { get; set; }
    }

    /// <summary>
    /// Example description
    /// </summary>
    public string Description {
      get { return "Performs camera calibration based on checkerboard patterns."; }
    }

    public void Run(string[] args) {
      CommandLineArguments a = new CommandLineArguments();
      CommandLineParser.ParseArguments(a, args);

      if (a.Help.GetValueOrDefault(false)) {
        CommandLineParser.PrintUsage(a);
        return;
      }

      string image_dir = Default.Get(a.ImageDirectory, "./");
      string output_path = Default.Get(a.CalibrationResultOutput, "result.cr");
      float size = Default.Get(a.PatternSquareLength, 25);
      System.Drawing.Size corners = Default.Get(a.PatternSize, new System.Drawing.Size(9, 6));
      bool verbose = Default.Get(a.Verbose, false);
      
      // Prepare pattern and detector
      Calib3D.CheckerBoard.CheckerBoardPattern pattern = new Calib3D.CheckerBoard.CheckerBoardPattern();
      pattern.CornerCount = corners;
      pattern.SquareLength = size;

      Calib3D.CheckerBoard.CheckerBoardDetector detect = new Calib3D.CheckerBoard.CheckerBoardDetector();
      detect.Pattern = pattern;

      // Load all images and collect model/image correspondences.
      System.Drawing.Size image_size = System.Drawing.Size.Empty;
      Calib3D.Correspondences c = new Calib3D.Correspondences();

      List<Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte>> images = new List<Emgu.CV.Image<Emgu.CV.Structure.Bgr,byte>>();
      foreach (string path in Calib3D.IO.Directory.GetFiles(image_dir, "*.png;*.jpg")) {
        System.Console.Write(String.Format("  Processing image {0} ", new FileInfo(path).Name));
        Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> i = Calib3D.IO.Images.FromPath(path).First();
        images.Add(i);
        image_size = i.Size;
        
        Calib3D.DetectionResult dr = detect.FindPattern(i);
        c.AddView(dr);

        System.Console.WriteLine(dr.Success ? "OK" : "FAILED");

        if (verbose) {
          dr.ResultRenderer.Render(i);
        }
      }

      // Perform intrinsic calibration 
      Calib3D.CalibrationResult cr = Calib3D.Calibration.GetIntrinsics(c, image_size);

      // Pretty print results to console
      System.Console.WriteLine();
      System.Console.WriteLine(String.Format("Reprojection Error {0} pixels.", cr.ReprojectionError));
      System.Console.WriteLine(cr.Intrinsics.PrettyPrint());

      // Export as binary archieve
      Calib3D.IO.Exporter export = new Calib3D.IO.Exporter();
      export.ToFile(output_path, new Calib3D.IO.BinaryCalibrationResultExportFormatter(), cr);

      if (verbose) {
        for (int i = 0; i < c.ViewCount; ++i) {
          System.Console.WriteLine();
          System.Console.WriteLine(cr.Extrinsics[i].PrettyPrint());
          cr.ResultRenderer.Render(images[i], i);
          Calib3D.IO.Images.Show(images[i], 0);
        }
      }

    }
  }
}
