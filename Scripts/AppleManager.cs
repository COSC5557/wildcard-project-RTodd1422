/* Russell Todd
 * The idea for this script is to have it manage the apples themselves. 
 * This should be where we can keep track of apples, spawn apples, and destroy apples. 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleManager : MonoBehaviour
{
    public static int numOfApples;
    public GameObject redApplePrefab;   // This NEEDS to be set in the Unity Editor window by dragging the RED prefab apple into the script slot in the inspector window.
    public GameObject greenApplePrefab; // This NEEDS to be set in the Unity Editor window by dragging the GREEN prefab apple into the script slot in the inspector window.
    public GameObject SliderHandle;
    public static GameObject spawnPoint;
    public GameObject HeightAppleHome;
    public GameObject topPlane, bottomPlane, leftPlane, rightPlane, backPlane, frontPlane, planeParent;
    public GameObject treeRoot, treeModel;
    public float normHeight;
    public float scaleDist;
    public Vector3 TreeInitScale;

    public static List<Vector3> applePosits;
    public static float appleMinDist = 0.25f;
    public static int appleSpawnAttemptLimit = 10;
    public static int appleSpawnAttemptIter;
    public float xUpperSpawnRange = 0.6f;
    public float yUpperSpawnRange = 0.2f;
    public float zUpperSpawnRange = 0.2f;
    public static GameObject tempPos;


    public float xLowerSpawnRange = 0.6f;
    public float yLowerSpawnRange = 0.2f;
    public float zLowerSpawnRange = 0.2f;

    void Awake()
    {
        numOfApples = 0;
        spawnPoint = GameObject.Find("AppleSpawnPoint");
        applePosits = new List<Vector3>();
    }

    private void Start()
    {
        SliderHandle = GameObject.Find("SliderHandle");
        HeightAppleHome = GameObject.Find("HeightAppleHome");
        topPlane = GameObject.Find("TopPlane");
        bottomPlane = GameObject.Find("BottomPlane");
        leftPlane = GameObject.Find("LeftPlane");
        rightPlane = GameObject.Find("RightPlane");
        backPlane = GameObject.Find("BackPlane");
        frontPlane = GameObject.Find("FrontPlane");
        planeParent = GameObject.Find("PlaneParent");
        treeRoot = GameObject.Find("TreeRoot");
        treeModel = GameObject.Find("AppleTree");
        TreeInitScale= treeModel.transform.localScale;
        scaleDist = spawnPoint.transform.position.y - treeRoot.transform.position.y;



        SetSpawnRangePlanes();
        HidePlanes();
    }

    void Update()
    {
        if(numOfApples == 0 && ApplePickingGame.gameFinished == false)
        {
            if(ApplePickingGame.difficultyLevel == 3)
            {
                DestroyAllApples();
                SpawnMixedApples(ApplePickingGame.applesPerRound, spawnPoint.transform);
            }
            else
            {
                DestroyAllApples();
                SpawnApples(ApplePickingGame.applesPerRound, spawnPoint.transform);
            }
        }

    }

    public void SpawnApples(int quantity, Transform spawn)
    {
        for(int i = 0; i<quantity; i++)
        {
            Vector3 randPos = new Vector3((spawn.position.x + Random.Range(-xLowerSpawnRange, xUpperSpawnRange)), (spawn.position.y + Random.Range(-yLowerSpawnRange, yUpperSpawnRange)), (spawn.position.z + Random.Range(-zLowerSpawnRange, zUpperSpawnRange)));
            GameObject tempPos = new GameObject();
            tempPos.transform.position = randPos;
            tempPos.transform.RotateAround(spawn.position, Vector3.up, Vector3.Angle(Vector3.forward, spawn.forward));
            randPos = tempPos.transform.position;
            Destroy(tempPos);
            if(applePosits.Count != 0)
            {
                appleSpawnAttemptIter = 0;
                while(CheckForAppleOverlap(randPos) == true)
                {
                    randPos = new Vector3((spawn.position.x + Random.Range(-xLowerSpawnRange, xUpperSpawnRange)), (spawn.position.y + Random.Range(-yLowerSpawnRange, yUpperSpawnRange)), (spawn.position.z + Random.Range(-zLowerSpawnRange, zUpperSpawnRange)));
                    tempPos = new GameObject();
                    tempPos.transform.position = randPos;
                    tempPos.transform.RotateAround(spawn.position, Vector3.up, Vector3.Angle(Vector3.forward, spawn.forward));
                    randPos = tempPos.transform.position;
                    appleSpawnAttemptIter++;
                    if(appleSpawnAttemptIter >= appleSpawnAttemptLimit)
                    {
                        break;
                    }
                }
                applePosits.Add(randPos);
                Instantiate(redApplePrefab, randPos, Quaternion.identity, spawnPoint.transform);
            }
            else
            {
                applePosits.Add(randPos);
                Instantiate(redApplePrefab, randPos, Quaternion.identity, spawnPoint.transform);
            }
        }
        numOfApples = GameObject.FindGameObjectsWithTag("Apple").Length;
    }

    public void SpawnMixedApples(int quantity, Transform spawn)
    {
        int redQuantity = Random.Range(1, quantity-1);
        
        for (int i = 0; i < redQuantity; i++)
        {
            Vector3 randPos = new Vector3((spawn.position.x + Random.Range(-xLowerSpawnRange, xUpperSpawnRange)), (spawn.position.y + Random.Range(-yLowerSpawnRange, yUpperSpawnRange)), (spawn.position.z + Random.Range(-zLowerSpawnRange, zUpperSpawnRange)));
            tempPos = new GameObject();
            tempPos.transform.position = randPos;
            tempPos.transform.RotateAround(spawn.position, Vector3.up, Vector3.Angle(Vector3.forward, spawn.forward));
            randPos = tempPos.transform.position;
            if (applePosits.Count != 0)
            {
                appleSpawnAttemptIter = 0;
                while (CheckForAppleOverlap(randPos) == true)
                {
                    randPos = new Vector3((spawn.position.x + Random.Range(-xLowerSpawnRange, xUpperSpawnRange)), (spawn.position.y + Random.Range(-yLowerSpawnRange, yUpperSpawnRange)), (spawn.position.z + Random.Range(-zLowerSpawnRange, zUpperSpawnRange)));
                    tempPos = new GameObject();
                    tempPos.transform.position = randPos;
                    tempPos.transform.RotateAround(spawn.position, Vector3.up, Vector3.Angle(Vector3.forward, spawn.forward));
                    randPos = tempPos.transform.position;
                    appleSpawnAttemptIter++;
                    if (appleSpawnAttemptIter >= appleSpawnAttemptLimit)
                    {
                        break;
                    }
                }
                applePosits.Add(randPos);
                Instantiate(redApplePrefab, randPos, Quaternion.identity, spawnPoint.transform);
            }
            else
            {
                applePosits.Add(randPos);
                Instantiate(redApplePrefab, randPos, Quaternion.identity, spawnPoint.transform);
            }
        }
        for (int i = 0; i < (quantity - redQuantity); i++)
        {
            Vector3 randPos = new Vector3((spawn.position.x + Random.Range(-xLowerSpawnRange, xUpperSpawnRange)), (spawn.position.y + Random.Range(-yLowerSpawnRange, yUpperSpawnRange)), (spawn.position.z + Random.Range(-zLowerSpawnRange, zUpperSpawnRange)));
            tempPos = new GameObject();
            tempPos.transform.position = randPos;
            tempPos.transform.RotateAround(spawn.position, Vector3.up, Vector3.Angle(Vector3.forward, spawn.forward));
            randPos = tempPos.transform.position;
            appleSpawnAttemptIter = 0;
            while (CheckForAppleOverlap(randPos) == true)
            {
                randPos = new Vector3((spawn.position.x + Random.Range(-xLowerSpawnRange, xUpperSpawnRange)), (spawn.position.y + Random.Range(-yLowerSpawnRange, yUpperSpawnRange)), (spawn.position.z + Random.Range(-zLowerSpawnRange, zUpperSpawnRange)));
                tempPos = new GameObject();
                tempPos.transform.position = randPos;
                tempPos.transform.RotateAround(spawn.position, Vector3.up, Vector3.Angle(Vector3.forward, spawn.forward));
                randPos = tempPos.transform.position;
                appleSpawnAttemptIter++;
                if (appleSpawnAttemptIter >= appleSpawnAttemptLimit)
                {
                    break;
                }
            }
            applePosits.Add(randPos);
            Instantiate(greenApplePrefab, randPos, Quaternion.identity, spawnPoint.transform);
        }
        numOfApples = GameObject.FindGameObjectsWithTag("Apple").Length;
    }

    public bool CheckForAppleOverlap(Vector3 newPos)
    {
        for(int i = 0; i < applePosits.Count; i++)
        {
            if(Vector3.Distance(newPos, applePosits[i]) < appleMinDist)
            {
                return true;
            }
        }

        return false;
    }

    public void SetMaxAppleHeight()
    {
        float maxHeight = SliderHandle.transform.position.y;
        spawnPoint.transform.position = new Vector3(spawnPoint.transform.position.x, maxHeight - yUpperSpawnRange, spawnPoint.transform.position.z);
        ApplePickingGame.ResetGame();
    }

    public static void DestroyAllApples()
    {
        GameObject[] allApples = GameObject.FindGameObjectsWithTag("Apple");
        print(allApples.Length + " apples to be destroyed.");
        for (int i = 0; i < allApples.Length; i++)
        {
            GameObject.Destroy(allApples[i]);
        }
        numOfApples = GameObject.FindGameObjectsWithTag("Apple").Length;
    }

    public void SetSpawnRangePlanes()
    {

        topPlane.transform.position = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y + yUpperSpawnRange, spawnPoint.transform.position.z);
        bottomPlane.transform.position = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y - yLowerSpawnRange, spawnPoint.transform.position.z);
        topPlane.transform.rotation = spawnPoint.transform.rotation;
        bottomPlane.transform.rotation = spawnPoint.transform.rotation;

        float tempY = ((spawnPoint.transform.position.y + yUpperSpawnRange) + (spawnPoint.transform.position.y - yLowerSpawnRange))/2;

        leftPlane.transform.position = new Vector3(spawnPoint.transform.position.x - xLowerSpawnRange, tempY, spawnPoint.transform.position.z);
        rightPlane.transform.position = new Vector3(spawnPoint.transform.position.x + xUpperSpawnRange, tempY, spawnPoint.transform.position.z);
        
        tempPos = new GameObject();
        tempPos.transform.position = leftPlane.transform.position;
        tempPos.transform.RotateAround(spawnPoint.transform.position, Vector3.up, Vector3.Angle(Vector3.forward, spawnPoint.transform.forward));
        leftPlane.transform.position = tempPos.transform.position;
        tempPos = new GameObject();
        tempPos.transform.position = rightPlane.transform.position;
        tempPos.transform.RotateAround(spawnPoint.transform.position, Vector3.up, Vector3.Angle(Vector3.forward, spawnPoint.transform.forward));
        rightPlane.transform.position = tempPos.transform.position;

        leftPlane.transform.rotation = spawnPoint.transform.rotation;
        rightPlane.transform.rotation = spawnPoint.transform.rotation;

        
        backPlane.transform.position = new Vector3(spawnPoint.transform.position.x, tempY, spawnPoint.transform.position.z + zLowerSpawnRange);
        frontPlane.transform.position = new Vector3(spawnPoint.transform.position.x, tempY, spawnPoint.transform.position.z - zUpperSpawnRange);

        tempPos = new GameObject();
        tempPos.transform.position = backPlane.transform.position;
        tempPos.transform.RotateAround(spawnPoint.transform.position, Vector3.up, Vector3.Angle(Vector3.forward, spawnPoint.transform.forward));
        backPlane.transform.position = tempPos.transform.position;
        tempPos = new GameObject();
        tempPos.transform.position = frontPlane.transform.position;
        tempPos.transform.RotateAround(spawnPoint.transform.position, Vector3.up, Vector3.Angle(Vector3.forward, spawnPoint.transform.forward));
        frontPlane.transform.position = tempPos.transform.position;

        backPlane.transform.rotation = spawnPoint.transform.rotation;
        frontPlane.transform.rotation = spawnPoint.transform.rotation;
    }

    public void ResetAppleSliderPos()
    {
        StartCoroutine(ResPos());
        
    }

    public void KeepAppleSliderInPlace()
    {
        SliderHandle.transform.position = HeightAppleHome.transform.position;
        SliderHandle.transform.rotation = HeightAppleHome.transform.rotation;
    }

    public void UpdateSpawnPosY()
    {
        spawnPoint.transform.position = new Vector3(spawnPoint.transform.position.x, SliderHandle.transform.position.y - yUpperSpawnRange, spawnPoint.transform.position.z);
        //spawnPoint.transform.position = SliderHandle.transform.position;
        //spawnPoint.transform.rotation = SliderHandle.transform.rotation;
    }

    public void HidePlanes()
    {
        foreach (Renderer r in planeParent.GetComponentsInChildren<Renderer>())
        {
            r.enabled = false;
        }
    }

    public void ShowPlanes()
    {
        foreach (Renderer r in planeParent.GetComponentsInChildren<Renderer>())
        {
            r.enabled = true;
        }
    }

    private IEnumerator ResPos()
    {
        yield return new WaitForSeconds(1);
        SliderHandle.transform.position = HeightAppleHome.transform.position;
        SliderHandle.transform.rotation = HeightAppleHome.transform.rotation;
    }

    public void SetTreeYScale()
    {
        float newScale = spawnPoint.transform.position.y - treeRoot.transform.position.y;

        treeModel.transform.localScale = new Vector3(TreeInitScale.x, TreeInitScale.y*(newScale / scaleDist), TreeInitScale.z);

    }
}
