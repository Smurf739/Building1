using UnityEngine;
using System;

public class Building : MonoBehaviour
{
    public Vector2Int Size = Vector2Int.one;
    public GameObject gridSelector;
    private void Start()
    {
        for (int x= 0; x < Size.x; x++)
        {
            for (int y = 0; y < Size.y; y++)
            { 
                GameObject newGridSelector = Instantiate(gridSelector, transform.position + new Vector3(x, 0.2f, y), Quaternion.Euler(0, 0, 0));
                newGridSelector.transform.parent = gameObject.transform.Find("GridArray");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        for (int x = 0; x < Size.x; x++)
        {
            for (int y = 0; y < Size.y; y++)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawCube(transform.position + new Vector3(x, 0, y), new Vector3(1, 0.1f, 1));

            }
        }
    }


}
