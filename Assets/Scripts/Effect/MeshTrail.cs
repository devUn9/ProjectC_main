using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteTrail : MonoBehaviour
{
    public float activeTime = 3f;         // 실행시간
    public float refreshRate = 0.05f;     // 잔상 나오는 간격
    public Transform spawnPoint;          // 잔상 스폰위치
    public Material spriteMaterial;     
    public int poolSize = 45;
    int trailIndex = 0;

    SpriteRenderer spriteRenderer;
    bool isTrailActive = false;

    List<GameObject> trailPool = new List<GameObject>();
    int poolCursor = 0;

    Color[] trailColors = {
        new Color(0f, 1f, 1f),
        new Color(0f, 0.8f, 1f),
        new Color(0f, 0.6f, 1f),
        new Color(0.2f, 0.4f, 1f),
        new Color(0.4f, 0.2f, 1f),
        new Color(0.6f, 0f, 1f),
        new Color(0.8f, 0f, 1f),
        new Color(1f, 0f, 0.6f),
        new Color(1f, 0f, 0.3f),
        new Color(1f, 0.3f, 0f),
        new Color(1f, 0.6f, 0f),
        new Color(1f, 1f, 0f),
        new Color(0.5f, 1f, 0f),
        new Color(0f, 1f, 0.5f),
        new Color(0f, 1f, 0.8f)
    };


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        InitializeTrailPool();
    }

    public void StartTrail()
    {
        if (!isTrailActive)
        {
            isTrailActive = true;
            StartCoroutine(SpawnSpriteTrail(activeTime));
        }
    }

    void InitializeTrailPool()
    {
        GameObject trailFolder = new GameObject("TrailPool"); 

        for (int i = 0; i < poolSize; i++)
        {
            GameObject trail = new GameObject("SpriteTrail_" + i);
            trail.transform.SetParent(trailFolder.transform); 
            trail.SetActive(false);

            SpriteRenderer sr = trail.AddComponent<SpriteRenderer>();
            sr.material = new Material(spriteMaterial); 

            trailPool.Add(trail);
        }
    }

    GameObject GetTrailFromPool()
    {
        GameObject trail = trailPool[poolCursor];
        poolCursor = (poolCursor + 1) % trailPool.Count;
        return trail;
    }

    IEnumerator SpawnSpriteTrail(float time)
    {
        while (time > 0)
        {
            GameObject trail = GetTrailFromPool();
            trail.transform.position = spawnPoint.position;
            trail.transform.rotation = spawnPoint.rotation;
            trail.transform.localScale = transform.localScale;

            SpriteRenderer sr = trail.GetComponent<SpriteRenderer>();
            sr.sprite = spriteRenderer.sprite;
            sr.sortingLayerID = spriteRenderer.sortingLayerID;
            sr.sortingOrder = spriteRenderer.sortingOrder - 1;

            Color color = trailColors[trailIndex % trailColors.Length];
            trailIndex++;

            sr.material.SetColor("_Trail_Color", color);
            trail.SetActive(true);

            StartCoroutine(DisableAfterDelay(trail, time));
            yield return new WaitForSeconds(refreshRate);

            time -= refreshRate;
        }

        isTrailActive = false;

        Debug.Log("Stop Trail");
    }

    IEnumerator DisableAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }

}