using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingGrid : MonoBehaviour
{

    public Vector2Int GridSize = new Vector2Int(0, 0); // ������ �����
    private Building[,] grid; // �������� ������� �����
    public Building GridBulding; // �������� �������, �� ������� ����� ������ ������� 

    private int NumberHouse = 1;

    public Camera MainCamera;

    public List<GameObject> Grid = new List<GameObject>();

    public bool isBuild = false;


    public Building[] buildingPrefab; // ����� ����� ������

    private void Awake()
    {
        grid = new Building[GridSize.x, GridSize.y]; //������������� �����
        MainCamera = Camera.main;
    }


    public void StartPlacingBuilding(Building buildingPrefab) // �������, ��� �� ������������� ������ ��� ���������, ���������� ��� ���
    {

        if (isBuild == false)// ��������, ��������� �� �� ����� ��� ��� 
        {
            GridBulding = Instantiate(buildingPrefab);
            GridBulding.transform.position += new Vector3(0, 0.03f, 0);
            GridBulding.transform.parent = GameObject.Find("Houses").transform;
            GridBulding.name = GridBulding.name + NumberHouse;
            NumberHouse++;
            isBuild = true;

        }
            
    }

    public void Update()
    {
        if (GridBulding != null) // ���� �� ���������� ������ ��..
        {
            var GroundPlane = new Plane(Vector3.down, Vector3.zero); // �������� ��������� plane, �� �������� ����� ������������ ��� ������
            Ray raycast = MainCamera.ScreenPointToRay(Input.mousePosition); // ������� ����, �� �������� ����� ��������� ��� ������

            if (GroundPlane.Raycast(raycast, out float position)) // ���� ������� �������� ������ ������� ���������� ������, �� ...
            {
                Vector3 WrldPos = raycast.GetPoint(position); // �������� ������� ��������

                int x = Mathf.RoundToInt(WrldPos.x); //��� �� ����� �������� ��� �� ������ 
                int y = Mathf.RoundToInt(WrldPos.z);

                GridBulding.transform.position = WrldPos;
                bool available = true; // ����������, ����� �� ���������� ��� ���. false - �� �����, true - �����

                if (x <= 0 || x > GridSize.x - GridBulding.Size.x) // ��������, �� �������� ��� ������ ��� ���(�� X)
                {
                    available = false;
                    
                }

                if (y <= 0 || y > GridSize.y - GridBulding.Size.y) // ��������, �� �������� ��� ������ ��� ���(�� Y)
                {
                    available = false;
                }

                if (available == false) // ���� �� �� ����� ����������, �� ���� ����� �������
                {
                    for (int i = 0; i < GridBulding.transform.Find("GridArray").childCount; i++)
                    {
                        GridBulding.transform.Find("GridArray").GetChild(i).GetComponent<Renderer>().material.color = Color.red;
                    }
                }
                else
                {
                    for (int i = 0; i < GridBulding.transform.Find("GridArray").childCount; i++)
                    {
                        GridBulding.transform.Find("GridArray").GetChild(i).GetComponent<Renderer>().material.color = Color.green;
                    }
                }


                if (available && IsBuilding(x, y)) // ���� ��� ������������� �� ���� ��������� ������ ������, �� ������ ���� ����� �� ������� � �� ����� �������
                {
                    available = false;
                    for (int i = 0; i < GridBulding.transform.Find("GridArray").childCount; i++)
                    {
                        GridBulding.transform.Find("GridArray").GetChild(i).GetComponent<Renderer>().material.color = Color.red;
                    }
                }

                GridBulding.transform.position = new Vector3(x, 0, y); // ��� � ������������� ������� ������, ��� �� �����

                if (Input.GetMouseButtonUp(1) && available) // ���� ������ ��� � �� � ���� �����
                {

                    for (int i = 0; i < GridBulding.transform.Find("GridArray").childCount; i++)
                    {
                        Grid.Add(GridBulding.transform.Find("GridArray").GetChild(i).gameObject);
                    }
                    for (int i = 0; i < GridBulding.transform.Find("GridArray").childCount; i++)
                    {
                        GridBulding.transform.Find("GridArray").GetChild(i).GetComponent<Renderer>().material.color = Color.gray;
                    }

                    GridBulding.transform.position += new Vector3(0, -0.03f, 0);

                    PlaceBuildingGrid(x, y);
                }

            }


        }
        DoWithBuild();


    }

    private bool IsBuilding(int placeX, int PlaceY)
    {
        for (int x = 0; x < GridBulding.Size.x; x++)
        {
            for (int y = 0; y < GridBulding.Size.y; y++)
            {
                if (grid[placeX + x, PlaceY + y] != null)
                {
                    return true;
                }
            }
        }

        return false;
    } // � ��� ��� ���������, ����������� �� ������ ������ � ��� ���������

    private void PlaceBuildingGrid(int placeX, int PlaceY)
    {
       
        for (int x = 0; x < GridBulding.Size.x; x++)
        {
            for (int y = 0; y < GridBulding.Size.y; y++)
            {
                grid[placeX + x, PlaceY + y] = GridBulding;
            }
        }

        GridBulding = null;
        isBuild = false;
        for (int i = 0; i < Grid.Count; i++)
        {
            Grid[i].SetActive(!enabled);
        }
    } // ����� � ������ grid �������� ������������� ��� ������

    private void StopBuild()
    {
        Destroy(GameObject.Find(GridBulding.name));
        Destroy(GridBulding);
        isBuild = false;
        NumberHouse--;
    }

    private void DoWithBuild() // ��� �������� ��� �������� 
    {


        if ((Input.GetKeyDown(KeyCode.Z)) && GridBulding == null) // ������, �� ������� ���� ����������� ������� StartPlacingBuilding
        {

            StartPlacingBuilding(buildingPrefab[0]); // ��� ������ �� ������ Z ����� ��������������� ������ ��� �������� 0 � �������.

            for (int i = 0; i < Grid.Count; i++)
            {
                Grid[i].SetActive(enabled);
            }
        }

        if (Input.GetKeyDown(KeyCode.X) && GridBulding == null) // ������, �� ������� ���� ����������� ������� StartPlacingBuilding
        {   
            StartPlacingBuilding(buildingPrefab[1]); // ��� ������ �� ������ Z ����� ��������������� ������ ��� �������� 0 � �������.   
            for (int i = 0; i < Grid.Count; i++)
            {
                Grid[i].SetActive(enabled);
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && GridBulding != null) // ������, �� ������� ���� ����������� ������� StartPlacingBuilding
        {
            GridBulding.transform.Find("Home").transform.Rotate(0, 90, 0);
        }

        if (Input.GetKeyDown(KeyCode.C)) // ������, �� ������� ���� ����������� ������� StartPlacingBuilding
        {
            StopBuild();
        }
    }

}

