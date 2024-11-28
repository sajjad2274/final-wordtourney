using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class AnimateObject : MonoBehaviour
{

    GameObject initialPos;
    public GameObject targetPos;
    // Start is called before the first frame update
    void Start()
    {
        initialPos = transform.gameObject;
    }

    public void SendToTarget(GameObject target, string val)
    {
        targetPos = target;
        Vector3[] waypoints = GenerateBezierWaypoints(transform.position, target.transform.position, 10);

        transform.DOPath(waypoints, 1, PathType.CatmullRom)
            .SetEase(Ease.Linear)
            .OnComplete(() => DestroyObj(val));

        transform.DOScale(Vector3.one * 2, 0.65f);
        transform.DOScale(Vector3.one * 0.5f, 0.35f).SetDelay(0.65f);
    }

    void DestroyObj(string rewardAmount)
    {
        Destroy(gameObject, 0.1f);
        if (targetPos.GetComponent<DOTweenAnimation>() != null)
        {
            //targetPos.GetComponent<DOTweenAnimation>().DORewind();
            targetPos.GetComponent<DOTweenAnimation>().DORestart();
            TMP_Text txt = targetPos.transform.parent.transform.GetChild(2).gameObject.GetComponent<TMP_Text>();
            //string val = txt.text;

            //int value = int.Parse(val);
            //int toAdd = 0;
            //if(numberOfItems < 15)
            //    toAdd = 1;
            //else
            //{
            //    toAdd = numberOfItems / 15;
            //}

            txt.text = rewardAmount.ToString();
        }
    }
    Vector3[] GenerateBezierWaypoints(Vector3 start, Vector3 end, int count)
    {
        Vector3[] waypoints = new Vector3[count];
        Vector3 controlPoint = (start + end) / 2 + Vector3.up * 2; // Simple control point for the curve

        for (int i = 0; i < count; i++)
        {
            float t = i / (float)(count - 1);
            waypoints[i] = CalculateBezierPoint(t, start, controlPoint, end);
        }

        return waypoints;
    }

    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        return uu * p0 + 2 * u * t * p1 + tt * p2;
    }
}
