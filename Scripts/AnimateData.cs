using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateData : MonoBehaviour
{
    bool startMessage;
    bool endMessage;
    bool dataCopied;

    public int frameNum;
    public float screenWidthReal;
    public float screenHeightReal;
    public float viewportWidthReal;
    public float viewportHeightReal;
    public float viewportDepthReal;
    public float tobiiScreenDist;
    public double screenPixWidth = 2560;
    public double screenPixHeight = 1440;
    public double viewportPixWidth = 1920;
    public double viewportPixHeight = 1080;
    public float tobiiHorizFOV = 95f;//degrees
    public float tobiiVertFOV = 63f;//degrees
    public float fovHeight, fovWidth;
    
    public GameObject TobiiPointPrefab;
    public GameObject TobiiGlassesOrigin;
    public GameObject DataPointSphereRed;
    public GameObject DataPointSphereGreen;
    public GameObject ScreenPlane;
    public GameObject ScreenEdgeMarker1;
    public GameObject ScreenEdgeMarker2;
    public GameObject ScreenEdgeMarker3;
    public GameObject ScreenEdgeMarker4;
    public GameObject ScreenEdgeMarker5;
    public GameObject ScreenEdgeMarker6;
    public GameObject ScreenEdgeMarker7;
    public GameObject ScreenEdgeMarker8;
    public GameObject ViewportCorner;
    public GameObject ScreenEdgeCube;
    public GameObject VideoPlane;
    public GameObject GreenGhost;
    public GameObject RedGhost;
    public GameObject ScreenEdgeTopLine;
    public GameObject ScreenEdgeBottomLine;
    public GameObject ScreenEdgeLeftLine;
    public GameObject ScreenEdgeRightLine;
    public GameObject fovTopPoint;
    public GameObject fovBottomPoint;
    public GameObject fovLeftPoint;
    public GameObject fovRightPoint;
    public Plane ScreenPlanePlane;
    public Ray FOVRay;


    public Vector3 screenLeftEdgePos;
    public Vector3 screenRightEdgePos;
    public Vector3 screenTopEdgePos;
    public Vector3 screenBottomEdgePos;
    public Vector3 screenTopLeft;
    public Vector3 screenTopRight;
    public Vector3 screenBottomLeft;
    public Vector3 screenBottomRight;
    public Vector3 viewportLeftEdge;
    public Vector3 viewportBottomEdge;
    public Vector3 viewportRightEdge;
    public Vector3 viewportTopEdge;
    public Vector3 fovTopLeft;
    public Vector3 fovTopRight;
    public Vector3 fovBottomLeft;
    public Vector3 fovBottomRight;
    public Vector3 fovMiddlePoint;
    public Vector3 tobiiFOVvecToTop;
    public Vector3 tobiiFOVvecToBottom;
    public Vector3 tobiiFOVvecToLeft;
    public Vector3 tobiiFOVvecToRight;

    void Start()
    {
        //Debug.Log("AnimateData.cs started");
        frameNum = 0;
        TobiiGlassesOrigin = GameObject.Find("TobiiGlassesOrigin");
        DataPointSphereRed = GameObject.Find("DataPointSphereRed");
        DataPointSphereGreen = GameObject.Find("DataPointSphereGreen");
        fovTopPoint = GameObject.Find("fovTopPoint");
        fovBottomPoint = GameObject.Find("fovBottomPoint");
        fovLeftPoint = GameObject.Find("fovLeftPoint");
        fovRightPoint = GameObject.Find("fovRightPoint");
        ScreenPlane = GameObject.Find("ScreenPlane");
        ScreenPlane.GetComponent<Renderer>().enabled = false;

        //ScreenEdgeMarkers are numbered clockwise around edge of screen
        ScreenEdgeMarker1 = GameObject.Find("ScreenEdgeMarker1");
        ScreenEdgeMarker2 = GameObject.Find("ScreenEdgeMarker2");
        ScreenEdgeMarker3 = GameObject.Find("ScreenEdgeMarker3");
        ScreenEdgeMarker4 = GameObject.Find("ScreenEdgeMarker4");
        ScreenEdgeMarker5 = GameObject.Find("ScreenEdgeMarker5");
        ScreenEdgeMarker6 = GameObject.Find("ScreenEdgeMarker6");
        ScreenEdgeMarker7 = GameObject.Find("ScreenEdgeMarker7");
        ScreenEdgeMarker8 = GameObject.Find("ScreenEdgeMarker8");

        //ScreenPlanePlane = new Plane(ScreenPlane.transform.up, ScreenPlane.transform.position);
        ScreenPlanePlane = new Plane(ScreenEdgeMarker7.transform.position, ScreenEdgeMarker2.transform.position, ScreenEdgeMarker6.transform.position);
        ScreenPlanePlane = Plane.Translate(ScreenPlanePlane, new Vector3(0f, 0f, -0.003f));

        screenTopLeft = ScreenEdgeMarker7.transform.position + Vector3.Project(ScreenEdgeMarker8.transform.position - ScreenEdgeMarker7.transform.position, (ScreenEdgeMarker2.transform.position - ScreenEdgeMarker7.transform.position));
        screenTopRight = ScreenEdgeMarker2.transform.position + Vector3.Project(ScreenEdgeMarker1.transform.position - ScreenEdgeMarker2.transform.position, -(ScreenEdgeMarker2.transform.position - ScreenEdgeMarker7.transform.position));
        screenBottomLeft = ScreenEdgeMarker6.transform.position + Vector3.Project(ScreenEdgeMarker5.transform.position - ScreenEdgeMarker6.transform.position, (ScreenEdgeMarker3.transform.position - ScreenEdgeMarker6.transform.position));
        screenBottomRight = ScreenEdgeMarker3.transform.position + Vector3.Project(ScreenEdgeMarker4.transform.position - ScreenEdgeMarker3.transform.position, -(ScreenEdgeMarker3.transform.position - ScreenEdgeMarker6.transform.position));

        Instantiate(ViewportCorner, screenTopLeft, Quaternion.identity);
        Instantiate(ViewportCorner, screenTopRight, Quaternion.identity);
        Instantiate(ViewportCorner, screenBottomLeft, Quaternion.identity);
        Instantiate(ViewportCorner, screenBottomRight, Quaternion.identity);

        screenLeftEdgePos = AverageVecs(screenTopLeft, screenBottomLeft);
        screenRightEdgePos = AverageVecs(screenTopRight, screenBottomRight);
        screenTopEdgePos = AverageVecs(screenTopLeft, screenTopRight);
        screenBottomEdgePos = AverageVecs(screenBottomLeft, screenBottomRight);

        screenWidthReal = Vector3.Distance(screenLeftEdgePos, screenRightEdgePos);
        screenHeightReal = Vector3.Distance(screenTopEdgePos, screenBottomEdgePos);
        //Debug.Log("ScreenWidthReal: " + screenWidthReal);
        //Debug.Log("ScreenHeightReal: " + screenHeightReal);

        viewportLeftEdge = Vector3.Lerp(screenLeftEdgePos, screenRightEdgePos, (float)(1 - (viewportPixWidth / screenPixWidth)));
        viewportRightEdge = Vector3.Lerp(screenLeftEdgePos, screenRightEdgePos, (float)(viewportPixWidth / screenPixWidth));
        viewportTopEdge = Vector3.Lerp(screenTopEdgePos, screenBottomEdgePos, (float)(1 - (viewportPixHeight / screenPixHeight)));
        viewportBottomEdge = Vector3.Lerp(screenTopEdgePos, screenBottomEdgePos, (float)(viewportPixHeight / screenPixHeight));

        viewportDepthReal = ((viewportLeftEdge.z + viewportRightEdge.z + viewportTopEdge.z + viewportBottomEdge.z) / 4) + (0.003f);// same offset as for screenplaneplane
        viewportWidthReal = Vector3.Distance(viewportLeftEdge, viewportRightEdge);
        viewportHeightReal = Vector3.Distance(viewportTopEdge, viewportBottomEdge);
        //Debug.Log("viewportWidthReal: " + viewportWidthReal.ToString());
        //Debug.Log("viewportHeightReal: " + viewportHeightReal.ToString());
        
        Instantiate(ViewportCorner, new Vector3(viewportLeftEdge.x, viewportTopEdge.y, viewportDepthReal), Quaternion.identity);
        Instantiate(ViewportCorner, new Vector3(viewportLeftEdge.x, viewportBottomEdge.y, viewportDepthReal), Quaternion.identity);
        Instantiate(ViewportCorner, new Vector3(viewportRightEdge.x, viewportTopEdge.y, viewportDepthReal), Quaternion.identity);
        Instantiate(ViewportCorner, new Vector3(viewportRightEdge.x, viewportBottomEdge.y, viewportDepthReal), Quaternion.identity);

        tobiiFOVvecToTop = Quaternion.AngleAxis(-tobiiVertFOV / 2,TobiiGlassesOrigin.transform.right) * TobiiGlassesOrigin.transform.forward;
        tobiiFOVvecToBottom = Quaternion.AngleAxis(tobiiVertFOV / 2, TobiiGlassesOrigin.transform.right) * TobiiGlassesOrigin.transform.forward;
        tobiiFOVvecToLeft = Quaternion.AngleAxis(-tobiiHorizFOV / 2, TobiiGlassesOrigin.transform.up) * TobiiGlassesOrigin.transform.forward;
        tobiiFOVvecToRight = Quaternion.AngleAxis(tobiiHorizFOV / 2, TobiiGlassesOrigin.transform.up) * TobiiGlassesOrigin.transform.forward; 

        float rayDist;
        FOVRay = new Ray(TobiiGlassesOrigin.transform.position, tobiiFOVvecToTop);
        ScreenPlanePlane.Raycast(FOVRay, out rayDist);
        fovTopPoint.transform.position = FOVRay.GetPoint(rayDist);

        FOVRay = new Ray(TobiiGlassesOrigin.transform.position, tobiiFOVvecToBottom);
        ScreenPlanePlane.Raycast(FOVRay, out rayDist);
        fovBottomPoint.transform.position = FOVRay.GetPoint(rayDist);

        FOVRay = new Ray(TobiiGlassesOrigin.transform.position, tobiiFOVvecToLeft);
        ScreenPlanePlane.Raycast(FOVRay, out rayDist);
        fovLeftPoint.transform.position = FOVRay.GetPoint(rayDist);

        FOVRay = new Ray(TobiiGlassesOrigin.transform.position, tobiiFOVvecToRight);
        ScreenPlanePlane.Raycast(FOVRay, out rayDist);
        fovRightPoint.transform.position = FOVRay.GetPoint(rayDist);

        fovHeight = Vector3.Distance(fovTopPoint.transform.position, fovBottomPoint.transform.position);
        fovWidth = Vector3.Distance(fovLeftPoint.transform.position, fovRightPoint.transform.position);

        fovTopLeft = fovTopPoint.transform.position - Vector3.Project(fovTopPoint.transform.position - fovLeftPoint.transform.position, -(fovLeftPoint.transform.position - fovRightPoint.transform.position));
        fovTopRight = fovTopPoint.transform.position - Vector3.Project(fovTopPoint.transform.position - fovRightPoint.transform.position, (fovLeftPoint.transform.position - fovRightPoint.transform.position));
        fovBottomLeft = fovBottomPoint.transform.position - Vector3.Project(fovBottomPoint.transform.position - fovLeftPoint.transform.position, -(fovLeftPoint.transform.position - fovRightPoint.transform.position));
        fovBottomRight = fovBottomPoint.transform.position - Vector3.Project(fovBottomPoint.transform.position - fovRightPoint.transform.position, (fovLeftPoint.transform.position - fovRightPoint.transform.position));

        Instantiate(ViewportCorner, fovTopLeft, Quaternion.identity);
        Instantiate(ViewportCorner, fovTopRight, Quaternion.identity);
        Instantiate(ViewportCorner, fovBottomLeft, Quaternion.identity);
        Instantiate(ViewportCorner, fovBottomRight, Quaternion.identity);

        ScreenEdgeTopLine = Instantiate(ScreenEdgeCube, screenTopEdgePos, Quaternion.Euler(ScreenPlanePlane.normal));
        ScreenEdgeBottomLine = Instantiate(ScreenEdgeCube, screenBottomEdgePos, Quaternion.Euler(ScreenPlanePlane.normal));
        ScreenEdgeLeftLine = Instantiate(ScreenEdgeCube, screenLeftEdgePos, Quaternion.Euler(ScreenPlanePlane.normal));
        ScreenEdgeRightLine = Instantiate(ScreenEdgeCube, screenRightEdgePos, Quaternion.Euler(ScreenPlanePlane.normal));

        ScreenEdgeTopLine.transform.localScale = new Vector3(ScreenEdgeTopLine.transform.localScale.x, ScreenEdgeTopLine.transform.localScale.y, Vector3.Distance(screenTopLeft, screenTopRight));
        ScreenEdgeBottomLine.transform.localScale = new Vector3(ScreenEdgeBottomLine.transform.localScale.x, ScreenEdgeBottomLine.transform.localScale.y, Vector3.Distance(screenBottomLeft, screenBottomRight));
        ScreenEdgeLeftLine.transform.localScale = new Vector3(ScreenEdgeLeftLine.transform.localScale.x, ScreenEdgeLeftLine.transform.localScale.y, Vector3.Distance(screenTopLeft, screenBottomLeft));
        ScreenEdgeRightLine.transform.localScale = new Vector3(ScreenEdgeRightLine.transform.localScale.x, ScreenEdgeRightLine.transform.localScale.y, Vector3.Distance(screenTopRight, screenBottomRight));

        //ScreenEdgeTopLine.transform.LookAt(screenTopLeft);
        //ScreenEdgeBottomLine.transform.LookAt(screenBottomLeft);
        //ScreenEdgeLeftLine.transform.LookAt(screenTopLeft);
        //ScreenEdgeRightLine.transform.LookAt(screenTopRight);

        ScreenEdgeTopLine.transform.rotation = Quaternion.LookRotation(ScreenEdgeTopLine.transform.position - screenTopLeft, ScreenEdgeMarker7.transform.position - ScreenEdgeMarker6.transform.position);
        ScreenEdgeBottomLine.transform.rotation = Quaternion.LookRotation(ScreenEdgeBottomLine.transform.position - screenBottomLeft, ScreenEdgeMarker7.transform.position - ScreenEdgeMarker6.transform.position);
        ScreenEdgeLeftLine.transform.rotation = Quaternion.LookRotation(ScreenEdgeLeftLine.transform.position - screenTopLeft, ScreenEdgeMarker2.transform.position - ScreenEdgeMarker7.transform.position);
        ScreenEdgeRightLine.transform.rotation = Quaternion.LookRotation(ScreenEdgeRightLine.transform.position - screenTopRight, ScreenEdgeMarker2.transform.position - ScreenEdgeMarker7.transform.position);

        fovMiddlePoint = (fovTopLeft + fovBottomLeft + fovTopRight + fovBottomRight) / 4;
        //fovMiddlePoint = AverageVecs(fovLeftPoint.transform.position, fovRightPoint.transform.position);
        //GameObject vidPlane = Instantiate(VideoPlane, fovMiddlePoint, Quaternion.LookRotation(fovBottomPoint.transform.position -fovTopPoint.transform.position, Vector3.Cross(fovLeftPoint.transform.position - fovRightPoint.transform.position, fovTopPoint.transform.position - fovBottomPoint.transform.position)));
        GameObject vidPlane = Instantiate(VideoPlane, fovMiddlePoint, TobiiGlassesOrigin.transform.rotation);
        //GameObject vidPlane = Instantiate(VideoPlane, fovMiddlePoint, Quaternion.Euler(ScreenPlanePlane.normal));
        vidPlane.transform.localScale = new Vector3(Vector3.Distance(fovLeftPoint.transform.position, fovRightPoint.transform.position)/10, 1f, Vector3.Distance(fovTopPoint.transform.position, fovBottomPoint.transform.position)/10);
    }
    
    void Update()
    {
        if(DataLoadFromTxt.tobiiDataLoaded == false)
        {
            DataLoadFromTxt.LoadTobiiData("Assets/Resources/ValidGridTest/tobiieyedata.csv");
        }

        if(DataLoadFromTxt.screenDataLoaded == false)
        {
            DataLoadFromTxt.LoadScreenData("Assets/Resources/ValidGridTest/screenData.csv");
        }

        

        while (DataLoadFromTxt.tobiiDataList[0] != null && (float)DataLoadFromTxt.tobiiDataList[0][0] < Time.time)
        {
            DataLoadFromTxt.tobiiDataList.RemoveAt(0);
        }
        //Debug.Log("tobiiTime: " + DataLoadFromTxt.tobiiDataList[0][0]);

        while (DataLoadFromTxt.screenDataList[0] != null && (float)DataLoadFromTxt.screenDataList[0][2] < Time.time)
        {
            DataLoadFromTxt.screenDataList.RemoveAt(0);
        }
        //Debug.Log("screenTime: " + (float)DataLoadFromTxt.screenDataList[0][2]);

        if (DataLoadFromTxt.tobiiDataList[0] != null) // Avoid going out of range
        {
            //Vector3 tobiiPointPos = new Vector3((float)DataLoadFromTxt.tobiiDataList[frameNum][3], (float)DataLoadFromTxt.tobiiDataList[frameNum][4], (float)DataLoadFromTxt.tobiiDataList[frameNum][5]);
            //Vector3 tobiiPointPos = new Vector3(viewportLeftEdge.x - (float)DataLoadFromTxt.tobiiDataList[0][1] *screenHeightReal, screenTopEdgePos.y - (float)DataLoadFromTxt.tobiiDataList[0][2] * screenHeightReal, viewportDepthReal);
            Vector3 tobiiPointPos = new Vector3((Vector3.Lerp(fovTopLeft, fovBottomLeft, (float)DataLoadFromTxt.tobiiDataList[0][2]).x) - (float)DataLoadFromTxt.tobiiDataList[0][1] * fovWidth, (Vector3.Lerp(fovTopLeft, fovTopRight, (float)DataLoadFromTxt.tobiiDataList[0][1]).y) - (float)DataLoadFromTxt.tobiiDataList[0][2] * fovHeight, viewportDepthReal);
            
            DataPointSphereRed.transform.position = tobiiPointPos;

            if(frameNum % 5 == 0)
            {
                Instantiate(RedGhost, DataPointSphereRed.transform.position, Quaternion.identity, TobiiGlassesOrigin.transform);
            }
            
        }

        if(DataLoadFromTxt.screenDataList[0] != null)
        {

            //Vector3 screenPointPos = new Vector3(viewportLeftEdge.x - ((float)DataLoadFromTxt.screenDataList[0][0]) * viewportWidthReal, viewportBottomEdge.y + ((float)DataLoadFromTxt.screenDataList[0][1]) * viewportHeightReal, viewportDepthReal);
            //Vector3 screenPointPos = new Vector3((screenLeftEdgePos.x) - ((float)DataLoadFromTxt.screenDataList[0][0]) * screenWidthReal, (screenBottomEdgePos.y) + ((float)DataLoadFromTxt.screenDataList[0][1]) * screenHeightReal, viewportDepthReal);
            Vector3 screenPointPos = new Vector3((Vector3.Lerp(screenTopLeft, screenBottomLeft, (float)DataLoadFromTxt.screenDataList[0][1]).x) - ((float)DataLoadFromTxt.screenDataList[0][0]) * screenWidthReal, (Vector3.Lerp(screenBottomLeft, screenBottomRight, (float)DataLoadFromTxt.screenDataList[0][0]).y) + ((float)DataLoadFromTxt.screenDataList[0][1]) * screenHeightReal, viewportDepthReal);
           
            DataPointSphereGreen.transform.position = screenPointPos;

            if (frameNum % 5 == 0)
            {
                Instantiate(GreenGhost, DataPointSphereGreen.transform.position, Quaternion.identity);
            }
            frameNum++;
        }
        //frameNum++;
        //if(frameNum%100 == 0)
        //{
        //    Debug.Log("TobiiScreenTimeDif: " + Mathf.Abs((float)DataLoadFromTxt.tobiiDataList[0][0] - (float)DataLoadFromTxt.screenDataList[0][2]));
        //}
    }

    Vector3 AverageVecs(Vector3 PointOne, Vector3 PointTwo)
    {
        return new Vector3((PointOne.x + PointTwo.x) / 2, (PointOne.y + PointTwo.y) / 2, (PointOne.z + PointTwo.z) / 2);
    }
}
