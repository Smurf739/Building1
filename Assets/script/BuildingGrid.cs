using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingGrid : MonoBehaviour
{

    public Vector2Int GridSize = new Vector2Int(0, 0); // размер сетки
    private Building[,] grid; // создание массива сетки
    public Building GridBulding; // создание объекта, на котором висит скрипт билдинг 

    private int NumberHouse = 1;

    public Camera MainCamera;

    public List<GameObject> Grid = new List<GameObject>();

    public bool isBuild = false;


    public Building[] buildingPrefab; // масив наших зданий

    private void Awake()
    {
        grid = new Building[GridSize.x, GridSize.y]; //инициализация сетки
        MainCamera = Camera.main;
    }


    public void StartPlacingBuilding(Building buildingPrefab) // функция, где мы устанавливаем объект или проверяем, установили или нет
    {

        if (isBuild == false)// проверка, посатвили ли мы обект или нет 
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
        if (GridBulding != null) // если мы перемещаем объект то..
        {
            var GroundPlane = new Plane(Vector3.down, Vector3.zero); // создание условного plane, по которому будет перемещаться наш объект
            Ray raycast = MainCamera.ScreenPointToRay(Input.mousePosition); // райкаст мыши, по которому будет следовать наш объект

            if (GroundPlane.Raycast(raycast, out float position)) // если райкаст касается нашего условно созданного плэёна, то ...
            {
                Vector3 WrldPos = raycast.GetPoint(position); // создание позиции райкаста

                int x = Mathf.RoundToInt(WrldPos.x); //что бы обект двигался как по стетке 
                int y = Mathf.RoundToInt(WrldPos.z);

                GridBulding.transform.position = WrldPos;
                bool available = true; // показывает, можем мы устоновить или нет. false - не можем, true - можем

                if (x <= 0 || x > GridSize.x - GridBulding.Size.x) // проверка, за границей наш объект или нет(по X)
                {
                    available = false;
                    
                }

                if (y <= 0 || y > GridSize.y - GridBulding.Size.y) // проверка, за границей наш объект или нет(по Y)
                {
                    available = false;
                }

                if (available == false) // если мы не можем установить, то цвет сетки красный
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


                if (available && IsBuilding(x, y)) // если при строительсвте на пути появилось другое здание, то меняем цвет сетки на красный и не можем ставить
                {
                    available = false;
                    for (int i = 0; i < GridBulding.transform.Find("GridArray").childCount; i++)
                    {
                        GridBulding.transform.Find("GridArray").GetChild(i).GetComponent<Renderer>().material.color = Color.red;
                    }
                }

                GridBulding.transform.position = new Vector3(x, 0, y); // тут и устанавливаем позицию объект, как по сетке

                if (Input.GetMouseButtonUp(1) && available) // если нажата ПКМ и мы в зоне сетки
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
    } // а тут уже проверяем, пересеклись ли другие здания с уже имеющимся

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
    } // задаём в массив grid значения установленных уже зданий

    private void StopBuild()
    {
        Destroy(GameObject.Find(GridBulding.name));
        Destroy(GridBulding);
        isBuild = false;
        NumberHouse--;
    }

    private void DoWithBuild() // все операции над зданиями 
    {


        if ((Input.GetKeyDown(KeyCode.Z)) && GridBulding == null) // кнопки, по которым буде срабатывать функция StartPlacingBuilding
        {

            StartPlacingBuilding(buildingPrefab[0]); // при нажати на кнопку Z будет устанавливаться здание под индексом 0 в массиве.

            for (int i = 0; i < Grid.Count; i++)
            {
                Grid[i].SetActive(enabled);
            }
        }

        if (Input.GetKeyDown(KeyCode.X) && GridBulding == null) // кнопки, по которым буде срабатывать функция StartPlacingBuilding
        {   
            StartPlacingBuilding(buildingPrefab[1]); // при нажати на кнопку Z будет устанавливаться здание под индексом 0 в массиве.   
            for (int i = 0; i < Grid.Count; i++)
            {
                Grid[i].SetActive(enabled);
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && GridBulding != null) // кнопки, по которым буде срабатывать функция StartPlacingBuilding
        {
            GridBulding.transform.Find("Home").transform.Rotate(0, 90, 0);
        }

        if (Input.GetKeyDown(KeyCode.C)) // кнопки, по которым буде срабатывать функция StartPlacingBuilding
        {
            StopBuild();
        }
    }

}

