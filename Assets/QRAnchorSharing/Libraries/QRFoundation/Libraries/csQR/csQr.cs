using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ZXing;
using ZXing.QrCode.Internal;

namespace CSQR
{
    public delegate void ScanFinishedCallback(QRScanResult result);

    public class QRScanResult
    {
        public QRLocation position;
        public QRLocation bounds;
        public string content;
        public Sample[] samples;
        public bool mirrored;

        public QRScanResult(QRLocation position, QRLocation bounds, string content, Sample[] samples, bool mirrored)
        {
            this.position = position;
            this.bounds = bounds;
            this.content = content;
            this.samples = samples;
            this.mirrored = mirrored;
        }
    }
    /*
    public class MyBinarizer : ZXing.Common.GlobalHistogramBinarizer
    {
        public MyBinarizer(LuminanceSource luminanceSource) : base(luminanceSource)
        {
        }

        public override ZXing.Common.BitMatrix BlackMatrix 

        public override ZXing.Binarizer createBinarizer(LuminanceSource source)
        {
            throw new NotImplementedException();
        }

        public override ZXing.Common.BitArray getBlackRow(int y, ZXing.Common.BitArray row)
        {
            throw new NotImplementedException();
        }
    }
    */
    public class CsQr
    {

        public static QRScanResult ScanZXing(UnityEngine.Color32[] colors, int width, int height, bool tryInverted = false)
        {
            Result scanResult = null;
            StopWatch watch = new StopWatch("round trip");
            watch.Start();

            ZXing.Binarizer binarizer = null;

            ZXing.Common.BitMatrix qrMatrix = null;
            Func<Point, Point> mappingFunction = null;
            QRLocation location = null;
            Sample[] samples = null;
            bool mirrored = false;

            try
            {
                var reader = new BarcodeReader(new MultiFormatReader(), (c, w, h) =>
                {
                    return new Color32LuminanceSource(c, w, h);
                }, (luminanceSource) =>
                {
                    binarizer = new ZXing.Common.GlobalHistogramBinarizer(luminanceSource);
                    return binarizer;
                });
                reader.AutoRotate = false;
                reader.TryInverted = tryInverted;
                reader.Options.TryHarder = false;
                reader.Options.PossibleFormats = new BarcodeFormat[] { BarcodeFormat.QR_CODE };

                reader.Options.PossibleFormats = new List<BarcodeFormat>(new BarcodeFormat[] { BarcodeFormat.QR_CODE });
                scanResult = reader.Decode(colors, width, height);
                watch.Round("scanning");
                var bitMatrix = binarizer.BlackMatrix;
                var ul = new Point(scanResult.ResultPoints[1].X, scanResult.ResultPoints[1].Y);
                var ur = new Point(scanResult.ResultPoints[0].X, scanResult.ResultPoints[0].Y);
                var ll = new Point(scanResult.ResultPoints[2].X, scanResult.ResultPoints[2].Y);

                var ulur = new Point(ur.x - ul.x, ur.y - ul.y);
                var ulll = new Point(ll.x - ul.x, ll.y - ul.y);
                var cross = ulur.x * ulll.y - ulur.y * ulll.x;
                if (cross > 0)
                {
                    var temp = ur;
                    ur = ll;
                    ll = temp;
                    mirrored = true;
                }
                (int dimension, double moduleSize) = Locator.ComputeDimension(ul, ur, ll, bitMatrix);

                Point ap = new Point();
                if (scanResult.ResultPoints.Length > 3)
                {
                    // There is an alignment pattern
                    ap = new Point(scanResult.ResultPoints[3].X, scanResult.ResultPoints[3].Y);
                } else
                {
                    // There is no alignment pattern. Estimate the position
                    ap = Locator.EstimateAlignmentPattern(ul, ur, ll, moduleSize);
                }
                Extractor extractor = new Extractor();
                location = new QRLocation(ul, ur, ap, ll, dimension);
                extractor.Extract(bitMatrix, location, out qrMatrix, out mappingFunction);

                Sampler sampler = new Sampler(bitMatrix, qrMatrix, mappingFunction);
                samples = sampler.GetSamples(4 * 6);
            }
            catch (Exception)
            {
                //UnityEngine.Debug.LogError(e);
                return null;
            }
            //watch.PrintResults();

            return new QRScanResult(location,
                new QRLocation(
                mappingFunction(new Point(0, 0)),
                mappingFunction(new Point(0, location.dimension)),
                mappingFunction(new Point(location.dimension, location.dimension)),
                mappingFunction(new Point(location.dimension, 0)), 0),
                scanResult.Text, samples, mirrored);
        }

        public static void ScanAsyncZXing(UnityEngine.Color32[] colors, int width, int height, bool tryInverted, ScanFinishedCallback callback)
        {
            Task.Run(() =>
            {
                Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
                QRScanResult result;
                result = ScanZXing(colors, width, height, tryInverted);
                callback(result);
            });
        }
    }
}