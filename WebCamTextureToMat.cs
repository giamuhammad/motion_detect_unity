using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity;
using OpenCVForUnitySample;

[RequireComponent(typeof(WebCamTextureToMatHelper))]
public class WebCamTextureToMat : MonoBehaviour
{
    Color32[] colors;
    Texture2D texture;
    WebCamTextureToMatHelper webCamTextureToMatHelper;


    // Use this for initialization
    void Start()
    {
        webCamTextureToMatHelper = gameObject.GetComponent<WebCamTextureToMatHelper>();
        webCamTextureToMatHelper.Init();
    }

    public Mat GetMat()
    {
        return webCamTextureToMatHelper.GetMat();
    }
    
    public void OnWebCamTextureToMatHelperInited()
    {
        Debug.Log("OnWebCamTextureToMatHelperInited");

        Mat webCamTextureMat = webCamTextureToMatHelper.GetMat();

        colors = new Color32[webCamTextureMat.cols() * webCamTextureMat.rows()];
        texture = new Texture2D(webCamTextureMat.cols(), webCamTextureMat.rows(), TextureFormat.RGBA32, false);


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

    }
    
    public void OnWebCamTextureToMatHelperDisposed()
    {
        Debug.Log("OnWebCamTextureToMatHelperDisposed");

    }

    // Update is called once per frame
    void Update()
    {

        if (webCamTextureToMatHelper.isPlaying() && webCamTextureToMatHelper.didUpdateThisFrame())
        {

            Mat rgbaMat = webCamTextureToMatHelper.GetMat();

            //Imgproc.putText(rgbaMat, "W:" + rgbaMat.width() + " H:" + rgbaMat.height() + " SO:" + Screen.orientation, new Point(5, rgbaMat.rows() - 10), Core.FONT_HERSHEY_SIMPLEX, 1.0, new Scalar(255, 255, 255, 255), 2, Imgproc.LINE_AA, false);

            Utils.matToTexture2D(rgbaMat, texture, colors);
        }

    }
    
    void OnDisable()
    {
        webCamTextureToMatHelper.Dispose();
    }
    
    public void OnBackButton()
    {
    }
    
    public void OnPlayButton()
    {
        webCamTextureToMatHelper.Play();
    }
    
    public void OnPauseButton()
    {
        webCamTextureToMatHelper.Pause();
    }
    
    public void OnStopButton()
    {
        webCamTextureToMatHelper.Stop();
    }
    
    public void OnChangeCameraButton()
    {
        webCamTextureToMatHelper.Init(null, webCamTextureToMatHelper.requestWidth, webCamTextureToMatHelper.requestHeight, !webCamTextureToMatHelper.requestIsFrontFacing);
    }
}