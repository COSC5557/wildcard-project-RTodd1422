using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System;
using System.IO;
using System.Threading;
using System.Globalization;
using ViveSR.anipal.Eye;
using ViveSR.anipal;
using ViveSR;
using Tobii.XR;
using Tobii.G2OM;

public class EyeTracker : MonoBehaviour
{
    public static System.DateTime fileTime;
    public static string UserID;
    public static string File_Path1, File_Path2, File_Path3;
    public static List<string> DataHolder;
    

    public static int cnt_callback = 0;
    //public int cnt_saccade = 0, Endbuffer = 3, SaccadeTimer = 30;
    float Timeout = 1.0f, InitialTimer = 0.0f;
    private static long SaccadeEndTime = 0;
    private static long MeasureTime, CurrentTime, MeasureEndTime, initTime = 0;
    public static float time_stamp;
    private static int frame;

    // ********************************************************************************************************************
    //
    //  Parameters for eye data.
    //
    // ********************************************************************************************************************
    private static EyeData_v2 eyeData = new EyeData_v2();
    public EyeParameter eye_parameter = new EyeParameter();
    public GazeRayParameter gaze = new GazeRayParameter();
    private static bool eye_callback_registered = false;

    private const int maxframe_count = 120 * 30;                    // Maximum number of samples for eye tracking (120 Hz * time in seconds).
    private static UInt64 eye_valid_L, eye_valid_R;                 // The bits explaining the validity of eye data.
    private static float openness_L, openness_R;                    // The level of eye openness.
    private static float pupil_diameter_L, pupil_diameter_R;        // Diameter of pupil dilation.
    private static Vector2 pos_sensor_L, pos_sensor_R;              // Positions of pupils.
    private static Vector3 gaze_origin_L, gaze_origin_R;            // Position of gaze origin.
    private static Vector3 gaze_direct_L, gaze_direct_R;            // Direction of gaze ray.
    private static float frown_L, frown_R;                          // The level of user's frown.
    private static float squeeze_L, squeeze_R;                      // The level to show how the eye is closed tightly.
    private static float wide_L, wide_R;                            // The level to show how the eye is open widely.
    private static double gaze_sensitive;                           // The sensitive factor of gaze ray.
    private static float distance_C;                                // Distance from the central point of right and left eyes.
    private static bool distance_valid_C;                           // Validity of combined data of right and left eyes.
    public bool cal_need;                                           // Calibration judge.
    public bool result_cal;                                         // Result of calibration.
    private static int track_imp_cnt = 0;
    private static TrackingImprovement[] track_imp_item;

    public bool isTrackingEyeData;

    public struct fixationEntry
    {
        public string name;
        public double startTimeStamp;
        public double endTimeStamp;
    }

    public static List<fixationEntry> fixHolder;

    // ********************************************************************************************************************
    //
    //  Start is called before the first frame update. The Start() function is performed only one time.
    //
    // ********************************************************************************************************************
    private void Start()
    {
        fileTime = DateTime.Now;
        initTime = DateTime.Now.Ticks;

        UserID = "User_" + fileTime.Year.ToString() + "--" + fileTime.Month.ToString() + "--" + fileTime.Day.ToString() + "--" + fileTime.Hour.ToString() + "-" + fileTime.Minute.ToString() + "-" + fileTime.Second.ToString();
        Debug.Log("UserID: " + UserID);
        File_Path1 = Application.dataPath + "\\" + "Data_Recording" + "\\" + UserID + "_data" + ".txt";
        File_Path2 = Application.dataPath + "\\" + "Data_Recording" + "\\" + UserID + "_data_Tobii" + ".txt";
        File_Path3 = Application.dataPath + "\\" + "Data_Recording" + "\\" + UserID + "_fixationData" + ".txt";
        Debug.Log("File_Path1: " + File_Path1);
        Debug.Log("File_Path2: " + File_Path2);
        Debug.Log("File_Path3: " + File_Path3);

        DataHolder = new List<string>();
        fixHolder = new List<fixationEntry>();
        
        isTrackingEyeData = true;

        //var settings = new TobiiXR_Settings();
        //TobiiXR.Start(settings);

        Invoke("SystemCheck", 0.5f);
        //Invoke("Measurement2", 0.6f);
        //SRanipal_Eye_v2.LaunchEyeCalibration();     // Perform calibration for eye tracking.
        //Invoke("Calibration", 0.5f);                      //Calibration();

        //Invoke("Measurement", 0.5f);                // Start the measurement of ocular movements in a separate callback function.  
    }

    private void Update()
    {
        if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING) return;
        //
        //if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eye_callback_registered == false)
        //{
        //    SRanipal_Eye.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
        //    eye_callback_registered = true;
        //}
        //else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eye_callback_registered == true)
        //{
        //    SRanipal_Eye.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
        //    eye_callback_registered = false;
        //}

        //EyeParameter eye_parameter = new EyeParameter();
        //SRanipal_Eye_API.GetEyeParameter(ref eye_parameter);


        if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eye_callback_registered == false)
        {
            Debug.Log("TF");
            SRanipal_Eye.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
            eye_callback_registered = true;
        }
        
        else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eye_callback_registered == true)
        {
            Debug.Log("FT");
            SRanipal_Eye.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
            eye_callback_registered = false;
        }

        //var eyeTrackingData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);

        //DataHolder.Add(eyeTrackingData.Timestamp.ToString() + "," + eyeTrackingData.ConvergenceDistance.ToString() + "," + eyeTrackingData.ConvergenceDistanceIsValid.ToString() + "," + eyeTrackingData.GazeRay.Origin.ToString() + "," + eyeTrackingData.GazeRay.Direction.ToString() + "," + eyeTrackingData.GazeRay.IsValid.ToString() + "," + eyeTrackingData.IsLeftEyeBlinking.ToString() + "," + eyeTrackingData.IsRightEyeBlinking.ToString());
    }


    // ********************************************************************************************************************
    //
    //  Check if the system works properly.
    //
    // ********************************************************************************************************************
    void SystemCheck()
    {
        if (SRanipal_Eye_API.GetEyeData_v2(ref eyeData) == ViveSR.Error.WORK)
        {
            Debug.Log("Device is working properly.");
        }

        if (SRanipal_Eye_API.GetEyeParameter(ref eye_parameter) == ViveSR.Error.WORK)
        {
            Debug.Log("Eye parameters are measured.");
        }

        //  Check again if the initialisation of eye tracking functions successfully. If not, we stop playing Unity.
        Error result_eye_init = SRanipal_API.Initial(SRanipal_Eye_v2.ANIPAL_TYPE_EYE_V2, IntPtr.Zero);

        if (result_eye_init == Error.WORK)
        {
            Debug.Log("[SRanipal] Initial Eye v2: " + result_eye_init);
        }
        else
        {
            Debug.LogError("[SRanipal] Initial Eye v2: " + result_eye_init);

            //if (UnityEditor.EditorApplication.isPlaying)
            //{
            //    UnityEditor.EditorApplication.isPlaying = false;    // Stops Unity editor.
            //}
        }
    }



    // ********************************************************************************************************************
    //
    //  Calibration is performed if the calibration is necessary.
    //
    // ********************************************************************************************************************
    void Calibration()
    {
        //SRanipal_Eye_API.IsUserNeedCalibration(ref cal_need);           // Check the calibration status. If needed, we perform the calibration.

        if (cal_need == true)
        {
            result_cal = SRanipal_Eye_v2.LaunchEyeCalibration();

            if (result_cal == true)
            {
                Debug.Log("Calibration is done successfully.");
            }

            else
            {
                Debug.Log("Calibration is failed.");
                //if (UnityEditor.EditorApplication.isPlaying)
                //{
                //    UnityEditor.EditorApplication.isPlaying = false;    // Stops Unity editor if the calibration if failed.
                //}
            }
        }

        if (cal_need == false)
        {
            Debug.Log("Calibration is not necessary");
        }
    }

    // <summary>
    // Required class for IL2CPP scripting backend support
    // </summary>
    internal class MonoPInvokeCallbackAttribute : System.Attribute
    {
        public MonoPInvokeCallbackAttribute() { }
    }

    /// <summary>
    /// Release callback thread when disabled or quit
    /// </summary>
    private static void Release()
    {
        if (eye_callback_registered == true)
        {
            SRanipal_Eye.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
            eye_callback_registered = false;
        }
    }

    private void OnDisable()
    {
        Release();
    }

    private void OnApplicationQuit()
    {
        Release();
        System.IO.Directory.CreateDirectory(Application.dataPath + "\\" + "Data_Recording");
        using (StreamWriter dataWriter = File.AppendText(File_Path1))
        {
            dataWriter.WriteLine(
                "Unity_Timestamp" + "," +
                "TobiiXR_Timestamp" + "," +                      // 0
                "TobiiXR_ConvergenceDistance (meters)" + "," +   // 1
                "TobiiXR_ConvergenceDistanceIsValid" + "," +     // 2
                "TobiiXR_GazeRayOrigin.x" + "," +                // 3
                "TobiiXR_GazeRayOrigin.y" + "," +                // 4
                "TobiiXR_GazeRayOrigin.z" + "," +                // 5
                "TobiiXR_GazeRayDirection.x" + "," +             // 6
                "TobiiXR_GazeRayDirection.y" + "," +             // 7
                "TobiiXR_GazeRayDirection.z" + "," +             // 8
                "TobiiXR_GazeRayIsValid" + "," +                 // 9
                "TobiiXR_IsLeftEyeBlinking" + "," +              // 10
                "TobiiXR_IsRightEyeBlinking" + "," +             // 11

                "SR_time(100ns)" + "," +                         // 12
                "SR_time_stamp(ms)" + "," +                      // 13
                "SR_frame" + "," +                               // 14
                "SR_eye_valid_L" + "," +                         // 15
                "SR_eye_valid_R" + "," +                         // 16
                "SR_openness_L" + "," +                          // 17
                "SR_openness_R" + "," +                          // 18
                "SR_pupil_diameter_L(mm)" + "," +                // 19
                "SR_pupil_diameter_R(mm)" + "," +                // 20
                "SR_pos_sensor_L.x" + "," +                      // 21
                "SR_pos_sensor_L.y" + "," +                      // 22
                "SR_pos_sensor_R.x" + "," +                      // 23
                "SR_pos_sensor_R.y" + "," +                      // 24
                "SR_gaze_origin_L.x(mm)" + "," +                 // 25
                "SR_gaze_origin_L.y(mm)" + "," +                 // 26
                "SR_gaze_origin_L.z(mm)" + "," +                 // 27
                "SR_gaze_origin_R.x(mm)" + "," +                 // 28
                "SR_gaze_origin_R.y(mm)" + "," +                 // 29
                "SR_gaze_origin_R.z(mm)" + "," +                 // 30
                "SR_gaze_direct_L.x" + "," +                     // 31
                "SR_gaze_direct_L.y" + "," +                     // 32
                "SR_gaze_direct_L.z" + "," +                     // 33
                "SR_gaze_direct_R.x" + "," +                     // 34
                "SR_gaze_direct_R.y" + "," +                     // 35
                "SR_gaze_direct_R.z" + "," +                     // 36
                //"gaze_sensitive" + "," +                       // 
                "SR_frown_L" + "," +                             // 37
                "SR_frown_R" + "," +                             // 38
                "SR_squeeze_L" + "," +                           // 39
                "SR_squeeze_R" + "," +                           // 40
                "SR_wide_L" + "," +                              // 41
                "SR_wide_R" + "," +                              // 42
                "SR_distance_valid_C" + "," +                    // 43
                "SR_distance_C(mm)" + "," +                      // 44
                "SR_track_imp_cnt");

            for (int i = 0; i < DataHolder.Count; i++)
            {
                dataWriter.WriteLine(DataHolder[i]);
            }
        }

        using (StreamWriter dataWriter = File.AppendText(File_Path2))
        {
            for (int i = 0; i < DataHolder.Count; i++)
            {
                dataWriter.WriteLine(Convert2TobiiFormat(DataHolder[i]));
            }
        }

        using (StreamWriter dataWriter = File.AppendText(File_Path3))
        {
            dataWriter.WriteLine("Fixation Name, Start Time, End Time");
            for (int i = 0; i < fixHolder.Count; i++)
            {
                dataWriter.WriteLine(fixHolder[i].name + "," + fixHolder[i].startTimeStamp + "," + fixHolder[i].endTimeStamp);
            }
        }
    }

    private string Convert2TobiiFormat(string curData)
    {
        var dat = curData.Split(',');
        string outString = "";

        print("dat size: " + dat.Length);
        
        outString += "{\"type\":\"gaze\",\"timestamp\":"
                        + dat[0]
                        + ",\"data\":{\"gaze2d\":["
                        + ((Decimal.Parse(dat[21]) + Decimal.Parse(dat[23]))/2) 
                        + "," 
                        + ((Decimal.Parse(dat[22]) + Decimal.Parse(dat[24])) / 2)
                        + "],\"gaze3d\":["
                        + (dat[3]) + "," + (dat[4]) + "," + (dat[5])    // Position of the vergence point of the left
                                                                        // and right gaze vector relative to the
                                                                        // scene camera.Measured in millimeters.

                        + "],\"eyeleft\":{\"gazeorigin\":["
                        + (dat[25]) + "," + (dat[26]) + "," + (dat[27])
                        + "],\"gazedirection\":["
                        + (dat[31]) + "," + (dat[32]) + "," + (dat[33])
                        + "],\"pupildiameter\":"
                        + (dat[19]) 
                        + "},\"eyeright\":{\"gazeorigin\":["
                        + (dat[28]) + "," + (dat[29]) + "," + (dat[30])
                        + "],\"gazedirection\":["
                        + (dat[34]) + "," + (dat[35]) + "," + (dat[36])
                        + "],\"pupildiameter\":"
                        + (dat[20]) + "}}}";
        

        return outString;
    }



    // ********************************************************************************************************************
    //
    //  Measure eye movements in a callback function that HTC SRanipal provides.
    //
    // ********************************************************************************************************************
    void Measurement()
    {
        EyeParameter eye_parameter = new EyeParameter();
        SRanipal_Eye_API.GetEyeParameter(ref eye_parameter);


        if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eye_callback_registered == false)
        {
            //Debug.Log("TF");
            SRanipal_Eye_v2.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
            eye_callback_registered = true;
        }

        else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eye_callback_registered == true)
        {
            //Debug.Log("FT");
            SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
            eye_callback_registered = false;
        }
    }



    // ********************************************************************************************************************
    //
    //  Callback function to record the eye movement data.
    //  Note that SRanipal_Eye_v2 does not work in the function below. It only works under UnityEngine.
    //
    // ********************************************************************************************************************
    [MonoPInvokeCallback]
    private static void EyeCallback(ref EyeData_v2 eye_data)
    {
        //Debug.Log("entered EyeCallback");

        var eyeTrackingData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);

        string tobiiXRData = 
            eyeTrackingData.Timestamp.ToString() + "," + 
            eyeTrackingData.ConvergenceDistance.ToString() + "," + 
            eyeTrackingData.ConvergenceDistanceIsValid.ToString() + "," + 
            eyeTrackingData.GazeRay.Origin.x.ToString() + "," +
            eyeTrackingData.GazeRay.Origin.y.ToString() + "," +
            eyeTrackingData.GazeRay.Origin.z.ToString() + "," +
            eyeTrackingData.GazeRay.Direction.x.ToString() + "," +
            eyeTrackingData.GazeRay.Direction.y.ToString() + "," +
            eyeTrackingData.GazeRay.Direction.z.ToString() + "," +
            eyeTrackingData.GazeRay.IsValid.ToString() + "," + 
            eyeTrackingData.IsLeftEyeBlinking.ToString() + "," + 
            eyeTrackingData.IsRightEyeBlinking.ToString();

        //EyeParameter eye_parameter = new EyeParameter();
        //SRanipal_Eye_API.GetEyeParameter(ref eye_parameter);
        eyeData = eye_data;
        
        //ViveSR.Error error = SRanipal_Eye_API.GetEyeData_v2(ref eyeData);
        
        //if (error == ViveSR.Error.WORK)
        //{
        // --------------------------------------------------------------------------------------------------------
        //  Measure each parameter of eye data that are specified in the guideline of SRanipal SDK.
        // --------------------------------------------------------------------------------------------------------
        


        MeasureTime = DateTime.Now.Ticks - initTime;
        time_stamp = eyeData.timestamp;
        frame = eyeData.frame_sequence;
        eye_valid_L = eyeData.verbose_data.left.eye_data_validata_bit_mask;
        eye_valid_R = eyeData.verbose_data.right.eye_data_validata_bit_mask;
        openness_L = eyeData.verbose_data.left.eye_openness;
        openness_R = eyeData.verbose_data.right.eye_openness;
        pupil_diameter_L = eyeData.verbose_data.left.pupil_diameter_mm;
        pupil_diameter_R = eyeData.verbose_data.right.pupil_diameter_mm;
        pos_sensor_L = eyeData.verbose_data.left.pupil_position_in_sensor_area;
        pos_sensor_R = eyeData.verbose_data.right.pupil_position_in_sensor_area;
        gaze_origin_L = eyeData.verbose_data.left.gaze_origin_mm;
        gaze_origin_R = eyeData.verbose_data.right.gaze_origin_mm;
        gaze_direct_L = eyeData.verbose_data.left.gaze_direction_normalized;
        gaze_direct_R = eyeData.verbose_data.right.gaze_direction_normalized;
        //gaze_sensitive = eye_parameter.gaze_ray_parameter.sensitive_factor;
        frown_L = eyeData.expression_data.left.eye_frown;
        frown_R = eyeData.expression_data.right.eye_frown;
        squeeze_L = eyeData.expression_data.left.eye_squeeze;
        squeeze_R = eyeData.expression_data.right.eye_squeeze;
        wide_L = eyeData.expression_data.left.eye_wide;
        wide_R = eyeData.expression_data.right.eye_wide;
        distance_valid_C = eyeData.verbose_data.combined.convergence_distance_validity;
        distance_C = eyeData.verbose_data.combined.convergence_distance_mm;
        track_imp_cnt = eyeData.verbose_data.tracking_improvements.count;
        //track_imp_item = eyeData.verbose_data.tracking_improvements.items;
        //  Convert the measured data to string data to write in a text file.

        string dataFrame =
            AppleTimer.timer.Elapsed.TotalSeconds.ToString() + "," +
            tobiiXRData + "," +
            MeasureTime.ToString() + "," +
            time_stamp.ToString() + "," +
            frame.ToString() + "," +
            eye_valid_L.ToString() + "," +
            eye_valid_R.ToString() + "," +
            openness_L.ToString() + "," +
            openness_R.ToString() + "," +
            pupil_diameter_L.ToString() + "," +
            pupil_diameter_R.ToString() + "," +
            pos_sensor_L.x.ToString() + "," +
            pos_sensor_L.y.ToString() + "," +
            pos_sensor_R.x.ToString() + "," +
            pos_sensor_R.y.ToString() + "," +
            gaze_origin_L.x.ToString() + "," +
            gaze_origin_L.y.ToString() + "," +
            gaze_origin_L.z.ToString() + "," +
            gaze_origin_R.x.ToString() + "," +
            gaze_origin_R.y.ToString() + "," +
            gaze_origin_R.z.ToString() + "," +
            gaze_direct_L.x.ToString() + "," +
            gaze_direct_L.y.ToString() + "," +
            gaze_direct_L.z.ToString() + "," +
            gaze_direct_R.x.ToString() + "," +
            gaze_direct_R.y.ToString() + "," +
            gaze_direct_R.z.ToString() + "," +
            //gaze_sensitive.ToString() + "," +
            frown_L.ToString() + "," +
            frown_R.ToString() + "," +
            squeeze_L.ToString() + "," +
            squeeze_R.ToString() + "," +
            wide_L.ToString() + "," +
            wide_R.ToString() + "," +
            distance_valid_C.ToString() + "," +
            distance_C.ToString() + "," +
            track_imp_cnt.ToString();
        
        DataHolder.Add(dataFrame);
        //Debug.Log("created/added frame.");
        //File.AppendAllText(File_Path, dataFrame);
        //}
    }
}