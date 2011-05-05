﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Xml;
using Calib3D.IO.Extensions;

namespace Calib3D.IO {

  /// <summary>
  /// Serialize calibration result to multiple files allowing other OpenCV programs to
  /// use it. 
  /// </summary>
  /// <remarks>XML and YAML is supported.</remarks>
  /// <remarks>Only matrix compatible types are serialized. Hence, the reprojection error
  /// information is lost.</remarks>
  public class FileStorageSerializer : ICalibrationResultExporter, ICalibrationResultImporter {

    /// <summary>
    /// Create a new instance of the OpenCVFileSerializer class.
    /// </summary>
    public FileStorageSerializer() {
      FileName = "calibration_result.xml";
    }

    /// <summary>
    /// Get/Set filename associated with the intrinsic camera matrix.
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// Export calibration result
    /// </summary>
    /// <param name="cr">Calibration result</param>
    public void Export(CalibrationResult cr) {
      IntPtr fs = IntPtr.Zero;
      try {
        fs = Emgu.CV.CvInvoke.cvOpenFileStorage(FileName, IntPtr.Zero, Emgu.CV.CvEnum.STORAGE_OP.WRITE);

        // Intrinsics
        ExtendedInterop.cvWrite(fs, "intrinsic", cr.Intrinsics.IntrinsicMatrix.Ptr, new ExtendedInterop.CvAttrList());
        ExtendedInterop.cvWrite(fs, "distortion", cr.Intrinsics.DistortionCoeffs.Ptr, new ExtendedInterop.CvAttrList());
        ExtendedInterop.cvWriteReal(fs, "error", cr.ReprojectionError);

        // Extrinsics
        for (int i = 0; i < cr.Extrinsics.Length; ++i) {
          string node_name = string.Format("extrinsic_{0}", i);
          ExtendedInterop.cvWrite(fs, node_name, cr.Extrinsics[i].ExtrinsicMatrix.Ptr, new ExtendedInterop.CvAttrList());
        }
      } finally {
        Emgu.CV.CvInvoke.cvReleaseFileStorage(ref fs);
      }
    }

    /// <summary>
    /// Import calibration result
    /// </summary>
    /// <returns>Calibration result</returns>
    public CalibrationResult Import() {
      IntPtr fs = IntPtr.Zero;
      CalibrationResult cr = null;
      try {
        fs = Emgu.CV.CvInvoke.cvOpenFileStorage(FileName, IntPtr.Zero, Emgu.CV.CvEnum.STORAGE_OP.READ);
        cr = new CalibrationResult();

        cr.Intrinsics = new Emgu.CV.IntrinsicCameraParameters();
        cr.Intrinsics.IntrinsicMatrix = ReadMatrix(fs, "intrinsic");
        cr.Intrinsics.DistortionCoeffs = ReadMatrix(fs, "distortion");
        cr.ReprojectionError = (float)ExtendedInterop.cvReadRealByName(fs, IntPtr.Zero, "error");

        int i = 0;
        List<Emgu.CV.ExtrinsicCameraParameters> ecps = new List<Emgu.CV.ExtrinsicCameraParameters>();
        Emgu.CV.Matrix<double> ext = null;
        while ((ext = ReadMatrix(fs, string.Format("extrinsic_{0}", i))) != null) {
          Emgu.CV.Matrix<double> rot = ext.GetSubRect(new System.Drawing.Rectangle(0,0,3,3));
          Emgu.CV.Matrix<double> t = ext.GetCol(3);

          Emgu.CV.RotationVector3D rot_v = new Emgu.CV.RotationVector3D();
          Emgu.CV.CvInvoke.cvRodrigues2(rot.Ptr, rot_v.Ptr, IntPtr.Zero); 
          Emgu.CV.ExtrinsicCameraParameters ecp = new Emgu.CV.ExtrinsicCameraParameters(rot_v, t);
          ecps.Add(ecp);
          i += 1;
        }
        cr.Extrinsics = ecps.ToArray();

      } finally {
        Emgu.CV.CvInvoke.cvReleaseFileStorage(ref fs);
      }
      return cr;
    }


    /// <summary>
    /// Helps in deserializing matrix from xml file.
    /// </summary>
    /// <param name="path">Path to file</param>
    /// <param name="rows">Number of rows</param>
    /// <param name="cols">Number of cols</param>
    /// <returns>Matrix</returns>
    private Emgu.CV.Matrix<double> ReadMatrix(IntPtr fs, string name) {
      IntPtr mat = Emgu.CV.CvInvoke.cvReadByName(fs, IntPtr.Zero, name);
      if (mat != IntPtr.Zero) {
        System.Drawing.Size s = Emgu.CV.CvInvoke.cvGetSize(mat);
        Emgu.CV.Matrix<double> m = new Emgu.CV.Matrix<double>(s.Height, s.Width);
        Emgu.CV.CvInvoke.cvCopy(mat, m, IntPtr.Zero);
        Emgu.CV.CvInvoke.cvReleaseMat(ref mat);
        return m;
      } else {
        return null;
      }
    }

    /// <summary>
    /// Provides extended OpenCV interop currently missing by Emgu.
    /// </summary>
    private static class ExtendedInterop {
      const string CxCoreDll = "cxcore110.dll";

      [StructLayout(LayoutKind.Sequential)]
      public struct CvAttrList {
        public IntPtr attr;
        public IntPtr next;
      }

      [StructLayout(LayoutKind.Explicit)]
      public struct CvFileNode {
        [FieldOffset(0)]
        int tag;
        [FieldOffset(4)]
        IntPtr info;
        [FieldOffset(8)]
        public double f;
        [FieldOffset(8)]
        public int i;
      }

      [DllImport(CxCoreDll, EntryPoint = "cvWrite", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
      public static extern void cvWrite(IntPtr fs, [MarshalAs(UnmanagedType.LPStr)] string name, IntPtr ptr, CvAttrList attributes);

      [DllImport(CxCoreDll, EntryPoint = "cvWriteReal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
      public static extern void cvWriteReal(IntPtr fs, [MarshalAs(UnmanagedType.LPStr)] string name, double value);

      [DllImport(CxCoreDll, EntryPoint = "cvGetFileNodeByName", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
      public static extern IntPtr cvGetFileNodeByName(IntPtr fs, IntPtr map, [MarshalAs(UnmanagedType.LPStr)] string name);

      public static double cvReadRealByName(IntPtr fs, IntPtr map, string name) {
        IntPtr n = cvGetFileNodeByName(fs, map, name);
        CvFileNode fn = (CvFileNode)Marshal.PtrToStructure(n, typeof(CvFileNode));
        return fn.f;
      }
    };

   
  }
}
