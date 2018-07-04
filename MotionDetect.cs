using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OpenCVForUnitySample;
using OpenCVForUnity;


/// <summary>
/// Optical flow sample.
/// http://stackoverflow.com/questions/6505779/android-optical-flow-with-opencv?rq=1
/// </summary>

   
public class MotionDetect : MonoBehaviour
{

    /// <summary>
    /// The colors.
    /// </summary>
    Color32[] colors;

    /// <summary>
    /// The mat op flow this.
    /// </summary>
    Mat matOpFlowThis;

    /// <summary>
    /// The mat op flow previous.
    /// </summary>
    Mat matOpFlowPrev;

    /// <summary>
    /// The i GFFT max.
    /// </summary>
    int iGFFTMax = 40;

    /// <summary>
    /// The MO pcorners.
    /// </summary>
    MatOfPoint MOPcorners;

    /// <summary>
    /// The m MO p2fpts this.
    /// </summary>
    MatOfPoint2f mMOP2fptsThis;

    /// <summary>
    /// The m MO p2fpts previous.
    /// </summary>
    MatOfPoint2f mMOP2fptsPrev;

    /// <summary>
    /// The m MO p2fpts safe.
    /// </summary>
    MatOfPoint2f mMOP2fptsSafe;

    /// <summary>
    /// The m MOB status.
    /// </summary>
    MatOfByte mMOBStatus;

    /// <summary>
    /// The m MO ferr.
    /// </summary>
    MatOfFloat mMOFerr;

    /// <summary>
    /// The color red.
    /// </summary>
    Scalar colorRed = new Scalar(255, 0, 0, 255);

    /// <summary>
    /// The i line thickness.
    /// </summary>
    int iLineThickness = 3;

    /// <summary>
    /// The texture.
    /// </summary>
    Texture2D texture;

    /// <summary>
    /// The web cam texture to mat helper.
    /// </summary>
    //CameraImageToMatSample webCamTextureToMatHelper;
    WebCamTextureToMat webCamTextureToMat;
    bool IsStarted = false;
    void Start()
    {
        //webCamTextureToMatHelper = gameObject.GetComponent<WebCamTextureToMatHelper>();
        //webCamTextureToMatHelper.Init();

        
        

        //yukKesiniAudio = yukKesini.GetComponent<AudioSource>() as AudioSource;

        //c = FindObjectOfType<CameraImageToMatSample>() as CameraImageToMatSample;
        //c = webCamTextureToMatHelper.GetMat();
        //webCamTextureToMatHelper = gameObject.GetComponent<WebCamTextureToMatHelper>();
        //webCamTextureToMatHelper.Init();
        StartCoroutine(Init());
    }
    
    
    IEnumerator Init()
    {
        yield return new WaitForSeconds(1);
        IsStarted = true;
        webCamTextureToMat = FindObjectOfType<WebCamTextureToMat>() as WebCamTextureToMat;
        Debug.Log("MotionDetectInited");


        Mat webCamTextureMat = webCamTextureToMat.GetMat();
        Debug.Log("webCamTextureMat -- c : " + webCamTextureMat.cols() + " r : " + webCamTextureMat.rows());
        //Mat webCamTextureMat = webCamTextureToMatHelper.GetMat();

        colors = new Color32[webCamTextureMat.cols() * webCamTextureMat.rows()];
        texture = new Texture2D(webCamTextureMat.cols(), webCamTextureMat.rows(), TextureFormat.RGBA32, false);

        matOpFlowThis = new Mat();
        matOpFlowPrev = new Mat();
        MOPcorners = new MatOfPoint();
        mMOP2fptsThis = new MatOfPoint2f();
        mMOP2fptsPrev = new MatOfPoint2f();
        mMOP2fptsSafe = new MatOfPoint2f();
        mMOBStatus = new MatOfByte();
        mMOFerr = new MatOfFloat();


        gameObject.transform.localScale = new Vector3(webCamTextureMat.cols(), webCamTextureMat.rows(), 1);

        Debug.Log("Screen.width " + Screen.width + " Screen.height " + Screen.height + " Screen.orientation " + Screen.orientation);

        float width = 0;
        float height = 0;

        width = gameObject.transform.localScale.x;
        height = gameObject.transform.localScale.y;

        float widthScale = (float)Screen.width / width;
        float heightScale = (float)Screen.height / height;
        if (widthScale < heightScale)
        {
            Camera.main.orthographicSize = (width * (float)Screen.height / (float)Screen.width) / 2;
        }
        else
        {
            Camera.main.orthographicSize = height / 2;
        }

        gameObject.GetComponent<Renderer>().material.mainTexture = texture;

        //          webCamTextureToMatHelper.Play ();
        yield return null;
    }
        
    // Update is called once per frame
    void Update()
    {
        if (!IsStarted)
            return;
        Mat grayMat = webCamTextureToMat.GetMat();
        Imgproc.cvtColor(grayMat, grayMat, Imgproc.COLOR_RGBA2GRAY);
        //Debug.Log("mMOP2fptsPrev.rows() : " + mMOP2fptsPrev.rows().ToString());

        //Debug.Log("rgbaMat.rows() : " + rgbaMat.rows().ToString());
        //Debug.Log("matOpFlowThis.rows() : " + matOpFlowThis.rows().ToString());

        if (mMOP2fptsPrev.rows() == 0)
        {

            // first time through the loop so we need prev and this mats
            // plus prev points
            // get this mat
            //rgbaMat.copyTo(matOpFlowThis);

            grayMat.copyTo(matOpFlowThis);
            grayMat.copyTo(matOpFlowPrev);
            //matOpFlowThis = rgbaMat;
            //matOpFlowPrev = rgbaMat;
            matOpFlowPrev.empty();
            //matOpFlowPrev = new Mat(rgbaMat.size(), rgbaMat.type());
            //Imgproc.cvtColor(rgbaMat, matOpFlowThis, Imgproc.COLOR_RGBA2GRAY);

            // copy that to prev mat
            matOpFlowThis.copyTo(matOpFlowPrev);

            // get prev corners
            Imgproc.goodFeaturesToTrack(matOpFlowPrev, MOPcorners, iGFFTMax, 0.1, 100);
            mMOP2fptsPrev.fromArray(MOPcorners.toArray());
                
            // get safe copy of this corners
            mMOP2fptsPrev.copyTo(mMOP2fptsSafe);
            //Debug.Log("opencv optical flow --- 1 ");
        }
        else
        {
            // we've been through before so
            // this mat is valid. Copy it to prev mat
            //rgbaMat.copyTo(matOpFlowThis);
            //matOpFlowPrev = new Mat(rgbaMat.size(), rgbaMat.type());
            matOpFlowThis.copyTo(matOpFlowPrev);

            //matOpFlowThis = new Mat(rgbaMat.size(), rgbaMat.type());

            // get this mat
            grayMat.copyTo(matOpFlowThis);
            //matOpFlowThis = rgbaMat;
            //Imgproc.cvtColor(rgbaMat, matOpFlowThis, Imgproc.COLOR_RGBA2GRAY);

            // get the corners for this mat
            Imgproc.goodFeaturesToTrack(matOpFlowThis, MOPcorners, iGFFTMax, 0.1, 100);
            mMOP2fptsThis.fromArray(MOPcorners.toArray());

            // retrieve the corners from the prev mat
            // (saves calculating them again)
            mMOP2fptsSafe.copyTo(mMOP2fptsPrev);

            // and save this corners for next time through

            mMOP2fptsThis.copyTo(mMOP2fptsSafe);

            //Debug.Log("opencv optical flow --- 2 ");
        }


        /*
            Parameters:
                prevImg first 8-bit input image
                nextImg second input image
                prevPts vector of 2D points for which the flow needs to be found; point coordinates must be single-precision floating-point numbers.
                nextPts output vector of 2D points (with single-precision floating-point coordinates) containing the calculated new positions of input features in the second image; when OPTFLOW_USE_INITIAL_FLOW flag is passed, the vector must have the same size as in the input.
                status output status vector (of unsigned chars); each element of the vector is set to 1 if the flow for the corresponding features has been found, otherwise, it is set to 0.
                err output vector of errors; each element of the vector is set to an error for the corresponding feature, type of the error measure can be set in flags parameter; if the flow wasn't found then the error is not defined (use the status parameter to find such cases).
            */
        Video.calcOpticalFlowPyrLK(matOpFlowPrev, matOpFlowThis, mMOP2fptsPrev, mMOP2fptsThis, mMOBStatus, mMOFerr);

        if (!mMOBStatus.empty())
        {
            List<Point> cornersPrev = mMOP2fptsPrev.toList();
            List<Point> cornersThis = mMOP2fptsThis.toList();
            List<byte> byteStatus = mMOBStatus.toList();

            int x = 0;
            int y = byteStatus.Count - 1;

            int num_distance = 0;
            for (x = 0; x < y; x++)
            {
                if (byteStatus[x] == 1)
                {
                    Point pt = cornersThis[x];
                    Point pt2 = cornersPrev[x];

                    Imgproc.circle(grayMat, pt, 5, colorRed, iLineThickness - 1);

                    Imgproc.line(grayMat, pt, pt2, colorRed, iLineThickness);
                    double distance = System.Math.Sqrt(System.Math.Pow((pt2.x - pt.x), 2.0) + System.Math.Pow((pt2.y - pt.y), 2.0));
                    if (distance > 20)
                        num_distance++;

                    //Utilities.Debug("Distance[" + x + "] : " + distance);
                    //Debug.Log("Distance[" + x + "] : " + distance);
                }
            }
            Debug.Log("Num of Distance : " + num_distance);
            if (num_distance > 0)
            {
                Debug.Log("Movement Detected !!");
                
            }
        }

        //              Imgproc.putText (rgbaMat, "W:" + rgbaMat.width () + " H:" + rgbaMat.height () + " SO:" + Screen.orientation, new Point (5, rgbaMat.rows () - 10), Core.FONT_HERSHEY_SIMPLEX, 1.0, new Scalar (255, 255, 255, 255), 2, Imgproc.LINE_AA, false);
        //this.GetComponent<CVCore>().Add(0, rgbaMat);
        Utils.matToTexture2D(grayMat, texture, colors);
        gameObject.GetComponent<Renderer>().material.mainTexture = texture;
    }
    
    
    void OnDisable()
    {
        Debug.Log("OnDisable");

        if (matOpFlowThis != null)
            matOpFlowThis.Dispose();
        if (matOpFlowPrev != null)
            matOpFlowPrev.Dispose();
        if (MOPcorners != null)
            MOPcorners.Dispose();
        if (mMOP2fptsThis != null)
            mMOP2fptsThis.Dispose();
        if (mMOP2fptsPrev != null)
            mMOP2fptsPrev.Dispose();
        if (mMOP2fptsSafe != null)
            mMOP2fptsSafe.Dispose();
        if (mMOBStatus != null)
            mMOBStatus.Dispose();
        if (mMOFerr != null)
            mMOFerr.Dispose();
        //webCamTextureToMatHelper.Dispose();
    }
        
}