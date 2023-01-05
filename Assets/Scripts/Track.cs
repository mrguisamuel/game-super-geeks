using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
    public GameObject[] buildings;
    private float zPoint;
    public Transform reference;
    public Vector3 maxOffset;
    public Vector3 minOffset;
    public int numberBuildings;

    void Start() {
        ResetZPoint();
        InstantiateBuildings();
    }

    void Update()
    {

    }

    void ResetZPoint() {
        zPoint = minOffset.z;
    }

    void InstantiateBuildings() {
        for(int i = 0; i < numberBuildings; i++) {
            StartCoroutine(BuildingsPerTime(10f, zPoint));
            zPoint += Random.Range(maxOffset.z, minOffset.z);
        }
        ResetZPoint();
    }

    IEnumerator BuildingsPerTime(float time, float offset) {
        int idBuild = (int)Random.Range(0, buildings.Length - 1);
        
        float xSort = Random.Range(minOffset.x, maxOffset.x);
        float ySort = Random.Range(minOffset.y, maxOffset.y);

        Instantiate(
            buildings[idBuild], 
            new Vector3(xSort, ySort, offset), 
            Quaternion.EulerAngles(0f, Random.Range(0f, 180f), 0f)
        );

        yield return new WaitForSeconds(time);
    }
}